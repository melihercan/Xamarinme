using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Xamarinme
{
    public static class AppLifecycleStopExtension
    {
        public static void Register(this Application application, Action onStop)
        {
            var cross = CrossAppLifecycleStop.Current;
            cross.Register(onStop);
        }
    }
}
