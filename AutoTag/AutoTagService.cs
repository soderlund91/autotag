using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Tasks;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Serialization;
using MediaBrowser.Model.Services;
using MediaBrowser.Model.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoTag
{
    [Route("/AutoTag/TestUrl", "GET", Summary = "Tests a tag source URL")]
    public class TestUrlRequest : IReturn<TestUrlResponse>
    {
        public string Url { get; set; } = string.Empty;
        public int Limit { get; set; } = 10;
    }

    [Route("/AutoTag/RunSync", "POST", Summary = "Triggers the AutoTag Scheduled Task")]
    public class RunSyncRequest : IReturn<RunSyncResponse> { }

    public class TestUrlResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class RunSyncResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class AutoTagService : IService
    {
        private readonly IHttpClient _httpClient;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly ITaskManager _taskManager;
        private readonly ILibraryManager _libraryManager;
        private readonly ILogger _logger;

        public AutoTagService(IHttpClient httpClient, IJsonSerializer jsonSerializer, ITaskManager taskManager, ILibraryManager libraryManager, ILogManager logManager)
        {
            _httpClient = httpClient;
            _jsonSerializer = jsonSerializer;
            _taskManager = taskManager;
            _libraryManager = libraryManager;
            _logger = logManager.GetLogger("AutoTag");
        }

        public object Get(TestUrlRequest request)
        {
            var task = TestUrlAsync(request);
            task.Wait();
            return task.Result;
        }

        public object Post(RunSyncRequest request)
        {
            var taskWorker = _taskManager.ScheduledTasks.FirstOrDefault(t => t.ScheduledTask.Key == "AutoTagSyncTask");

            if (taskWorker == null)
            {
                return new RunSyncResponse { Success = false, Message = "Could not find AutoTag task." };
            }

            _taskManager.Execute(taskWorker, new TaskOptions());

            return new RunSyncResponse { Success = true, Message = "AutoTag sync started successfully." };
        }

        private async Task<TestUrlResponse> TestUrlAsync(TestUrlRequest request)
        {
            var config = Plugin.Instance?.Configuration;
            if (config == null) return new TestUrlResponse { Success = false, Message = "Configuration not found." };

            string traktId = config.TraktClientId;
            string mdbKey = config.MdblistApiKey;

            var fetcher = new ListFetcher(_httpClient, _jsonSerializer);

            try
            {
                var items = await fetcher.FetchItems(request.Url, request.Limit, traktId, mdbKey, CancellationToken.None);

                if (items.Count == 0)
                {
                    return new TestUrlResponse
                    {
                        Success = false,
                        Message = "Could not find any items. Check URL or API Key."
                    };
                }

                return new TestUrlResponse
                {
                    Success = true,
                    Count = items.Count,
                    Message = $"Successfully found {items.Count} items."
                };
            }
            catch (System.Exception ex)
            {
                return new TestUrlResponse { Success = false, Message = $"Error: {ex.Message}" };
            }
        }
    }
}