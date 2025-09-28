using Microsoft.AspNetCore.Mvc;
using PlatformMonitor.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlatformMonitor.Controllers
{
    public class ServicesController : Controller
    {
        private readonly PlatformMonitorConfig _config;

        public ServicesController(Microsoft.Extensions.Options.IOptions<PlatformMonitorConfig> config)
        {
            _config = config.Value;
        }

        public async Task<IActionResult> Status()
        {
            // Map config services to ServiceStatus (version and status would be fetched in real scenario)
            var services = new List<ServiceStatus>();
            foreach (var svc in _config.Services)
            {
                services.Add(new ServiceStatus
                {
                    Name = svc.Name,
                    Version = "N/A", // Placeholder, replace with actual version fetch
                    IsUp = false // Placeholder, replace with actual health check
                });
            }
            await Task.Delay(100);
            return View(services);
        }
    }
}
