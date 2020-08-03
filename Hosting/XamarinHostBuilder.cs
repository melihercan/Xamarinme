using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using XamarinmeHosting;

namespace Xamarinme
{
    public sealed class XamarinHostBuilder 
    {


        public XamarinHostConfiguration Configuration { get; }
        public IServiceCollection Services { get; }
        public IXamarinHostEnvironment HostEnvironment { get; }

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
            HostEnvironment = InitializeEnvironment();

            Configuration
                .Add(new EmbeddedResourceConfigurationSource 
                { 
                    Options = configurationOptions, 
                    Environment = HostEnvironment.Environment 
                })
                .Build();

            Services.AddLogging(loggingBuilder => 
            { 
                loggingBuilder.AddConfiguration(Configuration.GetSection("Logging"));
                loggingBuilder.AddDebug();
            });
        }

        private XamarinHostEnvironment InitializeEnvironment()
        {
            // "ASPNETCORE_ENVIRONMENT" has priority over "DOTNET_ENVIRONMENT".
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ??
                Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ??
                Environments.Production;
            var hostEnvironment = new XamarinHostEnvironment(environment);
            Services.AddSingleton<IXamarinHostEnvironment>(hostEnvironment);

            return hostEnvironment;
        }
    }
}
