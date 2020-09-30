using DemoApp.Models;
using Microsoft.Extensions.Configuration;
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
            ConfigurationItems = new ObservableCollection<ConfigurationItem>(); 
            foreach (var kvp in App.Configuration.AsEnumerable())
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
