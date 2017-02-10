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
    public static Vector3 new_v3(float xx,float yy,float zz)
    {
        return new Vector3(xx,yy,zz);
    }

    public static void pic_reverse(GameObject go)
    {
        go.transform.localScale = new Vector3(-1f, 1f, 1f);
        go.transform.localPosition = go.transform.localPosition - new Vector3(512f, 0, 0);
        
    }
}
