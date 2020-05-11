using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AudioPlayManager : MonoBehaviour
{
    #region 属性

    public static Audio2DPlayer a2DPlayer;
    public static Audio3DPlayer a3DPlayer;
    /// <summary>
    ///  音乐播放完成 回调（参数 ：资源名，channel，flag（标识：用于在多个相同音频名称时分辨））
    /// </summary>
    public static CallBack<string, int, string> OnMusicStopCallBack;
    /// <summary>
    /// 音乐播放将要完成 回调，提前1秒回调，当clip时长不足1秒则在OnMusicStopCallBack前回调（参数 ：资源名，channel，flag（标识：用于在多个相同音频名称时分辨））
    /// </summary>
    public static CallBack<string, int, string> OnMusicPreStopCallBack;

    /// <summary>
    /// SFX播放完成 回调（参数 ：资源名，flag（标识：用于在多个相同音频名称时分辨））
    /// </summary>
    public static CallBack<string, string> OnSFXStopCallBack;

    #endregion

    #region 外部调用

    public static void Init()
    {
        GameObject obj = new GameObject("[AudioManager]");
        AudioPlayManager audioManager = obj.AddComponent<AudioPlayManager>();
        DontDestroyOnLoad(obj);

        a2DPlayer = new Audio2DPlayer(audioManager);
        a3DPlayer = new Audio3DPlayer(audioManager);
        TotleVolume = RecordManager.GetFloatRecord("GameSettingData", "TotleVolume", 1f);
        MusicVolume = RecordManager.GetFloatRecord("GameSettingData", "MusicVolume", 1f);
        SFXVolume = RecordManager.GetFloatRecord("GameSettingData", "SFXVolume", 1f);
    }

    #region Volume

    private static float totleVolume = 1f;
    public static float TotleVolume
    {
        get { return totleVolume; }
        set
        {
            totleVolume = Mathf.Clamp01(value);
            SetMusicVolume();
            SetSFXVolume();

        }
    }

    public static float MusicVolume
    {
        get
        {
            return musicVolume;
        }

        set
        {
            musicVolume = Mathf.Clamp01(value);
            SetMusicVolume();
        }
    }

    public static float SFXVolume
    {
        get
        {
            return sfxVolume;
        }

        set
        {
            sfxVolume = Mathf.Clamp01(value);
            SetSFXVolume();
        }
    }

    private static float musicVolume = 1f;

    private static float sfxVolume = 1f;

    private static void SetMusicVolume()
    {
        a2DPlayer.SetMusicVolume(totleVolume * musicVolume);
        a3DPlayer.SetMusicVolume(totleVolume * musicVolume);
    }
    private static void SetSFXVolume()
    {
        a2DPlayer.SetSFXVolume(totleVolume * sfxVolume);
        a3DPlayer.SetSFXVolume(totleVolume * sfxVolume);
    }

    public static void SaveVolume()
    {
        RecordManager.SaveRecord("GameSettingData", "TotleVolume", TotleVolume);
        RecordManager.SaveRecord("GameSettingData", "MusicVolume", MusicVolume);
        RecordManager.SaveRecord("GameSettingData", "SFXVolume", SFXVolume);
    }
    #endregion

    #region 播放接口

    public static AudioAsset PlayMusic2D(string name, int channel, float volumeScale = 1, bool isLoop = true, float fadeTime = 0.5f, float delay = 0f, string flag = "")
    {
        return a2DPlayer.PlayMusic(channel, name, isLoop, volumeScale, delay, fadeTime, flag);
    }
    public static void PauseMusic2D(int channel, bool isPause, float fadeTime = 0.5f)
    {
        a2DPlayer.PauseMusic(channel, isPause, fadeTime);
    }
    public static void PauseMusicAll2D(bool isPause, float fadeTime = 0.5f)
    {
        a2DPlayer.PauseMusicAll(isPause, fadeTime);
    }

    public static void StopMusic2D(int channel, float fadeTime = 0.5f)
    {

        a2DPlayer.StopMusic(channel, fadeTime);
    }

    public static void StopMusicAll2D()
    {
        a2DPlayer.StopMusicAll();
    }

    public static void PlaySFX2D(string name, float volumeScale = 1f, float delay = 0f, float pitch = 1, string flag = "")
    {
        a2DPlayer.PlaySFX(name, volumeScale, delay, pitch, flag);
    }
    public static void PauseSFXAll2D(bool isPause)
    {
        a2DPlayer.PauseSFXAll(isPause);

    }

    public static AudioAsset PlayMusic3D(GameObject owner, string audioName, int channel = 0, float volumeScale = 1, bool isLoop = true, float fadeTime = 0.5f, float delay = 0f, string flag = "")
    {
       return  a3DPlayer.PlayMusic(owner, audioName, channel, isLoop, volumeScale, delay, fadeTime, flag);
    }
    public static void PauseMusic3D(GameObject owner, int channel, bool isPause, float fadeTime = 0.5f)
    {
        a3DPlayer.PauseMusic(owner, channel, isPause, fadeTime);
    }
    public static void PauseMusicAll3D(bool isPause, float fadeTime = 0.5f)
    {
        a3DPlayer.PauseMusicAll(isPause, fadeTime);
    }

    public static void StopMusic3D(GameObject owner, int channel, float fadeTime = 0.5f)
    {
        a3DPlayer.StopMusic(owner, channel, fadeTime);

    }
    public static void StopMusicOneAll3D(GameObject owner)
    {
        a3DPlayer.StopMusicOneAll(owner);
    }
    public static void StopMusicAll3D()
    {
        a3DPlayer.StopMusicAll();
    }
    public static void ReleaseMusic3D(GameObject owner)
    {
        a3DPlayer.ReleaseMusic(owner);
    }
    public static void ReleaseMusicAll3D()
    {
        a3DPlayer.ReleaseMusicAll();
    }

    public static void PlaySFX3D(GameObject owner, string name, float delay = 0f, float volumeScale = 1f)
    {
        a3DPlayer.PlaySFX(owner, name, volumeScale, delay);
    }
    public static void PlaySFX3D(Vector3 position, string name, float delay = 0f, float volumeScale = 1)
    {
        a3DPlayer.PlaySFX(position, name, volumeScale, delay);
    }

    public static void PauseSFXAll3D(bool isPause)
    {
        a3DPlayer.PauseSFXAll(isPause);
    }
    public static void ReleaseSFX3D(GameObject owner)
    {
        a3DPlayer.ReleaseSFX(owner);
    }
    public static void ReleaseSFXAll3D()
    {
        a3DPlayer.ReleaseSFXAll();
    }

    #endregion

    #endregion

    void Update()
    {
        a3DPlayer.ClearDestroyObjectData();
        a2DPlayer.ClearMoreAudioAsset();

        a2DPlayer.UpdateFade();
        a3DPlayer.UpdateFade();
    }
}

