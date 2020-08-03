using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarinme;

namespace DemoApp
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private readonly ILogger<MainPage> _logger;
        private readonly IXamarinHostEnvironment _environment;
        private readonly IConfiguration _configuration;

        public MainPage()
        {
            _logger = App.Host.Services.GetRequiredService<ILogger<MainPage>>();
            _environment = App.Host.Services.GetRequiredService<IXamarinHostEnvironment>();
            _configuration = App.Host.Services.GetRequiredService<IConfiguration>();

            InitializeComponent();

            _logger.LogError("========> I AM ERROR");
            _logger.LogWarning("========> I AM WARNING");
            _logger.LogInformation("========> I AM INFORMATION");
            _logger.LogDebug("========> I AM DEBUG");


            _logger.LogInformation($"---- ENVIRONMENT: {_environment.Environment}");
            _logger.LogInformation($"---- BUILD: {_configuration["Build"]}");

        }
    }
}
