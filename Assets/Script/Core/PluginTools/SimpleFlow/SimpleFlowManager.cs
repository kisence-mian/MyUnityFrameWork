using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public  class SimpleFlowManager
{
    private  Dictionary<string, FlowItemBase> allFlowItems = new Dictionary<string, FlowItemBase>();
    private  Dictionary<string, object> globalVariables = new Dictionary<string, object>();

    /// <summary>
    /// 节点开始回调（节点名字（GetType().Name））
    /// </summary>
    public Action<FlowItemBase> OnStart;
    /// <summary>
    /// 节点完成回调（节点名字（GetType().Name），error错误信息）
    /// </summary>
    public Action<FlowItemBase, string> OnFinished;
    #region global Variables
    public  void SetVariable(string key,object value)
    {
        if (globalVariables.ContainsKey(key))
        {
            globalVariables[key] = value;
        }
        else
        {
            globalVariables.Add(key, value);
        }
    }
    public  object GetVariable(string key)
    {
        object value = null;
        globalVariables.TryGetValue(key, out value);
        return value;
    }
    public  T GetVariable<T>(string key)
    {
        object value = null;
        globalVariables.TryGetValue(key, out value);
        if (value == null)
            return default(T);
        return (T)value;
    }
    #endregion

    public  void AddFlowItems(FlowItemBase[] flowItems)
    {
        foreach (var item in flowItems)
        {
            AddFlowItem(item);
        }
    }
    public  void AddFlowItem(FlowItemBase flowItem)
    {
        flowItem.flowManager = this;

        if (allFlowItems.ContainsKey(flowItem.Name))
        {
            allFlowItems[flowItem.Name] = flowItem;
        }
        else
        {
            allFlowItems.Add(flowItem.Name, flowItem);
        }
    }

    public  void RemoveFlowItem(string name)
    {
        if (allFlowItems.ContainsKey(name))
        {
            allFlowItems.Remove(name);
        }
    }
    public  T GetFlowItem<T>() where T :FlowItemBase
    {
        string name = typeof(T).Name;
        if (allFlowItems.ContainsKey(name))
        {
            return (T)allFlowItems[name];
        }
        return null;
    }

    public  FlowItemBase CurrentRunFlowItem;
    public void RunFlowItem<T>( bool forceRestartIfSameName = false, params object[] paras)
    {
        RunFlowItem(typeof(T), forceRestartIfSameName, paras);
    }
    public  void RunFlowItem(Type type,bool forceRestartIfSameName =false,params object[] paras)
    {
        RunFlowItem(type.Name,forceRestartIfSameName,paras);
    }
    public  void RunFlowItem(string name,bool forceRestartIfSameName = false, params object[] paras)
    {
        FlowItemBase newItem = null;
        if (allFlowItems.ContainsKey(name))
        {
            newItem = allFlowItems[name];
        }

        if(newItem==null)
        {
            Debug.LogError("No Flow Item：" + name);
            return;
        }
        if (CurrentRunFlowItem != null)
        {
            if (CurrentRunFlowItem.Name == name)
            {
                if (!forceRestartIfSameName)
                    return;
                else
                {
                    CurrentRunFlowItem.Start(paras);
                    return;
                }

            }
            //else
            //{
            //    CurrentRunFlowItem.Finish(null);
            //}
        }

        CurrentRunFlowItem = newItem;
        CurrentRunFlowItem.Start(paras);
    }
}

public abstract class FlowItemBase
{
    public SimpleFlowManager flowManager;

    public bool Enable = true;
    public string Name
    {
        get
        {
            return GetType().Name;
        }
    }
    public Action<FlowItemBase> OnStart;
    /// <summary>
    /// 节点完成回调（节点名字（GetType().Name），error错误信息）
    /// </summary>
    public Action<FlowItemBase, string> OnFinished;

     
    public  void Start(params object[] paras)
    {
        Debug.Log("FlowItemBase.start:" + Name);
        if (!Enable)
        {
            FinishCallBack(null);
            return;
        }
        OnFlowStart( paras);
        if (OnStart != null)
        {
            OnStart(this);
        }
        if (flowManager.OnStart != null)
        {
            flowManager.OnStart(this);
        }
    }

    protected virtual void OnFlowStart(params object[] paras)
    {

    }


    public void Finish(string error)
    {
        Debug.Log("FlowItemBase.Finish:" + Name+" error:" + error);
        OnFlowFinished();
        FinishCallBack(error);
    }

    private void FinishCallBack(string error)
    {
        if (OnFinished != null)
        {
            OnFinished(this, error);
        }

        if (flowManager.OnFinished != null)
        {
            flowManager.OnFinished(this, error);
        }
    }
    protected virtual void OnFlowFinished()
    {
       
    }
}
