using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using XamarinmeWebHost;
using HarmonyLib;
using Microsoft.Net.Http.Headers;

namespace Xamarinme
{
    // aspnetcore v2.2.7 = commit be0a4a7f4c
    // runtime v5.0 = commit cf258a14b70

    public class WebHost
    {
#if true
        public class Startup
        {
            private static readonly byte[] _helloWorldBytes = Encoding.UTF8.GetBytes("Hello Xamarin, greetings from Kestrel");

            public void Configure(IApplicationBuilder app)
            {
                app.Run((httpContext) =>
                {
                    var response = httpContext.Response;
                    response.StatusCode = 200;
                    response.ContentType = "text/plain";

                    var helloWorld = _helloWorldBytes;
                    response.ContentLength = helloWorld.Length;
                    try
                    {
                        return response.Body.WriteAsync(helloWorld, 0, helloWorld.Length);
                    }
                    catch(Exception ex)
                    {
                        var x = ex.Message;
                        return Task.CompletedTask;
                    }
                });
            }
        }

        public static Task Main(string[] args)
        {
            var harmony = new Harmony("org.melihercan.Xamarinme.WebHost");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            var webHost = new WebHostBuilder()
                .ConfigureAppConfiguration((config) =>
                {
                    config.AddEmbeddedResource(
                        new EmbeddedResourceConfigurationOptions
                        {
                            Assembly = Assembly.GetExecutingAssembly(),
                            Prefix = "XamarinmeWebHost"
                        });

                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<IHostLifetime, ConsoleLifetimeEx>();
                })
                .UseKestrel(options =>
                {
                    options.Listen(IPAddress.Loopback, 5000);
                })
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build();

            return webHost.RunExAsync();
        }
#endif


#if false
        public static async Task Main(string[] args)
        {
            //var host = new HostBuilder()
            //  .ConfigureHostConfiguration((config) =>
            //  {
            //      config.AddEmbeddedResource(
            //          new EmbeddedResourceConfigurationOptions
            //          {
            //              Assembly = Assembly.GetExecutingAssembly(),
            //              Prefix = "XamarinmeWebHost"
            //          });
            //    })
            //    .ConfigureAppConfiguration((hostContext, config) =>
            //    {
            //    })
            //    .ConfigureServices((hostContext, services) =>
            //    {
            //      // Override the default host lifetime with ours.
            //      services.AddSingleton<IHostLifetime, ConsoleLifetimeEx>();
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


#if true
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
                // Override the default host lifetime with ours.
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
#endif



        }
#endif
    }

    [HarmonyPatch(typeof(HeaderUtilities), "FormatDate", new Type[] { typeof(DateTimeOffset), typeof(bool) })]
    class FormatDatePatch
    {
        [HarmonyPrefix]
        static bool MyFormatDate(ref string __result, DateTimeOffset dateTime, bool quoted)
        {
            __result = "Patched code";
            return false;
        }
    }

}
