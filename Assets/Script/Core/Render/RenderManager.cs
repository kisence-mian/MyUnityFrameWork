using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderManager  
{
    static int s_LOD = 3;

    public static int LOD
    {
        get { return RenderManager.s_LOD; }
        //set { RenderManager.s_LOD = value; }
    }

    public static void Init()
    {
        
    }

    public static void SetLOD(int LOD)
    {
        //QualitySettings.
    }
}
