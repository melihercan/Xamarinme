using DemoApp.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarinme;

namespace DemoApp.Services
{
    public class SampleService : ISampleService
    {
        public IEnumerable<ConfigurationItem> GetConfigurationItems()
        {
            throw new NotImplementedException();
        }

        public ILogger<SampleService> GetSampleLogger()
        {
            throw new NotImplementedException();
        }

        public IXamarinHostEnvironment GetXamarinHostEnvironment()
        {
            throw new NotImplementedException();
        }
    }
}
