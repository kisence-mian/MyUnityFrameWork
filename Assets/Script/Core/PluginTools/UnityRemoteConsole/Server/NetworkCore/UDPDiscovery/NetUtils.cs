using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SimpleNetManager
{
    public class NetworkInfo
    {
        public string IPAddress { get; set; }
        public string SubnetMask { get; set; }
    }

    public  class NetUtils
    {
        public static NetworkInfo[] GetAllLocalNetworks(NetworkInterfaceType networkInterfaceType)
        {
            var networkInfos = new List<NetworkInfo>();
            foreach (var networkInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (networkInterface.NetworkInterfaceType == networkInterfaceType && networkInterface.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (var ip in networkInterface.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            networkInfos.Add(new NetworkInfo
                            {
                                IPAddress = ip.Address.ToString(),
                                SubnetMask = ip.IPv4Mask.ToString()
                            });
                        }
                    }
                }
            }
            return networkInfos.ToArray();
        }
        public static NetworkInfo[] GetAllLocalNetworks()
        {
            var networkInfos = new List<NetworkInfo>();
            foreach (var networkInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Loopback ||
                       networkInterface.OperationalStatus != OperationalStatus.Up)
                    continue;

                foreach (var ip in networkInterface.GetIPProperties().UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        networkInfos.Add(new NetworkInfo
                        {
                            IPAddress = ip.Address.ToString(),
                            SubnetMask = ip.IPv4Mask.ToString()
                        });
                    }
                }

            }
            return networkInfos.ToArray();
        }
    }
}
