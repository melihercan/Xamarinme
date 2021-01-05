using DemoApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarinme;

namespace DemoApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageLifecyclePage : ContentPage
    {
        private readonly PageLifecycleViewModel _pageLifecycleViewModel;

        public PageLifecyclePage()
        {
            InitializeComponent();
            _pageLifecycleViewModel = new PageLifecycleViewModel();
            BindingContext = _pageLifecycleViewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await this.OnPageAppearing(_pageLifecycleViewModel);
        }

        protected override async void OnDisappearing()
        {
            base.OnDisappearing();
            await this.OnPageDisappearing(_pageLifecycleViewModel);
        }
    }
}