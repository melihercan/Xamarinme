using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Xamarin.Essentials
{
    public partial class FileSystem
    {
        /// <summary>
        /// Extension method for Xamarin.Essentials to open a file from embedded resource.
        /// </summary>
        /// <param name="prefix">
        ///     Prefix to embedded resource file.
        ///     Format: <default namespace>.<nested folders that contains the file each separated with a dot>
        ///     Examples:
        ///          default namespace: "MyApp",     file is on root folder         => Prefix = "MyApp"
        ///          default namespace: "MyApp",     file is on "res" folder        => Prefix = "MyApp.res"
        ///          default namespace: "MyApp",     file is on "res/x/y" folder    => Prefix = "MyApp.res.x.y"
        ///          default namespace: "MyApp.iOS", file is on "res/x/y" folder    => Prefix = "MyApp.iOS.res.x.y"
        /// </param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static Stream OpenEmbeddedResourceFile(string prefix, string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var name = $"{prefix}.{fileName}";
            var stream = assembly.GetManifestResourceStream(name);
            return stream;
        }
    }
}
