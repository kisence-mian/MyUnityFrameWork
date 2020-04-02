using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System;

namespace GameConsoleController {
    public class GameConsolePanelSettingConfig
    {

        public int netPort = 9123;
        //boot
        public bool autoBoot = true;
        /// <summary>
        /// 当第一次触发自定义启动后下一次自动启动
        /// </summary>
        public bool whenFirstCustomBootThenAutoBoot = true;
        public string KeyboardBoot = "F12";
        public int tapCount = 40;
        //login
        public string loginKey = "123456";
        public string loginPassword = "123456";

        

        public const string FileName = "GameConsolePanelSettingConfig";
        private static GameConsolePanelSettingConfig configData;

        private const string KeyBase64 = "cmMyOWpKM0NQVGxmRmhlSHFSQXd3SWdRbEkweVJEWEJ3ZnduUmJ3TFNGR1R6RzFMNjRudzVBUzdYRHowdWVLbFZBSkFMUFJwNE4zR2JuMTVxRFR6eEJnS21Rcm1EVTJOVTRYTVhSWkZDWlJHaG02Sm1UaUZteU1zNFl6WDlEQTg=";
        public static GameConsolePanelSettingConfig GetCofig()
        {
            if (Application.isPlaying)
            {
                if (configData != null)
                    return configData;
            }

            TextAsset textAsset = Resources.Load<TextAsset>(FileName);
            if (textAsset == null || string.IsNullOrEmpty(textAsset.text))
            {
                configData= new GameConsolePanelSettingConfig();
            }
            else
            {
                string json = textAsset.text;
                try
                {
                  
                    byte[] keyBytes = Convert.FromBase64String(KeyBase64);
                    string _aesKeyStr = Encoding.UTF8.GetString(keyBytes);
                    json = AESUtils.AESDecrypt(json, _aesKeyStr);

                  
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
                configData = SimpleJsonUtils.FromJson<GameConsolePanelSettingConfig>(json);
                if (configData == null)
                {
                    configData = new GameConsolePanelSettingConfig();
                }
            }

            return configData;
        }
        private const string SavePathDir = "Assets/GameConsolePanel/Resources/";
        public static void SaveConfig(GameConsolePanelSettingConfig config)
        {
            string json = SimpleJsonUtils.ToJson(config);
            byte[] keyBytes = Convert.FromBase64String(KeyBase64);
            string _aesKeyStr = Encoding.UTF8.GetString(keyBytes);
            json = AESUtils.AESEncrypt(json, _aesKeyStr);
            CreateTextFile(SavePathDir + GameConsolePanelSettingConfig.FileName + ".txt", json);
        }

        /// <summary>
        /// 保存文件数据
        /// </summary>
        /// <param name="path">全路径</param>
        /// <param name="_data">数据</param>
        /// <returns></returns>
        public static bool CreateTextFile(string path, string _data)
        {

            byte[] dataByte = Encoding.GetEncoding("UTF-8").GetBytes(_data);

            return CreateFile(path, dataByte);
        }
        public static bool CreateFile(string path, byte[] _data)
        {
            if (string.IsNullOrEmpty(path))
                return false;
            string temp = Path.GetDirectoryName(path);
            if (!Directory.Exists(temp))
            {
                Directory.CreateDirectory(temp);
            }

            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                FileStream stream = new FileStream(path, FileMode.OpenOrCreate);
                stream.Write(_data, 0, _data.Length);
                stream.Close();

                Debug.Log("File written: " + path);
            }
            catch (Exception e)
            {
                Debug.LogError("File written fail: " + path + "  ---:" + e);
                return false;
            }

            return true;
        }
    }
}
