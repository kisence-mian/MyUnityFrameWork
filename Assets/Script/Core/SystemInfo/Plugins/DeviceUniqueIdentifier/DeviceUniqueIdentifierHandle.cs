using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 获取设备唯一ID
/// </summary>
public class DeviceUniqueIdentifierHandle
{
    private const string DUID = "deviceUniqueIdentifier";
    public static string GetUniqueIdentifier()
    {
        string id = SystemInfo.deviceUniqueIdentifier;

#if UNITY_IPHONE || UNITY_IOS
         id = Keychain.GetValue(DUID);
        if (string.IsNullOrEmpty(id))
        {
            id = SystemInfo.deviceUniqueIdentifier;
            Keychain.SetValue(DUID, id);
        }
#endif
        return id;

    }

}

