using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(ImageLoadComponent))]
public class ImageLoadComponentEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ImageLoadComponent component = (ImageLoadComponent)target;

        if(string.IsNullOrEmpty(component.iconName))
        {
            EditorGUILayout.HelpBox("不能为空!!!", MessageType.Error);

            return;
        }

        //ResourcesConfigManager.Initialize();
        if (!ResourcesConfigManager.GetIsExitRes(component.iconName))
        {
            EditorGUILayout.HelpBox("没有资源!!!", MessageType.Error);
            return;
        }
      //  if (GUI.changed)
        {
            Image image = component.LoadImage();
            image.sprite = null;
        }
    }
    
}
