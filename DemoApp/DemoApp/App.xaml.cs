using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using DemoApp.Services;
using DemoApp.Views;
using Microsoft.Extensions.Hosting;
using Xamarinme;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DemoApp
{
    public partial class App : Application
    {
        public static IConfiguration Configuration { get; private set; }
        public static IHost Host { get; private set; }


        public App()
        {
            InitializeXamarinConfiguration();
            InitializeXamarinHostBuilder();

            InitializeComponent();
            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        void OnStop()
        {

        }


        private void InitializeXamarinConfiguration()
        {
            Configuration = new ConfigurationBuilder()
            .AddEmbeddedResource(new EmbeddedResourceConfigurationOptions 
            { 
                Assembly = Assembly.GetExecutingAssembly(), 
                Prefix = "DemoApp" 
            })
            .Build();
        }

        private void InitializeXamarinHostBuilder()
        {
            var hostBuilder = XamarinHostBuilder.CreateDefault(new EmbeddedResourceConfigurationOptions
            {
                Assembly = Assembly.GetExecutingAssembly(),
                Prefix = "DemoApp"
            });

            hostBuilder.Services.AddSingleton<ISampleService, SampleService>();

            Host = hostBuilder.Build();
        }
    }
}
