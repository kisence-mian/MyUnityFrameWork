using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public  class SaveRecordManager
{
    private  Dictionary<string, Dictionary<string, string>> allRecords = new Dictionary<string, Dictionary<string, string>>();

    public  IRecordConverter converter = new JsonRecordConverter();
    /// <summary>
    /// 标准非自定义目录的储存
    /// </summary>
    public readonly static SaveRecordManager standard = new SaveRecordManager();
    /// <summary>
    /// 自定义储存目录
    /// </summary>
    private  string customDirectory = "";
    /// <summary>
    /// 设定自定义储存目录，如:Name或Name/PPP
    /// </summary>
    /// <param name="dirName"></param>
    public SaveRecordManager SetCustomDirectory(string dirName)
    {
        customDirectory = dirName;
        return this;
    }
    public SaveRecordManager SetRecordConverter(IRecordConverter converter)
    {
        this.converter = converter;
        return this;
    }
    /// <summary>
    /// 读取记录
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fileName"></param>
    /// <param name="key"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public  T GetRecord<T>(string fileName,string key,T defaultValue = default(T))
    {
        Dictionary<string, string> fileContent = null;
        try
        {
            if (!allRecords.TryGetValue(fileName, out fileContent))
            {
                string text = GetFileTextData(fileName);
                fileContent = converter.String2Object<Dictionary<string, string>>(text);
                if (fileContent == null)
                {
                    fileContent = new Dictionary<string, string>();
                }
                allRecords.Add(fileName, fileContent);
            }

            if (fileContent != null)
            {
                string valueStr = null;
                if (fileContent.TryGetValue(key, out valueStr))
                {
                    return converter.String2Object<T>(valueStr);
                }
            }
        }catch(Exception e)
        {
            Debug.LogError(e);
        }

        return defaultValue;
    }


    /// <summary>
    /// 保存记录
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public   void SaveRecord(string fileName, string key,object value)
    {
        Dictionary<string, string> fileContent = null;

        if (allRecords.ContainsKey(fileName))
        {
            fileContent = allRecords[fileName];
        }
        else
        {
            
            string text = GetFileTextData(fileName);
            fileContent = converter.String2Object<Dictionary<string, string>>(text);
            if (fileContent == null)
            {
                fileContent = new Dictionary<string, string>();
            }
            allRecords.Add(fileName, fileContent);
        }
        string valueStr = converter.Object2String(value);

        if (fileContent.ContainsKey(key))
        {
            fileContent[key] = valueStr;
        }
        else
        {
            fileContent.Add(key, valueStr);
        }
        string ss = converter.Object2String(fileContent);

        FileUtils.CreateTextFile(GetFilePath(fileName), ss);
    }
    /// <summary>
    /// 清除某个文件记录
    /// </summary>
    /// <param name="fileName"></param>
    public  void DeleteRecord(string fileName)
    {
        if (allRecords.ContainsKey(fileName))
        {
            allRecords.Remove(fileName);
        }
        string path = GetFilePath(fileName);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
    /// <summary>
    /// 清除所有记录
    /// </summary>
    public  void DeleteAllRecord()
    {
        allRecords.Clear();

        string pathDir = GetFileDir();
        if (Directory.Exists(pathDir))
        {
            Directory.Delete(pathDir, true);
        }
    }

    private  string GetFilePath(string fileName)
    {
        return GetFileDir() + "/" + fileName + converter.GetFileExtend();
    }
    private  string GetFileDir()
    {
        string dir = Application.persistentDataPath + "/" + converter.GetSaveDirectoryName();
        if (!string.IsNullOrEmpty(customDirectory))
            dir += "/" + customDirectory;
        return dir;
    }

    private  string GetFileTextData(string fileName)
    {
        string path = GetFilePath(fileName);
        string text = null;
        if (File.Exists(path))
        {
             text = FileUtils.LoadTextFileByPath(path);

        }
        return text;
    }
}

