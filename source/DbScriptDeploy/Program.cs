using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DbScriptDeploy.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DbScriptDeploy
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {

            var config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: false)
                    .Build();
            IAppSettings settings = new AppSettings();
            config.GetSection("AppSettings").Bind(settings);

            IHostBuilder builder =  Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseContentRoot(AppDomain.CurrentDomain.BaseDirectory)
                        .UseKestrel(o => o.AllowSynchronousIO = true)
                        .UseStartup<Startup>()
                        .UseUrls($"http://localhost:{settings.Port}/");
                })
                .UseWindowsService();

            builder.ConfigureServices(services => services.AddHostedService<BackgroundScriptService>());
            return builder;

        }
    }
}
