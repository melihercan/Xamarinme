using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarinme;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace DemoApp.WebHost.WebHost
{
    class Program
    {
        public static Task Main(string[] args)
        {
            var ipString = CrossIpAddress.Current.Get();
            IPAddress ipAddress;
            IPAddress.TryParse(ipString, out ipAddress);

            var webHost = new WebHostBuilder()
                .ConfigureAppConfiguration((config) =>
                {
                    config.AddEmbeddedResource(
                        new EmbeddedResourceConfigurationOptions
                        {
                            Assembly = Assembly.GetExecutingAssembly(),
                            Prefix = "DemoApp.WebHost"
                        });
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<IHostLifetime, ConsoleLifetimePatch>();
                })
                .UseKestrel(options =>
                {
                    options.Listen(ipAddress, 5000);
                })
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build();

            App.Host = webHost;

            return webHost.RunPatchedAsync();
        }

    }
}
