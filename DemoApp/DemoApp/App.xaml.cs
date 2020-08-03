using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using DemoApp.Services;
using DemoApp.Views;
using Xamarinme;
using System.Reflection;

namespace DemoApp
{
    public partial class App : Application
    {

        public App()
        {
            InitializeXamarinHostBuilder();

            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            MainPage = new AppShell();
        }

        private void InitializeXamarinHostBuilder()
        {
            var build = XamarinHostBuilder.CreateDefault(new EmbeddedResourceConfigurationOptions 
            { 
                Assembly = Assembly.GetExecutingAssembly(),
                Prefix = "DemoApp"
            });
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
    }
}
