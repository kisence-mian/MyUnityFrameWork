using UnityEditor;
using UnityEngine;

public class AppyPrefabs : Editor
{
    [@MenuItem("Tools/BatchExportPrefab")]
    static void BatchExportPrefab()
    {
        GameObject[] objs = Selection.gameObjects;
        //int len = objs.Length;
        foreach (GameObject item in objs)
        {
            //if (PrefabUtility.GetPrefabType(item) == PrefabType.PrefabInstance){}
            Object parentObject = PrefabUtility.GetPrefabParent(item);
            PrefabUtility.ReplacePrefab(item, parentObject);
            PrefabUtility.RevertPrefabInstance(item);
        }
        AssetDatabase.Refresh();
    }

    [@MenuItem("Tools/BatchRestPrefab")]
    static void BatchRestPrefab()
    {
        GameObject[] objs = Selection.gameObjects;
        //int len = objs.Length;
        foreach (GameObject item in objs)
        {
            //if (PrefabUtility.GetPrefabType(item) == PrefabType.PrefabInstance){}
            //Object parentObject = PrefabUtility.GetPrefabParent(item);
            PrefabUtility.RevertPrefabInstance(item);
        }
        AssetDatabase.Refresh();
    }
}