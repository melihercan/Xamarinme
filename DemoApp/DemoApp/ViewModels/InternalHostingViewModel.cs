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
    public class InternalHostingViewModel : HostingViewModel<InternalHostingViewModel>
    {
        public InternalHostingViewModel() : base(
            App.Host.Services.GetRequiredService<ILogger<InternalHostingViewModel>>(),
            App.Host.Services.GetRequiredService<IXamarinHostEnvironment>(),
            App.Host.Services.GetRequiredService<IConfiguration>(),
            App.Host.Services.GetService<ISampleService>(),
            "View Model")
        {
        }
    }
}
