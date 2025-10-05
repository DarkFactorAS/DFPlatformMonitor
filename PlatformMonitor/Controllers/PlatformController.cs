using Microsoft.AspNetCore.Mvc;
using PlatformMonitor.Models;
using PlatformMonitor.Provider;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlatformMonitor.Controllers
{
    public class PlatformController : Controller
    {
        private readonly IPlatformProvider _platformProvider;

        public PlatformController(IPlatformProvider platformProvider)
        {
            _platformProvider = platformProvider;
        }

        public async Task<IActionResult> Status()
        {
            var platforms = _platformProvider.GetAllPlatforms();
            var httpClient = new System.Net.Http.HttpClient();
            foreach (var platform in platforms)
            {
                string version = "N/A";
                bool isUp = false;
                try
                {
                    // Health check: try to GET /Ping endpoint
                    var healthResponse = await httpClient.GetAsync($"{platform.Url}/Ping");
                    isUp = healthResponse.IsSuccessStatusCode;

                    // Version fetch: try to GET /version endpoint
                    var versionResponse = await httpClient.GetAsync($"{platform.Url}/version");
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
            }
            return View(platforms);
        }
    }
}
