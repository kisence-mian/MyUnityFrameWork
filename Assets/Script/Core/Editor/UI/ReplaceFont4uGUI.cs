using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using UnityEngine.UI;

public class ReplaceFont4uGUI : EditorWindow
{
    [MenuItem("Tool/替换UI字体")]
    public static void OpenWindow()
    {
        EditorWindow.GetWindow<ReplaceFont4uGUI>();
        
    }
    private Font matchingfont;
    private Font replaceFont;
    void OnGUI()
    {
         Object[] selectObjs = Selection.GetFiltered(typeof(GameObject), SelectionMode.DeepAssets);
         GUILayout.Label("选中数目：" + selectObjs.Length);
         matchingfont = (Font)EditorGUILayout.ObjectField("需要替换的字体：",matchingfont, typeof(Font),true);
         EditorGUILayout.Separator();
         replaceFont = (Font)EditorGUILayout.ObjectField("替换的字体：", replaceFont, typeof(Font), true);
         EditorGUILayout.Separator();
         GUILayout.Space(15);
         if (GUILayout.Button("替换"))
         {
            int num =  CorrectionPublicFont(replaceFont, matchingfont);
            EditorUtility.DisplayDialog("提示", "成功替换" + num + "处", "OK");
         }
    }
    private static int CorrectionPublicFont(Font replace, Font matching)
    {
        int replaceNum = 0;
            Object[] selectObjs = Selection.GetFiltered(typeof(GameObject), SelectionMode.DeepAssets);
            foreach (Object selectObj in selectObjs)
            {
                GameObject obj = (GameObject)selectObj;
                if (obj == null || selectObj == null)
                {
                    Debug.LogWarning("ERROR:Obj Is Null !!!");
                    continue;
                }
                string path = AssetDatabase.GetAssetPath(selectObj);
                if (path.Length < 1 || path.EndsWith(".prefab") == false)
                {
                    Debug.LogWarning("ERROR:Folder=" + path);
                }
                else
                {
                    Debug.Log("Selected Folder=" + path);
                    GameObject clone = GameObject.Instantiate(obj) as GameObject;
                    Text[] labels = clone.GetComponentsInChildren<Text>(true);
                    foreach (Text label in labels)
                    {
                        if (label.font == matching)
                        {
                            label.font = replace;
                            replaceNum++;
                        }
                    }
                    SaveDealFinishPrefab(clone, path);
                    GameObject.DestroyImmediate(clone);
                    Debug.Log("Connect Font Success=" + path);
                }
            }
            AssetDatabase.Refresh();

            return replaceNum;
    }
    private static void SaveDealFinishPrefab(GameObject go, string path)
    {
        if (File.Exists(path))
        {
            Object prefab = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
            PrefabUtility.ReplacePrefab(go, prefab);
        }
        else
        {
            PrefabUtility.CreatePrefab(path, go);
        }
    }
}
