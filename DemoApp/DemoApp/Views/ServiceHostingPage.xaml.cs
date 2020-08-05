using DemoApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DemoApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ServiceHostingPage : ContentPage
    {
        public ServiceHostingPage()
        {
            InitializeComponent();
            BindingContext = new ServiceHostingViewModel();
        }
    }
}