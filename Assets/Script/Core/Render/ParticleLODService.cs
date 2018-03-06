using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 粒子系统LOD管理器
/// </summary>
public class ParticleLODService : PoolObject 
{
    public Transform m_LOD_1;
    public Transform m_LOD_2;
    public Transform m_LOD_3;

    #if UNITY_EDITOR
    public void Reset()
    {
        m_LOD_1 = transform.Find("LOD_1");
        m_LOD_2 = transform.Find("LOD_2");
        m_LOD_3 = transform.Find("LOD_3");
    }
    #endif

    public override void OnCreate()
    {
        ResetLOD(RenderManager.LOD);
        GlobalEvent.AddEvent(RenderEventEnum.UpdateLOD, ReceviceUpdateLOD);

        //Debug.Log("OnCreate " + (RenderManager.LOD));
    }

    public override void OnObjectDestroy()
    {
        GlobalEvent.RemoveEvent(RenderEventEnum.UpdateLOD, ReceviceUpdateLOD);
    }

    public void ResetLOD(int LOD)
    {
        if(m_LOD_3 != null)
        {
            m_LOD_3.gameObject.SetActive(LOD >= 3);
        }

        if (m_LOD_2 != null)
        {
            m_LOD_2.gameObject.SetActive(LOD >= 2);
        }
    }

    public void ReceviceUpdateLOD(params object[] objs)
    {
        ResetLOD(RenderManager.LOD);
    }
}
