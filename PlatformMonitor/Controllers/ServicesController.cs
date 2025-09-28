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
            var services = new List<ServiceStatus>();
            var httpClient = new System.Net.Http.HttpClient();
            foreach (var svc in _config.Services)
            {
                string version = "N/A";
                bool isUp = false;
                try
                {
                    // Health check: try to GET /Ping endpoint
                    var healthResponse = await httpClient.GetAsync($"{svc.Url}/Ping");
                    isUp = healthResponse.IsSuccessStatusCode;

                    // Version fetch: try to GET /version endpoint
                    var versionResponse = await httpClient.GetAsync($"{svc.Url}/version");
                    if (versionResponse.IsSuccessStatusCode)
                    {
                        version = await versionResponse.Content.ReadAsStringAsync();
                        version = version.Trim('"'); // Remove quotes if JSON string
                    }
                }
                catch
                {
                    isUp = false;
                    version = "N/A";
                }
                services.Add(new ServiceStatus
                {
                    Name = svc.Name,
                    Version = version,
                    IsUp = isUp,
                    Environment = svc.Environment
                });
            }
            return View(services);
        }
    }
}
