using Microsoft.Extensions.Configuration;
using System;
using System.Reflection;

namespace Xamarinme
{
    public static class EmbeddedResourceConfigurationExtensions
    {
        public static IConfigurationBuilder AddEmbeddedResource(this IConfigurationBuilder builder, 
            EmbeddedResourceConfigurationOptions options, string environment = "Production")
        {
            builder.Add(new EmbeddedResourceConfigurationSource 
            { 
                Options = options,
                Environment = environment
            });
            return builder;
        }


        public static IConfigurationBuilder AddEmbeddedResource(this IConfigurationBuilder builder,
            Action<EmbeddedResourceConfigurationSource> configurationSource)
                => builder.Add(configurationSource);
    }
}
