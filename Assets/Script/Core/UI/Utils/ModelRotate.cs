#if UNITY_5_5_OR_NEWER
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ModelRotate : MonoBehaviour {

    public float rotateSpeed = 1;

    private void Rotate(Vector2 delta)
    {
        transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles + new Vector3(0,-delta.x * rotateSpeed, 0));
    }

    Vector2 mousePos = Vector2.zero;


    public void OnDrag(BaseEventData arg0)
    {
        if (mousePos == Vector2.zero)
        {
            mousePos = arg0.currentInputModule.input.mousePosition;
            return;
        }
        Vector2 delta = arg0.currentInputModule.input.mousePosition - mousePos;
        if (delta != Vector2.zero)
            mousePos = arg0.currentInputModule.input.mousePosition;
        Rotate(delta);
    }
}
#endif
