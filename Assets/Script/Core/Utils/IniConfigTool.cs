using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class IniConfigTool
{
    public Dictionary<string, string> configData;
    //string fullFileName;
    public IniConfigTool(string filePath)
    {
        configData = new Dictionary<string, string>();
     
        bool hasCfgFile = File.Exists(filePath);
        if (hasCfgFile == false)
        {
            return;
        }
        StreamReader reader = new StreamReader(filePath, Encoding.Default);
        string line;
        int indx = 0;
        while ((line = reader.ReadLine()) != null)
        {
            try
            {
                if (line.StartsWith("#") || string.IsNullOrEmpty(line))
                    configData.Add("#" + indx++, line);
                else
                {
                    string[] key_value = line.Split('=');
                    if (key_value.Length >= 2)
                        configData.Add(key_value[0], key_value[1]);
                    else
                        configData.Add("#" + indx++, line);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
            }
          
        }
        reader.Close();
    }
    private string Get(string key)
    {
        if (configData.Count <= 0)
            return null;
        else if (configData.ContainsKey(key))
            return configData[key];
        else
            return null;
    }

    public bool GetBool(string key,bool defaultValue)
    {
        return GetValue(key, defaultValue);
    }
    public int GetInt(string key, int defaultValue)
    {
        return GetValue(key, defaultValue);
    }
    public string GetString(string key, string defaultValue)
    {
        return GetValue(key, defaultValue);
    }

    public T GetValue<T>(string key, T defaultValue)
    {
        string res = Get(key);
        if (string.IsNullOrEmpty(res))
        {
            return defaultValue;
        }
        else
        {
            try
            {
                return (T)Convert.ChangeType(res,typeof(T));
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
            }
            return defaultValue;
        }
    }
    public void Set(string key, string value)
    {
        if (configData.ContainsKey(key))
            configData[key] = value;
        else
            configData.Add(key, value);
    }
    //public void Save()
    //{
    //    StreamWriter writer = new StreamWriter(fullFileName, false, Encoding.Default);
    //    IDictionaryEnumerator enu = configData.GetEnumerator();
    //    while (enu.MoveNext())
    //    {
    //        if (enu.Key.ToString().StartsWith("#"))
    //            writer.WriteLine(enu.Key + "" + enu.Value);
    //        else
    //            writer.WriteLine(enu.Key + "=" + enu.Value);
    //    }
    //    writer.Close();
    //}
}