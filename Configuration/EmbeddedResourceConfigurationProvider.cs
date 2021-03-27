using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using XamarinmeConfiguration;

namespace Xamarinme
{
    public class EmbeddedResourceConfigurationProvider : ConfigurationProvider
    {
        private readonly EmbeddedResourceConfigurationOptions _options;
        private readonly string _environment;

        public EmbeddedResourceConfigurationProvider(EmbeddedResourceConfigurationOptions options, string environment)
        {
            _options = options;
            _environment = environment;
        }

        public override void Load()
        {
            var configFiles = new[]
            {
                $"{_options.Prefix}.appsettings.json",
                $"{_options.Prefix}.appsettings.{_environment}.json"
            };

            ////var resources= _options.Assembly.GetManifestResourceNames();

            foreach (var configFile in configFiles)
            {
                if (_options.Assembly.GetManifestResourceInfo(configFile) != null)
                {
                    using (var stream = _options.Assembly.GetManifestResourceStream(configFile))
                    {
                        var kvList = JsonConfigurationFileParser.Parse(stream);
                        foreach (var kv in kvList)
                        {
                            if (Data.ContainsKey(kv.Key))
                            {
                                Data[kv.Key] = kv.Value;
                            }
                            else
                            {
                                Data.Add(kv.Key, kv.Value);
                            }
                        }
                    }
                }
            }
        }
    }
}
