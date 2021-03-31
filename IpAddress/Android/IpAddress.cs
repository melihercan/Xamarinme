using Android.Net.Wifi;
using System;
using System.Collections.Generic;

namespace Xamarinme
{
    class IpAddress : IIpAddress
    {
        public string Get()
        {
            WifiManager wifiManager = (WifiManager)Android.App.Application.Context.GetSystemService(
                Android.Content.Context.WifiService);
            WifiInfo info = wifiManager.ConnectionInfo;
            var ipAsInt = info.IpAddress;
            if (ipAsInt != 0)
            {
                var ipAddress = String.Format("{0:d}.{1:d}.{2:d}.{3:d}",
                    (ipAsInt & 0xff),
                    (ipAsInt >> 8 & 0xff),
                    (ipAsInt >> 16 & 0xff),
                    (ipAsInt >> 24 & 0xff));

                return ipAddress;
            }

            return string.Empty;
        }

        public IEnumerable<string> GetAll()
        {
            throw new NotImplementedException();
        }
    }
}
