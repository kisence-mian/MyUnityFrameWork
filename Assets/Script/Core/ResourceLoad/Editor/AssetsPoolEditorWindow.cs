using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class AssetsPoolEditorWindow : EditorWindow {

    [MenuItem("Window/资源池管理(1003)", priority = 1003)]
    public static void OpenWindow()
    {
        AssetsPoolEditorWindow win = GetWindow<AssetsPoolEditorWindow>();
        win.autoRepaintOnSceneChange = true;
        win.wantsMouseMove = true;
        EditorWindow.FocusWindowIfItsOpen<AssetsPoolEditorWindow>();
        win.Ini();
    }

    private void Ini()
    {
       
    }

    private void OnGUI()
    {
        GUILayout.Box("内存占用：" + MemoryManager.totalAllocatedMemory.ToString("F") + "MB");
        EditorDrawGUIUtil.DrawScrollView(this, () =>
         {
             GUILayout.BeginHorizontal();
             GUILayout.BeginVertical();
             Dictionary<string, int> loadedAssets = AssetsPoolManager.GetLoadedAssets();
             GUILayout.Box("加载记录：");
             foreach (var item in loadedAssets)
             {
                 GUILayout.Label("  =>" + item.Key + " : " + item.Value);
             }
             GUILayout.EndVertical();
            // GUILayout.FlexibleSpace();
             GUILayout.BeginVertical();
            
             List<string> reCover = AssetsPoolManager.GetRecycleAssets();
             GUILayout.Box("回收池记录("+reCover.Count+")：");
             foreach (var item in reCover)
             {
             
                 GUILayout.Label("  ==>>"+ item);
             }
            
             GUILayout.EndVertical();
             GUILayout.EndHorizontal();
         });
    }
}
