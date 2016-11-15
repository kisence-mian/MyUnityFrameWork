using UnityEngine;
using System.Collections;

public class InputOperationEventProxy : IInputProxyBase
{
    public void Init()
    {

    }

    public void LoadEventCreater<T> () where T: IInputOperationEventCreaterBase
    {

    }

    public void Update()
    {
        if(IsAvtive)
        {

        }
    }
}
