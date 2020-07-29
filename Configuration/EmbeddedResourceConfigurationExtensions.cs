using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarinme
{
    public static class EmbeddedResourceConfigurationExtensions
    {
        public static IConfigurationBuilder AddEmbeddedResource(this IConfigurationBuilder builder, 
            string[] fileNames)
        {
            builder.Add(new EmbeddedResourceConfigurationSource { FileNames = fileNames });
            return builder;
        }

        public static IConfigurationBuilder AddEmbeddedResource(this IConfigurationBuilder builder,
            Action<EmbeddedResourceConfigurationSource> configureSource)
                => builder.Add(configureSource);
    }
}
