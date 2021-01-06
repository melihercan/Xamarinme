using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DemoAppNfc
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NfcPage();
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
