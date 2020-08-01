using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Xamarinme
{
    public class EmbeddedResourceConfigurationSource : IConfigurationSource
    {
        public EmbeddedResourceConfigurationOptions Options { get; set; }
        public string Environment { get; set; } = "Production";

        public IConfigurationProvider Build(IConfigurationBuilder builder) => 
            new EmbeddedResourceConfigurationProvider(Options, Environment);
    }
}
