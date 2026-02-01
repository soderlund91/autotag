using MediaBrowser.Model.Plugins;
using System.Collections.Generic;

namespace AutoTag
{
    public class PluginConfiguration : BasePluginConfiguration
    {
        public string TraktClientId { get; set; } = "";
        public string MdblistApiKey { get; set; } = "";
        public bool ExtendedConsoleOutput { get; set; } = false;
        public bool DryRunMode { get; set; } = false;
        public List<TagConfig> Tags { get; set; } = new List<TagConfig>();
    }

    public class TagConfig
    {
        public bool Active { get; set; } = true;
        public string Tag { get; set; } = "";
        public string Url { get; set; } = "";
        public int Limit { get; set; } = 50;
    }
}