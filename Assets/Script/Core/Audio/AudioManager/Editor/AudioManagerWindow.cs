using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class AudioManagerWindow : EditorWindow {

    [MenuItem("Window/音频状态管理(1002)",priority =1002)]
	private static void OpenWindow()
    {
        AudioManagerWindow win = GetWindow<AudioManagerWindow>();
        win.autoRepaintOnSceneChange = true;
        win.wantsMouseMove = true;
        EditorWindow.FocusWindowIfItsOpen<AudioManagerWindow>();
        win.Init();
    }

    private void Init()
    {
        
    }
    private int toolbarOption = 0;
    private string[] toolbarTexts = { "2D Player", "3D Player" };
    private void OnGUI()
    {
        toolbarOption = GUILayout.Toolbar(toolbarOption, toolbarTexts, GUILayout.Width(Screen.width));

        if (!Application.isPlaying)
            return;
        switch (toolbarOption)
        {
            case 0:
                A2DPlayerGUI();
                break;
            case 1:
                A3DPlayerGUI();
                break;

        }
    }

    private void A3DPlayerGUI()
    {
        
    }

    private void A2DPlayerGUI()
    {
        Dictionary<int, AudioAsset> bgMusicDic = AudioPlayManager.a2DPlayer.bgMusicDic;

        EditorGUILayout.Slider("Music Volume : ", AudioPlayManager.a2DPlayer.MusicVolume, 0, 1);
        EditorGUILayout.Slider("SFX Volume : ", AudioPlayManager.a2DPlayer.SFXVolume, 0, 1);

        EditorDrawGUIUtil.DrawFoldout(bgMusicDic, "Music Channel Count:"+ bgMusicDic.Count, () =>
          {
              EditorDrawGUIUtil.DrawScrollView(bgMusicDic, () =>
              {
                  foreach (var item in bgMusicDic)
                  {
                      GUILayout.Label("Channel : " + item.Key);
                      ShowAudioAssetGUI(item.Value,false);
                  }
              },"box");
          });

        List<AudioAsset> sfxList = AudioPlayManager.a2DPlayer.sfxList;

        EditorDrawGUIUtil.DrawFoldout(sfxList, "SFX Count:" + sfxList.Count, () =>
        {
            EditorDrawGUIUtil.DrawScrollView(sfxList, () =>
            {
                for (int i = 0; i < sfxList.Count; i++)
                {
                    AudioAsset au = sfxList[i];
                    GUILayout.Label("Item : " + i);
                    ShowAudioAssetGUI(au,false);
                }
               
            }, "box");
        });

    }

    private void ShowAudioAssetGUI(AudioAsset au,bool isShowAudioSource)
    {
        Color color = Color.white;
        switch (au.PlayState)
        {
            case AudioPlayState.Playing:
                color = Color.green;
                break;
            case AudioPlayState.Pause:
                color = Color.yellow;
                break;
            case AudioPlayState.Stop:
                break;
           
        }
        GUI.color = color;
        GUILayout.BeginVertical("box");
        GUILayout.Label("Asset Name : " + au.assetName);
        GUILayout.Label("Play State : " + au.PlayState);
        GUILayout.Label("flag : " + au.flag);
        EditorGUILayout.Slider("VolumeScale : ",au.VolumeScale, 0, 1);

        //EditorGUILayout.Slider("Volume : ", au.Volume, 0, au.GetMaxRealVolume());
        if (isShowAudioSource)
            EditorGUILayout.ObjectField("AudioSource : ", au.audioSource, typeof(AudioSource), true);
        GUILayout.EndVertical();
        GUI.color = Color.white;
    }
}
