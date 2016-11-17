using UnityEngine;
using System.Collections;

public class CustomEvent : IInputOperationEventBase, IInputOperationEventCreater
{

    public void EventTriggerLogic()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            CustomEvent tmp = new CustomEvent();
            InputManager.Dispatch<CustomEvent>(tmp);
        }
    }
}
