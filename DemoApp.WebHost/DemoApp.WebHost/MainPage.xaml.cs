using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarinme;

namespace DemoApp.WebHost
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            Ip.Text = App.WebHostParameters.ServerIpEndpoint.Address.ToString();
            Url.Text = $"http://{Ip.Text}:{App.WebHostParameters.ServerIpEndpoint.Port}";
        }
    }
}
