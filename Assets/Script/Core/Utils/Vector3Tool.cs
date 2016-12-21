using UnityEngine;

public class Vector3Tool {

    public static Vector3 es_y(Vector3 org, float _y = 0)
    {
        return new Vector3(org.x, _y, org.z);
    }

    public static float get_y(Vector3 pos,string[] layerstr)
    {
        float _y = 0;
        RaycastHit hit;
        Physics.Raycast(es_y(pos,150), Vector3.down, out hit, 300, LayerMask.GetMask(layerstr));//射线只检测
        if (hit.transform!=null) _y = hit.transform.position.y;
        return _y;
    }
}
