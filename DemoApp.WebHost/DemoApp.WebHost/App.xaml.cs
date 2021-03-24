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

            new Thread(async () => await Xamarinme.WebHost.Main(null)).Start();
            
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
    }
}
