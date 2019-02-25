using FrameWork.GuideSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class GuideSystemEditor
{
    [MenuItem("Tools/新手引导/增加Guide引导层")]
    public static void ShowAllMethod()
    {
        //增加Guide引导层
        EditorExpand.AddSortLayerIfNotExist("Guide");
    }

    [MenuItem("Tools/新手引导/初始化")]
    public static void InitGuideSystem()
    {
        if(!GetGuideIsInit())
        {
            //创建数据表
            SaveDataTable();

            //创建脚本
            CreateGuideWindowScript();

            //增加Guide引导层

            EditorExpand.AddSortLayerIfNotExist("Guide");

            //添加引导开关
            RecordManager.SaveRecord(GuideSystemBase.c_guideRecordName, GuideSystemBase.c_guideSwitchName, true);
            
        }
        else
        {
            Debug.Log("新手引导已经初始化");
        }
    }

    [MenuItem("Tools/新手引导/创建新手引导预设")]
    public static void CreateGuideSystemWidnow()
    {
        if (GetGuideIsInit())
        {
            if(!GetGuideIsCreate())
            {
                //创建预设
                UICreateService.CreateGuideWindow();
            }
            else
            {
                Debug.LogError("新手引导预设已经创建");
            }
        }
        else
        {
            Debug.LogError("新手引导没有初始化");
        }
    }

    static bool GetGuideIsInit()
    {
        string path = Application.dataPath + "/Resources/"+ DataManager .c_directoryName + "/" + GuideSystemBase.c_guideDataName + "." + DataManager.c_expandName;
        return File.Exists(path);
    }

    static bool GetGuideIsCreate()
    {
        string path = Application.dataPath + "/Resources/UI/GuideWindow/GuideWindow.perfab" ;
        return File.Exists(path);
    }

    static void SaveDataTable()
    {
        DataTable data = new DataTable();

        data.TableKeys.Add("GuideID");

        data.TableKeys.Add(GuideSystemBase.c_guideStartPoint);
        data.SetDefault(GuideSystemBase.c_guideStartPoint, "False");
        data.SetNote(GuideSystemBase.c_guideStartPoint, "引导开始点");
        data.SetFieldType(GuideSystemBase.c_guideStartPoint, FieldType.Bool, null);

        data.TableKeys.Add(GuideSystemBase.c_guideEndPoint);
        data.SetDefault(GuideSystemBase.c_guideEndPoint, "False");
        data.SetNote(GuideSystemBase.c_guideEndPoint, "引导结束点");
        data.SetFieldType(GuideSystemBase.c_guideEndPoint, FieldType.Bool, null);

        data.TableKeys.Add(GuideSystemBase.c_guideClosePoint);
        data.SetDefault(GuideSystemBase.c_guideEndPoint, "False");
        data.SetNote(GuideSystemBase.c_guideEndPoint, "引导关闭点");
        data.SetFieldType(GuideSystemBase.c_guideClosePoint, FieldType.Bool, null);

        data.TableKeys.Add(GuideSystemBase.c_PremiseKey);
        data.SetDefault(GuideSystemBase.c_PremiseKey, "Null");
        data.SetNote(GuideSystemBase.c_PremiseKey, "前提条件");
        data.SetFieldType(GuideSystemBase.c_PremiseKey, FieldType.String, null);

        data.TableKeys.Add(GuideSystemBase.c_NextGuideNameKey);
        data.SetDefault(GuideSystemBase.c_NextGuideNameKey, "Null");
        data.SetNote(GuideSystemBase.c_NextGuideNameKey, "下一步引导,如果为空,则为下一条记录");
        data.SetFieldType(GuideSystemBase.c_NextGuideNameKey, FieldType.String, null);

        data.TableKeys.Add(GuideSystemBase.c_ClickToNextKey);
        data.SetDefault(GuideSystemBase.c_ClickToNextKey, "False");
        data.SetNote(GuideSystemBase.c_ClickToNextKey, "是否接收点击去下一步引导");
        data.SetFieldType(GuideSystemBase.c_ClickToNextKey, FieldType.Bool, null);

        data.TableKeys.Add(GuideSystemBase.c_CallToNextKey);
        data.SetDefault(GuideSystemBase.c_CallToNextKey, "False");
        data.SetNote(GuideSystemBase.c_CallToNextKey, "是否接收调用去下一步引导");
        data.SetFieldType(GuideSystemBase.c_CallToNextKey, FieldType.Bool, null);

        data.TableKeys.Add(GuideSystemBase.c_CustomEventKey);
        data.SetDefault(GuideSystemBase.c_CustomEventKey, "Null");
        data.SetNote(GuideSystemBase.c_CustomEventKey, "自定义事件名称");
        data.SetFieldType(GuideSystemBase.c_CustomEventKey, FieldType.StringArray, null);

        data.TableKeys.Add(GuideSystemBase.c_ConditionToNextKey);
        data.SetDefault(GuideSystemBase.c_ConditionToNextKey, "False");
        data.SetNote(GuideSystemBase.c_ConditionToNextKey, "是否自动判断条件去下一步引导");
        data.SetFieldType(GuideSystemBase.c_ConditionToNextKey, FieldType.Bool, null);

        data.TableKeys.Add(GuideSystemBase.c_GuideWindowNameKey);
        data.SetDefault(GuideSystemBase.c_GuideWindowNameKey, "Null");
        data.SetNote(GuideSystemBase.c_GuideWindowNameKey, "引导的界面名字");
        data.SetFieldType(GuideSystemBase.c_GuideWindowNameKey, FieldType.String, null);

        data.TableKeys.Add(GuideSystemBase.c_GuideObjectNameKey);
        data.SetDefault(GuideSystemBase.c_GuideObjectNameKey, "Null");
        data.SetNote(GuideSystemBase.c_GuideObjectNameKey, "高亮显示的对象名字");
        data.SetFieldType(GuideSystemBase.c_GuideObjectNameKey, FieldType.StringArray, null);

        data.TableKeys.Add(GuideSystemBase.c_GuideItemNameKey);
        data.SetDefault(GuideSystemBase.c_GuideItemNameKey, "Null");
        data.SetNote(GuideSystemBase.c_GuideItemNameKey, "高亮的Item名字");
        data.SetFieldType(GuideSystemBase.c_GuideItemNameKey, FieldType.StringArray, null);

        data.TableKeys.Add(GuideSystemBase.c_TipContentKey);
        data.SetDefault(GuideSystemBase.c_TipContentKey, "Null");
        data.SetNote(GuideSystemBase.c_TipContentKey, "提示文本内容");
        data.SetFieldType(GuideSystemBase.c_TipContentKey, FieldType.String, null);

        data.TableKeys.Add(GuideSystemBase.c_TipContentPosKey);
        data.SetDefault(GuideSystemBase.c_TipContentPosKey, "0,0,0");
        data.SetNote(GuideSystemBase.c_TipContentPosKey, "提示文本位置");
        data.SetFieldType(GuideSystemBase.c_TipContentPosKey, FieldType.Vector3, null);

        data.TableKeys.Add(GuideSystemBase.c_MaskAlphaKey);
        data.SetDefault(GuideSystemBase.c_MaskAlphaKey, "0.75");
        data.SetNote(GuideSystemBase.c_MaskAlphaKey, "遮罩Alpha");
        data.SetFieldType(GuideSystemBase.c_MaskAlphaKey, FieldType.Float, null);

        TableDataEditor.SaveData(GuideSystemBase.c_guideDataName, data);
    }

    static void CreateGuideWindowScript()
    {
        string LoadPath = Application.dataPath + "/Script/Core/Editor/res/UIGuideWindowClassTemplate.txt";
        string SavePath = Application.dataPath + "/Script/UI/" + GuideSystemBase.c_guideWindowName + "/" + GuideSystemBase.c_guideWindowName + ".cs";

        string UItemplate = ResourceIOTool.ReadStringByFile(LoadPath);

        EditorUtil.WriteStringByFile(SavePath, UItemplate);

        LoadPath = Application.dataPath + "/Script/Core/Editor/res/GuideSyetemTemplate.txt";
        SavePath = Application.dataPath + "/Script/GuideSystem/GuideSyetem.cs";

        UItemplate = ResourceIOTool.ReadStringByFile(LoadPath);
        EditorUtil.WriteStringByFile(SavePath, UItemplate);

        AssetDatabase.Refresh();
    }

    public string[] GetSortingLayerNames()
    {
        Type internalEditorUtilityType = typeof(InternalEditorUtility);
        PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
        return (string[])sortingLayersProperty.GetValue(null, new object[0]);
    }
}
