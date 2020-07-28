using System;
using System.Collections.Generic;
using System.Text;
using Xamarinme;

namespace XamarinmeHosting
{
    internal class XamarinHostEnvironment : IXamarinHostEnvironment
    {
        public XamarinHostEnvironment(string environment, string application, string rootPath)
        {
            Environment = environment;
            Application = application;
            RootPath = rootPath;
        }

        public string Environment { get; }

        public string Application { get; }

        public string RootPath { get; }
    }
}
