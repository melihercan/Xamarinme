using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using XamarinmeConfiguration;

namespace Xamarinme
{
    public class EmbeddedResourceConfigurationProvider : ConfigurationProvider
    {
        private readonly Assembly _assembly;
        private readonly string _defaultNamespace;
        private readonly IEnumerable<string> _fileNames;

        public EmbeddedResourceConfigurationProvider(Assembly assembly, string defaultNamespace, IEnumerable<string> fileNames)
        {
            _assembly = assembly;
            _defaultNamespace = defaultNamespace;
            _fileNames = fileNames;
        }

        public override void Load()
        {
            if (_fileNames == null)
            {
                return;
            }

            foreach (var fullFileName in _fileNames)
            {
                var fileFolder = Path.GetDirectoryName(fullFileName).ToLower().Replace('/', '.').Replace('\\', '.');
                var fileName = Path.GetFileName(fullFileName).ToLower();
                var fileExtension = Path.GetExtension(fullFileName).ToLower();

                var resourceFileName = _defaultNamespace + ".";
                if (!string.IsNullOrEmpty(fileFolder))
                {
                    resourceFileName += fileFolder + ".";
                }
                resourceFileName += fileName;

                using (var stream = _assembly.GetManifestResourceStream(resourceFileName))
                {
                    switch (fileExtension)
                    {
                        case ".json":
                            var kvList = JsonConfigurationFileParser.Parse(stream);
                            foreach (var kv in kvList)
                            {
                                Data.Add(kv.Key, kv.Value);
                            }
                            break;

                        case ".xml":
                            break;
                    }
                }
            }

        }
    }
}
