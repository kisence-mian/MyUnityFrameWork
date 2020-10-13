using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System;
using SimpleNetManager;

namespace UnityRemoteConsole {
    public class URCSettingData
    {
        /// <summary>
        /// 监听端口
        /// </summary>
        public int netPort = 9123;
        ///// <summary>
        ///// 启动游戏自动启动，false：只能使用手动调用启动
        ///// </summary>
        //public bool autoBoot = false;
        //login
        public string loginKey = "123456";
        //123456 to md5
        public string loginPassword = "86749e9b9f5c3880e46cd82af5813ec2";


        public const string FileName = "UnityRemoteConsoleSettingData";
        private static URCSettingData configData;

        private const string KeyBase64 = "cmMyOWpKM0NQVGxmRmhlSHFSQXd3SWdRbEkweVJEWEJ3ZnduUmJ3TFNGR1R6RzFMNjRudzVBUzdYRHowdWVLbFZBSkFMUFJwNE4zR2JuMTVxRFR6eEJnS21Rcm1EVTJOVTRYTVhSWkZDWlJHaG02Sm1UaUZteU1zNFl6WDlEQTg=";
        public static URCSettingData GetCofig()
        {
            if (Application.isPlaying)
            {
                if (configData != null)
                    return configData;
            }

            TextAsset textAsset = Resources.Load<TextAsset>(FileName);
            if (textAsset == null || string.IsNullOrEmpty(textAsset.text))
            {
                configData= new URCSettingData();
            }
            else
            {
                string json = textAsset.text;
                try
                {
                  
                    byte[] keyBytes = Convert.FromBase64String(KeyBase64);
                    string _aesKeyStr = Encoding.UTF8.GetString(keyBytes);
                    json = SimpleNetManager.AESUtils.AESDecrypt(json, _aesKeyStr);

                  
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
                configData = SimpleJsonUtils.FromJson<URCSettingData>(json);
                if (configData == null)
                {
                    configData = new URCSettingData();
                }
            }

            return configData;
        }
        private const string SavePathDir = "Assets/UnityRemoteConsole/Resources/";
        public static void SaveConfig(URCSettingData config)
        {
            string json = SimpleJsonUtils.ToJson(config);
            byte[] keyBytes = Convert.FromBase64String(KeyBase64);
            string _aesKeyStr = Encoding.UTF8.GetString(keyBytes);
            json = SimpleNetManager.AESUtils.AESEncrypt(json, _aesKeyStr);
            CreateTextFile(SavePathDir + URCSettingData.FileName + ".txt", json);
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
                using (FileStream stream = new FileStream(path.ToString(), FileMode.OpenOrCreate))
                {
                    stream.Write(_data, 0, _data.Length);
                    stream.Flush();
                    stream.Close();
                }
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
