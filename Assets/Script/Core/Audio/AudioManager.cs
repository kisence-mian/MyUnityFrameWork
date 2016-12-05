using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(AudioListener))]
public class AudioManager : MonoBehaviour
{
    private static AudioManager s_instance;
    public static AudioManager s_Instance
    {
        get {if (s_instance == null) Init();
            return s_instance;}
    }

    static float s_globalVolume = 1;
    /// <summary>
    /// 全局音量，范围从0到1，与音效音量和音乐音量是相乘关系
    /// </summary>
    public static float s_GlobalVolume
    {
        get { return s_globalVolume; }
        set { s_globalVolume = Mathf.Clamp01(value);
              OnMusicVolumeChange();
              OnSoundVolumeChange();
        }
    }

    private static float s_musicVolume = 1;
    /// <summary>
    /// 音乐音量，范围从0到1
    /// </summary>
    public static float s_MusicVolume
    {
        get { return s_musicVolume * s_GlobalVolume; }
        set { s_musicVolume = Mathf.Clamp01(value);
              OnMusicVolumeChange();
        }
    }
   
    private static float s_soundVolume = 1;
    /// <summary>
    /// 音效音量，范围从0到1
    /// </summary>
    public static float s_SoundVolume
    {
        get { return s_soundVolume * s_GlobalVolume; }
        set { s_soundVolume = Mathf.Clamp01(value);
              OnSoundVolumeChange();
        }
    }

    public static AudioSource s_2Dmusic;
    public static bool s_MusicIsPlaying = false;
    static List<AudioSource> s_2Dplayers = new List<AudioSource>();
    static List<AudioSource> s_3Dplayers = new List<AudioSource>();

    public static AudioCallBack s_OnMusicComplete;
    public static AudioCallBack s_OnMusicVolumeChange;
    public static AudioCallBack s_OnSoundVolumeChange;

    public static void Init()
    {
        s_instance = new GameObject("AudioManager").AddComponent<AudioManager>();
    }

    public void Update()
    {
        if (s_2Dmusic != null && s_MusicIsPlaying == true)
        {
            if (s_2Dmusic.isPlaying == false)
            {
                s_MusicIsPlaying = false;

                try
                {
                    if (s_OnMusicComplete != null)
                    {
                        s_OnMusicComplete(SoundType.Music);
                    }
                }
                catch(Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
        }
    }

    /// <summary>
    /// 播放一个2D音乐
    /// </summary>
    /// <param name="l_musicName">音乐名</param>
    /// <param name="l_isLoop">是否循环</param>
    public  AudioSource PlayMusic2D(string l_musicName, bool l_isLoop)
    {
        s_MusicIsPlaying = true;

        AudioSource audioTmp = GetAudioSource2D(SoundType.Music);
        audioTmp.clip = GetAudioClip(l_musicName);
        audioTmp.loop = l_isLoop;
        audioTmp.volume = s_MusicVolume;
        audioTmp.Play();
        return audioTmp;
    }

    /// <summary>
    /// 播放一个2D音效
    /// </summary>
    /// <param name="l_soundName">音效名</param>
    public static AudioSource PlaySound2D(string l_soundName)
    {
        AudioSource audioTmp = GetAudioSource2D(SoundType.Sound);
        audioTmp.clip = GetAudioClip(l_soundName);
        audioTmp.loop = false;
        audioTmp.volume = s_SoundVolume;

        return audioTmp;
    }

    /// <summary>
    /// 播放一个3D音效
    /// </summary>
    /// <param name="l_soundName">音效名</param>
    /// <param name="l_gameObject">音效绑在哪个对象上</param>
    public static AudioSource PlaySound3D(string l_soundName, GameObject l_gameObject)
    {
        AudioSource audioTmp = GetAudioSource3D(l_gameObject);
        audioTmp.clip = GetAudioClip(l_soundName);
        audioTmp.loop = false;
        audioTmp.volume = s_SoundVolume;

        return audioTmp;
    }

    public static AudioSource GetAudioSource2D(SoundType l_SoundType)
    {
        if (l_SoundType == SoundType.Music)
        {
            if(s_2Dmusic == null)
            {
                s_2Dmusic = s_instance.gameObject.AddComponent<AudioSource>();
            }

            return s_2Dmusic;
        }
        else
        {
            AudioSource AudioSourceTmp = null;
            for (int i = 0; i < s_2Dplayers.Count;i++ )
            {
                AudioSourceTmp = s_2Dplayers[i];
                if(AudioSourceTmp.isPlaying == false)
                {
                    return AudioSourceTmp;
                }
            }

            AudioSourceTmp = s_instance.gameObject.AddComponent<AudioSource>();

            s_2Dplayers.Add(AudioSourceTmp);

            return AudioSourceTmp;
        }
    }

    public static AudioSource GetAudioSource3D(GameObject l_obj)
    {
        AudioSource[] l_players = l_obj.GetComponents<AudioSource>();

        for (int i = 0; i < l_players.Length; i++)
        {
            if (!l_players[i].isPlaying)
            {
                return l_players[i];
            }
        }

        AudioSource l_newAudioPlayer = l_obj.AddComponent<AudioSource>();

        return l_newAudioPlayer;
    }

    public static AudioClip GetAudioClip(string l_soundName)
    {
        AudioClip clipTmp = null;

        clipTmp = ResourceManager.Load(l_soundName) as AudioClip;

        if (clipTmp == null)
        {
            Debug.LogError("AudioManager GetAudioClip error: " + l_soundName + "is not AudioClip ! ");
        }

        return clipTmp;
    }

    static void OnMusicVolumeChange()
    {
        if(s_2Dmusic != null)
        {
            s_2Dmusic.volume = s_MusicVolume;
        }

        try
        {
            if (s_OnMusicVolumeChange != null)
            {
                s_OnMusicVolumeChange(SoundType.Music);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }

    static void OnSoundVolumeChange()
    {
        for (int i = 0; i < s_2Dplayers.Count; i++)
        {
            if (s_2Dplayers[i].isPlaying)
            {
                s_2Dplayers[i].volume = s_SoundVolume;
            }
            else
            {
                s_2Dplayers.RemoveAt(i);
                i--;
            }
        }

        for (int i = 0; i < s_3Dplayers.Count; i++)
        {
            if (s_3Dplayers[i] != null && s_3Dplayers[i].isPlaying)
            {
                s_3Dplayers[i].volume = s_SoundVolume;
            }
            else
            {
                s_3Dplayers.RemoveAt(i);
                i--;
            }
        }

        try
        {
            if (s_OnSoundVolumeChange != null)
            {
                s_OnSoundVolumeChange(SoundType.Sound);
            }
        }
        catch(Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }
}

public enum SoundType { Sound, Music, };
public delegate void AudioCallBack(SoundType l_soundType);