using DemoApp.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarinme;

namespace DemoApp.Services
{
    public interface ISampleService
    {
        ILogger<SampleService> GetSampleLogger();
        IXamarinHostEnvironment GetXamarinHostEnvironment();
        IConfiguration GetConfiguration();
    }
}
