using DemoApp.Models;
using DemoApp.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Xamarinme;

namespace DemoApp.ViewModels
{
    public class InternalHostingViewModel
    {
        public ObservableCollection<ConfigurationItem> ConfigurationItems { get; set; }

        private readonly ILogger<InternalHostingViewModel> _logger;
        private readonly IXamarinHostEnvironment _environment;
        private readonly IConfiguration _configuration;
        private readonly ISampleService _sampleService;

        public InternalHostingViewModel()
        {
            _logger = App.Host.Services.GetRequiredService<ILogger<InternalHostingViewModel>>();
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

            foreach (var kvp in _configuration.AsEnumerable())
            {
                if (kvp.Value != null)
                {
                    ConfigurationItems.Add(new ConfigurationItem
                    {
                        Key = kvp.Key,
                        Value = kvp.Value
                    });
                }
            }
        }
    }
}
