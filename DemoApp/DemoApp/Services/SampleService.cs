using DemoApp.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarinme;

namespace DemoApp.Services
{
    public class SampleService : ISampleService
    {
        private readonly ILogger<SampleService> _logger;
        private readonly IXamarinHostEnvironment _environment;
        private readonly IConfiguration _configuration;

        public SampleService(ILogger<SampleService> logger, IXamarinHostEnvironment environment, IConfiguration configuration)
        {
            _logger = logger;
            _environment = environment;
            _configuration = configuration;
        }

        public ILogger<SampleService> GetSampleLogger() => _logger;

        public IXamarinHostEnvironment GetXamarinHostEnvironment() => _environment;

        public IConfiguration GetConfiguration() => _configuration;
    }
}
