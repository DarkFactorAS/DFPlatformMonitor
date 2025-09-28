using System.Collections.Generic;

namespace PlatformMonitor.Models
{
    public class PlatformMonitorConfig
    {
        public List<ServiceConfig> Services { get; set; } = new();
    }

    public class ServiceConfig
    {
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }
}
