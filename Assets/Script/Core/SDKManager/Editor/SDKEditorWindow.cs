using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.IO;
using FrameWork.SDKManager;

public class SDKEditorWindow : EditorWindow
{
    public const string s_editorConfigName = "SDKEditorConfig";
    public const string s_schemeKey = "schemeList";
    public const string s_currentSchemeKey = "current";

    public SchemeData m_currentSchemeData;

    public List<LoginInterface> m_LoginScheme    = new List<LoginInterface>();
    public List<ADInterface> m_ADScheme          = new List<ADInterface>();
    public List<PayInterface> m_PayScheme        = new List<PayInterface>();
    public List<LogInterface> m_LogScheme        = new List<LogInterface>();
    public List<RealNameInterface> m_RealNameScheme = new List<RealNameInterface>();
    public List<OtherSDKInterface> m_otherScheme = new List<OtherSDKInterface>();

    int m_currentSelectIndex = 0;

    [MenuItem("Window/SDK管理器")]
    public static void ShowWindow()
    {
        GetWindow(typeof(SDKEditorWindow));
    }

    void OnEnable()
    {
        //ResourcesConfigManager.Initialize();
        EditorGUIStyleData.Init();
        SchemeDataService.ReloadEditorSchemeData();
        m_currentSchemeData = SDKManager.LoadGameSchemeConfig();
        m_currentSelectIndex = GetCurrentSelectIndex();
        LoadSchemeData(m_currentSchemeData);

        CreateReadMe();
    }

    void OnProjectChange()
    {
        SchemeDataService.ReloadEditorSchemeData();
        m_currentSchemeData = SDKManager.LoadGameSchemeConfig();
        m_currentSelectIndex = GetCurrentSelectIndex();
        LoadSchemeData(m_currentSchemeData);

        CreateReadMe();
    }

    #region GUI

    void OnGUI()
    {
        titleContent.text = "插件管理器";
        SelectConfigGUI();

        EditorSDKGUI();

        CreateConfigGUI();

        SaveConfigGUI();
    }

    #endregion

    #region 选择方案

    void SelectConfigGUI()
    {
        string[] mask = SchemeDataService.ConfigNameList.ToArray();
        int newIndex = EditorGUILayout.Popup("当前方案：", m_currentSelectIndex, mask);
        if (mask.Length != 0)
        {
            if (m_currentSelectIndex != newIndex)
            {
                if (EditorUtility.DisplayDialog("警告", "确定要切换方案吗？", "是", "取消"))
                {
                    string oldName = SchemeDataService.ConfigNameList[m_currentSelectIndex];
                    string newName = SchemeDataService.ConfigNameList[newIndex];

                    ChangeScheme(newName,oldName);
                }
            }
        }
    }

    bool isConfigFold = false;
    string configName = "";

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

            if (!SchemeDataService.IsExitsSchemeName(configName) && configName != "")
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                if (GUILayout.Button("新增", GUILayout.Width(position.width - 60)))
                {
                    CreateScheme(configName);
                    configName = "";
                }

                EditorGUILayout.Space();
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                if (SchemeDataService.ConfigNameList.Contains(configName))
                {
                    EditorGUILayout.LabelField("方案名重复");
                }
            }
        }

        EditorGUILayout.Space();
    }

    void SaveConfigGUI()
    {
        if (GUILayout.Button("保存"))
        {
            SaveScheme();
        }
    }

    void SaveScheme()
    {
        m_currentSchemeData = SchemeDataService.CreateSchemeData(
                m_currentSchemeData.SchemeName,
                m_currentSchemeData.UseNewSDKManager,
            m_LoginScheme,
            m_ADScheme,
            m_PayScheme,
            m_LogScheme,
            m_otherScheme
            );

        SchemeDataService.SaveGameSchemeConfig(m_currentSchemeData);

        SchemeDataService.UpdateSchemeData(m_currentSchemeData);
        SchemeDataService.SaveEditorSchemeData();
    }

    void CreateScheme(string SchemeName)
    {
        SchemeData data = SchemeDataService.AddScheme(SchemeName);

        //如果当前没有方案，则把新建的方案设为当前方案
        if (m_currentSchemeData == null)
        {
            m_currentSchemeData = data;
            m_currentSelectIndex = GetCurrentSelectIndex();
            LoadSchemeData(m_currentSchemeData);

            SaveScheme();

            //设置宏定义
            EditorExpand.ChangeDefine(new string[] { SchemeName }, new string[] { });
        }
    }

    void ChangeScheme(string newScheme,string oldScheme)
    {
        SchemeDataService.ChangeScheme(newScheme);
        m_currentSchemeData = SDKManager.LoadGameSchemeConfig();
        m_currentSelectIndex = GetCurrentSelectIndex();
        LoadSchemeData(m_currentSchemeData);
    }

    void LoadSchemeData(SchemeData data)
    {
        SDKManager.AnalyzeSchemeData(data,
            out m_LoginScheme,
            out m_ADScheme,
            out m_PayScheme,
            out m_LogScheme,
            out m_RealNameScheme,
            out m_otherScheme
            );
    }

    int GetCurrentSelectIndex()
    {
        Debug.Log("GetCurrentSelectIndex " + m_currentSchemeData );

        if(m_currentSchemeData == null)
        {
            return 0;
        }

        Debug.Log("m_currentSchemeData.SchemeName " + m_currentSchemeData.SchemeName);

        for (int i = 0; i < SchemeDataService.ConfigNameList.Count; i++)
        {
            if(m_currentSchemeData.SchemeName == SchemeDataService.ConfigNameList[i])
            {
                return i;
            }
        }

        return 0;
    }

    bool GetUseNewSDKManager()
    {
        if (m_currentSchemeData == null)
        {
            return false;
        }

        return m_currentSchemeData.UseNewSDKManager;
    }

    #endregion

    #region 删除方案

    void DelectSchemeGUI()
    {
        if (GUILayout.Button("删除当前方案"))
        {
            if (EditorUtility.DisplayDialog("警告", "删除方案会删除对应的插件文件夹\n要继续吗？", "是", "取消"))
            {
                DelectScheme();
            }
        }
    }

    void DelectScheme()
    {
        SchemeDataService.DelectScheme(m_currentSchemeData);

        //移除宏定义
        EditorExpand.ChangeDefine(new string[] { }, new string[] { m_currentSchemeData.SchemeName });

        m_currentSchemeData = null;
        m_currentSelectIndex = GetCurrentSelectIndex();
    }

    #endregion

    #region 选择插件类型

    bool m_isFoldSDKGUI = true;

    bool m_isFoldlogin = false;
    bool m_isFoldAd    = false;
    bool m_isFoldPay   = false;
    bool m_isFoldLog   = false;
    bool m_isFoldOther = false;
    bool m_isFoldRealName = false;

    int selectLoginIndex = 0;
    int selectADIndex = 0;
    int selectPayIndex = 0;
    int selectLogIndex = 0;
    int selectOtherIndex = 0;
    int selectRealNameIndex = 0;

    List<bool> m_loginFoldList = new List<bool>();
    List<bool> m_AdFoldList = new List<bool>();
    List<bool> m_PayFoldList = new List<bool>();
    List<bool> m_LogFoldList = new List<bool>();
    List<bool> m_OtherFoldList = new List<bool>();
    List<bool> m_RealNameFoldList = new List<bool>();

    Vector2 m_pos = new Vector2();
    void EditorSDKGUI()
    {
        if (m_currentSchemeData != null)
        {
            m_currentSchemeData.UseNewSDKManager = GUILayout.Toggle(m_currentSchemeData.UseNewSDKManager, "使用新版本SDKManager");

            m_isFoldSDKGUI = EditorGUILayout.Foldout(m_isFoldSDKGUI, "配置插件类型和参数：");

            if (m_isFoldSDKGUI)
            {
                EditorGUI.indentLevel++;
                m_pos = EditorGUILayout.BeginScrollView(m_pos);

                EditorSDKListGUI(ref m_isFoldlogin, ref selectLoginIndex, m_loginFoldList, typeof(LoginInterface), m_LoginScheme, "登陆SDK");
                EditorSDKListGUI(ref m_isFoldAd, ref selectADIndex, m_AdFoldList, typeof(ADInterface), m_ADScheme, "广告SDK");
                EditorSDKListGUI(ref m_isFoldPay, ref selectPayIndex, m_PayFoldList, typeof(PayInterface), m_PayScheme, "支付SDK");
                EditorSDKListGUI(ref m_isFoldLog, ref selectLogIndex, m_LogFoldList, typeof(LogInterface), m_LogScheme, "事件上报SDK");
                EditorSDKListGUI(ref m_isFoldRealName, ref selectRealNameIndex, m_RealNameFoldList, typeof(RealNameInterface), m_RealNameScheme, "实名制SDK");
                EditorSDKListGUI(ref m_isFoldOther, ref selectOtherIndex, m_OtherFoldList, typeof(OtherSDKInterface), m_otherScheme, "其他SDK");

                EditorGUILayout.EndScrollView();
                EditorGUI.indentLevel--;
            }
        }
        else
        {
            EditorGUILayout.LabelField("没有方案");
        }

        DelectSchemeGUI();
    }



    void EditorSDKListGUI<T>(ref bool isFold, ref int selectIndex ,List<bool> foldList,Type SDKType, List<T> list, string title) where T: SDKInterfaceBase
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
                list[i] = SelectSDKInterfaceGUI(ref foldTmp, SDKType, list[i], list[i].m_SDKName);
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
            selectIndex = EditorGUILayout.Popup("新增SDK类型：", selectIndex, mask);

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.Space();

            if(GUILayout.Button("新增SDK"))
            {
                Type type = Assembly.Load("Assembly-CSharp").GetType(mask[selectIndex]);

                if (type != null)
                {
                    T service = (T)Activator.CreateInstance(type);
                    service.m_SDKName = service.GetType().Name;
                    list.Add(service);
                    foldList.Add(true);
                }
                else
                {
                    Debug.LogError("Load " + mask[selectIndex] + " Fail!");
                }

                selectIndex = 0;
            }

            EditorGUILayout.Space();

            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel--;
        }
    }

    T SelectSDKInterfaceGUI<T>(ref bool isFold,Type SDKType, T sdk,string title) where T: SDKInterfaceBase
    {
        isFold = EditorGUILayout.Foldout(isFold, title + ":");

        if (isFold)
        {
            EditorGUI.indentLevel++;
            sdk.m_SDKName = EditorGUILayout.TextField( "SDK名称：", sdk.m_SDKName);
            string[] mask = GetSDKNameList(SDKType);

            int currentIndex = GetNameListIndex(mask,sdk);
            int index = EditorGUILayout.Popup("当前SDK：", currentIndex, mask);

            if (sdk == null || mask[index] != sdk.GetType().Name)
            {
                Type type = Assembly.Load("Assembly-CSharp").GetType(mask[index]);

                if (type != null)
                    sdk = (T)Activator.CreateInstance(type);
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

    #region ReadMe

    void CreateReadMe()
    {
        if(Directory.Exists(Application.dataPath + "/" + SchemeDataService.c_SDKCachePath))
        {
            string path = Application.dataPath + "/" + SchemeDataService.c_SDKCachePath + "/Readme.txt";
            if (!File.Exists(path))
            {
                string LoadPath = Application.dataPath + "/Script/Core/Editor/res/readme/SDKCacheReadme.txt";
                string content = ResourceIOTool.ReadStringByFile(LoadPath);
                ResourceIOTool.WriteStringByFile(path, content);
            }
        }
    }

    #endregion
}
