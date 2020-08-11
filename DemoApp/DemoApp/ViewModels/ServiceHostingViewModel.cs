using DemoApp.Models;
using DemoApp.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms.Xaml;
using Xamarinme;

namespace DemoApp.ViewModels
{

    public class ServiceHostingViewModel : HostingViewModel<ISampleService>
    {
        public ServiceHostingViewModel() : base(
            App.Host.Services.GetService<ISampleService>().GetSampleLogger(),
            App.Host.Services.GetService<ISampleService>().GetXamarinHostEnvironment(),
            App.Host.Services.GetService<ISampleService>().GetConfiguration(),
            App.Host.Services.GetService<ISampleService>(),
            "Service")
        {
        }
    }
}
