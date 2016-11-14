using UnityEngine;
using System.Collections;

public abstract class IInputManagerBase<T>
{
    /// <summary>
    /// 激活状态
    /// </summary>
    public bool isAvtive = true;

    
}

public class InputEventRegisterInfo<T>
{
    public string eventKey;
    public InputEventHandle<T> callBack;

    public void AddListener()
    {

    }
}
