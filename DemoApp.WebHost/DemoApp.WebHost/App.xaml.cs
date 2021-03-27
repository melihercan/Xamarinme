using System;
using System.Threading;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarinme;

namespace DemoApp.WebHost
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();

            new Thread(async () =>
            {
                try
                {
                    await Xamarinme.WebHost.Main(null);
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
