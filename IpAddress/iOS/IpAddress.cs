using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Net.Sockets;


namespace Xamarinme
{
    class IpAddress : IIpAddress
    {
        public string Get()
        {
            foreach (var netInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (netInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211
                    || netInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet
                    )
                {
                    foreach (var addrInfo in netInterface.GetIPProperties().UnicastAddresses)
                    {
                        if (addrInfo.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            var ipAddress = addrInfo.Address.ToString();
                            if (!ipAddress.StartsWith("169"))
                            {
                                return ipAddress;
                            }
                        }
                    }
                }
            }

            return string.Empty;
        }

        public IEnumerable<string> GetAll()
        {
            var ipAddresses = new List<string>();

            foreach (var netInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (netInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                    netInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    foreach (var addrInfo in netInterface.GetIPProperties().UnicastAddresses)
                    {
                        if (addrInfo.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            var ipAddress = addrInfo.Address.ToString();
                            ipAddresses.Add(ipAddress);
                        }
                    }
                }
            }

            return ipAddresses;
        }
    }
}
