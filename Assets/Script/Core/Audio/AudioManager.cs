using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    public static AudioManager Instance
    {
        get {if (instance == null) Init();
            return instance;}
    }

    /// <summary>
    /// 全局音量，范围从0到1，与音效音量和音乐音量是相乘关系
    /// </summary>
    static float s_globalVolume = 1;
    public static float s_GlobalVolume
    {
        get { return s_globalVolume; }
        set { s_globalVolume = Mathf.Clamp01(value); }
    }

    /// <summary>
    /// 音乐音量，范围从0到1
    /// </summary>
    private static float s_musicVolume = 1;
    public static float s_MusicVolume
    {
        get { return s_musicVolume * s_GlobalVolume; }
        set { s_musicVolume = Mathf.Clamp01(value); }
    }
    /// <summary>
    /// 音效音量，范围从0到1
    /// </summary>
    private static float s_soundVolume = 1;
    public static float s_SoundVolume
    {
        get { return s_soundVolume * s_GlobalVolume; }
        set { s_soundVolume = Mathf.Clamp01(value); }
    }

    public static void Init()
    {
        instance = new GameObject("AudioManager").AddComponent<AudioManager>();
    }

    public static void PlayMusic2D(string l_musicName)
    {

    }

    public static void PlaySound2D(string l_musicName)
    {

    }
}

public delegate void AudioCallBack(string AudioName,params object[] objs);