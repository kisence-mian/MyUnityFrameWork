using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class SystemInfoManager
{
    /// <summary>
    /// 设备唯一ID
    /// </summary>
    public static string deviceUniqueIdentifier
    {
        get
        {
            return DeviceUniqueIdentifierHandle.GetUniqueIdentifier();
        }
    }
    public const string Device = "Device";
    /// <summary>
    /// 显示器
    /// </summary>
    public const string Display = "Display";
    /// <summary>
    /// 操作系统
    /// </summary>
    //public const string OperatingSystem = "Operating System";

    //public const string Memory = "Memory";
    public const string CPU = "CPU";
    public const string GPU = "GPU";
    //public const string Network = "Network";
    public const string Storage = "Storage";

    public static Dictionary<string, List<SystemInfoData>> systemInfos = new Dictionary<string, List<SystemInfoData>>();
    private static bool isInit = false;
    [RuntimeInitializeOnLoadMethod]
    public static void Init()
    {
        if (isInit)
            return;
        isInit = true;
        systemInfos.Clear();
        List<SystemInfoData> infos = new List<SystemInfoData>();
        systemInfos.Add(Device, infos);
        infos.Add(new SystemInfoData(Device, "UniqueIdentifier", deviceUniqueIdentifier));
        infos.Add(new SystemInfoData(Device, "DeviceName", SystemInfo.deviceName.ToString()));
        infos.Add(new SystemInfoData(Device, "DeviceModel", SystemInfo.deviceModel.ToString()));
        infos.Add(new SystemInfoData(Device, "OperatingSystem", SystemInfo.operatingSystem));
        infos.Add(new SystemInfoData(Device, "SystemLanguage", Application.systemLanguage.ToString()));
        infos.Add(new SystemInfoData(Device, "Memory", SystemInfo.systemMemorySize + "MB"));
        infos.Add(new SystemInfoData(Device, "Network", Application.internetReachability.ToString()));



        infos = new List<SystemInfoData>();
        systemInfos.Add(Display, infos);
        infos.Add(new SystemInfoData(Display, "Resolution", Screen.currentResolution.ToString()));
        infos.Add(new SystemInfoData(Display, "DPI", Screen.dpi.ToString()));
        infos.Add(new SystemInfoData(Display, "ScreenOrientation", Screen.orientation.ToString()));
        infos.Add(new SystemInfoData(Display, "ScreenTimeout", Screen.sleepTimeout.ToString()));

        //infos = new List<SystemInfoData>();
        //systemInfos.Add(Memory, infos);


        infos = new List<SystemInfoData>();
        systemInfos.Add(CPU, infos);
        infos.Add(new SystemInfoData(CPU, "ProcessorType", SystemInfo.processorType.ToString()));
        infos.Add(new SystemInfoData(CPU, "ProcessorFrequency", SystemInfo.processorFrequency + "MHz"));
        infos.Add(new SystemInfoData(CPU, "ProcessorCount", SystemInfo.processorCount.ToString()));


        infos = new List<SystemInfoData>();
        systemInfos.Add(GPU, infos);
        infos.Add(new SystemInfoData(GPU, "GraphicsDeviceName", SystemInfo.graphicsDeviceName.ToString()));
        infos.Add(new SystemInfoData(GPU, "DeviceType", SystemInfo.deviceType.ToString()));
        infos.Add(new SystemInfoData(GPU, "GraphicsMemorySize", SystemInfo.graphicsMemorySize + "MB"));
        infos.Add(new SystemInfoData(GPU, "GraphicsDeviceVersion", SystemInfo.graphicsDeviceVersion.ToString()));

        //infos = new List<SystemInfoData>();
        //systemInfos.Add(Network, infos);

        //暂时注释：在Unity2017.3.35f1上.net 4.6环境会引起停止运行时Unity崩溃，原因暂时不明
        //try
        //{
        //    infos = new List<SystemInfoData>();
        //    systemInfos.Add(Storage, infos);

        //    System.IO.DriveInfo[] drives = System.IO.DriveInfo.GetDrives();

        //    foreach (System.IO.DriveInfo drive in drives)
        //    {
        //        float totalSize = drive.TotalSize / (1024 * 1024 * 1024f);
        //        float totalFreeSpace = drive.TotalFreeSpace / (1024 * 1024 * 1024f);
        //        infos.Add(new SystemInfoData(Storage, drive.Name, totalFreeSpace.ToString("f1") + "/" + totalSize.ToString("f1") + "GB"));
        //    }
        //}
        //catch (Exception)
        //{

        //}

    }

    public static string GetTxetInfo(bool isHaveColor = false)
    {
        Init();

        StringBuilder sb = new StringBuilder();
        int index = 0;
        foreach (var item in systemInfos)
        {
            sb.Append(item.Key);
            sb.Append("\n");
            foreach (var info in item.Value)
            {
                sb.Append("\t");
                if (isHaveColor)
                {
                    sb.Append(info.name + "\t<color=green>" + info.content + "</color>");
                }
                else
                    sb.Append(info);
                if (item.Value.IndexOf(info) != item.Value.Count - 1)
                    sb.Append("\n");
            }
            if (index < systemInfos.Count - 1)
                sb.Append("\n");
            index++;
        }
        return sb.ToString();
    }


    public struct SystemInfoData
    {
        /// <summary>
        /// 分的大类
        /// </summary>
        public string type;

        public string name;
        public string content;

        public SystemInfoData(string type, string name, string content)
        {
            this.type = type;
            this.name = name;
            this.content = content;
        }

        public override string ToString()
        {
            return name + "\t" + content;
        }
    }
}