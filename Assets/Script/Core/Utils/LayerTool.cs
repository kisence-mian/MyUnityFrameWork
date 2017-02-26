using UnityEngine;

public class LayerTool : MonoBehaviour {
    public static void set_layer(Transform ts, string[] str)
    {
        ts.gameObject.layer = LayerMask.GetMask(str);
        int len = ts.childCount;
        for (int i = 0; i < len; i++)
        {
            Transform _ts = ts.GetChild(i);
            _ts.gameObject.layer = LayerMask.GetMask(str);
        }
    }
    public static void set_screen_pos(GameObject own, GameObject tar, float off_x = 0, float off_y = 0)
    {
        if (!(Camera.main != null && Camera.main.enabled)) return;
        Vector3 screenpos = Camera.main.WorldToScreenPoint(tar.transform.position);
        own.transform.localPosition = new Vector3(screenpos.x - (Screen.width >> 1) + off_x, screenpos.y + off_y - (Screen.height >> 1), screenpos.z);
    }
}
