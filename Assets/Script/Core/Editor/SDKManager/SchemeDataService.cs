using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class SchemeDataService
{
    public static List<SchemeData> m_configList = new List<SchemeData>();
    public static List<string> m_configNameList = new List<string>();

    public static SchemeData currentSchemeData;

    public static SDKInterfaceBase m_LoginScheme;
    public static SDKInterfaceBase m_ADScheme;
    public static SDKInterfaceBase m_PayScheme;
    public static List<SDKInterfaceBase> m_LogScheme = new List<SDKInterfaceBase>();
    public static List<SDKInterfaceBase> m_otherScheme = new List<SDKInterfaceBase>();

    public static void LoadSchemeConfig()
    {
        m_configList = new List<SchemeData>();
        m_configNameList = new List<string>();

        Dictionary<string, object> editConfig = ConfigEditorWindow.GetEditorConfigData(SDKEditorWindow.s_editorConfigName);
        if (editConfig != null)
        {
            string currentSchemeName = editConfig[SDKEditorWindow.s_currentSchemeKey].ToString();

            List<object> list = (List<object>)editConfig[SDKEditorWindow.s_schemeKey];

            for (int i = 0; i < list.Count; i++)
            {
                SchemeData tmp = JsonUtility.FromJson<SchemeData>(list[i].ToString());
                if (tmp.SchemeName == currentSchemeName)
                {
                    currentSchemeData = tmp;
                    LoadCurrentSchemeConfig(tmp);
                }
                m_configList.Add(tmp);
                m_configNameList.Add(tmp.SchemeName);
            }
        }
    }

    public static void LoadCurrentSchemeConfig(SchemeData data)
    {
        m_LoginScheme = AnalysisConfig(data.LoginScheme);
        m_ADScheme = AnalysisConfig(data.ADScheme);
        m_PayScheme = AnalysisConfig(data.PayScheme);

        m_LogScheme = new List<SDKInterfaceBase>();
        for (int i = 0; i < data.LogScheme.Count; i++)
        {
            m_LogScheme.Add(AnalysisConfig(data.LogScheme[i]));
        }

        m_otherScheme = new List<SDKInterfaceBase>();
        for (int i = 0; i < data.OtherScheme.Count; i++)
        {
            m_otherScheme.Add(AnalysisConfig(data.OtherScheme[i]));
        }
    }

    public static void SaveSchemeConfig()
    {
        SaveCurrentSchemeConfig();

        Dictionary<string, object> editConfig = new Dictionary<string, object>();
        Dictionary<string, SingleField> config = new Dictionary<string, SingleField>();

        List<string> list = new List<string>();

        for (int i = 0; i < m_configList.Count; i++)
        {
            list.Add(JsonUtility.ToJson(m_configList[i]));
        }

        editConfig.Add(SDKEditorWindow.s_schemeKey, list);
        editConfig.Add(SDKEditorWindow.s_currentSchemeKey, currentSchemeData.SchemeName);

        config.Add(SDKManager.c_KeyName, new SingleField(JsonUtility.ToJson(currentSchemeData)));

        ConfigEditorWindow.SaveEditorConfigData(SDKEditorWindow.s_editorConfigName, editConfig);
        ConfigEditorWindow.SaveData(SDKManager.c_ConfigName, config);
    }

    public static void SaveCurrentSchemeConfig()
    {
        if (currentSchemeData != null)
        {
            currentSchemeData.LoginScheme = SerializeConfig(m_LoginScheme);
            currentSchemeData.ADScheme = SerializeConfig(m_ADScheme);
            currentSchemeData.PayScheme = SerializeConfig(m_PayScheme);

            currentSchemeData.LogScheme.Clear();

            for (int i = 0; i < m_LogScheme.Count; i++)
            {
                currentSchemeData.LogScheme.Add(SerializeConfig(m_LogScheme[i]));
            }

            currentSchemeData.OtherScheme.Clear();

            for (int i = 0; i < m_otherScheme.Count; i++)
            {
                currentSchemeData.OtherScheme.Add(SerializeConfig(m_otherScheme[i]));
            }

            Debug.Log(JsonUtility.ToJson(currentSchemeData));
        }
    }


    public static SDKInterfaceBase AnalysisConfig(SDKConfigData data)
    {
        if (data == null)
        {
            return new NullSDKInterface();
        }
        else
        {
            return (SDKInterfaceBase)JsonUtility.FromJson(data.SDKContent, Assembly.Load("Assembly-CSharp").GetType(data.SDKName));
        }
    }

    static SDKConfigData SerializeConfig(SDKInterfaceBase sdkInterface)
    {
        SDKConfigData result = new SDKConfigData();

        if (sdkInterface != null)
        {
            result.SDKName = sdkInterface.GetType().Name;
            result.SDKContent = JsonUtility.ToJson(sdkInterface);
        }
        else
        {
            result.SDKName = "Null";
            result.SDKContent = "";
        }

        return result;
    }

    public static bool IsExitsSchemeName(string name)
    {
        for (int i = 0; i < m_configList.Count; i++)
        {
            if (m_configList[i].SchemeName == name)
            {
                return true;
            }
        }

        return false;
    }

    public static int GetSchemeIndex(string name)
    {
        for (int i = 0; i < m_configList.Count; i++)
        {
            if (m_configList[i].SchemeName == name)
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// 切换方案，打包用
    /// </summary>
    /// <param name="SchemeName"></param>
    public static void ChangeScheme(string SchemeName)
    {
        LoadSchemeConfig();

        if (IsExitsSchemeName(SchemeName))
        {
            //切换方案
            LoadCurrentSchemeConfig(m_configList[GetSchemeIndex(SchemeName)]);
            SaveCurrentSchemeConfig();

            //替换文件夹
            SDKEditorWindow.ChangeSchemeFile(SchemeName, currentSchemeData.SchemeName);
        }
        else
        {
            Debug.Log("不存在的方案名！ " + SchemeName);
        }
    }

}
