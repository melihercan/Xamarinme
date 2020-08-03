using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        ////public RootComponentMappingCollection RootComponents { get; }
        public IServiceCollection Services { get; }
        public IXamarinHostEnvironment HostEnvironment { get; }
        public ILoggingBuilder Logging { get; }

        public static XamarinHostBuilder CreateDefault(EmbeddedResourceConfigurationOptions configurationOptions)
        {
            var builder = new XamarinHostBuilder(configurationOptions);
            return builder;
        }

        public XamarinHost Build()
        {
            return null;
        }
        //public void ConfigureContainer<TBuilder>(IServiceProviderFactory<TBuilder> factory, Action<TBuilder> configure = null);


        internal XamarinHostBuilder(EmbeddedResourceConfigurationOptions configurationOptions)
        {
            Configuration = new XamarinHostConfiguration();
            Services = new ServiceCollection();
            HostEnvironment = InitializeEnvironment();
            Logging = new LoggingBuilder(Services);

            Configuration
                .Add(new EmbeddedResourceConfigurationSource 
                { 
                    Options = configurationOptions, 
                    Environment = HostEnvironment.Environment 
                })
                .Build();

            ////            InitializeDefaultServices();
            Logging.AddConfiguration(Configuration.GetSection("Logging"));
            Logging.AddConsole();
        }

        private void InitializeDefaultServices()
        {
            Services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConfiguration(Configuration.GetSection("Logging"));
                loggingBuilder.AddConsole();
            });
        }

        private XamarinHostEnvironment InitializeEnvironment()
        {
            // "ASPNETCORE_ENVIRONMENT" has priority over "DOTNET_ENVIRONMENT".
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ??
                Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ??
                Microsoft.Extensions.Hosting.Environments.Production;
            var hostEnvironment = new XamarinHostEnvironment(environment);
            Services.AddSingleton<IXamarinHostEnvironment>(hostEnvironment);

            return hostEnvironment;
        }
    }
}
