using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.IO;

public class SDKEditorWindow : EditorWindow
{
    public const string s_editorConfigName = "SDKEditorConfig";
    public const string s_schemeKey = "schemeList";
    public const string s_currentSchemeKey = "current";

    static string m_filePath = null;

    public static string FilePath
    {
        get
        {
            if(m_filePath == null)
            {
                m_filePath = Application.dataPath + "/SDKPath";
            }

            return m_filePath;
        }
    }
    const string m_pluginsPath = "/Plugins/Android";

    int m_currentSelectIndex = 0;


    [MenuItem("Window/SDK管理器")]
    public static void ShowWindow()
    {
        GetWindow(typeof(SDKEditorWindow));
    }

    void OnEnable()
    {
        ResourcesConfigManager.Initialize();
        EditorGUIStyleData.Init();

        SchemeDataService.LoadSchemeConfig();

        CreateSDKFile();
    }

    void OnProjectChange()
    {
        SchemeDataService.LoadSchemeConfig();
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
        string[] mask = SchemeDataService.m_configNameList.ToArray();
        int newIndex = EditorGUILayout.Popup("当前方案：", m_currentSelectIndex, mask);
        if (mask.Length != 0)
        {
            if (m_currentSelectIndex != newIndex)
            {
                if (EditorUtility.DisplayDialog("警告", "确定要切换方案吗？", "是", "取消"))
                {
                    string oldName = SchemeDataService.m_configList[m_currentSelectIndex].SchemeName;
                    string newName = SchemeDataService.m_configList[newIndex].SchemeName;

                    m_currentSelectIndex = newIndex;
                    SchemeDataService.LoadCurrentSchemeConfig(SchemeDataService.m_configList[newIndex]);

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

            if (!SchemeDataService.IsExitsSchemeName(configName) && configName != "")
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
                if (SchemeDataService.m_configNameList.Contains(configName))
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

            if (!SchemeDataService.IsExitsSchemeName(configName) && configName != "")
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                if (GUILayout.Button("新增", GUILayout.Width(position.width - 60)))
                {
                    SchemeData data = new SchemeData();
                    data.SchemeName = configName;

                    SchemeDataService.m_configList.Add(data);
                    SchemeDataService.m_configNameList.Add(data.SchemeName);

                    SchemeDataService.currentSchemeData = data;

                    configName = "";
                }

                EditorGUILayout.Space();
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                if (SchemeDataService.m_configNameList.Contains(configName))
                {
                    EditorGUILayout.LabelField("方案名重复");
                }
            }
        }

        EditorGUILayout.Space();
    }

    void SaveConfigGUI()
    {
        if (SchemeDataService.currentSchemeData != null)
        {
            if (GUILayout.Button("保存"))
            {
                SchemeDataService.SaveSchemeConfig();
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
        if (SchemeDataService.currentSchemeData != null)
        {
            m_isFoldSDKGUI = EditorGUILayout.Foldout(m_isFoldSDKGUI, "配置插件类型和参数：");

            if (m_isFoldSDKGUI)
            {
                EditorGUI.indentLevel++;
                m_pos = EditorGUILayout.BeginScrollView(m_pos);

                SchemeDataService.m_LoginScheme = SelectSDKInterfaceGUI(ref m_isFoldlogin, typeof(LoginInterface), SchemeDataService.m_LoginScheme, "登陆SDK");
                SchemeDataService.m_ADScheme = SelectSDKInterfaceGUI(ref m_isFoldAd, typeof(ADInterface), SchemeDataService.m_ADScheme, "广告SDK");
                SchemeDataService.m_PayScheme = SelectSDKInterfaceGUI(ref m_isFoldPay, typeof(PayInterface), SchemeDataService.m_PayScheme, "支付SDK");
                EditorSDKListGUI(ref m_isFoldLog, m_logFoldList, typeof(LogInterface), SchemeDataService.m_LogScheme, "事件上报SDK");
                EditorSDKListGUI(ref m_isFoldOther, m_OtherFoldList, typeof(OtherSDKInterface), SchemeDataService.m_otherScheme, "其他SDK");
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

    //默认旧文件夹是当前SDK文件夹
    public static void ChangeSchemeFile(string newSchemeName,string oldSchemeName)
    {
        string oldPath = FilePath + "/." + oldSchemeName;
        string newPath = FilePath + "/." + newSchemeName;
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
        FileTool.CopyDirectory(FilePath + m_pluginsPath, oldPath);
        FileTool.DeleteDirectory(FilePath + m_pluginsPath);

        //找当前方案有没有文件夹
        //如果有，则全部复制过来
        if (Directory.Exists(newPath))
        {
            FileTool.CopyDirectory(newPath, FilePath + m_pluginsPath);
        }

        AssetDatabase.Refresh();
    }

    void CreateSDKFile()
    {
        FileTool.CreatPath(FilePath);
    }

    #endregion
}
