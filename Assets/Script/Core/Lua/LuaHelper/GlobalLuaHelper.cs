using UnityEngine;

public class GlobalLuaHelper : MonoBehaviour {

    public static string GetSysInfo(string name)
    {
        name = name.ToLower();
        string str = "";
        if (name == "bundleidentifier") str = Application.bundleIdentifier;
        if (name == "version") str = Application.version;
        if (name == "devicemodel") str = SystemInfo.deviceModel;
        if (name == "devicename") str = SystemInfo.deviceName;
        if (name == "deviceuniqueidentifier") str = SystemInfo.deviceUniqueIdentifier;
        if (name == "graphicsdevicename") str = SystemInfo.graphicsDeviceName;
        if (name == "graphicsmemorysize") str = SystemInfo.graphicsMemorySize.ToString();
        if (name == "graphicsmultithreaded") str = SystemInfo.graphicsMultiThreaded.ToString();
        if (name == "operatingsystem") str = SystemInfo.operatingSystem;
        if (name == "processorcount") str = SystemInfo.processorCount.ToString();
        if (name == "processortype") str = SystemInfo.processorType;
        if (name == "systemmemorysize") str = SystemInfo.systemMemorySize.ToString();
        return str;
    }
}
