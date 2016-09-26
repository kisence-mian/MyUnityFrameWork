using UnityEngine;
using System.Collections;

public class URLManager 
{
    const string s_configName = "URLConfig";

    static DataTable s_URLTable;

    public static string GetURL(string urlKey)
    {
        Init();

        if (s_URLTable.ContainsKey(urlKey))
        {
            return s_URLTable[urlKey].GetString("URL");
        }
        else
        {
            return null;
        }
    }

    public static string GetPort(string urlKey)
    {
        Init();

        if (s_URLTable.ContainsKey(urlKey))
        {
            return s_URLTable[urlKey].GetString("Port");
        }
        else
        {
            return null;
        }
    }

    public static void Init()
    {
        if (s_URLTable == null)
        {
            s_URLTable = DataManager.GetData(s_configName);
        }
    }
}
