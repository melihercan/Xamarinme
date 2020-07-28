using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

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
            return null;
        }

        public XamarinHost Build()
        {
            return null;
        }
        //public void ConfigureContainer<TBuilder>(IServiceProviderFactory<TBuilder> factory, Action<TBuilder> configure = null);
    }
}
