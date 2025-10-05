namespace PlatformMonitor.Models
{
    public class Platform
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Version { get; set; }
        public bool IsUp { get; set; }
        public string? Environment { get; set; }
        public string? Url { get; set; }

        public Platform()
        {
            Version = "N/A";
            IsUp = false;
        }
    }
}
