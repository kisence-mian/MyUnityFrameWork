using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AudioButtonClickComponent))]
public class AudioButtonClickComponentEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        AudioButtonClickComponent component = (AudioButtonClickComponent)target;

        if (string.IsNullOrEmpty(component.audioName))
        {
            EditorGUILayout.HelpBox("不能为空!!!", MessageType.Error);

            return;
        }

        //ResourcesConfigManager.Initialize();
        if (!ResourcesConfigManager.GetIsExitRes(component.audioName))
        {
            EditorGUILayout.HelpBox("没有资源!!!", MessageType.Error);
            return;
        }
        if (GUILayout.Button("Play",GUILayout.Height(60)))
        {
            AudioClip clip = ResourceManager.Load<AudioClip>(component.audioName);
            AudioEditorUtils.PlayClip(clip);
        }
    }
}
