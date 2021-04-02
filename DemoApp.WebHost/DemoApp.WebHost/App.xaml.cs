using DemoApp.WebHost.KestrelWebHost;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Net;
using System.Threading;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarinme;

namespace DemoApp.WebHost
{
    public partial class App : Application
    {
        public static IWebHost Host { get; set; }
        public static WebHostParameters WebHostParameters { get; set; } = new WebHostParameters();

        public App()
        {
            WebHostParameters.ServerIpEndpoint = new IPEndPoint(NetworkHelper.GetIpAddress(), 5000);
            
            InitializeComponent();
            MainPage = new MainPage();

            new Thread(async () =>
            {
                try
                {
                    await Program.Main(WebHostParameters);
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
