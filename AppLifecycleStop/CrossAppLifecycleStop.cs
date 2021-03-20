using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarinme
{
    internal static class CrossAppLifecycleStop
    {
        private static Lazy<IAppLifecycleStop> stop = new Lazy<IAppLifecycleStop>(() => CreateAppLifecycleStop());

        public static IAppLifecycleStop Current
        {
            get
            {
                IAppLifecycleStop ret = stop.Value;
                if (ret == null)
                {
                    throw new NotImplementedException("This platform has no IAppLifecycleStop implementation.");
                }
                return ret;
            }
        }

        private static IAppLifecycleStop CreateAppLifecycleStop()
        {
#if NETSTANDARD2_0
            return null;
#else
            return new AppLifecycleStop();
#endif
        }

    }
}
