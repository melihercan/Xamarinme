using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarinme
{
    public class EmbeddedResourceConfigurationSource : IConfigurationSource
    {
        public IEnumerable<string> FileNames { get; set; }

        public IConfigurationProvider Build(IConfigurationBuilder builder) => 
            new EmbeddedResourceConfigurationProvider(FileNames);
    }
}
