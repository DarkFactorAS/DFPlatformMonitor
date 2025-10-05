using Microsoft.AspNetCore.Mvc;
using PlatformMonitor.Models;
using PlatformMonitor.Provider;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;

namespace PlatformMonitor.Controllers
{
    public class PlatformController : Controller
    {
        private readonly IPlatformProvider _platformProvider;
        private readonly IHttpClientFactory _httpClientFactory;

        public PlatformController(IPlatformProvider platformProvider, IHttpClientFactory httpClientFactory)
        {
            _platformProvider = platformProvider;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var platforms = _platformProvider.GetAllPlatforms();
            var httpClient = _httpClientFactory.CreateClient();
            foreach (var platform in platforms)
            {
                try
                {
                    // Health check: try to GET /Ping endpoint
                    var healthResponse = await httpClient.GetAsync($"{platform.Url}/Ping");
                    platform.IsUp = healthResponse.IsSuccessStatusCode;

                    // Version fetch: try to GET /version endpoint
                    var versionResponse = await httpClient.GetAsync($"{platform.Url}/version");
                    if (versionResponse.IsSuccessStatusCode)
                    {
                        platform.Version = await versionResponse.Content.ReadAsStringAsync();
                        platform.Version = platform.Version.Trim('"'); // Remove quotes if JSON string
                    }
                }
                catch
                {
                    platform.IsUp = false;
                    platform.Version = "N/A";
                }
            }
            return View(platforms);
        }
    }
}
