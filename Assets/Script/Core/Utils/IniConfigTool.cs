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

        if (string.IsNullOrEmpty(filePath))
            return;
        bool hasCfgFile = File.Exists(filePath);
        if (hasCfgFile == false)
        {
            return;
        }
        StreamReader reader = new StreamReader(filePath, Encoding.Default);
        string line;
        int index = 0;
        while ((line = reader.ReadLine()) != null)
        {

            Parse(ref index, line);
        }
        reader.Close();
    }

    public void ReInit(string content)
    {
        if (string.IsNullOrEmpty(content))
            return;
        string[] lines = content.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        int index = 0;
        foreach (var line in lines)
        {
            Parse(ref index, line);
        }
    }

    private void Parse(ref int index, string line)
    {
        try
        {
            if (line.StartsWith("#") || string.IsNullOrEmpty(line))
                configData.Add("#" + index++, line);
            else
            {
                if (line.Contains("="))
                {
                    int i = line.IndexOf('=');
                    string key = line.Substring(0, i);
                    string value = line.Substring(i + 1);
                    if (configData.ContainsKey(key))
                    {
                        Debug.LogError("已包含key:" + key);
                        return;
                    }
                    configData.Add(key, value);
                }
                else
                    configData.Add("#" + index++, line);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }
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

    public bool GetBool(string key, bool defaultValue)
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
                return (T)Convert.ChangeType(res, typeof(T));
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