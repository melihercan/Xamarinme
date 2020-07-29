﻿using Microsoft.Extensions.Configuration;
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

        public static XamarinHostBuilder CreateDefault(string[] args = null)
        {
            var builder = new XamarinHostBuilder(args);
            return builder;
        }

        public XamarinHost Build()
        {
            return null;
        }
        //public void ConfigureContainer<TBuilder>(IServiceProviderFactory<TBuilder> factory, Action<TBuilder> configure = null);


        internal XamarinHostBuilder(string[] args)
        {
            Configuration = new XamarinHostConfiguration();
            Services = new ServiceCollection();
            HostEnvironment = InitializeEnvironment();
            Logging = new LoggingBuilder(Services);
        }

        private XamarinHostEnvironment InitializeEnvironment()
        {
            var hostEnvironment = new XamarinHostEnvironment("Production", "MyApp", "/");
            return hostEnvironment;
        }
    }
}
