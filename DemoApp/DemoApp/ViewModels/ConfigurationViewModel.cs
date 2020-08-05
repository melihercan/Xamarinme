using DemoApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace DemoApp.ViewModels
{
    public class ConfigurationViewModel
    {
        public ObservableCollection<ConfigurationItem> ConfigurationItems { get; set; }

        public ConfigurationViewModel()
        {
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
