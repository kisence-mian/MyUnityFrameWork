using UnityEngine;

public class GameObjectLuaHelper : MonoBehaviour {

    public static void DeleteAllChild(GameObject go)
    {
        if (go.transform == null) return;
        int len = go.transform.childCount;
        for (int i = 0; i < len; i++)
        {
            Transform child = go.transform.GetChild(i);
            if (child == null) continue;
            Destroy(child.gameObject);
        }
    }

    public static GameObject Find(string name)
    {
        return GameObject.Find(name);
    }


    public static void Destroy(GameObject go)
    {
        GameObject.Destroy(go);
    }
}
