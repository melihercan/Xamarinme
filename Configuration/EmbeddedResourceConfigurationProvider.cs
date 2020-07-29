using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarinme
{
    public class EmbeddedResourceConfigurationProvider : ConfigurationProvider
    {
        private readonly IEnumerable<string> _fileNames;

        public EmbeddedResourceConfigurationProvider(IEnumerable<string> fileNames = null)
        {
            _fileNames = fileNames ?? new string[] { "appsettings.json" };

        }
    }
}
