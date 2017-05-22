using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Xml;
using System;
using System.Reflection;
using System.IO;

public class SDKEditorWindow : EditorWindow
{
    const string s_editorConfigName = "SDKEditorConfig";

    const string s_schemeKey = "schemeList";
    const string s_currentSchemeKey = "current";

    string m_filePath = "";
    string m_pluginsPath = "/Plugins/Android";

    int m_currentSelectIndex = 0;
    SchemeData currentSchemeData;

    List<SchemeData> m_configList = new List<SchemeData>();
    List<string> m_configNameList = new List<string>();

    SDKInterfaceBase m_LoginScheme;
    SDKInterfaceBase m_ADScheme;
    SDKInterfaceBase m_PayScheme;
    List<SDKInterfaceBase> m_LogScheme= new List<SDKInterfaceBase>();
    List<SDKInterfaceBase> m_otherScheme = new List<SDKInterfaceBase>();


    [MenuItem("Window/SDK管理器")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(SDKEditorWindow));
    }

    void OnEnable()
    {
        m_filePath = Application.dataPath + "/SDKPath";

        ResourcesConfigManager.Initialize();
        EditorGUIStyleData.Init();

        LoadSchemeConfig();

        CreateSDKFile();
    }

    void OnProjectChange()
    {
        LoadSchemeConfig();
    }

    #region GUI

    void OnGUI()
    {
        titleContent.text = "安卓插件管理器";
        SelectConfigGUI();

        EditorSDKGUI();

        CreateConfigGUI();

        SaveConfigGUI();
    }

    #endregion

    #region 选择方案GUI

    void SelectConfigGUI()
    {
        string[] mask = m_configNameList.ToArray();
        int newIndex = EditorGUILayout.Popup("当前方案：", m_currentSelectIndex, mask);
        if (mask.Length != 0)
        {
            if (m_currentSelectIndex != newIndex)
            {
                if (EditorUtility.DisplayDialog("警告", "确定要切换方案吗？", "是", "取消"))
                {
                    string oldName = m_configList[m_currentSelectIndex].SchemeName;
                    string newName = m_configList[newIndex].SchemeName;

                    m_currentSelectIndex = newIndex;
                    LoadCurrentSchemeConfig(m_configList[newIndex]);

#if UNITY_ANDROID
                    ChangeSchemeFile(newName,oldName);
#else
                    Debug.Log(oldName +"-->>"+ newName);
#endif
                }
            }
        }
    }

    bool isConfigFold = false;
    string configName = "";
    void GetAllConfigName()
    {
        EditorGUI.indentLevel = 0;

        isConfigFold = EditorGUILayout.Foldout(isConfigFold, "新增方案");

        if (isConfigFold)
        {
            EditorGUI.indentLevel = 1;

            configName = EditorGUILayout.TextField("方案名", configName);

            if (!IsExitsSchemeName(configName) && configName != "")
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                if (GUILayout.Button("新增", GUILayout.Width(position.width - 60)))
                {
                    Dictionary<string, SingleField> dict = new Dictionary<string, SingleField>();
                    ConfigEditorWindow.SaveData(configName, dict);

                    //LoadConfig(configName);

                    configName = "";
                }

                EditorGUILayout.Space();
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                if (m_configNameList.Contains(configName))
                {
                    EditorGUILayout.LabelField("方案名重复");
                }
            }
        }

        EditorGUILayout.Space();
    }

    /// <summary>
    /// 新建方案
    /// </summary>
    void CreateConfigGUI()
    {
        EditorGUI.indentLevel = 0;

        isConfigFold = EditorGUILayout.Foldout(isConfigFold, "新增方案");

        if (isConfigFold)
        {
            EditorGUI.indentLevel = 1;

            configName = EditorGUILayout.TextField("方案名", configName);

            if (!IsExitsSchemeName(configName) && configName != "")
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                if (GUILayout.Button("新增", GUILayout.Width(position.width - 60)))
                {
                    SchemeData data = new SchemeData();
                    data.SchemeName = configName;

                    m_configList.Add(data);
                    m_configNameList.Add(data.SchemeName);

                    currentSchemeData = data;

                    configName = "";
                }

                EditorGUILayout.Space();
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                if (m_configNameList.Contains(configName))
                {
                    EditorGUILayout.LabelField("方案名重复");
                }
            }
        }

        EditorGUILayout.Space();
    }

    void SaveConfigGUI()
    {
        if (currentSchemeData != null)
        {
            if (GUILayout.Button("保存"))
            {
                SaveSchemeConfig();
            }
        }
    }

    #endregion

    #region 删除方案

    void DelectSchemeGUI()
    {

    }

    #endregion

    #region 选择插件类型

    bool m_isFoldSDKGUI = true;

    bool m_isFoldlogin = false;
    bool m_isFoldAd = false;
    bool m_isFoldPay = false;

    bool m_isFoldLog = false;
    List<bool> m_logFoldList = new List<bool>();

    bool m_isFoldOther = false;
    List<bool> m_OtherFoldList = new List<bool>();
    Vector2 m_pos = new Vector2();
    void EditorSDKGUI()
    {
        if (currentSchemeData != null)
        {
            m_isFoldSDKGUI = EditorGUILayout.Foldout(m_isFoldSDKGUI, "配置插件类型和参数：");

            if (m_isFoldSDKGUI)
            {
                EditorGUI.indentLevel++;
                m_pos = EditorGUILayout.BeginScrollView(m_pos);

                m_LoginScheme = SelectSDKInterfaceGUI(ref m_isFoldlogin, typeof(LoginInterface), m_LoginScheme, "登陆SDK");
                m_ADScheme = SelectSDKInterfaceGUI(ref m_isFoldAd, typeof(ADInterface), m_ADScheme, "广告SDK");
                m_PayScheme = SelectSDKInterfaceGUI(ref m_isFoldPay, typeof(PayInterface), m_PayScheme, "支付SDK");
                EditorSDKListGUI(ref m_isFoldLog, m_logFoldList, typeof(LogInterface), m_LogScheme, "事件上报SDK");
                EditorSDKListGUI(ref m_isFoldOther, m_OtherFoldList, typeof(OtherSDKInterface), m_otherScheme, "其他SDK");
                EditorGUILayout.EndScrollView();
                EditorGUI.indentLevel--;
            }
        }
    }

    int selectTmp = 0;
    void EditorSDKListGUI(ref bool isFold,List<bool> foldList,Type SDKType, List<SDKInterfaceBase> list, string title)
    {
       
        isFold = EditorGUILayout.Foldout(isFold, title + ":");

        if (isFold)
        {
            while (foldList.Count < list.Count)
            {
                foldList.Add(false);
            }

            EditorGUI.indentLevel++;
            for (int i = 0; i < list.Count; i++)
            {
                bool foldTmp = foldList[i];
                list[i] = SelectSDKInterfaceGUI(ref foldTmp, SDKType, list[i], list[i].GetType().Name);
                foldList[i] = foldTmp;

                if (foldTmp)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.Space();
                    if (GUILayout.Button("删除"))
                    {
                        if (EditorUtility.DisplayDialog("警告", "确定要删除该SDK吗？", "是", "取消"))
                        {
                            list.RemoveAt(i);
                            i--;
                        }
                    }
                    EditorGUILayout.Space();
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.Space();

            string[] mask = GetSDKNameList(SDKType);
            selectTmp = EditorGUILayout.Popup("新增SDK类型：", selectTmp, mask);

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.Space();

            if(GUILayout.Button("新增SDK"))
            {

                Type type = Assembly.Load("Assembly-CSharp").GetType(mask[selectTmp]);

                if (type != null)
                {
                    list.Add((SDKInterfaceBase)Activator.CreateInstance(type));
                    foldList.Add(true);
                }
                else
                {
                    Debug.LogError("Load " + mask[selectTmp] + " Fail!");
                }

                selectTmp = 0;
            }

            EditorGUILayout.Space();

            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel--;
        }
    }

    SDKInterfaceBase SelectSDKInterfaceGUI(ref bool isFold,Type SDKType,SDKInterfaceBase sdk,string title)
    {
        isFold = EditorGUILayout.Foldout(isFold, title + ":");

        if (isFold)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("接口类型：" + SDKType.Name);
            string[] mask = GetSDKNameList(SDKType);

            int currentIndex = GetNameListIndex(mask,sdk);
            int index = EditorGUILayout.Popup("当前SDK：", currentIndex, mask);

            if (sdk == null || mask[index] != sdk.GetType().Name)
            {
                Type type = Assembly.Load("Assembly-CSharp").GetType(mask[index]);

                if (type != null)
                    sdk = (SDKInterfaceBase)Activator.CreateInstance(type);
                else
                {
                    Debug.LogError("Load " + mask[index] + " Fail!");
                }
            }

            //显示界面

            EditorUtilGUI.DrawClassData(sdk);

            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
        }

        return sdk;
    }

    string[] GetSDKNameList(Type SdkType)
    {
        List<string> listTmp = new List<string>();
        Type[] types = Assembly.Load("Assembly-CSharp").GetTypes();

        for (int i = 0; i < types.Length; i++)
        {
            if (types[i].IsSubclassOf(SdkType))
            {
                listTmp.Add(types[i].Name);
            }
        }

        return listTmp.ToArray();
    }

    int GetNameListIndex(string[] list,SDKInterfaceBase sdk)
    {
        if(sdk ==null)
        {
            return 0;
        }

        for (int i = 0; i < list.Length; i++)
        {
           if(sdk.GetType().Name == list[i])
           {
               return i;
           }
        }

        return 0;
    }

    #endregion

    #region 文件/插件管理

    void ChangeSchemeFile(string newSchemeName,string oldSchemeName)
    {
        string oldPath = m_filePath + "/." + oldSchemeName;
        string newPath = m_filePath + "/." + newSchemeName;
        //删除旧文件夹下所有文件
        if (Directory.Exists(oldPath))
        {
            FileTool.DeleteDirectory(oldPath);
        }
        else
        {
            FileTool.CreatPath(oldPath);
        }
        //把当前文件加下的文件拷贝到旧文件夹下
        FileTool.CopyDirectory(m_filePath + m_pluginsPath, oldPath);
        FileTool.DeleteDirectory(m_filePath + m_pluginsPath);

        //找当前方案有没有文件夹
        //如果有，则全部复制过来
        if (Directory.Exists(newPath))
        {
            FileTool.CopyDirectory(newPath, m_filePath + m_pluginsPath);
        }

        UnityEditor.AssetDatabase.Refresh();
    }

    void CreateSDKFile()
    {
        FileTool.CreatPath(m_filePath);
    }

    #endregion

    #region Scheme

    //获取所有的方案
    void LoadSchemeConfig()
    {
        m_configList = new List<SchemeData>();
        m_configNameList = new List<string>();

        Dictionary<string, object> editConfig = ConfigEditorWindow.GetEditorConfigData(s_editorConfigName);
        if (editConfig != null)
        {
            string currentSchemeName = editConfig[s_currentSchemeKey].ToString();

            List<object> list = (List<object>)editConfig[s_schemeKey];

            for (int i = 0; i < list.Count; i++)
            {
                SchemeData tmp = JsonUtility.FromJson<SchemeData>(list[i].ToString());
                if(tmp.SchemeName == currentSchemeName)
                {
                    currentSchemeData = tmp;
                    LoadCurrentSchemeConfig(tmp);
                }
                m_configList.Add(tmp);
                m_configNameList.Add(tmp.SchemeName);
            }
        }
    }

    void SaveCurrentSchemeConfig()
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

    void LoadCurrentSchemeConfig(SchemeData data)
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

    void SaveSchemeConfig()
    {
        SaveCurrentSchemeConfig();

        Dictionary<string, object> editConfig = new Dictionary<string, object>();
        Dictionary<string, SingleField> config = new Dictionary<string, SingleField>();

        List<string> list = new List<string>();

        for (int i = 0; i < m_configList.Count; i++)
        {
            list.Add(JsonUtility.ToJson(m_configList[i]));
        }

        editConfig.Add(s_schemeKey, list);
        editConfig.Add(s_currentSchemeKey, currentSchemeData.SchemeName);

        config.Add(SDKManager.c_KeyName, new SingleField(JsonUtility.ToJson(currentSchemeData)));

        ConfigEditorWindow.SaveEditorConfigData(s_editorConfigName, editConfig);
        ConfigEditorWindow.SaveData(SDKManager.c_ConfigName, config);

        UnityEditor.AssetDatabase.Refresh();
    }

    SDKConfigData SerializeConfig(SDKInterfaceBase sdkInterface)
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

    SDKInterfaceBase AnalysisConfig(SDKConfigData data)
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

    bool IsExitsSchemeName(string name)
    {
        for (int i = 0; i < m_configList.Count; i++)
        {
            if(m_configList[i].SchemeName == name)
            {
                return true;
            }
        }

        return false;
    }

    #endregion

}
