using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarinme
{
    internal interface IAppLifecycleStop
    {
        void Register(Action onStop);
    }
}
