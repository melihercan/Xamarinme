using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

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


            var assembly = _assembly;//// Assembly.GetExecutingAssembly();

            foreach (var fileName in _fileNames)
            {
                var resourceFileName = /*assembly.GetName().Name*/ _defaultNamespace + ".";
                var fileFolder = Path.GetDirectoryName(fileName).ToLower();
                if (!string.IsNullOrEmpty(fileFolder))
                {
                    resourceFileName += fileFolder + ".";
                }
                resourceFileName += fileName;
                using (var readerStream = new StreamReader(assembly.GetManifestResourceStream(resourceFileName)))
                {
                    var fileExtension = Path.GetExtension(fileName).ToLower();
                    switch (fileExtension)
                    {
                        case ".json":
                            var kvList = JsonConvert.DeserializeObject<Dictionary<string, string>>
                                (readerStream.ReadToEnd());
                            foreach (var kv in kvList)
                            {
                                Data.Add(kv.Key, kv.Value);
                            }
                            break;

                        case "xml":
                            break;
                    }
                }
            }

        }
    }
}
