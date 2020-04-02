
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AudioPlayerBase
{
    protected MonoBehaviour mono;
    private float musicVolume = 1f;
    private Queue<AudioAsset> audioAssetsPool = new Queue<AudioAsset>();
    public float MusicVolume
    {
        get
        {
            return musicVolume;
        }

    }

    private float sfxVolume = 1f;
    public float SFXVolume
    {
        get
        {
            return sfxVolume;
        }
    }
    public AudioPlayerBase(MonoBehaviour mono)
    {
        this.mono = mono;
    }
    public virtual void SetMusicVolume(float volume)
    {
        musicVolume = volume;
    }
    public virtual void SetSFXVolume(float volume)
    {
        sfxVolume = volume;

    }

    public AudioClip GetAudioClip(string name)
    {
        AudioClip red = ResourceManager.Load<AudioClip>(name);
        if (red != null)
        {
            return red;
        }
        Debug.LogError("Can not find AudioClip:" + name);
        return null;
    }

    public AudioAsset CreateAudioAssetByPool(GameObject gameObject, bool is3D, AudioSourceType  sourceType)
    {
        
        AudioAsset au = new AudioAsset();
        if (audioAssetsPool.Count > 0)
        {
            au = audioAssetsPool.Dequeue();
            au.ResetData();
        }
        else
        {
            au = new AudioAsset();
            au.audioSource = gameObject.AddComponent<AudioSource>();
        }
       
        au.audioSource.spatialBlend = is3D ? 1 : 0;
        au.sourceType = sourceType;

        if (sourceType== AudioSourceType.Music)
            au.TotleVolume = musicVolume;
        else
            au.TotleVolume = sfxVolume;
        return au;
    }
    public void DestroyAudioAssetByPool(AudioAsset asset)
    {
        UnloadClip(asset);
        audioAssetsPool.Enqueue(asset);
        //string name = asset.assetName;
        //asset.audioSource.clip = null;
        //ResourceManager.UnLoad(name);
        // GameObject.DestroyImmediate(asset.audioSource, true);
        //Resources.UnloadAsset(asset.audioSource.clip);
    }
    public void UnloadClip(AudioAsset asset)
    {
        if (asset.audioSource.clip != null)
        {
            string name = asset.assetName;
            asset.audioSource.clip = null;
            ResourceManager.DestoryAssetsCounter(name);
        }
    }
         
    protected void PlayClip(AudioAsset au, string audioName, bool isLoop = true, float volumeScale = 1, float delay = 0f,float pitch =1)
    {
        if (!ResourcesConfigManager.GetIsExitRes(audioName))
        {
            Debug.LogError("不存在音频：" + audioName);
            return;
        }
        //if (au.PlayState == AudioPlayState.Playing)
        //    au.Stop();
        UnloadClip(au);
        au.assetName = audioName;
        AudioClip ac = GetAudioClip(au.assetName);
        au.audioSource.clip = ac;
        au.audioSource.loop = isLoop;
        au.audioSource.pitch = pitch;
        au.VolumeScale = volumeScale;
        au.Play(delay);
        
    }

    protected void PlayMusicControl(AudioAsset au, string audioName, bool isLoop = true, float volumeScale = 1, float delay = 0f, float fadeTime = 0.5f, string flag = "")
    {
       
        if (au.assetName == audioName)
        {
            if (au.PlayState != AudioPlayState.Playing)
            {
                AddFade(au, VolumeFadeType.FadeIn, fadeTime, delay,flag, null, null);
                au.Play();
            }
        }
        else
        {

            if (au.PlayState != AudioPlayState.Playing)
            {
                fadeTime = fadeTime / 2f;
            }
          
            AddFade(au, VolumeFadeType.FadeOut2In, fadeTime, delay, flag, null, (value) =>
            {
                PlayClip(value, audioName, isLoop, volumeScale, delay);

            });

        }
    }
    protected void PauseMusicControl(AudioAsset au, bool isPause, float fadeTime = 0.5f)
    {
        if (isPause)
        {
            //Debug.Log("PauseMusicControl Pause : "+ au.PlayState);
            if (au.PlayState == AudioPlayState.Playing)
            {
                au.SetPlayState(AudioPlayState.Pause);
                //Debug.Log("PauseMusicControl Pause");
                AddFade(au, VolumeFadeType.FadeOut, fadeTime, 0, null, (value) =>
                {
                        //Debug.LogWarning("PauseMusicControl Pause fade CallBack");
                        value.Pause();
                }, null);
            }
        }
        else
        {
            //Debug.Log("PauseMusicControl play : "+ au.PlayState);
            if (au.PlayState == AudioPlayState.Pause)
            {
                au.Play();
                AddFade(au, VolumeFadeType.FadeIn, fadeTime, 0,null, null, null);
            }
        }
    }
    protected void StopMusicControl(AudioAsset au, float fadeTime = 0.5f)
    {
        if (au.PlayState != AudioPlayState.Stop)
        {
            au.SetPlayState(AudioPlayState.Stoping);
            //Debug.Log("StopMusicControl Stop");
            AddFade(au, VolumeFadeType.FadeOut, fadeTime, 0, null,(value) =>
            {
                //Debug.LogWarning("StopMusicControl Stop fade CallBack");
                value.Stop();
            }, null);
        }
    }


    protected Dictionary<AudioAsset, VolumeFadeData> fadeData = new Dictionary<AudioAsset, VolumeFadeData>();
    private List<AudioAsset> deleteAssets = new List<AudioAsset>();

    private Queue<VolumeFadeData> catcheData = new Queue<VolumeFadeData>();



    public void UpdateFade()
    {
        //Debug.LogWarning("UpdateFade Count: " + fadeData.Count);
        if (fadeData.Count > 0)
        {
            foreach (var item in fadeData.Values)
            {
                bool isComplete = false;
                switch (item.fadeType)
                {
                    case VolumeFadeType.FadeIn:
                        isComplete = FadeIn(item);
                        break;
                    case VolumeFadeType.FadeOut:
                        isComplete = FadeOut(item);
                        break;
                    case VolumeFadeType.FadeOut2In:
                        //Debug.Log("FadeOut2In");
                        isComplete = FadeOut2In(item);
                        break;
                }
                if (isComplete)
                {
                    //Debug.Log("isComplete");
                    deleteAssets.Add(item.au);
                }
            }

            if (deleteAssets.Count > 0)
            {
                for (int i = 0; i < deleteAssets.Count; i++)
                {
                    AudioAsset au = deleteAssets[i];
                    VolumeFadeData data = fadeData[au];
                    if (data.fadeCompleteCallBack != null)
                    {
                        data.fadeCompleteCallBack(au);
                    }

                    data.fadeCompleteCallBack = null;
                    data.fadeOutCompleteCallBack = null;
                    catcheData.Enqueue(data);
                    //Debug.Log("Remove");
                    fadeData.Remove(au);
                }

                deleteAssets.Clear();
            }
        }
    }
    public void AddFade(AudioAsset au, VolumeFadeType fadeType, float fadeTime, float delay,string flag, CallBack<AudioAsset> fadeCompleteCallBack, CallBack<AudioAsset> fadeOutCompleteCallBack)
    {
        VolumeFadeData data = null;

        if (fadeData.ContainsKey(au))
        {
            data = fadeData[au];

        }
        else
        {
            if (catcheData.Count > 0)
            {
                data = catcheData.Dequeue();
            }
            else
            {
                data = new VolumeFadeData();
            }
            fadeData.Add(au, data);
            //Debug.Log("Add");
        }
        if (data.fadeOutCompleteCallBack != null)
            data.fadeOutCompleteCallBack(data.au);
        if (data.fadeCompleteCallBack != null)
            data.fadeCompleteCallBack(data.au);

        data.fadeCompleteCallBack = null;
        data.fadeOutCompleteCallBack = null;
        data.au = au;
        if (flag != null)
            au.flag = flag;
        data.fadeType = fadeType;
        data.fadeTime = fadeTime;
        if (data.fadeTime <= 0)
            data.fadeTime = 0.000001f;
        data.fadeCompleteCallBack = fadeCompleteCallBack;
        data.fadeOutCompleteCallBack = fadeOutCompleteCallBack;

        switch (data.fadeType)
        {
            case VolumeFadeType.FadeIn:
                //   data.au.Volume = 0;
                data.fadeState = VolumeFadeStateType.FadeIn;
                break;
            case VolumeFadeType.FadeOut:
                data.fadeState = VolumeFadeStateType.FadeOut;
                //   data.au.ResetVolume();
                break;
            case VolumeFadeType.FadeOut2In:
                data.fadeState = VolumeFadeStateType.FadeOut;
                // data.au.ResetVolume();
                break;
        }
        data.tempVolume = data.au.Volume;
        data.delayTime = delay;
        //Debug.Log("AddFade:"+ data.fadeType);

    }

    /// <summary>
    /// 声音淡入
    /// </summary>
    /// <param name="data"></param>
    /// <returns>返回true，淡入完成</returns>
    private bool FadeIn(VolumeFadeData data)
    {
        //Debug.Log("FadeIn");
        if (string.IsNullOrEmpty(data.au.assetName))
        {
            data.au.ResetVolume();
            return true;
        }
        float oldVolume = data.tempVolume;

        float speed = data.au.GetMaxRealVolume() / data.fadeTime * 2f;
        oldVolume = oldVolume + speed * Time.unscaledDeltaTime;
        //Debug.Log("FadeIn:data.au.Volume " + data.au.Volume + "  dowm :" + oldVolume);
        data.au.Volume = oldVolume;
        data.tempVolume = oldVolume;

        if (oldVolume < data.au.GetMaxRealVolume())
            return false;
        else
        {
            data.au.ResetVolume();

            return true;
        }
    }

    public bool FadeOut(VolumeFadeData data)
    {
        //Debug.Log("FadeOut");
        if (string.IsNullOrEmpty(data.au.assetName))
        {
            data.au.Volume = 0;
            return true;
        }

        float oldVolume = data.tempVolume;

        float speed = data.au.GetMaxRealVolume() / data.fadeTime;
        oldVolume = oldVolume - speed * Time.unscaledDeltaTime;
        //Debug.Log("FadeOut:data.au.Volume " + data.au.Volume+"  dowm :"+ oldVolume);
        //Debug.Log(" FadeOut fade State :" + data.fadeState);
        data.au.Volume = oldVolume;
        data.tempVolume = oldVolume;

        if (oldVolume > 0)
            return false;
        else
        {
            data.au.Volume = 0;

            return true;
        }
    }

    public bool FadeOut2In(VolumeFadeData data)
    {
        //Debug.Log(" FadeOut2In :" + data.fadeTime);      

        if (data.fadeState == VolumeFadeStateType.FadeOut)
        {
            if (FadeOut(data))
            {
                data.fadeState = VolumeFadeStateType.Delay;

                if (data.fadeOutCompleteCallBack != null)
                    data.fadeOutCompleteCallBack(data.au);
                return false;
            }
        }
        else if (data.fadeState == VolumeFadeStateType.Delay)
        {
            data.delayTime -= Time.unscaledDeltaTime;
            if (data.delayTime <= 0)
            {
                data.fadeState = VolumeFadeStateType.FadeIn;
                return false;
            }
        }
        else if (data.fadeState == VolumeFadeStateType.FadeIn)
        {
            if (FadeIn(data))
            {
                data.fadeState = VolumeFadeStateType.Complete;
                return true;
            }
        }

        return false;
    }
}


