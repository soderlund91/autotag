using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Model.Tasks;
using MediaBrowser.Controller.Library;
using MediaBrowser.Common.Net;
using MediaBrowser.Model.Serialization;
using MediaBrowser.Model.Logging;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Model.Querying;
using MediaBrowser.Model.Entities;
using System.IO;

namespace AutoTag
{
    public class AutoTagTask : IScheduledTask
    {
        private readonly ILibraryManager _libraryManager;
        private readonly IHttpClient _httpClient;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly ILogger _logger;

        public AutoTagTask(ILibraryManager libraryManager, IHttpClient httpClient, IJsonSerializer jsonSerializer, ILogManager logManager)
        {
            _libraryManager = libraryManager;
            _httpClient = httpClient;
            _jsonSerializer = jsonSerializer;
            _logger = logManager.GetLogger("AutoTag");
        }

        public string Key => "AutoTagSyncTask";
        public string Name => "AutoTag: Sync Tags";
        public string Description => "Syncs tags from MDBList and Trakt based on configuration.";
        public string Category => "Library";

        public IEnumerable<TaskTriggerInfo> GetDefaultTriggers()
        {
            return new[] { new TaskTriggerInfo { Type = TaskTriggerInfo.TriggerDaily, TimeOfDayTicks = TimeSpan.FromHours(4).Ticks } };
        }

        public async Task Execute(CancellationToken cancellationToken, IProgress<double> progress)
        {
            var config = Plugin.Instance?.Configuration;
            if (config == null) return;

            bool debug = config.ExtendedConsoleOutput;
            bool dryRun = config.DryRunMode;

            _logger.Info($"--- STARTING AUTOTAG (v1.1.3) ---");
            if (dryRun) _logger.Info("--- DRY RUN MODE ENABLED (No changes will be saved) ---");

            var currentTags = config.Tags
                .Where(t => !string.IsNullOrWhiteSpace(t.Tag))
                .Select(t => t.Tag.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var previousTags = LoadTagHistory();
            var orphanedTags = previousTags.Except(currentTags, StringComparer.OrdinalIgnoreCase).ToList();

            if (orphanedTags.Count > 0)
            {
                foreach (var orphan in orphanedTags) CleanUpTag(orphan, debug, dryRun);
            }

            if (!dryRun)
            {
                SaveTagHistory(currentTags);
            }

            progress.Report(5);

            foreach (var tagName in currentTags) CleanUpTag(tagName, debug, dryRun);
            progress.Report(10);

            var fetcher = new ListFetcher(_httpClient, _jsonSerializer);

            double step = 90.0 / (config.Tags.Count > 0 ? config.Tags.Count : 1);
            double currentProgress = 10;

            foreach (var tagConfig in config.Tags)
            {
                if (!tagConfig.Active || string.IsNullOrWhiteSpace(tagConfig.Tag))
                {
                    currentProgress += step;
                    continue;
                }

                string tagName = tagConfig.Tag.Trim();
                if (debug) _logger.Info($"[SOURCE] Processing '{tagName}' from: {tagConfig.Url}");

                try
                {
                    var itemsFound = await fetcher.FetchItems(tagConfig.Url, tagConfig.Limit, config.TraktClientId, config.MdblistApiKey, cancellationToken);

                    if (itemsFound.Count > tagConfig.Limit)
                        itemsFound = itemsFound.Take(tagConfig.Limit).ToList();

                    if (debug) _logger.Info($"    -> Source returned {itemsFound.Count} items.");

                    int addedCount = 0;
                    var processedIds = new HashSet<string>();

                    foreach (var item in itemsFound)
                    {
                        string uid = !string.IsNullOrEmpty(item.Imdb) ? item.Imdb : $"tmdb-{item.Tmdb}";
                        if (processedIds.Contains(uid)) continue;
                        processedIds.Add(uid);

                        if (MatchAndTag(item, tagName, debug, dryRun))
                            addedCount++;
                    }
                    _logger.Info($"    -> Tagged {addedCount} items with '{tagName}'.");
                }
                catch (Exception ex)
                {
                    _logger.Error($"Error processing {tagConfig.Url}: {ex.Message}");
                }

                currentProgress += step;
                progress.Report(currentProgress);
            }

            progress.Report(100);
            _logger.Info("--- Finished ---");
        }

        private string GetHistoryFilePath() => Path.Combine(Plugin.Instance.DataFolderPath, "autotag_history.txt");

        private List<string> LoadTagHistory()
        {
            try
            {
                var path = GetHistoryFilePath();
                if (File.Exists(path)) return File.ReadAllLines(path).Select(l => l.Trim()).Where(l => !string.IsNullOrEmpty(l)).ToList();
            }
            catch { }
            return new List<string>();
        }

        private void SaveTagHistory(List<string> tags)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(GetHistoryFilePath()));
                File.WriteAllLines(GetHistoryFilePath(), tags);
            }
            catch { }
        }

        private void CleanUpTag(string tagName, bool debug, bool dryRun)
        {
            var query = new InternalItemsQuery { Recursive = true, DtoOptions = new MediaBrowser.Controller.Dto.DtoOptions(true) };
            var allItems = _libraryManager.GetItemList(query);
            int count = 0;
            foreach (var item in allItems)
            {
                if (item.Tags.Contains(tagName, StringComparer.OrdinalIgnoreCase))
                {
                    if (dryRun)
                    {
                        count++;
                        continue;
                    }

                    var master = _libraryManager.GetItemById(item.Id);
                    var actualTag = master?.Tags.FirstOrDefault(t => t.Equals(tagName, StringComparison.OrdinalIgnoreCase));
                    if (master != null && actualTag != null)
                    {
                        master.RemoveTag(actualTag);
                        _libraryManager.UpdateItem(master, master.Parent, ItemUpdateType.MetadataEdit, null);
                        count++;
                    }
                }
            }
            if (count > 0 && debug)
            {
                string prefix = dryRun ? "[DRY RUN] Would remove" : "[CLEANUP] Removed";
                _logger.Info($"      {prefix} '{tagName}' from {count} items.");
            }
        }

        private bool MatchAndTag(ExternalItemDto extItem, string tagName, bool debug, bool dryRun)
        {
            BaseItem? target = null;
            if (!string.IsNullOrEmpty(extItem.Imdb)) target = FindValidItem(new Dictionary<string, string> { { "imdb", extItem.Imdb } });
            if (target == null && !string.IsNullOrEmpty(extItem.Tmdb)) target = FindValidItem(new Dictionary<string, string> { { "tmdb", extItem.Tmdb } });

            if (target != null && !target.Tags.Contains(tagName, StringComparer.OrdinalIgnoreCase))
            {
                if (dryRun)
                {
                    if (debug) _logger.Info($"      [DRY RUN] Would ADD {tagName} -> {target.Name}");
                    return true;
                }

                var masterItem = _libraryManager.GetItemById(target.Id);
                if (masterItem != null && !string.IsNullOrEmpty(masterItem.Path))
                {
                    masterItem.AddTag(tagName);
                    _libraryManager.UpdateItem(masterItem, masterItem.Parent, ItemUpdateType.MetadataEdit, null);
                    if (debug) _logger.Info($"      [ADD] {tagName} -> {masterItem.Name}");
                    return true;
                }
            }
            return false;
        }

        private BaseItem? FindValidItem(Dictionary<string, string> ids)
        {
            var results = _libraryManager.GetItemList(new InternalItemsQuery
            {
                IncludeItemTypes = new[] { "Movie", "Series" },
                Recursive = true,
                AnyProviderIdEquals = ids,
                DtoOptions = new MediaBrowser.Controller.Dto.DtoOptions(true)
            });
            return results.FirstOrDefault(i => !i.IsVirtualItem && i.LocationType == LocationType.FileSystem && !string.IsNullOrEmpty(i.Path));
        }
    }
}