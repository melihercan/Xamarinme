using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using XamarinmeHosting;

namespace Xamarinme
{
    public sealed class XamarinHostBuilder 
    {


        public XamarinHostConfiguration Configuration { get; }
        public IServiceCollection Services { get; }
        public IXamarinHostEnvironment HostEnvironment { get; }
        public ILoggingBuilder Logging { get; private set; }


        public static XamarinHostBuilder CreateDefault(EmbeddedResourceConfigurationOptions configurationOptions)
        {
            var builder = new XamarinHostBuilder(configurationOptions);
            return builder;
        }

        public IHost Build()
        {
            Services.AddSingleton<IConfiguration>(Configuration);

            return new XamarinHost(Services.BuildServiceProvider());
        }

        internal XamarinHostBuilder(EmbeddedResourceConfigurationOptions configurationOptions)
        {
            Configuration = new XamarinHostConfiguration();
            Services = new ServiceCollection();
            HostEnvironment = InitializeEnvironment(configurationOptions);

            Configuration
                .Add(new EmbeddedResourceConfigurationSource 
                { 
                    Options = configurationOptions, 
                    Environment = HostEnvironment.Environment 
                })
                .Build();

            Services.AddLogging(loggingBuilder => 
            {
                Logging = loggingBuilder;
                loggingBuilder.AddConfiguration(Configuration.GetSection("Logging"));
                loggingBuilder.AddDebug();
            });
        }

        private XamarinHostEnvironment InitializeEnvironment(EmbeddedResourceConfigurationOptions configurationOptions)
        {
#if false
            // No straightforward way to get environment variables in Xamarin.
            // "Production" is the host environment by default.
            // It can be overridden in "appsettings.json" file by defining the "XAMARIN_ENVIRONMENT" value.
            // "ASPNETCORE_ENVIRONMENT" has priority over "DOTNET_ENVIRONMENT".
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ??
                Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ??
                Environments.Production;
#endif
            string environment = "Production";

            var assembly = configurationOptions.Assembly;
            var file = $"{configurationOptions.Prefix}.appsettings.json";

            if (assembly.GetManifestResourceInfo(file) != null)
            {
                using (var stream = configurationOptions.Assembly.GetManifestResourceStream(file))
                {
                    // Get "XAMARIN_ENVIRONMENT" entry if defined.
                    var json = new StreamReader(stream).ReadToEnd();
                    var content = JsonDocument.Parse(json);
                    try
                    {
                        environment = content.RootElement.GetProperty("XAMARIN_ENVIRONMENT").GetString();
                    }
                    catch { }
                }
            }

            var hostEnvironment = new XamarinHostEnvironment(environment);
            Services.AddSingleton<IXamarinHostEnvironment>(hostEnvironment);

            return hostEnvironment;
        }
    }
}
