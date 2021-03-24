using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using XamarinmeWebHost;

namespace Xamarinme
{
    public class WebHost
    {
        public static async Task Main(string[] args)
        {
            //var host = new HostBuilder()
            //    .ConfigureAppConfiguration((hostContext, config) =>
            //    {
            //        config.AddEnvironmentVariables();
            //        config.AddJsonFile("appsettings.json", optional: true);
            //        config.AddCommandLine(args);
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
            //    .UseConsoleLifetime()
            //    .Build();

            var hostBuilder = new HostBuilder();

            hostBuilder.ConfigureAppConfiguration((hostContext, config) =>
            {
                config.AddEnvironmentVariables();
                config.AddJsonFile("appsettings.json", optional: true);
                config.AddCommandLine(args);
            });

            hostBuilder.ConfigureServices((hostContext, services) =>
            {
            });

            hostBuilder.UseEmbedIoServer();

            hostBuilder.ConfigureWebHost((hostContext, app) =>
            {
                app.Run(async (context) =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });

            hostBuilder.UseConsoleLifetime();

            IHost host = null;

            try
            {
                host = hostBuilder.Build();
            }
            catch (Exception ex)
            {
                var x = ex.Message;
            }



            var s = host.Services;

            await host.RunAsync();
        }
    }
}
