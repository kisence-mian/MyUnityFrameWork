using UnityEngine;
using System.Collections;
using System.Net;
using System.Text;

namespace LiteNetLibManager
{
    public class RemoteTagetInfo
    {
        public RemoteDeviceInfo info;

        public IPAddress address;
        public IPAddress addressV6;
        public int port;
        //public IPEndPoint endPoint;

        public float timeOut = 1.5f;

        public RemoteTagetInfo(RemoteDeviceInfo info)
        {
            this.info = info;
            //this.endPoint = endPoint;
        }

        public IPAddress GetIPAddress()
        {
            if (address != null)
                return address;
            else
                return addressV6;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if(obj is RemoteTagetInfo)
            {
                RemoteTagetInfo other = (RemoteTagetInfo)obj;
                if ( info.Equals(other.info) && port == other.port)
                    return true;
            }
            return false;
        }

        public override string ToString()
        {
            return "Name:"+info.appName+" Ip:"+ address+","+ addressV6+" port:"+port;
        }

    }

    public class RemoteDeviceInfo
    {
        public string appName;
        public string appVersion;
        /// <summary>
        /// eg.Tom's iPhone
        /// </summary>
        public string deviceName;
        /// <summary>
        /// eg. iPhone 7,1
        /// </summary>
        public string deviceModel;

        public RuntimePlatform platform;

         
        public override int GetHashCode()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(appName);
            builder.Append(appVersion);
            builder.Append(deviceName);
            builder.Append(deviceModel);
            builder.Append(platform);
            return builder.ToString().GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (obj is RemoteDeviceInfo)
            {
                RemoteDeviceInfo other = (RemoteDeviceInfo)obj;
                if (appName ==other. appName &&
                    appVersion == other.appVersion&&
                    deviceName==other.deviceName&&
                    deviceModel==other.deviceModel&&
                    platform == other.platform)
                    return true;
            }
            return false;
        }

        public static RemoteDeviceInfo GetLocalDeviceInfo()
        {
            RemoteDeviceInfo deviceInfo = new RemoteDeviceInfo();
            deviceInfo.appName = Application.productName;
            deviceInfo.appVersion = Application.version;
            deviceInfo.deviceModel = SystemInfo.deviceModel;
            deviceInfo.deviceName = SystemInfo.deviceName;
            deviceInfo.platform = Application.platform;

            return deviceInfo;
        }
    }
}
