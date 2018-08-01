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

    public void PlayMusic(int channel, string audioName, bool isLoop = true, float volumeScale = 1, float delay = 0f, float fadeTime = 0.5f)
    {
        AudioAsset au;

        if (bgMusicDic.ContainsKey(channel))
        {
            au = bgMusicDic[channel];
        }
        else
        {
            au = CreateAudioAsset(mono.gameObject, false, true);
            bgMusicDic.Add(channel, au);
        }
        PlayMusicControl(au, audioName, isLoop, volumeScale, delay, fadeTime);
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

    public void PlaySFX(string name, float volumeScale = 1f, float delay = 0f)
    {
        AudioAsset au = GetEmptyAudioAssetFromSFXList();
        PlayClip(au, name, false, volumeScale, delay);

    }
    public void PauseSFXAll(bool isPause)
    {
        for (int i = 0; i < sfxList.Count; i++)
        {
            if (isPause)
            {
                if (sfxList[i].PlayState == AudioPlayState.Playing)
                    sfxList[i].Pause();
            }
            else
            {
                if (sfxList[i].PlayState == AudioPlayState.Pause)
                    sfxList[i].Play();
            }
        }
    }

    private AudioAsset GetEmptyAudioAssetFromSFXList()
    {
        AudioAsset au = null;
        if (sfxList.Count > 0)
        {
            for (int i = 0; i < sfxList.Count; i++)
            {
                sfxList[i].CheckState();
                if (sfxList[i].PlayState == AudioPlayState.Stop)
                {
                    au = sfxList[i];
                    break;
                }
            }
        }
        if (au == null)
        {
            au = CreateAudioAsset(mono.gameObject, false, false);
            sfxList.Add(au);
        }
        return au;
    }

    private List<AudioAsset> clearList = new List<AudioAsset>();
    public void ClearMoreAudioAsset()
    {
        if (sfxList.Count > maxSFXAudioAssetNum)
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
                if (sfxList.Count <= maxSFXAudioAssetNum)
                    break;
                Object.Destroy(clearList[i].audioSource);
                sfxList.Remove(clearList[i]);
            }
            clearList.Clear();
        }
    }

}


