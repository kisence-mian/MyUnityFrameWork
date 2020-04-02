using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Audio2DPlayer : AudioPlayerBase
{
    public Dictionary<int, AudioAsset> bgMusicDic = new Dictionary<int, AudioAsset>();
    public List<AudioAsset> sfxList = new List<AudioAsset>();

    public Audio2DPlayer(MonoBehaviour mono) : base(mono) { }

    public override void SetMusicVolume(float volume)
    {
        base.SetMusicVolume(volume);
        foreach (var item in bgMusicDic.Values)
        {
            item.TotleVolume = volume;
        }
    }
    public override void SetSFXVolume(float volume)
    {
        base.SetSFXVolume(volume);
        for (int i = 0; i < sfxList.Count; i++)
        {
            sfxList[i].TotleVolume = volume;
        }
    }

    public AudioAsset PlayMusic(int channel, string audioName, bool isLoop = true, float volumeScale = 1, float delay = 0f, float fadeTime = 0.5f, string flag = "")
    {
        AudioAsset au;

        if (bgMusicDic.ContainsKey(channel))
        {
            au = bgMusicDic[channel];
        }
        else
        {
            au = CreateAudioAssetByPool(mono.gameObject, false,  AudioSourceType.Music);
            bgMusicDic.Add(channel, au);
        }
        au.musicChannel = channel;
        PlayMusicControl(au, audioName, isLoop, volumeScale, delay, fadeTime, flag);
        return au;
    }
    public void PauseMusic(int channel, bool isPause, float fadeTime = 0.5f)
    {
        if (bgMusicDic.ContainsKey(channel))
        {
            AudioAsset au = bgMusicDic[channel];
            PauseMusicControl(au, isPause, fadeTime);

        }
    }
    public void PauseMusicAll(bool isPause, float fadeTime = 0.5f)
    {
        foreach (int i in bgMusicDic.Keys)
        {
            PauseMusic(i, isPause, fadeTime);
        }
    }

    public void StopMusic(int channel, float fadeTime = 0.5f)
    {
        if (bgMusicDic.ContainsKey(channel))
        {
            StopMusicControl(bgMusicDic[channel], fadeTime);
        }

    }

    public void StopMusicAll()
    {
        foreach (int i in bgMusicDic.Keys)
        {
            StopMusic(i);
        }
    }

    public void PlaySFX(string name, float volumeScale = 1f, float delay = 0f, float pitch = 1, string flag = "")
    {
        AudioAsset au = GetEmptyAudioAssetFromSFXList();
        au.flag = flag;
        PlayClip(au, name, false, volumeScale, delay, pitch);

    }
    public void PauseSFXAll(bool isPause)
    {
        for (int i = 0; i < sfxList.Count; i++)
        {
            if (isPause)
            {
                    sfxList[i].Pause();
            }
            else
            {
                    sfxList[i].Play();
            }
        }
    }

    private AudioAsset GetEmptyAudioAssetFromSFXList()
    {
        AudioAsset au = null;
        if (au == null)
        {
            au = CreateAudioAssetByPool(mono.gameObject, false,  AudioSourceType.SFX);
            sfxList.Add(au);
        }
        return au;
    }

    private List<AudioAsset> clearList = new List<AudioAsset>();
    //private List<int> channalClearList = new List<int>();
    public void ClearMoreAudioAsset()
    {
        for (int i = 0; i < sfxList.Count; i++)
        {
            sfxList[i].CheckState();
            if (sfxList[i].PlayState == AudioPlayState.Stop)
            {
                clearList.Add(sfxList[i]);
            }
        }
      

        for (int i = 0; i < clearList.Count; i++)
        {
            DestroyAudioAssetByPool(clearList[i]);
            sfxList.Remove(clearList[i]);
        }
        clearList.Clear();

        foreach (var item in bgMusicDic)
        {
            item.Value.CheckState();
            //if (item.Value.PlayState == AudioPlayState.Stop)
            //    channalClearList.Add(item.Key);
        }
        //foreach (var item in channalClearList)
        //{
        //    DestroyAudioAssetByPool(bgMusicDic[item]);
        //    bgMusicDic.Remove(item);
        //}
        //channalClearList.Clear();
    }

}


