using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using HDJ.Framework.Utils;

public class AudioGroupSystemEditorWindow : EditorWindow {

    private const string SaveDir = "Assets/Resources/GameConfigs/AudioGroupConfig/";
    [MenuItem("Tool/音乐组系统")]
    private static void OpenWindow()
    {
        AudioGroupSystemEditorWindow win = GetWindow<AudioGroupSystemEditorWindow>();
        win.Init();
    }

    private void OnEnable()
    {
        Init();
    }
    private List<AudioGroupData> datas;
    private void Init()
    {

        string text = FileUtils.LoadTextFileByPath(SaveDir + AudioGroupSystem.ConfigName + ".txt");

        datas = JsonUtils.FromJson<List<AudioGroupData>>(text);
        if (datas == null)
            datas = new List<AudioGroupData>();

    }

    private void OnGUI()
    {
        EditorDrawGUIUtil.DrawScrollView(this, () =>
         {
             EditorDrawGUIUtil.DrawList("", datas, itemTitleName:(item)=>
             {
                 AudioGroupData da = (AudioGroupData)item;
                 return da.keyName;
             });

         });
       

        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Save"))
        {
            string json = JsonUtils.ToJson(datas);
            FileUtils.CreateTextFile(SaveDir + AudioGroupSystem.ConfigName + ".txt", json);

            ShowNotification(new GUIContent("已保存!"));
        }
    }
}
