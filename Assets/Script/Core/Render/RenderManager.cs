using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderManager  
{
    /// <summary>
    /// 细节等级，值目前在1-3之间
    /// </summary>
    static int s_LOD = 3;

    public static int LOD
    {
        get { return RenderManager.s_LOD; }
        set {
            s_LOD = value;

            if (s_LOD > 3)
            {
                s_LOD = 3;
            }

            if (s_LOD < 1)
            {
                s_LOD = 1;
            }

            GlobalEvent.DispatchEvent(RenderEventEnum.UpdateLOD);
        }
    }

    public static void Init()
    {
        
    }
}

public enum RenderEventEnum
{
    UpdateLOD
}
