using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarinme
{
    public interface IXamarinHostEnvironment
    {
        /// <summary>
        /// Gets the name of the runningenvironment. 
        /// This is configured to use the environment of the application hosting the Xamarin application.
        /// Configured to "Production" when not specified by the host.
        /// </summary>
        string Environment { get; }
    }
}
