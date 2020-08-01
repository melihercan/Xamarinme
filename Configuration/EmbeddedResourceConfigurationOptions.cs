using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Xamarinme
{
    public class EmbeddedResourceConfigurationOptions
    {
        public Assembly Assembly { get; set; }

        public string Prefix { get; set; }
    }
}
