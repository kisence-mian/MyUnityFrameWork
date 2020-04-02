using System;
using UnityEngine;

//GameServerAreaDataGenerate类
//该类自动生成请勿修改，以避免不必要的损失
public class GameServerAreaData : DataGenerateBase
{
    public string m_key;
    public string m_Description; //大区的描述
    public string m_ClientHotUpdateURL; //客户端热更新地址
    public string m_SelectServerURL; //该大区的选服列表
    public string[] m_CountryCode; //连接该大区的国家code（ ISO 3166-1 alpha-2 ）
    public string m_SpecialServerHost; //选取大区中的一个服务器，用于不能获取IP归属地时使用Ping选大区
    public string[] m_ContinentName; //大洲英文缩写，用于直接使用大洲来划分大区，当此选项为空，则使用Countrycode来选择大区([AF]非洲, [EU]欧洲, [AS]亚洲, [OA]大洋洲, [NA]北美洲, [SA]南美洲, [AN]南极洲)

    public override void LoadData(string key)
    {
        DataTable table = DataManager.GetData("GameServerAreaData");

        if (!table.ContainsKey(key))
        {
            throw new Exception("GameServerAreaDataGenerate LoadData Exception Not Fond key ->" + key + "<-");
        }

        SingleData data = table[key];

        m_key = key;
        m_Description = data.GetString("Description");
        m_ClientHotUpdateURL = data.GetString("ClientHotUpdateURL");
        m_SelectServerURL = data.GetString("SelectServerURL");
        m_CountryCode = data.GetStringArray("CountryCode");
        m_SpecialServerHost = data.GetString("SpecialServerHost");


        try
        {
            m_ContinentName = data.GetStringArray("ContinentName");
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
        m_Description = data.GetString("Description");
        m_ClientHotUpdateURL = data.GetString("ClientHotUpdateURL");
        m_SelectServerURL = data.GetString("SelectServerURL");
        m_CountryCode = data.GetStringArray("CountryCode");
        m_SpecialServerHost = data.GetString("SpecialServerHost");
        try
        {
            m_ContinentName = data.GetStringArray("ContinentName");
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

    }
}
