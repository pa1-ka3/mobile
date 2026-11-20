using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace PhotoQwrekster
{
    public class NetworkChecker
    {
        private static readonly NetworkChecker _instance = new NetworkChecker();
        private NetworkChecker(){}
        public static NetworkChecker Instance
        {
            get
            {
                return _instance;
            }
        }

        public static bool CheckInternetConnection()
        {
            if (!CheckNetworkAbailable())
            {
                return false;
            }
            return true;
        }
                
        private static bool CheckNetworkAbailable()
        {
            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType net = Microsoft.Phone.Net.NetworkInformation.NetworkInterface.NetworkInterfaceType;
                switch (net)
                {
                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.Ethernet:
                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.Wireless80211:
                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.MobileBroadbandCdma:
                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.MobileBroadbandGsm:
                        // network is present
                        return true;
                        

                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.None:
                        // no network
                        return false;
                        
                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.AsymmetricDsl:
                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.Atm:
                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.BasicIsdn:
                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.Ethernet3Megabit:
                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.FastEthernetFx:
                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.FastEthernetT:
                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.Fddi:
                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.GenericModem:
                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.GigabitEthernet:
                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.HighPerformanceSerialBus:
                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.IPOverAtm:
                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.Isdn:
                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.Loopback:
                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.MultiRateSymmetricDsl:
                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.Ppp:
                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.PrimaryIsdn:
                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.RateAdaptDsl:
                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.Slip:
                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.SymmetricDsl:
                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.TokenRing:
                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.Tunnel:
                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.Unknown:
                    case Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.VeryHighSpeedDsl:
                    default://theoretically we can't get here but better be prepared
                        // connection present
                        return true;
                        
                }
                
            }
            return false;
        }

               

    }
}
