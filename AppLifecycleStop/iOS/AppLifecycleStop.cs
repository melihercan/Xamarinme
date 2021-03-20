using Foundation;
using System;
using System.Collections.Generic;
using System.Text;
using UIKit;

namespace Xamarinme
{
    internal class AppLifecycleStop : IAppLifecycleStop, IDisposable
    {
        private NSObject _notification;

        public void Register(Action onStop)
        {
            _notification = UIApplication.Notifications.ObserveWillTerminate( (s,e) => onStop());
            //UIApplication.Notifications.ObserveDidEnterBackground(DidEnterBackground);
        }

        //        private void DidEnterBackground(object sender, NSNotificationEventArgs e)
        //      {
        //    }


        public void Dispose() => _notification?.Dispose();

    }
}
