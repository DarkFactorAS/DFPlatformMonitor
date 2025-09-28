using Microsoft.AspNetCore.Mvc;
using PlatformMonitor.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlatformMonitor.Controllers
{
    public class ServicesController : Controller
    {
        public async Task<IActionResult> Status()
        {
            // Example services - replace with real data source or API calls
            var services = new List<ServiceStatus>
            {
                new ServiceStatus { Name = "AuthService", Version = "1.2.3", IsUp = true },
                new ServiceStatus { Name = "DataService", Version = "2.0.1", IsUp = false },
                new ServiceStatus { Name = "NotificationService", Version = "3.4.5", IsUp = true }
            };
            // Simulate async operation
            await Task.Delay(100);
            return View(services);
        }
    }
}
