using DemoApp.Models;
using DemoApp.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarinme;

namespace DemoApp.ViewModels
{
    public class HostingViewModel<T> : INotifyPropertyChanged where T: class 
    {
        public ObservableCollection<ConfigurationItem> ConfigurationItems { get; set; }

        private readonly ILogger<T> _logger;
        private readonly IXamarinHostEnvironment _environment;
        private readonly IConfiguration _configuration;
        private readonly ISampleService _sampleService;

        public event PropertyChangedEventHandler PropertyChanged;

        private string environment;
        public string Environment
        {
            get { return environment; }
            set 
            { 
                environment = value; 
                OnPropertyChanged(); 
            }
        }

        private string _derivedClass;
        public string DerivedClass
        {
            get { return _derivedClass; }
            set 
            {
                _derivedClass = value;
                OnPropertyChanged();
            }
        }


        void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public HostingViewModel(ILogger<T> logger, IXamarinHostEnvironment environment, 
            IConfiguration configuration, ISampleService sampleService, string derivedClass)
        {
            _logger = logger;
            _environment = environment;
            _configuration = configuration;
            _sampleService = sampleService;

            Environment = _environment.Environment;
            DerivedClass = derivedClass;

            ConfigurationItems = new ObservableCollection<ConfigurationItem>();
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
