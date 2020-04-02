using System;
using UnityEngine;

//SelectNetworkDataGenerate类
//该类自动生成请勿修改，以避免不必要的损失
public class SelectNetworkData : DataGenerateBase
{
    public string m_key;
    public string m_serverIP; //服务器IP
    public string m_description; //描述
    public int m_port; //端口
    public string[] m_androidVersion; //支持前端Android的版本
    public string[] m_iosVersion; //支持的前端IOS的版本
    public string[] m_channel; //包的渠道
    public string[] m_standaloneVersion; //包含Windows，mac，Linux

    public override void LoadData(string key)
    {
        DataTable table = DataManager.GetData("SelectNetworkData");

        if (!table.ContainsKey(key))
        {
            throw new Exception("SelectNetworkDataGenerate LoadData Exception Not Fond key ->" + key + "<-");
        }

        SingleData data = table[key];

        m_key = key;
        m_serverIP = data.GetString("serverIP");
        m_description = data.GetString("description");
        m_port = data.GetInt("port");
        m_androidVersion = data.GetStringArray("androidVersion");
        m_iosVersion = data.GetStringArray("iosVersion");
        m_channel = data.GetStringArray("channel");
        try
        {
            m_standaloneVersion = data.GetStringArray("standaloneVersion");
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public override void LoadData(DataTable table, string key)
    {
        SingleData data = table[key];

        m_key = key;
        m_serverIP = data.GetString("serverIP");
        m_description = data.GetString("description");
        m_port = data.GetInt("port");
        m_androidVersion = data.GetStringArray("androidVersion");
        m_iosVersion = data.GetStringArray("iosVersion");
        m_channel = data.GetStringArray("channel");

        try
        {
            m_standaloneVersion = data.GetStringArray("standaloneVersion");
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
}
