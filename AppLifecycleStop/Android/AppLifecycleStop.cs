using Android.App;
using Android.OS;
using Java.Interop;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Xamarinme
{
    class AppLifecycleStop : IDisposable, IAppLifecycleStop
    {
        private Action _onStop;

        public AppLifecycleStop()
        {
            Platform.ActivityStateChanged += Platform_ActivityStateChanged;
        }


        public void Dispose()
        {
            Platform.ActivityStateChanged -= Platform_ActivityStateChanged;
        }

        public void Register(Action onStop) => _onStop = onStop;


        private void Platform_ActivityStateChanged(object sender, ActivityStateChangedEventArgs e)
        {
            if (e.State == ActivityState.Stopped)
            {
                var x = "m";
            }
            
            if (e.State == ActivityState.Destroyed)
            {
                _onStop?.Invoke();
                Dispose();
            }
        }

    }
}
