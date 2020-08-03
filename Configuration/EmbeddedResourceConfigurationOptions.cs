using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Xamarinme
{
    public class EmbeddedResourceConfigurationOptions
    {
        // Assembly that contains the config files.
        public Assembly Assembly { get; set; }

        // Prefix to embedded resource files.
        //  Format: <default namespace>.<nested folders that contain config files each separated with dots>
        //  Examples:
        //          default namespace: "MyApp",     config files are on root folder         => Prefix = "MyApp"
        //          default namespace: "MyApp",     config files are on "res" folder        => Prefix = "MyApp.res"
        //          default namespace: "MyApp",     config files are on "res/x/y" folder    => Prefix = "MyApp.res.x.y"
        //          default namespace: "MyApp.iOS", config files are on "res/x/y" folder    => Prefix = "MyApp.iOS.res.x.y"
        public string Prefix { get; set; }
    }
}
