using DemoApp.Models;
using DemoApp.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarinme;

namespace DemoApp.ViewModels
{
    public class HostingViewModel
    {
        public ObservableCollection<ConfigurationItem> ConfigurationItems { get; set; }

        private readonly ILogger<HostingViewModel> _logger;
        private readonly IXamarinHostEnvironment _environment;
        private readonly IConfiguration _configuration;
        private readonly ISampleService _sampleService;

        public HostingViewModel()
        {
            _logger = App.Host.Services.GetRequiredService<ILogger<HostingViewModel>>();
            _environment = App.Host.Services.GetRequiredService<IXamarinHostEnvironment>();
            _configuration = App.Host.Services.GetRequiredService<IConfiguration>();
            _sampleService = App.Host.Services.GetService<ISampleService>();

            ConfigurationItems = new ObservableCollection<ConfigurationItem>
            {
                new ConfigurationItem
                {
                    Key = "Build",
                    Value = $"{App.Configuration["Build"]}"
                },
                new ConfigurationItem
                {
                    Key = "Logging:IncludeScopes",
                    Value = $"{App.Configuration["Logging:IncludeScopes"]}"
                },
                new ConfigurationItem
                {
                    Key = "Logging:LogLevel:Default",
                    Value = $"{App.Configuration["Logging:LogLevel:Default"]}"
                },
                new ConfigurationItem
                {
                    Key = "Logging:LogLevel:System",
                    Value = $"{App.Configuration["Logging:LogLevel:System"]}"
                },
                new ConfigurationItem
                {
                    Key = "Logging:LogLevel:Microsoft",
                    Value = $"{App.Configuration["Logging:LogLevel:Microsoft"]}"
                },
            };

        }
    }
}
