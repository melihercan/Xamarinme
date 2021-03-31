using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarinme
{
    public static class CrossIpAddress
    {
        private static Lazy<IIpAddress> ipAddress = new Lazy<IIpAddress>(() => CreateIpAddress());

        public static IIpAddress Current
        {
            get
            {
                IIpAddress ret = ipAddress.Value;
                if (ret == null)
                {
                    throw new NotImplementedException("This platform has no Nfc implementation.");
                }
                return ret;
            }
        }

        private static IIpAddress CreateIpAddress()
        {
#if NETSTANDARD2_0
            return null;
#else
            return new IpAddress();
#endif
        }
    }
}
