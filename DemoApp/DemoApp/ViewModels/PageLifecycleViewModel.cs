using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarinme;

namespace DemoApp.ViewModels
{
    public class PageLifecycleViewModel : IPageLifecycle
    {
        public Task OnPageAppearing()
        {
            return Task.CompletedTask;
        }

        public Task OnPageDisappearing()
        {
            return Task.CompletedTask;
        }
    }
}
