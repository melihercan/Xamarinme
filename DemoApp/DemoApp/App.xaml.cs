using Microsoft.Extensions.Hosting;
using System;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarinme;

namespace DemoApp
{
    public partial class App : Application
    {
        public static IHost Host { get; private set; }

        public App()
        {
            InitializeXamarinHostBuilder();

            InitializeComponent();

            MainPage = new MainPage();
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

        private void InitializeXamarinHostBuilder()
        {
            var hostBuilder = XamarinHostBuilder.CreateDefault(new EmbeddedResourceConfigurationOptions
            {
                Assembly = Assembly.GetExecutingAssembly(),
                Prefix = "DemoApp"
            });

            Host = hostBuilder.Build();
        }
    }
}
