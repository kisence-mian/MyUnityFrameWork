using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderManager  
{
    /// <summary>
    /// 细节等级，值目前在0-3之间
    /// </summary>
    static int s_LOD = 3;

    static int MaxLOD = 3;
    static int MinLOD = 0;

    public static int LOD
    {
        get { return RenderManager.s_LOD; }
        set {
            s_LOD = value;

            if (s_LOD > MaxLOD)
            {
                s_LOD = MaxLOD;
            }

            if (s_LOD < MinLOD)
            {
                s_LOD = MinLOD;
            }

            Shader.globalMaximumLOD = s_LOD * 100;

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
