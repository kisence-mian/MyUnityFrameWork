using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FrameWork;
using HDJ.Framework.Utils;

public class DataEditorWindow : EditorWindow
{
    private const string FontPlayerPrefKey = "DataEditorWindow.FontKey";

    UILayerManager m_UILayerManager;
    static DataEditorWindow win;

    [MenuItem("Window/数据编辑器 &2", priority = 501)]
    public static void ShowWindow()
    {
         win = EditorWindow.GetWindow<DataEditorWindow>();
        win.autoRepaintOnSceneChange = true;
        FocusWindowIfItsOpen<DataEditorWindow>();
        win.wantsMouseMove = true;
    }

    List<String> m_dataNameList = new List<string>();

    DataTable m_currentData;
    //Dictionary<string, bool> m_foldList = new Dictionary<string, bool>();
    private List<string> langKeys;
    void OnEnable()
    {
        //EditorGUIStyleData.Init();
        win = this;
        FindAllDataName();
        langKeys = LanguageDataEditorUtils.GetLanguageLayersKeyList();

        if (!string.IsNullOrEmpty(chooseFileName))
            LoadData(chooseFileName);

        configFileNames.Clear();
        string m_directoryPath = Application.dataPath + "/Resources/" + DataManager.c_directoryName;
        configFileNames.AddRange(PathUtils.GetDirectoryFileNames(m_directoryPath, new string[] { ".txt" }, false, false));

    }
    private void OnDestroy()
    {
        PlayerPrefs.SetInt(FontPlayerPrefKey, nowButtonFontSize);
        PlayerPrefs.Save();
    }
    public static void Refresh()
    {
        if (win)
            win.OnEnable();
    }
    ////当选择改变时
    //void OnSelectionChange()
    //{

    //}

    ////当工程改变时
    //void OnProjectChange()
    //{
    //    if (!Application.isPlaying)
    //    {
    //        //ConvertUtf8();

    //        FindAllDataName();

    //        if (chooseFileName != null
    //            && chooseFileName != ""
    //            && chooseFileName != "None")
    //        {
    //            LoadData(chooseFileName);
    //        }
    //    }
    //}

    #region GUI

    void OnGUI()
    {
        titleContent.text = "数据编辑器";

        EditorDrawGUIUtil.RichTextSupport = true;
        GUILayout.Space(8);
        ChooseFile();
        GUILayout.Space(5);
        DrawTableDataGUI();
        GUILayout.FlexibleSpace();

        SaveDataGUI();
        GenerateDataClassGUI();
        GenerateAllDataClassGUI();

        CleanCacheGUI();
    }
    private string chooseFileName = "";

    void ChooseFile()
    {
        GUILayout.BeginHorizontal();
        chooseFileName = EditorDrawGUIUtil.DrawPopup("选择文件", chooseFileName, m_dataNameList, LoadData);
        if (GUILayout.Button("删除", GUILayout.Width(50)))
        {
            if (EditorUtility.DisplayDialog("警告", "是否删除文件[" + chooseFileName + "]", "确定", "取消"))
            {
                File.Delete(Application.dataPath + "/Resources/" + DataManager.c_directoryName + "/" + chooseFileName + ".txt");
                AssetDatabase.Refresh();
                m_currentData = null;
                return;
            }
        }
        if (GUILayout.Button("添加", GUILayout.Width(50)))
        {
            GeneralDataModificationWindow.otherParameter = "";
            GeneralDataModificationWindow.OpenWindow(this, "添加新配置文件", "", (value) =>
            {
                value = EditorDrawGUIUtil.DrawBaseValue("新建配置文件:", value);
                GeneralDataModificationWindow.otherParameter = EditorDrawGUIUtil.DrawBaseValue("主键名:", GeneralDataModificationWindow.otherParameter);
                if (string.IsNullOrEmpty(value.ToString()) || string.IsNullOrEmpty(GeneralDataModificationWindow.otherParameter.ToString()))
                {
                    EditorGUILayout.HelpBox("文件名不能为空！！", MessageType.Error);
                }
                return value;
            },
            (value) =>
            {
                if (string.IsNullOrEmpty(value.ToString()) || string.IsNullOrEmpty(GeneralDataModificationWindow.otherParameter.ToString()))
                {
                    return false;
                }
                else
                    return true;

            },
            (value) =>
            {
                DataTable data = new DataTable();
                data.TableKeys.Add(GeneralDataModificationWindow.otherParameter.ToString());

                SaveData(value.ToString(), data);
                AssetDatabase.Refresh();

                LoadData(chooseFileName);
                AssetDatabase.Refresh();
            });
        }

        GUILayout.EndHorizontal();
    }
    private void LoadData(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            return;

        chooseFileName = fileName;

        if (chooseFileName != "None")
        {
           
            m_currentData = DataManager.GetData(fileName);
        }
        else
            return;

        int widthCount = m_currentData.TableKeys.Count;
        //Debug.Log(" m_currentData.TableKeys.Count :" + m_currentData.TableKeys.Count + "  withItemList.Count :" + withItemList.Count);
        if (widthCount > withItemList.Count)
        {
            int num = widthCount - withItemList.Count;
            for (int i = 0; i < num; i++)
            {
                withItemList.Add(wWith);
            }
        }
        else if (widthCount < withItemList.Count)
        {
            while(widthCount < withItemList.Count)
            {
                withItemList.RemoveAt(withItemList.Count - 1);
            }
        }
        //Debug.Log(" m_currentData.TableKeys.Count :" + m_currentData.TableKeys.Count + "  withItemList.Count :" + withItemList.Count);
        heightItemList.Clear();
        //所有的数据行+描述+默认值+空一行
        heightItemList.Add(wHeight);
        for (int i = 0; i < m_currentData.Count+2; i++)
        {
            heightItemList.Add(wHeight);
        }
        for (int i = 0; i < selectRowIndexs.Count; i++)
        {
            if (selectRowIndexs[i] >= heightItemList.Count)
            {
                selectRowIndexs.RemoveAt(i);
                i = i - 1;
            }
        }
        for (int i = 0; i < selectColumnIndexs.Count; i++)
        {
            if (selectColumnIndexs[i] >= withItemList.Count)
            {
                selectColumnIndexs.RemoveAt(i);
                i = i - 1;
            }
        }
        //selectRowIndexs.Clear();
        //selectColumnIndexs.Clear();
    }

    private List<float> withItemList = new List<float>();
    private List<float> heightItemList = new List<float>();
    private int minItem = 30;
    private const int wHeight = 45;
    private const int wWith = 110;
    private Vector2 svPos;

    //标记选择行
    private List<int> selectRowIndexs = new List<int>();
    private List<int> selectColumnIndexs = new List<int>();

    private GUIStyle buttonStyle;
    private GUIStyle helpBoxStyle;

    private Vector2 topGridSVPos;
    private Vector2 leftGridSVPos;

    /// <summary>
    /// 绘制所有格子
    /// </summary>
    void DrawTableDataGUI()
    {
        if (m_currentData == null)
            return;
        if (buttonStyle == null || helpBoxStyle==null)
        {
            buttonStyle = "Button";
            helpBoxStyle = "HelpBox";
        }
        Debug.unityLogger.logEnabled = false;
        GridTopFunctionGUI();
        GUILayout.Space(5);

        Rect r = new Rect(0, 60, Screen.width, 18);
        float detaH = wHeight;
        if (heightItemList.Count > 3)
        {
            detaH = heightItemList[0] + heightItemList[1] + heightItemList[2];
        }
        Rect svPos_temp = new Rect(new Vector2(0, r.y), new Vector2(Screen.width-wWith, Screen.height - r.y - 120- detaH));
        Vector2 v = GetCententSize();


        svPos = GUI.BeginScrollView(new Rect(svPos_temp.x + wWith, svPos_temp.y + detaH, svPos_temp.width, svPos_temp.height), svPos, new Rect(r.x + wWith, r.y + detaH, v.x, v.y));

        DrawGridItem(r.position + new Vector2(wWith, wHeight),2, heightItemList.Count);

        GUI.EndScrollView();

       //顶部格子
        topGridSVPos = GUI.BeginScrollView(new Rect(svPos_temp.x, svPos_temp.y, svPos_temp.width+ wWith, detaH), topGridSVPos, new Rect(r.x, r.y, v.x+wWith, detaH),false,false,"Label","Label");
        topGridSVPos = new Vector2(svPos.x, topGridSVPos.y);

        DrawGridItem(r.position + new Vector2(wWith, wHeight), 0, 2);

        float tempWith = wWith;
        GUI.Label(new Rect(r.position, new Vector2(wWith, wHeight)), "", "SelectionRect");
        for (int i = 0; i < withItemList.Count; i++)
        {
            Rect dragR = new Rect(tempWith, r.y, withItemList[i], wHeight);
            Rect maxR = new Rect(tempWith, r.y, Screen.width * 5, wHeight);
            dragR = EditorDrawGUIUtility.DrawCanDragArea(dragR, maxR, null, EditorCanDragAreaSide.Right);
            if (dragR.width < minItem)
                dragR.width = minItem;
            withItemList[i] = dragR.width;

            tempWith += withItemList[i];

            if (GUI.Button(new Rect(dragR.position, new Vector2(dragR.width, dragR.height / 3)), "▼"))
            {
                if (selectColumnIndexs.Contains(i))
                    selectColumnIndexs.Remove(i);
                else
                    selectColumnIndexs.Add(i);
            }
            if (GUI.Button(new Rect(dragR.x, dragR.y + dragR.height / 3, dragR.width, dragR.height * 2 / 3), i.ToString()))
            {
                if (i == 0)
                    continue;

                List<string> keys = m_currentData.TableKeys;
                string key = keys[i];
                if (EditorUtility.DisplayDialog("警告", "是否删除字段[" + key + "]", "确定", "取消"))
                {
                    DeleteField(m_currentData, key);
                    withItemList.RemoveAt(i);
                    return;
                }
            }

        }
        GUI.EndScrollView();

        //左边格子
        float tempHeight = r.y + wHeight;
        for (int i = 0; i < 2; i++)
        {
            Rect dragR = new Rect(r.x, tempHeight, wWith, heightItemList[i]);
            Rect maxR = new Rect(r.x, tempHeight, wWith, position.height);
            dragR = EditorDrawGUIUtility.DrawCanDragArea(dragR, maxR, null, EditorCanDragAreaSide.Bottom);
            if (dragR.height < minItem)
                dragR.height = minItem;
            heightItemList[i] = dragR.height;

            tempHeight += heightItemList[i];
            string vStr = "";
            if (i == 0)
            {
                vStr = "D";
                GUI.Label(dragR, vStr, buttonStyle);
            }
            else if (i == 1)
            {
                vStr = "description";
                GUI.Label(dragR, vStr, buttonStyle);
            }
          

        }

      
        leftGridSVPos = GUI.BeginScrollView(new Rect(svPos_temp.x, tempHeight,  wWith, svPos_temp.height), leftGridSVPos, new Rect(r.x, tempHeight,  wWith, v.y), false, false, "Label", "Label");
        leftGridSVPos = new Vector2(leftGridSVPos.x, svPos.y);
       
        for (int i = 2; i < heightItemList.Count; i++)
        {
            Rect dragR = new Rect(r.x, tempHeight, wWith, heightItemList[i]);
            Rect maxR = new Rect(r.x, tempHeight, wWith, position.height);
            dragR = EditorDrawGUIUtility.DrawCanDragArea(dragR, maxR, null, EditorCanDragAreaSide.Bottom);
            if (dragR.height < minItem)
                dragR.height = minItem;
            heightItemList[i] = dragR.height;

            tempHeight += heightItemList[i];
            string vStr = "";

            if(selectRowIndexs.Contains(i))
                GUI.color = Color.cyan;

            if (i == 2)
            {
                vStr = "default";
                GUI.Label(dragR, vStr, buttonStyle);
            }
            else
            {
                int num = i - 3;
                vStr = num.ToString();
                if (GUI.Button(new Rect(dragR.position, new Vector2(dragR.width / 4, dragR.height)), "►"))
                {
                    if (selectRowIndexs.Contains(i))
                        selectRowIndexs.Remove(i);
                    else
                        selectRowIndexs.Add(i);
                }
                if (GUI.Button(new Rect(dragR.x + dragR.width / 4, dragR.y, dragR.width * 3 / 4, dragR.height), vStr))
                {
                    if (EditorUtility.DisplayDialog("警告", "是否删除第[" + num + "]行数据", "确定", "取消"))
                    {
                        m_currentData.Remove(m_currentData.TableIDs[num]);
                        m_currentData.TableIDs.RemoveAt(num);


                        heightItemList.RemoveAt(i);
                        return;
                    }
                }
            }
            GUI.color = Color.white;

        }
        GUI.EndScrollView();

        Debug.unityLogger.logEnabled = true;
    }

    private Vector2 GetCententSize()
    {
        float tempWith = 0;
        for (int i = 0; i < withItemList.Count; i++)
        {
            tempWith += withItemList[i];
        }
        tempWith += wWith;

        float tempHeight = 0;
        for (int i = 2; i < heightItemList.Count; i++)
        {

            tempHeight += heightItemList[i];
        }
        tempHeight += wHeight;
        return new Vector2(tempWith, tempHeight);
    }

    private int oldButtonFontSize = 0;
    private int nowButtonFontSize = 0;
    private int MaxButtonFontSize = 40;

    private string searchValue = "";
    private void GridTopFunctionGUI()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("添加一行数据", GUILayout.Width(90)))
        {
            AddLineDataGUI();
        }
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("加载上一次保存", GUILayout.Width(90)))
        {
            DataManager.CleanCache();
            LoadData(chooseFileName);

        }
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("添加字段", GUILayout.Width(90)))
        {
            Add2FieldGUI();
        }
        GUILayout.FlexibleSpace();

        //字体大小调节
        helpBoxStyle.fontSize = helpBoxStyle.font.fontSize;
        oldButtonFontSize = helpBoxStyle.fontSize;
        if (nowButtonFontSize <= 0)
        {  
            nowButtonFontSize = PlayerPrefs.GetInt(FontPlayerPrefKey, oldButtonFontSize); 
        }

        nowButtonFontSize = EditorGUILayout.IntSlider("字体大小", nowButtonFontSize, oldButtonFontSize / 2, MaxButtonFontSize);
        GUILayout.FlexibleSpace();
        searchValue = EditorDrawGUIUtil.DrawSearchField(searchValue);
        GUILayout.EndHorizontal();
    }
    /// <summary>
    /// 绘制每个数据格子
    /// </summary>
    /// <param name="startPos"></param>
    private void DrawGridItem(Vector2 startPos,int heightStartIndex,int heightEndIndex)
    {
        helpBoxStyle.fontSize = nowButtonFontSize;
        float tempHeight = 0;
        for (int i = 0; i < heightStartIndex; i++)
        {
            tempHeight += heightItemList[i];
        }
        for (int i = heightStartIndex; i < heightEndIndex; i++)
        {
            float h = heightItemList[i];
            float tempWith = 0;

          
            for (int j = 0; j < withItemList.Count; j++)
            {
                float w = withItemList[j];

                Vector2 size = new Vector2(w, h);
                Vector2 pos = startPos + new Vector2(tempWith, tempHeight);

                object value = null;
                string showStr = "";

                string field = m_currentData.TableKeys[j];
                FieldType fieldValueType = GetFieldType(j, field);
                string enumType = GetEnumType(fieldValueType, field);
       
                string defaultValue = GetDefaultValue(fieldValueType, enumType, field);


                if (i == 0)
                {
                    GUI.color = Color.yellow;
                  
                    showStr = EditorDrawGUIUtil.GetFormatName(field, DataConfigUtils.ConfigFieldValueType2Type(fieldValueType, enumType), "red");
                }
                else if (i == 1)
                {
                    GUI.color = Color.cyan;

                    showStr = GetDescription(field);
                }
                else if (i == 2)
                {
                    GUI.color = Color.green;

                    showStr = defaultValue;
                }
                else
                {
                    SingleData data = m_currentData[m_currentData.TableIDs[i - 3]];

                    bool isDefault = false;
                    if (data.ContainsKey(field))
                    {
                        if (data[field] != defaultValue)
                        {
                            showStr = data[field];
                        }
                        else
                        {
                            showStr = defaultValue;
                            isDefault = true;
                        }      
                    }
                    else
                    {
                        showStr = defaultValue;
                        isDefault = true;
                    }
                    DataFieldAssetType fieldAssetType = GetDataFieldAssetType(field);
                    if (fieldAssetType == DataFieldAssetType.LocalizedLanguage)
                    {
                        string k = showStr;
                        if (LanguageManager.HaveKey(k))
                        {
                            showStr = LanguageManager.GetContentByKey(k);
                        }
                    }
                    if (!string.IsNullOrEmpty(searchValue))
                        showStr = ShowContainsChar(showStr, searchValue);

                    if (isDefault)
                        showStr = "<color=green>" + showStr + "</color>";
                }
              
                if (i == 1 || i == 2)
                {
                    GUI.Button(new Rect(pos, size), showStr, helpBoxStyle);
                }
                else
                {
                    if (selectColumnIndexs.Contains(j))
                        GUI.color = Color.magenta;
                    if (selectRowIndexs .Contains(i))
                        GUI.color = Color.cyan;
                    if (GUI.Button(new Rect(pos, size), showStr, helpBoxStyle))
                    {


                        modifiIndex = new TableIndex(i - 2, j);
                        if (i == 0)
                        {
                            TableConfigFieldInfo f = new TableConfigFieldInfo();
                            f.fieldName = field;
                            f.description = m_currentData.m_noteValue.ContainsKey(field) ? m_currentData.m_noteValue[field] : "";
                            f.fieldValueType = fieldValueType;
                            f.defultValue = DataConfigUtils.TableString2ObjectValue(defaultValue, fieldValueType, enumType);
                            f.enumType = enumType;
                            f.fieldAssetType = GetDataFieldAssetType(field);
                            value = f;
                        }
                        else
                        {
                            SingleData data = m_currentData[m_currentData.TableIDs[i - 3]];

                            if (data.ContainsKey(field))
                                defaultValue = data[field];
                            value = DataConfigUtils.TableString2ObjectValue(defaultValue, fieldValueType, enumType);
                        }
                        GeneralDataModificationWindow.OpenWindow(this, "修改数据", value, DrawModifiValueGUI, CheckModifiValueCallBack, ModificationCompleteCallBack);
                    }
                }
               
                GUI.color = Color.white;
                tempWith += w;
            }

            tempHeight += h;
        }
        helpBoxStyle.fontSize = oldButtonFontSize;
    }
    private string ShowContainsChar(string value,string searchValue)
    {
        string res = value;
        if (value.Contains(searchValue))
        {
           res= value.Replace(searchValue, "<color=red>" + searchValue + "</color>");
        }

        return res;
    }
    private DataFieldAssetType GetDataFieldAssetType(string field)
    {
        DataFieldAssetType type;
        if (!m_currentData.m_fieldAssetTypes.TryGetValue(field,out type))
        {
            type = DataFieldAssetType.Data;
        }
        return type;
    }

    private FieldType GetFieldType(int index, string field)
    {
        FieldType fieldValueType = index == 0 ? FieldType.String : m_currentData.m_tableTypes[field];
        return fieldValueType;
    }
    private string GetEnumType(FieldType fieldValueType,  string field)
    {
        string enumType = fieldValueType == FieldType.Enum ?
                   (m_currentData.m_tableEnumTypes.ContainsKey(field) ? m_currentData.m_tableEnumTypes[field] : EditorTool.GetAllEnumType()[0])
                   : "";
        return enumType;
    }

    private string GetDefaultValue(FieldType fieldValueType, string enumType, string field)
    {
        string defaultValue;

        if (!m_currentData.m_defaultValue.TryGetValue(field, out defaultValue))
        {
            Type t = DataConfigUtils.ConfigFieldValueType2Type(fieldValueType, enumType);
            object obj = ReflectionUtils.CreateDefultInstance(t);
            defaultValue = DataConfigUtils.ObjectValue2TableString(obj);
            m_currentData.m_defaultValue.Add(field, defaultValue);
        }

        return defaultValue;
    }
    private string GetDescription(string field)
    {
        string value;
        if (!m_currentData.m_noteValue.TryGetValue(field, out value))
        {
            value = "";
            m_currentData.m_noteValue.Add(field, value);
        }
        return value;
    }

    /// <summary>
    /// 绘制数据修改窗口
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    private object DrawModifiValueGUI(object t)
    {
        if (t is TableConfigFieldInfo)
        {
            TableConfigFieldInfo info = (TableConfigFieldInfo)t;
            DrawTableConfigFieldInfo(info);
            string df = DataConfigUtils.ObjectValue2TableString(info.defultValue);
            if (string.IsNullOrEmpty(df))
                EditorGUILayout.HelpBox("默认值不能为空！！", MessageType.Error);
            if (CheckIsNameRepeat(info))
                EditorGUILayout.HelpBox("字段名不能重复！！", MessageType.Error);
        }
        else
        {
            string field = m_currentData.TableKeys[modifiIndex.y];
            SingleData data = m_currentData[m_currentData.TableIDs[modifiIndex.x-1]];
            
            EditorDrawGUIUtil.CanEdit = false;
            field = EditorDrawGUIUtil.DrawBaseValue("字段名", field).ToString();
            EditorDrawGUIUtil.CanEdit = true;
            GUILayout.Space(7);

            string description = GetDescription(field);
            GUILayout.Label("字段描述：" + description);

            FieldType fieldValueType = GetFieldType(modifiIndex.y, field);
            string enumType = GetEnumType(fieldValueType, field);

            string defaultValue = GetDefaultValue(fieldValueType, enumType, field);
            DataFieldAssetType fieldAssetType = GetDataFieldAssetType(field);
            GUILayout.BeginHorizontal();

            GUILayout.Label("默认值 :" + defaultValue);
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("默认值", GUILayout.Width(60)))
            {
                t = DataConfigUtils.TableString2ObjectValue(defaultValue, fieldValueType, enumType);
            }
            GUILayout.EndHorizontal();

            string text = "值";
            if (fieldAssetType == DataFieldAssetType.LocalizedLanguage)
            {
                t = DrawLocalizedLanguageField(text, t);

            }
            else if (fieldAssetType == DataFieldAssetType.Prefab)
            {
                t = DrawPrefabGUI(text, t);
            }
            else if (fieldAssetType == DataFieldAssetType.Texture)
            {
                t = DrawTextureGUI(text, t);
            }
            else if (fieldAssetType == DataFieldAssetType.TableKey)
            {
                t = DrawTableGUI(text, t);
            }
            else
            {
                t = EditorDrawGUIUtil.DrawBaseValue(text, t);
            }
        }

        return t;
    }
    private bool CheckModifiValueCallBack(object t)
    {
        if (t is TableConfigFieldInfo)
        {
            TableConfigFieldInfo info = (TableConfigFieldInfo)t;

            if (string.IsNullOrEmpty(info.fieldName))
                return false;

            if (CheckIsNameRepeat(info))
                return false;

            string df = DataConfigUtils.ObjectValue2TableString(info.defultValue);
            if (string.IsNullOrEmpty(df))
                return false;

        }

        return true;
    }
    private bool CheckIsNameRepeat(TableConfigFieldInfo info)
    {
        string field = m_currentData.TableKeys[modifiIndex.y];

        for (int i = 0; i < m_currentData.TableKeys.Count; i++)
        {
            string f = m_currentData.TableKeys[i];
            if (i == modifiIndex.y)
                continue;
            else
            {
                if (f == info.fieldName)
                    return true;
            }
        }

        return false;
    }
    /// <summary>
    /// 绘制字段信息
    /// </summary>
    /// <param name="temp"></param>
    private void DrawTableConfigFieldInfo(TableConfigFieldInfo temp)
    {
        Type type = DataConfigUtils.ConfigFieldValueType2Type(temp.fieldValueType,temp.enumType);
        if (temp.defultValue == null || type.FullName != temp.defultValue.GetType().FullName)
            temp.defultValue = ReflectionUtils.CreateDefultInstance(type);

        //是否使用多语言字段
      

        GUILayout.BeginVertical("box");

        GUILayout.EndVertical();
        temp = (TableConfigFieldInfo)EditorDrawGUIUtil.DrawClassData("字段信息", temp, new List<string>() { "defultValue", "enumType" }, null, () =>
        {
            if (temp.fieldAssetType == DataFieldAssetType.LocalizedLanguage
            || temp.fieldAssetType== DataFieldAssetType.Prefab
            || temp.fieldAssetType == DataFieldAssetType.TableKey
            || temp.fieldAssetType== DataFieldAssetType.Texture)
            {
                if (type != typeof(string))
                {
                    temp.fieldAssetType = DataFieldAssetType.Data;
                }
            }
            string text = "默认值";
            if (temp.fieldAssetType== DataFieldAssetType.LocalizedLanguage)
            {
                temp.defultValue = DrawLocalizedLanguageField(text, temp.defultValue);
            }
            else if(temp.fieldAssetType == DataFieldAssetType.Prefab)
            {
              temp.defultValue= DrawPrefabGUI(text, temp.defultValue);
            }
            else if(temp.fieldAssetType == DataFieldAssetType.Texture)
            {
                temp.defultValue = DrawTextureGUI(text, temp.defultValue);
            }
            else if (temp.fieldAssetType == DataFieldAssetType.TableKey)
            {
                temp.defultValue = DrawTableGUI(text, temp.defultValue);
            }
            else
            {
                temp.defultValue = EditorDrawGUIUtil.DrawBaseValue(text, temp.defultValue);
            }
            if(temp.fieldValueType== FieldType.Enum)
            {
                List<string> enumList = new List<string>(EditorTool.GetAllEnumType());
                temp.enumType = EditorDrawGUIUtil.DrawPopup("枚举类型", temp.enumType, enumList);
            }
        });
    }

    private List<string> configFileNames = new List<string>();
    private List<string> fieldNames = new List<string>();
    private List<string> tableKeys = new List<string>();
    private string tableName = "";
    private string fieldName = "";
    private string tableKey = "";
    private object DrawTableGUI(string text, object defultValue)
    {
        string value = defultValue.ToString();
        GUILayout.Label(text + " : " + value);

        if(string.IsNullOrEmpty(tableName) && !string.IsNullOrEmpty(value))
        {
            string[] tempStrs = value.Split('/');
            if(tempStrs.Length==3)
            {
                tableName = tempStrs[0];
                fieldName = tempStrs[1];
                tableKey = tempStrs[2];
            }
        }
        tableName = EditorDrawGUIUtil.DrawPopup("表格名", tableName, configFileNames, (tName) =>
        {
            if (string.IsNullOrEmpty(tName))
                return;
            fieldNames.Clear();
            DataTable d = DataManager.GetData(tName);
            fieldNames.AddRange(d.TableKeys);
            tableKeys.AddRange(d.TableIDs);
        });
        
        fieldName = EditorDrawGUIUtil.DrawPopup("字段名", fieldName, fieldNames);
        tableKey = EditorDrawGUIUtil.DrawPopup("表格数据key", tableKey, tableKeys);

        defultValue = tableName + "/" + fieldName + "/" + tableKey;
        return defultValue;
    }

    private object DrawLocalizedLanguageField(string text, object value)
    {
        value = EditorDrawGUIUtil.DrawPopup(text, value.ToString(), langKeys);
        GUILayout.Space(6);
        GUILayout.Label("多语言字段[" + value + "] : " + LanguageManager.GetContentByKey(value.ToString()));
        GUILayout.Space(8);
        if (GUILayout.Button("打开编辑当前多语言字段"))
        {
            LanguageDataEditorWindow w = LanguageDataEditorWindow.ShowWindow();
            w.searchValue = value.ToString();
            w.toolbarOption = 1;
        }
        return value;
    }

    private Editor previewEditor;
    private GameObject previewObj;
    private object editPrefabValue = null;
    private object DrawPrefabGUI(string text, object value)
    {
        if (editPrefabValue != null)
        {
            value = editPrefabValue;
            editPrefabValue = null;
        }

        GUILayout.BeginHorizontal();
        value= EditorDrawGUIUtil.DrawBaseValue(text, value);
        if (GUILayout.Button("o", GUILayout.Width(20)))
        {
            ObjectSelectorWindow.Show(GeneralDataModificationWindow.GetInstance(), value.ToString(),
                new string[] { "Assets/Resources" },
                typeof(GameObject),
                (assetName, obj) =>
                {
                    editPrefabValue = assetName;
                }
                );
        }
        GUILayout.EndHorizontal();
        string content = value.ToString();
        bool isHave = false;
        try
        {
            ResourcesConfigManager.Initialize();
            if (ResourcesConfigManager.GetIsExitRes(content))
            {
                GameObject obj = ResourceManager.Load<GameObject>(content);
                if (obj)
                {

                    if(obj!= previewObj)
                    {
                        previewEditor = Editor.CreateEditor(obj);
                        previewObj = obj;
                    }

                    previewEditor.OnPreviewGUI(GUILayoutUtility.GetRect(300, 300), EditorStyles.helpBox);
                    isHave = true;
                }
            }
        }
        catch { }

        if (!isHave)
        {
            EditorGUILayout.HelpBox("没有预制 ["+value+"] ！！", MessageType.Error);
        }

        return value;
    }
    private Editor previewEditor1;
    private Texture2D previewTex;
    private object editTextureValue;
    private object DrawTextureGUI(string text, object value)
    {
        if (editTextureValue != null)
        {
            value = editTextureValue;
            editTextureValue = null;
        }

        GUILayout.BeginHorizontal();
        value = EditorDrawGUIUtil.DrawBaseValue(text, value);
        if (GUILayout.Button("o", GUILayout.Width(20)))
        {
            ObjectSelectorWindow.Show(GeneralDataModificationWindow.GetInstance(), value.ToString(),
                new string[] { "Assets/Resources" },
                typeof(Texture),
                (assetName, obj) =>
                {
                    editTextureValue = assetName;
                }
                );
        }
        GUILayout.EndHorizontal();
        string content = value.ToString();
        bool isHave = false;
        try
        {
            ResourcesConfigManager.Initialize();
            if (ResourcesConfigManager.GetIsExitRes(content))
            {
                Texture2D obj = ResourceManager.Load<Texture2D>(content);
                if (obj)
                {

                    if (obj != previewTex)
                    {
                        previewEditor1 = Editor.CreateEditor(obj);
                        previewTex = obj;
                    }
                    previewEditor1.OnPreviewGUI(GUILayoutUtility.GetRect(300, 300), EditorStyles.helpBox);
                    isHave = true;
                }
            }
        }
        catch { }

        if (!isHave)
        {
            EditorGUILayout.HelpBox("没有图片 [" + value + "] ！！", MessageType.Error);
        }

        return value;
    }
    private TableIndex modifiIndex = new TableIndex();


    /// <summary>
    /// 修改格子数据完成回调
    /// </summary>
    /// <param name="t"></param>
    private void ModificationCompleteCallBack(object t)
    {
        if (t is TableConfigFieldInfo)
        {
            TableConfigFieldInfo temp = (TableConfigFieldInfo)t;

            string field = m_currentData.TableKeys[modifiIndex.y];
            if (field != temp.fieldName)
            {
                RenameField(m_currentData, field, temp.fieldName);
                field = temp.fieldName;
            }
           // SingleData data = m_currentData[m_currentData.TableIDs[modifiIndex.y]];
            m_currentData.m_defaultValue[field] = DataConfigUtils.ObjectValue2TableString(temp.defultValue);

            if (m_currentData.m_noteValue.ContainsKey(field))
                m_currentData.m_noteValue[field] = temp.description;
            else
            {
                if (!string.IsNullOrEmpty(temp.description))
                {
                    m_currentData.m_noteValue.Add(field, temp.description);
                }
            }

            if (m_currentData.m_tableTypes[field] != temp.fieldValueType)
            {
                m_currentData.m_tableTypes[field] = temp.fieldValueType;
                ResetDataField(m_currentData, field, temp.fieldValueType, temp.enumType,temp.defultValue);
            }
            if (temp.fieldValueType == FieldType.Enum)
            {
                if (m_currentData.m_tableEnumTypes.ContainsKey(field))
                    
                    m_currentData.m_tableEnumTypes[field] = temp.enumType;

                else
                    m_currentData.m_tableEnumTypes.Add(field, temp.enumType);
            }
            m_currentData.m_fieldAssetTypes[field] = temp.fieldAssetType;
        }
        else
        {
            string field = m_currentData.TableKeys[modifiIndex.y];
            SingleData data = m_currentData[m_currentData.TableIDs[modifiIndex.x-1]];
            data[field] = DataConfigUtils.ObjectValue2TableString(t);
        }
    }

    /// <summary>
    /// 添加一个字段
    /// </summary>
    private void Add2FieldGUI()
    {     
        GeneralDataModificationWindow.OpenWindow(this, "添加字段", new TableConfigFieldInfo(), (value) =>
        {
            TableConfigFieldInfo info = (TableConfigFieldInfo)value;
            DrawTableConfigFieldInfo(info);
            
            if (string.IsNullOrEmpty(info.fieldName))
                EditorGUILayout.HelpBox("字段名不能为空！！", MessageType.Error);
            else if (m_currentData.TableKeys.Contains(info.fieldName))
                EditorGUILayout.HelpBox("字段名重复！！", MessageType.Error);
            string df = DataConfigUtils.ObjectValue2TableString(info.defultValue);
            if(string.IsNullOrEmpty(df))
                EditorGUILayout.HelpBox("默认值不能为空！！", MessageType.Error);
            return value;
        },
        (value) =>
        {
            TableConfigFieldInfo info = (TableConfigFieldInfo)value;
            if (string.IsNullOrEmpty(info.fieldName) || m_currentData.TableKeys.Contains(info.fieldName))
                return false;
            string df = DataConfigUtils.ObjectValue2TableString(info.defultValue);
            if (string.IsNullOrEmpty(df))
                return false;
            return true;

        },
        (value) =>
        {
            TableConfigFieldInfo info = (TableConfigFieldInfo)value;
            AddField(m_currentData, info);
            withItemList.Add(wWith);
        });
    }
  
    /// <summary>
    /// 添加一行数据
    /// </summary>
    private void AddLineDataGUI()
    {
        GeneralDataModificationWindow.OpenWindow(this, "插入一行数据", "", (value) =>
        { 
            value =  EditorDrawGUIUtil.DrawBaseValue("Key:", value);
            if (string.IsNullOrEmpty(value.ToString()))
                EditorGUILayout.HelpBox("Key不能为空！！", MessageType.Error);
            else if (m_currentData.TableKeys.Contains(value.ToString()))
                EditorGUILayout.HelpBox("Key重复！！", MessageType.Error);
            return value;
        },
         (value) =>
         {

             if (string.IsNullOrEmpty(value.ToString()) || m_currentData.TableKeys.Contains(value.ToString()))
                 return false;

             return true;

         },
       (value) =>
       {
           heightItemList.Add(30);
           SingleData data = new SingleData();
           DataTable table = m_currentData;
           List<string> keys = table.TableKeys;
           for (int i = 0; i < keys.Count; i++)
           {
               string keyTmp = keys[i];
               if (i == 0)
               {
                   data.Add(keyTmp, value.ToString());
               }
               else
               data.Add(keyTmp, table.m_defaultValue[keyTmp]);
           }
           m_currentData.AddData(data);
       });

       
    }
    #region 数据文件相关

  

    void CleanCacheGUI()
    {
        if (GUILayout.Button("清除缓存"))
        {
            LanguageManager.IsInit = false;
            DataManager.CleanCache();
        }
    }

    #endregion

    #region 记录相关


    #endregion

    #region 字段相关

    static private void RenameField(DataTable table,string oldFieldName,string newFieldName)
    {
        int indexFiled = table.TableKeys.IndexOf(oldFieldName);
        table.TableKeys[indexFiled] = newFieldName;

        table.m_noteValue = RenameDictionaryKey(table.m_noteValue, oldFieldName, newFieldName);
        table.m_tableTypes = RenameDictionaryKey(table.m_tableTypes, oldFieldName, newFieldName);
        table.m_tableEnumTypes = RenameDictionaryKey(table.m_tableEnumTypes, oldFieldName, newFieldName);
        table.m_defaultValue = RenameDictionaryKey(table.m_defaultValue, oldFieldName, newFieldName);
        foreach (var item in table.TableIDs)
        {
            table[item] = (SingleData)RenameDictionaryKey(table[item], oldFieldName, newFieldName);
        }
    }
    private static Dictionary<K, V> RenameDictionaryKey<K,V>(Dictionary<K,V> dic , K oldFieldName, K newFieldName)
    {
        if (!dic.ContainsKey(oldFieldName))
            return dic;
        List<K> keys = new List<K>(dic.Keys);
        List<V> values = new List<V>(dic.Values);
        int indexFiled = keys.IndexOf(oldFieldName);
       keys[indexFiled] = newFieldName;

        dic.Clear();
        for (int i = 0; i < keys.Count; i++)
        {
            dic.Add(keys[i], values[i]);
        }

        return dic;
    }
    /// <summary>
    /// 添加字段
    /// </summary>
    /// <param name="info"></param>
    private static  void AddField(DataTable table, TableConfigFieldInfo info)
    {
        table.TableKeys.Add(info.fieldName);
        table.m_noteValue.Add(info.fieldName, info.description);
        table.m_tableTypes.Add(info.fieldName, info.fieldValueType);
        table.m_fieldAssetTypes.Add(info.fieldName, info.fieldAssetType);
        if (info.fieldValueType == FieldType.Enum)
        {
            table.m_tableEnumTypes.Add(info.fieldName, info.enumType);
        }
        table.m_defaultValue.Add(info.fieldName, DataConfigUtils.ObjectValue2TableString(info.defultValue));

        foreach (var item in table.Values)
        {
            item.Add(info.fieldName, DataConfigUtils.ObjectValue2TableString(info.defultValue));
        }
    }

    static void DeleteField(DataTable table,string fieldName)
    {
        table.TableKeys.Remove(fieldName);
        if (table.m_noteValue.ContainsKey(fieldName))
        {
            table.m_noteValue.Remove(fieldName);
        }
        if (table.m_tableTypes.ContainsKey(fieldName))
        {
            table.m_tableTypes.Remove(fieldName);
        }
        if (table.m_tableEnumTypes.ContainsKey(fieldName))
        {
            table.m_tableEnumTypes.Remove(fieldName);
        }
        if (table.m_defaultValue.ContainsKey(fieldName))
        {
            table.m_defaultValue.Remove(fieldName);
        }
        foreach (var item in table.Values)
        {
            if (item.ContainsKey(fieldName))
                item.Remove(fieldName);
        }
    } 

    static void ResetDataField(DataTable data,string key,FieldType type,string enumType,object defaultValue)
    {
        string newContent = DataConfigUtils.ObjectValue2TableString(defaultValue);

        for (int i = 0; i < data.TableIDs.Count; i++)
        {
            SingleData tmp = data[data.TableIDs[i]];

            if (tmp.ContainsKey(key))
            {
                tmp[key] = newContent;
            }
        }

        if (data.m_defaultValue.ContainsKey(key))
        {
            data.m_defaultValue[key] = newContent;
        }
        else
        {
            data.m_defaultValue.Add(key, newContent);
        }
    }

    void SaveDataGUI()
    {
        //if (GUILayout.Button("Change"))
        //{
        //    foreach (var sd in m_currentData.Values)
        //    {
        //        foreach (var item in sd)
        //        {
        //            if(item.Key== "DescribeKey")
        //            {
        //                sd[item.Key]= "{[0,4][" + item.Value + "]}";
        //                Debug.Log(m_currentData.Count + " " + item.Key);
        //                break;
        //            }
        //        }
        //    }
        //}
        if (GUILayout.Button("保存"))
        {
            SaveData(chooseFileName, m_currentData);
            AssetDatabase.Refresh();
            //LoadData(chooseFileName);
        }
    }

    void GenerateDataClassGUI()
    {
        if (GUILayout.Button("生成Data类"))
        {
            CreatDataCSharpFile(chooseFileName, m_currentData);
            AssetDatabase.Refresh();
        }
    }

    void  GenerateAllDataClassGUI()
    {
        if (GUILayout.Button("生成全部Data类"))
        {
            CreatAllClass();
        }
    }

    #endregion

    #endregion

    #region FindData

    string m_directoryPath;

    void FindAllDataName()
    {
        AssetDatabase.Refresh();
        m_dataNameList = new List<string>();

        m_dataNameList.Add("None");

        m_directoryPath = Application.dataPath + "/Resources/" + DataManager.c_directoryName;

        FindConfigName(m_directoryPath);
    }

    public void FindConfigName(string path)
    {
        string[] allUIPrefabName = Directory.GetFiles(path);
        foreach (var item in allUIPrefabName)
        {
            if (item.EndsWith(".txt"))
            {
                //string configName = FileTool.RemoveExpandName(FileTool.GetFileNameByPath(item));
                m_dataNameList.Add(FileTool.RemoveExpandName(PathTool.GetDirectoryRelativePath(m_directoryPath + "/",item)));
            }
        }

        string[] dires = Directory.GetDirectories(path);
        for (int i = 0; i < dires.Length; i++)
        {
            FindConfigName(dires[i]);
        }
    }

    #endregion

    #region 自动生成代码

    void CreatAllClass()
    {
        for (int i = 0; i < m_dataNameList.Count; i++)
        {
            if (m_dataNameList[i] != null && m_dataNameList[i] !="None")
            {
                CreatDataCSharpFile(m_dataNameList[i],DataManager.GetData(m_dataNameList[i]));
            }
        }

        UnityEditor.AssetDatabase.Refresh();
    }

    void CreatDataCSharpFile(string dataName,DataTable data)
    {
        if (dataName.Contains("/"))
        {
            string[] tmp = dataName.Split('/');
            dataName = tmp[tmp.Length - 1];
        }

        string className = dataName + "Generate";
        string content = "";

        content += "using System;\n";
        content += "using UnityEngine;\n\n";

        content += @"//" + className + "类\n";
        content += @"//该类自动生成请勿修改，以避免不必要的损失";
        content += "\n";

        content += "public class " + className + " : DataGenerateBase \n";
        content += "{\n";

        content += "\tpublic string m_key;\n";

        //type
        List<string> type = new List<string>(data.m_tableTypes.Keys);

        Debug.Log("type count: "+type.Count);

        if (type.Count > 0)
        {
            for (int i = 1; i < data.TableKeys.Count; i++)
            {
                string key = data.TableKeys[i];
                string enumType = null;

                if (data.m_tableEnumTypes.ContainsKey(key))
                {
                    enumType = data.m_tableEnumTypes[key];
                }

                string note = ";";

                if (data.m_noteValue.ContainsKey(key))
                {
                    note = @"; //" + data.m_noteValue[key];
                }

                content +="\t";

                if (data.m_tableTypes.ContainsKey(key))
                {
                    //访问类型 + 字段类型  + 字段名
                    content += "public " + OutPutFieldName(data.m_tableTypes[key], enumType) + " m_" + key + note;
                }
                //默认字符类型
                else
                {
                    //访问类型 + 字符串类型 + 字段名 
                    content += "public " + "string" + " m_" + key + note;
                }

                content += "\n";
            }
        }

        content += "\n";

        content += "\tpublic override void LoadData(string key) \n";
        content += "\t{\n";
        content += "\t\tDataTable table =  DataManager.GetData(\"" + dataName + "\");\n\n";
        content += "\t\tif (!table.ContainsKey(key))\n";
        content += "\t\t{\n";
        content += "\t\t\tthrow new Exception(\"" + className + " LoadData Exception Not Fond key ->\" + key + \"<-\");\n";
        content += "\t\t}\n";
        content += "\n";
        content += "\t\tSingleData data = table[key];\n\n";

        content += "\t\tm_key = key;\n";

        if (type.Count > 0)
        {
            for (int i = 1; i < data.TableKeys.Count; i++)
            {
                string key = data.TableKeys[i];

                content += "\t\t";

                string enumType = null;

                if (data.m_tableEnumTypes.ContainsKey(key))
                {
                    enumType = data.m_tableEnumTypes[key];
                }

                if (data.m_tableTypes.ContainsKey(key))
                {
                    content += "m_" + key + " = data." + OutPutFieldFunction(data.m_tableTypes[key], enumType) + "(\"" + key + "\")";
                }
                //默认字符类型
                else
                {
                    content += "m_" + key + " = data." + OutPutFieldFunction(FieldType.String, enumType) + "(\"" + key + "\")";
                    Debug.LogWarning("字段 " + key + "没有配置类型！");
                }

                content += ";\n";
            }
        }

        content += "\t}\n";
        content += "}\n";

        string SavePath = Application.dataPath + "/Script/DataClassGenerate/" + className + ".cs";

        EditorUtil.WriteStringByFile(SavePath, content.ToString());
    }

    string OutPutFieldFunction(FieldType fileType,string enumType)
    {
        switch (fileType)
        {
            case FieldType.Bool: return "GetBool";
            case FieldType.Color: return "GetColor";
            case FieldType.Float: return "GetFloat";
            case FieldType.Int: return "GetInt";
            case FieldType.String: return "GetString";
            case FieldType.Vector2: return "GetVector2";
            case FieldType.Vector3: return "GetVector3";
            case FieldType.Enum: return "GetEnum<" + enumType + ">";

            case FieldType.StringArray: return "GetStringArray";
            case FieldType.IntArray: return "GetIntArray";
            case FieldType.FloatArray: return "GetFloatArray";
            case FieldType.BoolArray: return "GetBoolArray";
            case FieldType.Vector2Array: return "GetVector2Array";
            case FieldType.Vector3Array: return "GetVector3Array";
            default: return "";
        }
    }

    string OutPutFieldName(FieldType fileType, string enumType)
    {
        switch (fileType)
        {
            case FieldType.Bool: return "bool";
            case FieldType.Color: return "Color";
            case FieldType.Float: return "float";
            case FieldType.Int: return "int";
            case FieldType.String: return "string";
            case FieldType.Vector2: return "Vector2";
            case FieldType.Vector3: return "Vector3";
            case FieldType.Enum: return enumType;

            case FieldType.StringArray: return "string[]";
            case FieldType.IntArray: return "int[]";
            case FieldType.FloatArray: return "float[]";
            case FieldType.BoolArray: return "bool[]";
            case FieldType.Vector2Array: return "Vector2[]";
            case FieldType.Vector3Array: return "Vector3[]";
            default: return "";
        }
    }

    #endregion

    #region Utf-8转换

    public static void ConvertUtf8()
    {
        FileTool.RecursionFileExecute(Application.dataPath + "/Resources/" + DataManager.c_directoryName, "txt", (name) =>
        {
            FileTool.ConvertFileEncoding(name, null, System.Text.Encoding.UTF8);
        });
    }

    #endregion

    #region 保存数据

    public static bool GetIsExistDataEditor(string DataName)
    {
        return "" != ResourceIOTool.ReadStringByResource(
                        PathTool.GetRelativelyPath(DataManager.c_directoryName,
                                                    DataName,
                                                    DataManager.c_expandName));
    }

    public static void SaveData(string ConfigName, DataTable data)
    {
        EditorUtil.WriteStringByFile(
            PathTool.GetAbsolutePath(
                ResLoadLocation.Resource,
                PathTool.GetRelativelyPath(
                    DataManager.c_directoryName,
                    ConfigName,
                    DataManager.c_expandName)),
            DataTable.Serialize(data));

        UnityEditor.AssetDatabase.Refresh();
    }

    /// <summary>
    /// 读取编辑器数据
    /// </summary>
    /// <param name="ConfigName">数据名称</param>
    public static Dictionary<string, object> GetEditorData(string dataName)
    {
        UnityEditor.AssetDatabase.Refresh();

        string dataJson = ResourceIOTool.ReadStringByFile(PathTool.GetEditorPath(DataManager.c_directoryName, dataName, DataManager.c_expandName));

        if (dataJson == "")
        {
            Debug.Log(dataName + " dont find!");
            return new Dictionary<string, object>();
        }
        else
        {
            return Json.Deserialize(dataJson) as Dictionary<string, object>;
        }
    }

    /// <summary>
    /// 保存编辑器数据
    /// </summary>
    /// <param name="ConfigName">数据名称</param>
    /// <param name="data">数据表</param>
    public static void SaveEditorData(string ConfigName, Dictionary<string, object> data)
    {
        string configDataJson = Json.Serialize(data);

        EditorUtil.WriteStringByFile(PathTool.GetEditorPath(DataManager.c_directoryName, ConfigName, DataManager.c_expandName), configDataJson);

        UnityEditor.AssetDatabase.Refresh();
    }

#endregion
}

/// <summary>
/// 字段描述信息
/// </summary>
public class TableConfigFieldInfo
{
    [ShowGUIName("字段名")]
    public string fieldName = "";
    [ShowGUIName("描述")]
    public string description = "";
    [ShowGUIName("数据类型")]
    public FieldType fieldValueType;
    [ShowGUIName("数据用途")]
    public DataFieldAssetType fieldAssetType;
    [ShowGUIName("默认值")]
    public object defultValue = null;
    public string enumType = "";

}

public struct TableIndex
{
    public TableIndex(int x ,int y)
    {
        this.x = x;
        this.y = y;
    }

    public int x;
    public int y;
}


