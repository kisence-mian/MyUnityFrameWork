using FrameWork.GuideSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideSyetem : GuideSystemBase
{
    private static GuideSyetem s_instance;

    public static GuideSyetem Instance
    {
        get
        {
            if(s_instance == null)
            {
                s_instance = new GuideSyetem();
                s_instance.Init();
            }
            return s_instance;
        }

        set
        {
            s_instance = value;
        }
    }
}
