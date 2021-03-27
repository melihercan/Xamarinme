using System;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using XamarinmeWebHost;


namespace Xamarinme
{
    public class WebHost
    {
        public static async Task Main(string[] args)
        {
            //var host = new HostBuilder()
            //  .ConfigureHostConfiguration((config) =>
            //  {
            //      config.AddEmbeddedResource(
            //          new EmbeddedResourceConfigurationOptions
            //          {
            //              Assembly = Assembly.GetExecutingAssembly(),
            //              Prefix = "DemoApp.WebHost"
            //          });
            //    })
            //    .ConfigureAppConfiguration((hostContext, config) =>
            //    {
            //    })
            //    .ConfigureServices((hostContext, services) =>
            //    {
            //    })
            //    .UseEmbedIoServer()
            //    .ConfigureWebHost((hostContext, app) =>
            //    {
            //        app.Run(async (context) =>
            //        {
            //            await context.Response.WriteAsync("Hello World!");
            //        });
            //    })
            //    .Build();
            //
            //System.Diagnostics.Debug.WriteLine($"-------- Running WebHost");
            //await host.RunAsync();

            var hostBuilder = new HostBuilder();

            hostBuilder.ConfigureHostConfiguration((config) =>
            {
                config.AddEmbeddedResource(
                    new EmbeddedResourceConfigurationOptions
                    {
                        Assembly = Assembly.GetExecutingAssembly(),
                        Prefix = "XamarinmeWebHost"
                    });
            });

            hostBuilder.ConfigureAppConfiguration((hostContext, config) =>
            {
            });

            hostBuilder.ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<IHostLifetime, ConsoleLifetimeEx>();
            });

            hostBuilder.UseEmbedIoServer();

            hostBuilder.ConfigureWebHost((hostContext, app) =>
            {
                app.Run(async (context) =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });

            var host = hostBuilder.Build();

            System.Diagnostics.Debug.WriteLine($"-------- Running WebHost");
            await host.RunAsync();
        }
    }
}
