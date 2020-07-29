using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Xamarinme
{
    public class EmbeddedResourceConfigurationSource : IConfigurationSource
    {
        public Assembly Assembly { get; set; }
        public string DefaultNamespace { get; set; }
        public IEnumerable<string> FileNames { get; set; }

        public IConfigurationProvider Build(IConfigurationBuilder builder) => 
            new EmbeddedResourceConfigurationProvider(Assembly, DefaultNamespace, FileNames);
    }
}
