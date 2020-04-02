using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum AudioPlayState
{
    Playing,
    Pause,
    Stoping,
    Stop,
}
/// <summary>
/// 音乐资源类型，音乐还是音效
/// </summary>
public enum AudioSourceType
{
    Music,
    SFX,
}
public class AudioAsset
{
    public AudioSource audioSource;
    public AudioSourceType sourceType;
    public string flag = "";
    public string assetName = "";
    /// <summary>
    /// music，记录channel
    /// </summary>
    public int musicChannel = 0;
    private float totleVolume = 1;
    /// <summary>
    /// 总音量
    /// </summary>
    public float TotleVolume
    {
        get
        {
            return totleVolume;
        }

        set
        {

            totleVolume = value;
            Volume = TotleVolume * volumeScale;
        }
    }
    /// <summary>
    /// 当前AudioSource 实际音量
    /// </summary>
    public float Volume
    {
        get { return audioSource.volume; }
        set { audioSource.volume = value; }
    }
    /// <summary>
    /// 实际音量恢复到当前的最大
    /// </summary>
    public void ResetVolume()
    {
        Volume = TotleVolume * volumeScale;
    }
    public float GetMaxRealVolume()
    {
        return TotleVolume * volumeScale;
    }
    /// <summary>
    /// 相对于总音量当前当前AudioSource的音量缩放 Volume=TotleVolume * volumeScale
    /// </summary>
    private float volumeScale = 1f;
    public float VolumeScale
    {
        get { return volumeScale; }
        set
        {
            volumeScale = Mathf.Clamp01(value);
            ResetVolume();
        }
    }
    public bool IsPlay
    {
        get { return audioSource.isPlaying; }
    }

    private AudioPlayState playState = AudioPlayState.Stop;
    public AudioPlayState PlayState
    {
        get
        {
            return playState;
        }

    }



    public void SetPlayState(AudioPlayState state)
    {
        playState = state;
    }
    /// <summary>
    /// 检查音频是否播放完成
    /// </summary>
    public void CheckState()
    {
        if (playState == AudioPlayState.Stop)
            return;
        if (audioSource == null || (!audioSource.isPlaying && playState != AudioPlayState.Pause))
        {
            Stop();
        }

    }

    public void Play(float delay = 0f)
    {
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.PlayDelayed(delay);
            playState = AudioPlayState.Playing;

        }
    }
    public void Pause()
    {
        if (audioSource != null && audioSource.clip != null && audioSource.isPlaying)
        {
            audioSource.Pause();
            playState = AudioPlayState.Pause;
        }
    }
    public void Stop()
    {
        if (audioSource)
            audioSource.Stop();
        playState = AudioPlayState.Stop;

        if(sourceType== AudioSourceType.Music)
        {
            if (AudioPlayManager.OnMusicStopCallBack != null)
                AudioPlayManager.OnMusicStopCallBack(assetName, musicChannel, flag);
        }
        else
        {
            if (AudioPlayManager.OnSFXStopCallBack != null)
                AudioPlayManager.OnSFXStopCallBack(assetName, flag);
        }
    }

    /// <summary>
    /// 重置某些参数，防止回收后再使用参数不对
    /// </summary>
    public void ResetData()
    {
        assetName = "";
        audioSource.pitch = 1;
        flag = "";
    }
}

    public class VolumeFadeData
    {
        public AudioAsset au;
        public float fadeTime;
        /// <summary>
        /// 记录临时音量
        /// </summary>
        public float tempVolume;
        /// <summary>
        /// 延迟播放music
        /// </summary>
        public float delayTime;
        public VolumeFadeType fadeType;
        public VolumeFadeStateType fadeState;
        public CallBack<AudioAsset> fadeCompleteCallBack;
        /// <summary>
        /// 用于VolumeFadeType.FadeOut2In 当fade out完成时回调
        /// </summary>
        public CallBack<AudioAsset> fadeOutCompleteCallBack;
    }

    public enum VolumeFadeType
    {
        FadeIn,
        FadeOut,
        FadeOut2In,
    }
    public enum VolumeFadeStateType
    {
        FadeIn,
        FadeOut,
        Delay,
        Complete,
    }


