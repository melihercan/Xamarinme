using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarinme;

namespace DemoApp.WebHost
{
    public partial class App : Application
    {
        public static IWebHost Host { get; set; }

        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();

            new Thread(async () =>
            {
                try
                {
                    await WebHost.Program.Main(null);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"######## EXCEPTION: {ex.Message}");
                }
            }).Start();
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
