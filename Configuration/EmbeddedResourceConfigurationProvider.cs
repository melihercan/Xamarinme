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
        private readonly IEnumerable<string> _fileNames;

        public EmbeddedResourceConfigurationProvider(IEnumerable<string> fileNames)
        {
            _fileNames = fileNames;
        }

        public override void Load()
        {
            if (_fileNames == null)
            {
                return;
            }


            var assembly = Assembly.GetExecutingAssembly();

            foreach( var fileName in _fileNames)
            {
                var resourceFileName = assembly.GetName().Name + "." + _fileNames;
                using (var readerStream = new StreamReader(assembly.GetManifestResourceStream(resourceFileName)))
                {
                    var fileExtension = Path.GetExtension(fileName).ToLower();
                    switch (fileExtension)
                    {
                        case "json":
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
