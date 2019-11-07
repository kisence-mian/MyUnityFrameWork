using UnityEditor;
using UnityEngine;

public class AppyPrefabs : Editor
{
    [@MenuItem("Tools/Prefab/BatchExportPrefab")]
    static void BatchExportPrefab()
    {
        GameObject[] objs = Selection.gameObjects;
        foreach (GameObject item in objs)
        {
            if (item == null) continue;

            GameObject currobj = PrefabUtility.FindRootGameObjectWithSameParentPrefab(item);
            if (currobj == null) continue;

            PrefabType pt = PrefabUtility.GetPrefabType(currobj);
            if (pt == PrefabType.None) continue;

#if UNITY_2018_2_OR_NEWER
            Object tg = PrefabUtility.GetCorrespondingObjectFromSource(currobj);
#else
            Object tg = PrefabUtility.GetPrefabParent(currobj);
#endif       
            if (tg == null) continue;
            //Debug.Log(currobj.name + "====" + tg.name);
            PrefabUtility.ReplacePrefab(currobj, tg, ReplacePrefabOptions.ConnectToPrefab);
            PrefabUtility.ResetToPrefabState(currobj);
            //重设后有子节点，去掉，在导出后不会影响已经加过子节的Hierarchy Object
            //PrefabUtility.RevertPrefabInstance(currobj);
        }
        AssetDatabase.Refresh();
    }

    [@MenuItem("Tools/Prefab/BatchRestPrefab")]
    static void BatchRestPrefab()
    {
        GameObject[] objs = Selection.gameObjects;
        foreach (GameObject item in objs)
        {
            if (item == null) continue;

            GameObject currobj = PrefabUtility.FindRootGameObjectWithSameParentPrefab(item);
            if (currobj == null) continue;

            Debug.Log(currobj.name);

            PrefabUtility.ReconnectToLastPrefab(currobj);//must reconnect
            PrefabUtility.ResetToPrefabState(currobj);
            PrefabUtility.RevertPrefabInstance(currobj);//会全部重设
        }
        AssetDatabase.Refresh();
    }
}
