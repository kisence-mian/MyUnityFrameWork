using UnityEngine;

public class Vector3Tool {

    public static Vector3 es_y(Vector3 org, float _y = 0)
    {
        return new Vector3(org.x, _y, org.z);
    }
}
