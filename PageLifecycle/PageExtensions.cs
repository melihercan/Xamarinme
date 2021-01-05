using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Xamarinme
{
    public static class PageExtensions
    {
        public static Task OnPageAppearing(this Page page, object viewModel)
        {
            if (viewModel is IPageLifecycle)
            {
                IPageLifecycle pageLifecycle = viewModel as IPageLifecycle;
                return pageLifecycle.OnPageAppearing();
            }

            return Task.CompletedTask;
        }

        public static Task OnPageDisappearing(this Page page, object viewModel)
        {
            if (viewModel is IPageLifecycle)
            {
                IPageLifecycle pageLifecycle = viewModel as IPageLifecycle;
                return pageLifecycle.OnPageDisappearing();
            }

            return Task.CompletedTask;
        }

    }
}
