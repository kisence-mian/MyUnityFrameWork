using UnityEngine;
using System.Collections;

public interface IInputOperation 
{
    bool IsCreatOperation();

    IInputEventBase GetOperationEvent();
}
