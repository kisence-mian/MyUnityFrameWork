using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//[RequireComponent(typeof(AudioListener))]
        [Obsolete("AudioManager 已过时,请使用AudioPlayManager",false)]
public class AudioManager : MonoBehaviour
{
    private static AudioManager s_instance;
    public static AudioManager Instance
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
        if (s_instance == null)
        {
            s_instance = new GameObject("AudioManager").AddComponent<AudioManager>();
            DontDestroyOnLoad(s_instance.gameObject);

            Init2DpPlayer(10);
        }
    }

    static void Init2DpPlayer(int count)
    {
        AudioSource AudioSourceTmp = null;
        for (int i = 0; i < count; i++)
        {
            AudioSourceTmp = s_instance.gameObject.AddComponent<AudioSource>();
            s_2Dplayers.Add(AudioSourceTmp);
        }
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
    /// <param name="musicName">音乐名</param>
    /// <param name="isLoop">是否循环</param>
    public static AudioSource PlayMusic2D(string musicName, bool isLoop)
    {
        s_MusicIsPlaying = true;

        AudioSource audioTmp = GetAudioSource2D(SoundType.Music);
        audioTmp.clip = GetAudioClip(musicName);
        audioTmp.loop = isLoop;
        audioTmp.volume = s_MusicVolume;
        if (isLoop)
        {
            audioTmp.Play();
        }
        else
        {
            audioTmp.PlayOneShot(audioTmp.clip);
        }
            
        
        return audioTmp;
    }

    public static AudioSource StopMusic2D()
    {
        s_MusicIsPlaying = true;

        AudioSource audioTmp = GetAudioSource2D(SoundType.Music);
        audioTmp.volume = s_MusicVolume;

        audioTmp.Stop();

        return audioTmp;
    }

    /// <summary>
    /// 播放一个2D音效
    /// </summary>
    /// <param name="soundName">音效名</param>
    public static AudioSource PlaySound2D(string soundName)
    {
        AudioSource audioTmp = PlaySound2D(soundName,1,false);

        return audioTmp;
    }

    /// <summary>
    /// 播放一个2D音效, 可变音调
    /// </summary>
    /// <param name="soundName">音效名</param>
    public static AudioSource PlaySound2D(string soundName, float pitch, bool isLoop = false )
    {
        AudioSource audioTmp = GetAudioSource2D(SoundType.Sound);
        audioTmp.clip = GetAudioClip(soundName);
        audioTmp.loop = isLoop;
        audioTmp.volume = s_SoundVolume;
        audioTmp.PlayOneShot(audioTmp.clip);
        audioTmp.pitch = pitch;
        return audioTmp;
    }


    /// <summary>
    /// 延时播放一个2D音效
    /// </summary>
    /// <param name="soundName">音效名</param>
    public static void PlaySound2D(string soundName,float delay )
    {
        if (delay == 0)
        {
            PlaySound2D(soundName);
        }
        else
        {
            ApplicationManager.Instance.StartCoroutine(DelayPlay(soundName, delay));
        }
    }

    static IEnumerator DelayPlay(string soundName, float delay)
    {
        yield return new WaitForSeconds(delay);
        PlaySound2D(soundName);
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

    public static AudioSource GetAudioSource2D(SoundType SoundType)
    {
        if (SoundType == SoundType.Music)
        {
            if(s_2Dmusic == null)
            {
                s_2Dmusic = Instance.gameObject.AddComponent<AudioSource>();
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

            AudioSourceTmp = Instance.gameObject.AddComponent<AudioSource>();

            s_2Dplayers.Add(AudioSourceTmp);

            return AudioSourceTmp;
        }
    }

    public static AudioSource GetAudioSource3D(GameObject obj)
    {
        AudioSource[] l_players = obj.GetComponents<AudioSource>();

        for (int i = 0; i < l_players.Length; i++)
        {
            if (!l_players[i].isPlaying)
            {
                return l_players[i];
            }
        }

        AudioSource l_newAudioPlayer = obj.AddComponent<AudioSource>();

        return l_newAudioPlayer;
    }

    public static AudioClip GetAudioClip(string soundName)
    {
        AudioClip clipTmp = null;

        clipTmp = ResourceManager.Load<AudioClip>(soundName);

        if (clipTmp == null)
        {
            Debug.LogError("AudioManager GetAudioClip error: " + soundName + "is not AudioClip ! ");
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