using DFCommonLib.Config;
using DFCommonLib.DataAccess;
using DFCommonLib.Logger;
using DFCommonLib.Utils;
using PlatformMonitor.Models;
using PlatformMonitor.Provider;
using PlatformMonitor.Repository;

string AppName = "PlatformMonitor";

var builder = WebApplication.CreateBuilder(args);

// Load configuration files from Config folder
builder.Configuration
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("Config/appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"Config/appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

var services = builder.Services;

// Add services to the container.
services.AddControllersWithViews();

services.AddTransient<IConfigurationHelper, ConfigurationHelper<AppSettings>>();

services.AddTransient<IStartupDatabasePatcher, PlatformDatabasePatcher>();
services.AddTransient<IPlatformRepository, PlatformRepository>();
services.AddTransient<IPlatformProvider, PlatformProvider>();

new DFServices(services)
    .SetupLogger()
    .SetupMySql()
    .LogToConsole(DFLogLevel.INFO)
    .LogToMySQL(DFLogLevel.WARNING)
    .LogToEvent(DFLogLevel.ERROR, AppName);
;

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Services}/{action=Status}/{id?}");

app.Run();
