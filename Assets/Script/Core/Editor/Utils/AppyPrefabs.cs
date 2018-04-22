using UnityEditor;
using UnityEngine;

public class AppyPrefabs : Editor
{
    [@MenuItem("Tools/BatchExportPrefab")]
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

            Object tg = PrefabUtility.GetCorrespondingObjectFromSource(currobj);
            if (tg == null) continue;

            //Debug.Log(currobj.name +"===="+ tg.name);

            PrefabUtility.ReplacePrefab(currobj, tg,ReplacePrefabOptions.ConnectToPrefab);
            PrefabUtility.ResetToPrefabState(currobj);
        }
        AssetDatabase.Refresh();
    }

    [@MenuItem("Tools/BatchRestPrefab")]
    static void BatchRestPrefab()
    {
        GameObject[] objs = Selection.gameObjects;
        foreach (GameObject item in objs)
        {
            if (item == null) continue;

            GameObject currobj = PrefabUtility.FindRootGameObjectWithSameParentPrefab(item);
            if (currobj == null) continue;

            //Debug.Log(currobj.name);

			PrefabUtility.ReconnectToLastPrefab(currobj);//must reconnect
            PrefabUtility.ResetToPrefabState(currobj);
        }
        AssetDatabase.Refresh();
    }
}