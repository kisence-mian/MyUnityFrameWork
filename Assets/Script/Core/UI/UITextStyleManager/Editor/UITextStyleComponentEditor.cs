using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(UITextStyleComponent))]
public class UITextStyleComponentEditor : Editor {

    //TextStyleData oldData;
    Text text;
    UITextStyleComponent obj;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        obj = (UITextStyleComponent)target;

        if (text == null)
            text = obj.GetComponent<Text>();

        if(GUILayout.Button("Open Editor Window"))
        {
             UITextStyleManager.GetTextStyleDataFromText(text);
            UITextStyleManagerWindow.OpenWindow(obj);
        }
    }
}
