using Microsoft.AspNetCore.Mvc;
using PlatformMonitor.Models;
using PlatformMonitor.Provider;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using DFCommonLib.Logger;

namespace PlatformMonitor.Controllers
{
    public class PlatformController : Controller
    {
        private readonly IPlatformProvider _platformProvider;
        private readonly IHttpClientFactory _httpClientFactory;
        private IDFLogger<PlatformController> _logger;

        public PlatformController(IPlatformProvider platformProvider, IHttpClientFactory httpClientFactory, IDFLogger<PlatformController> logger)
        {
            _platformProvider = platformProvider;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var platforms = _platformProvider.GetAllPlatforms();
            var httpClient = _httpClientFactory.CreateClient();
            foreach (var platform in platforms)
            {
                platform.IsUp = false;

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
                catch (System.Exception ex)
                {
                    _logger.LogError($"Error connecting to platform {platform.Name} at {platform.Url} : {ex.Message}");
                    platform.Version = "N/A";
                }
            }
            return View(platforms);
        }
    }
}
