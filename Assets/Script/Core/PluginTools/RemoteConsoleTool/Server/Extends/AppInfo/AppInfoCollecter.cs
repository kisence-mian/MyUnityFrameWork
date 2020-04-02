using UnityEngine;
using System.Collections;
using LiteNetLibManager;

namespace GameConsoleController
{
    public class AppInfoCollecter : ServiceBase
    {
        public const string TypeName_SystemInfo = "System Info";
        public const string TypeName_UnityInfo = "Unity Info";
        public const string TypeName_CustomInfo = "Custom Info";
        public override void OnStart()
        {
            CreateSystemInfo();
            CreateUnityInfo();
        }

        public static void AddSystemInfoValue(string label, string key, object value, string description = null)
        {
           AppInfoService.AddInfoValue(TypeName_SystemInfo, label, key, value, description);
        }
        public static void AddUnityInfoValue(string label, string key, object value, string description = null)
        {
            AppInfoService.AddInfoValue(TypeName_UnityInfo, label, key, value, description);
        }
        public static void AddCustomInfoValue(string label, string key, object value, string description = null)
        {
            AppInfoService.AddInfoValue(TypeName_CustomInfo, label, key, value, description);
        }
        private void CreateSystemInfo()
        {
            string Device = "Device";

            AddSystemInfoValue(Device, "UniqueIdentifier", SystemInfo.deviceUniqueIdentifier.ToString());
            //用户定义的设备名称。
            AddSystemInfoValue(Device, "DeviceName", SystemInfo.deviceName.ToString());
            //设备型号
            AddSystemInfoValue(Device, "DeviceModel", SystemInfo.deviceModel.ToString());
            AddSystemInfoValue(Device, "DeviceType", SystemInfo.deviceType.ToString());
            AddSystemInfoValue(Device, "Operating System", SystemInfo.operatingSystem);
            AddSystemInfoValue(Device, "System Language", Application.systemLanguage.ToString());
            AddSystemInfoValue(Device, "Memory", SystemInfo.systemMemorySize + "MB");
            AddSystemInfoValue(Device, "Network", Application.internetReachability.ToString());

            /// <summary>
            /// 显示器
            /// </summary>
            string Display = "Display";
            AddSystemInfoValue(Display, "Resolution", Screen.currentResolution.ToString());
            AddSystemInfoValue(Display, "DPI", Screen.dpi.ToString());

#if UNITY_2019_2_OR_NEWER
            AddSystemInfoValue(Display, "Brightness", Screen.brightness.ToString());
#endif
            AddSystemInfoValue(Display, "Screen Orientation", Screen.orientation.ToString());
            AddSystemInfoValue(Display, "Fullscreen", Screen.fullScreen.ToString());
            AddSystemInfoValue(Display, "Screen Timeout", Screen.sleepTimeout.ToString());

            string CPU = "CPU";
            AddSystemInfoValue(CPU, "Processor Type", SystemInfo.processorType.ToString());
            AddSystemInfoValue(CPU, "Processor Frequency", SystemInfo.processorFrequency + "MHz");
            AddSystemInfoValue(CPU, "Processor Count", SystemInfo.processorCount.ToString());

            string Graphics = "Graphics";
            AddSystemInfoValue(Graphics, "Graphics Device Name", SystemInfo.graphicsDeviceName.ToString());
            AddSystemInfoValue(Graphics, "Device Type", SystemInfo.deviceType.ToString());
            AddSystemInfoValue(Graphics, "Graphics Memory Size", SystemInfo.graphicsMemorySize + "MB");
            //显卡的唯一标识符ID。
            AddSystemInfoValue(Graphics, "Graphics Device ID", SystemInfo.graphicsDeviceID.ToString());
            AddSystemInfoValue(Graphics, "Graphics Device Version", SystemInfo.graphicsDeviceVersion.ToString());
            AddSystemInfoValue(Graphics, "Device Vendor", UnityEngine.SystemInfo.graphicsDeviceVendor);
            AddSystemInfoValue(Graphics, "Max Tex Size", UnityEngine.SystemInfo.maxTextureSize);
            AddSystemInfoValue(Graphics, "NPOT Support", UnityEngine.SystemInfo.npotSupport);
            AddSystemInfoValue(Graphics, "Sparse Textures", UnityEngine.SystemInfo.supportsSparseTextures);
            AddSystemInfoValue(Graphics, "Compute Shaders", UnityEngine.SystemInfo.supportsComputeShaders);

            string Sensor = "Sensor";
            //是否支持用户触摸震动反馈
            AddSystemInfoValue(Sensor, "Vibration", SystemInfo.supportsVibration.ToString());
            AddSystemInfoValue(Sensor, "Location Service", SystemInfo.supportsLocationService.ToString());
            AddSystemInfoValue(Sensor, "Motion Vectors", SystemInfo.supportsMotionVectors.ToString());

            AddSystemInfoValue(Sensor, "Accelerometer", UnityEngine.SystemInfo.supportsAccelerometer);
            AddSystemInfoValue(Sensor, "Gyroscope", UnityEngine.SystemInfo.supportsGyroscope);


#if UNITY_IOS
             string IOSInfo = "IOS Infomation";
#if UNITY_5 || UNITY_5_3_OR_NEWER
            AddSystemInfoValue(IOSInfo, "System Version", UnityEngine.iOS.Device.systemVersion.ToString());
            AddSystemInfoValue(IOSInfo, "Generation", UnityEngine.iOS.Device.generation.ToString());
            AddSystemInfoValue(IOSInfo, "Vendor Identifier", UnityEngine.iOS.Device.vendorIdentifier.ToString());
            AddSystemInfoValue(IOSInfo, "Advertising Identifier", UnityEngine.iOS.Device.advertisingIdentifier.ToString());
            AddSystemInfoValue(IOSInfo, "Advertising Tracking Enabled", UnityEngine.iOS.Device.advertisingTrackingEnabled.ToString());
            AddSystemInfoValue(IOSInfo, "Hide Home Button", UnityEngine.iOS.Device.hideHomeButton.ToString());
#if UNITY_2019_2_OR_NEWER
            AddSystemInfoValue(IOSInfo, "Low Power Mode Enabled", UnityEngine.iOS.Device.lowPowerModeEnabled.ToString());
            AddSystemInfoValue(IOSInfo, "Wants Software Dimming", UnityEngine.iOS.Device.wantsSoftwareDimming.ToString());
#endif
            AddSystemInfoValue(IOSInfo, "Defer System Gestures Mode", UnityEngine.iOS.Device.deferSystemGesturesMode.ToString());
#else
            AddSystemInfoValue(IOSInfo, "Generation", iPhone.generation.ToString());
            AddSystemInfoValue(IOSInfo, "Vendor Identifier", iPhone.vendorIdentifier.ToString());
            AddSystemInfoValue(IOSInfo, "Advertising Identifier", iPhone.advertisingIdentifier.ToString());
            AddSystemInfoValue(IOSInfo, "Advertising Tracking Enabled", iPhone.advertisingTrackingEnabled.ToString());
#endif

#endif
            // AddSystemInfoValue(IOSInfo, "Hide Home Button", UnityEngine.iOS.Device.hideHomeButton.ToString());

        }
        private void CreateUnityInfo()
        {
            string Unity = "Unity";
            /// <summary>
            /// 显示器
            /// </summary>
            string App = "App";

            AddUnityInfoValue(Unity, "Unity Pro", Application.HasProLicense().ToString());
            AddUnityInfoValue(Unity, "Unity Version", Application.unityVersion.ToString());
            AddUnityInfoValue(Unity, "Absolute URL", Application.absoluteURL.ToString());
            AddUnityInfoValue(Unity, "Temporary Cache Path", Application.temporaryCachePath.ToString());
            AddUnityInfoValue(Unity, "Persistent Data Path", Application.persistentDataPath.ToString());
            AddUnityInfoValue(Unity, "Streaming Assets Path", Application.streamingAssetsPath);
            AddUnityInfoValue(Unity, "Data Path", Application.dataPath.ToString());
            AddUnityInfoValue(Unity, "Installer Name", Application.installerName);
            AddUnityInfoValue(Unity, "Target Frame Rate", Application.targetFrameRate.ToString());
            AddUnityInfoValue(Unity, "BuildGUID", Application.buildGUID.ToString());

#if ENABLE_IL2CPP
            const string IL2CPP = "Yes";
#else
            const string IL2CPP = "No";
#endif
            AddUnityInfoValue(Unity, "IL2CPP", IL2CPP);

            AddUnityInfoValue(App, "UnityRemoteConsole Version", VersionInfo.Version);
            AddUnityInfoValue(App, "Product Name", Application.productName.ToString());
            AddUnityInfoValue(App, "CompanyName", Application.companyName.ToString());
            AddUnityInfoValue(App, "BundleIdentifier", Application.identifier.ToString());
            AddUnityInfoValue(App, "Version", Application.version.ToString());
           

#if UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
             AddUnityInfoValue(App, "Current Level", Application.loadedLevelName);
#else
            var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            AddUnityInfoValue(App, "Current Level", string.Format("{0} (Index: {1})", activeScene.name, activeScene.buildIndex));
#endif

            AddUnityInfoValue(App, "Quality Level", QualitySettings.names[QualitySettings.GetQualityLevel()] + " (" + QualitySettings.GetQualityLevel() + ")");
            AddUnityInfoValue(App, "AntiAliasing", QualitySettings.antiAliasing.ToString());

            //AddUnityInfoValue(CPU, "Processor Type", SystemInfo.processorType.ToString());
            //AddUnityInfoValue(CPU, "Processor Frequency", SystemInfo.processorFrequency + "MHz");
            //AddUnityInfoValue(CPU, "Processor Count", SystemInfo.processorCount.ToString());

            //AddUnityInfoValue(GPU, "Graphics Device Name", SystemInfo.graphicsDeviceName.ToString());
            //AddUnityInfoValue(GPU, "DeviceType", SystemInfo.deviceType.ToString());
            //AddUnityInfoValue(GPU, "Graphics Memory Size", SystemInfo.graphicsMemorySize + "MB");
            ////显卡的唯一标识符ID。
            //AddUnityInfoValue(GPU, "Graphics Device ID", SystemInfo.graphicsDeviceID.ToString());
            //AddUnityInfoValue(GPU, "Graphics Device Version", SystemInfo.graphicsDeviceVersion.ToString());
            // SceneManager
        }
    }
}
