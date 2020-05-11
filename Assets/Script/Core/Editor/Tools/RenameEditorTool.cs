using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class RenameEditorTool :EditorWindow
{
    private static RenameEditorTool win;
    [MenuItem("Tool/批量重命名")]
    public static void ShowWindow()
    {
        win = EditorWindow.GetWindow<RenameEditorTool>();
        //Debug.Log(Screen.width + "X" + Screen.height);
        win.position = new Rect(new Vector2((Screen.height - 250) / 2,(Screen.width- 350 )/ 2), new Vector2(350, 250));
        win.autoRepaintOnSceneChange = true;
       
    }
    private string preName="NewName_";
    private void OnGUI()
    {
        //GUILayout.Label(win.position.ToString());
        Object[] m_objects = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);//选择的所以对象
        GUILayout.Label("选择数目：" + m_objects.Length);
        string paths = "";
        for (int i = 0; i < 4; i++)
        {
            if (i >= m_objects.Length)
                break;
            Object item = m_objects[i];
            paths +="Object:"+ item.name + "\n";

        }
        if ( m_objects.Length >= 4)
        {
            paths += ".....";
            
        }
        GUILayout.Label(paths);

        GUILayout.Box("批量重命名前缀");
        preName = GUILayout.TextField(preName);
        GUILayout.Label("按数字结尾排序，示例：" + preName + "0");
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("确定"))
        {
            ToRename(preName);
            Close();
        }
        GUILayout.EndHorizontal();
    }
   

    public static  void ToRename(string preName)
    {

        Object[] m_objects = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);//选择的所以对象

        int index = 0;//序号

        foreach (Object item in m_objects)
        {

            string extension = Path.GetExtension(AssetDatabase.GetAssetPath(item));
            if (! string.IsNullOrEmpty(extension))//判断路径是否为空
            {

                string path = AssetDatabase.GetAssetPath(item);
                //Directory.get

                AssetDatabase.RenameAsset(path, preName + index);
                index++;
            }

        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

       
    }
}
