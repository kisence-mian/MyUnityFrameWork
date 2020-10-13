using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public abstract class AppModuleBase
{
    private bool enable = true;

    public bool Enable
    {
        get => enable;
        set
        {
            if (enable && !value)
            {
                Debug.Log(GetType().Name + " Run OnDisable");
                OnDisable();
            }
            else if (!enable && value)
            {
                Debug.Log(GetType().Name + " Run OnEnable");
                OnEnable();
            }
            enable = value;

        }
    }
    /// <summary>
    /// 模块名
    /// </summary>
    /// <returns></returns>
    public virtual string GetModuleName() { return GetType().Name; }
    /// <summary>
    /// 模块版本
    /// </summary>
    /// <returns></returns>
    public virtual string GetModuleVersion() { return ""; }
    /// <summary>
    /// 模块创建时，只运行一次
    /// </summary>
    public virtual void OnCreate() { }
    /// <summary>
    /// 模块开始在OnCreate之后，只运行一次
    /// </summary>
    public virtual void OnStart() { }
    /// <summary>
    /// 启用时
    /// </summary>
    public virtual void OnEnable() { }

    public virtual void OnUpdate() { }

    public virtual void OnFixedUpdate() { }

    public virtual void OnLateUpdate() { }

    public virtual void OnGUIUpdate() { }

    public virtual void OnDisable() { }
    /// <summary>
    /// 安卓上可能不会调用
    /// </summary>
    public virtual void OnApplicationQuit() { }

    public virtual void OnDrawGizmosUpdate() { }
    /// <summary>
    /// 当要求模块清理缓存（用于清理内存时）
    /// </summary>
    public virtual void OnReleaseCache() { }

    public virtual void OnApplicationPause(bool pauseStatus)
    {

    }

    public virtual void OnApplicationFocus(bool focusStatus)
    {

    }

    public virtual void OnDestroy()
    {
        
    }
}
