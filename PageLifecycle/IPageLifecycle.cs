using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Xamarinme
{
    public interface IPageLifecycle
    {
        Task OnPageAppearing();

        Task OnPageDisappearing();

    }
}
