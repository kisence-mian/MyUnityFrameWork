using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GuideSystemEditor
{
    [MenuItem("Tools/新手引导/初始化")]
    public static void InitGuideSystem()
    {
        if(!GetGuideIsInit())
        {
            //创建数据表
            SaveDataTable();

            //创建脚本
            UICreateService.CreatGuideWindowUIScript();

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
            //创建预设
            UICreateService.CreateGuideWindow();
        }
        else
        {
            Debug.LogError("新手引导没有初始化");
        }
    }

    static bool GetGuideIsInit()
    {
        return DataManager.GetIsExistData(GuideSystemBase.c_guideDataName);
    }

    static void SaveDataTable()
    {
        DataTable data = new DataTable();

        data.TableKeys.Add("GuideID");

        data.TableKeys.Add(GuideSystemBase.c_PremiseKey);
        data.SetDefault(GuideSystemBase.c_PremiseKey, "Null");
        data.SetNote(GuideSystemBase.c_PremiseKey, "前提条件");
        data.SetFieldType(GuideSystemBase.c_PremiseKey, FieldType.String,null);

        data.TableKeys.Add(GuideSystemBase.c_NextGuideNameKey);
        data.SetDefault(GuideSystemBase.c_NextGuideNameKey, "Null");
        data.SetNote(GuideSystemBase.c_NextGuideNameKey, "下一步引导,如果为空,则为下一条记录");
        data.SetFieldType(GuideSystemBase.c_NextGuideNameKey, FieldType.String, null);

        data.TableKeys.Add(GuideSystemBase.c_CallToNextKey);
        data.SetDefault(GuideSystemBase.c_CallToNextKey, "False");
        data.SetNote(GuideSystemBase.c_CallToNextKey, "是否接收调用去下一步引导");
        data.SetFieldType(GuideSystemBase.c_CallToNextKey, FieldType.Bool, null);

        data.TableKeys.Add(GuideSystemBase.c_ClickToNextKey);
        data.SetDefault(GuideSystemBase.c_ClickToNextKey, "False");
        data.SetNote(GuideSystemBase.c_ClickToNextKey, "是否接收点击去下一步引导");
        data.SetFieldType(GuideSystemBase.c_ClickToNextKey, FieldType.Bool, null);

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
        data.SetDefault(GuideSystemBase.c_TipContentKey, "");
        data.SetNote(GuideSystemBase.c_TipContentKey, "提示文本内容");
        data.SetFieldType(GuideSystemBase.c_TipContentKey, FieldType.String, null);

        data.TableKeys.Add(GuideSystemBase.c_TipContentPosKey);
        data.SetDefault(GuideSystemBase.c_TipContentPosKey, "0|0|0");
        data.SetNote(GuideSystemBase.c_TipContentPosKey, "提示文本位置");
        data.SetFieldType(GuideSystemBase.c_TipContentPosKey, FieldType.Vector3, null);

        DataEditorWindow.SaveData(GuideSystemBase.c_guideDataName, data);
    }

    static void CreateGuideWindowScript()
    {

    }
}
