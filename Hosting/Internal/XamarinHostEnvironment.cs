using System;
using System.Collections.Generic;
using System.Text;
using Xamarinme;

namespace XamarinmeHosting
{
    internal class XamarinHostEnvironment : IXamarinHostEnvironment
    {
        public XamarinHostEnvironment(string environment)
        {
            Environment = environment;
        }

        public string Environment { get; }
    }
}
