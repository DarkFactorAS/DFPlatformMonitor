using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

using PlatformMonitor.Repository;
using PlatformMonitor.Provider;
using PlatformMonitor.Models;

using DFCommonLib.Config;
using DFCommonLib.Logger;
using DFCommonLib.Utils;
using DFCommonLib.DataAccess;


namespace PlatformMonitor
{
    class Program
    {
        public static string AppName = "PlatformMonitor";
        public static string AppVersion = "0.2.0";

        static void Main(string[] args)
        {
            var builder = CreateHostBuilder(args).Build();

            try
            {
                IConfigurationHelper configurationHelper = DFServices.GetService<IConfigurationHelper>();
                var config = configurationHelper.Settings;
                var msg = string.Format("Connecting to DB : {0}:{1}", config.DatabaseConnection.Server, config.DatabaseConnection.Port);
                DFLogger.LogOutput(DFLogLevel.INFO, "PlatformMonitor", msg);

                // Run database script
                IStartupDatabasePatcher startupRepository = DFServices.GetService<IStartupDatabasePatcher>();
                startupRepository.WaitForConnection();
                if (startupRepository.RunPatcher())
                {
                    DFLogger.LogOutput(DFLogLevel.INFO, "PlatformMonitor", "Database patcher ran successfully");
                }
                else
                {
                    DFLogger.LogOutput(DFLogLevel.ERROR, "PlatformMonitor", "Database patcher failed");
                    Environment.Exit(1);
                    return;
                }

                builder.Run();
            }
            catch (Exception ex)
            {
                DFLogger.LogOutput(DFLogLevel.WARNING, "Startup", ex.ToString());
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<IConfigurationHelper, ConfigurationHelper<AppSettings>>();
                services.AddHttpClient(); 

                new DFServices(services)
                    .SetupLogger()
                    .SetupMySql()
                    .LogToConsole(DFLogLevel.INFO)
                    .LogToMySQL(DFLogLevel.WARNING)
                    .LogToEvent(DFLogLevel.ERROR, AppName);

                services.AddTransient<IStartupDatabasePatcher, PlatformDatabasePatcher>();
                services.AddTransient<IPlatformRepository, PlatformRepository>();
                services.AddTransient<IPlatformProvider, PlatformProvider>();
                services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            }
        );
    }
}
