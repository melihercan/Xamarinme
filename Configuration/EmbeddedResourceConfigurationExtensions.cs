using Microsoft.Extensions.Configuration;
using System;
using System.Reflection;

namespace Xamarinme
{
    public static class EmbeddedResourceConfigurationExtensions
    {
        public static IConfigurationBuilder AddEmbeddedResource(this IConfigurationBuilder builder, 
            Assembly assembly, string defaultNamespace, string[] fileNames)
        {
            builder.Add(new EmbeddedResourceConfigurationSource 
            { 
                Assembly = assembly, 
                DefaultNamespace = defaultNamespace,
                FileNames = fileNames 
            });
            return builder;
        }

        public static IConfigurationBuilder AddEmbeddedResource(this IConfigurationBuilder builder,
            Action<EmbeddedResourceConfigurationSource> configureSource)
                => builder.Add(configureSource);
    }
}
