using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio3DPlayer : AudioPlayerBase
{

    public Dictionary<GameObject, Dictionary<int, AudioAsset>> bgMusicDic = new Dictionary<GameObject, Dictionary<int, AudioAsset>>();
    public Dictionary<GameObject, List<AudioAsset>> sfxDic = new Dictionary<GameObject, List<AudioAsset>>();

    public Audio3DPlayer(MonoBehaviour mono) : base(mono) { }

    public override void SetMusicVolume(float volume)
    {
        base.SetMusicVolume(volume);
        foreach (var dics in bgMusicDic.Values)
        {
            foreach (var item in dics.Values)
            {
                item.TotleVolume = volume;
            }

        }
    }
    public override void SetSFXVolume(float volume)
    {
        base.SetSFXVolume(volume);

        foreach (var item in sfxDic.Values)
        {
            for (int i = 0; i < item.Count; i++)
            {
                item[i].TotleVolume = volume;
            }
        }
    }

    public AudioAsset PlayMusic(GameObject owner, string audioName, int channel = 0, bool isLoop = true, float volumeScale = 1, float delay = 0f, float fadeTime = 0.5f, string flag = "")
    {
        if (owner == null)
        {
            Debug.LogError("can not play 3d player, owner is null");
            return null;
        }
        AudioAsset au;
        Dictionary<int, AudioAsset> tempDic;
        if (bgMusicDic.ContainsKey(owner))
        {
            tempDic = bgMusicDic[owner];
        }
        else
        {
            tempDic = new Dictionary<int, AudioAsset>();
            bgMusicDic.Add(owner, tempDic);
        }
        if (tempDic.ContainsKey(channel))
        {
            au = tempDic[channel];
        }
        else
        {
            au = CreateAudioAssetByPool(owner, true,  AudioSourceType.Music);
            tempDic.Add(channel, au);
        }

        PlayMusicControl(au, audioName, isLoop, volumeScale, delay, fadeTime,flag);
        return au;
    }
    public void PauseMusic(GameObject owner, int channel, bool isPause, float fadeTime = 0.5f)
    {
        if (owner == null)
        {
            Debug.LogError("can not Pause , owner is null");
            return;
        }
        if (bgMusicDic.ContainsKey(owner))
        {
            Dictionary<int, AudioAsset> tempDic = bgMusicDic[owner];
            if (tempDic.ContainsKey(channel))
            {
                AudioAsset au = tempDic[channel];
                PauseMusicControl(au, isPause, fadeTime);
            }

        }
    }
    public void PauseMusicAll(bool isPause, float fadeTime = 0.5f)
    {
        foreach (GameObject i in bgMusicDic.Keys)
        {
            foreach (int t in bgMusicDic[i].Keys)
                PauseMusic(i, t, isPause, fadeTime);
        }
    }

    public void StopMusic(GameObject owner, int channel, float fadeTime = 0.5f)
    {
        if (bgMusicDic.ContainsKey(owner))
        {
            Dictionary<int, AudioAsset> tempDic = bgMusicDic[owner];
            if (tempDic.ContainsKey(channel))
            {
                StopMusicControl(tempDic[channel], fadeTime);
            }
        }

    }
    public void StopMusicOneAll(GameObject owner)
    {
        if (bgMusicDic.ContainsKey(owner))
        {
            List<int> list = new List<int>(bgMusicDic[owner].Keys);
            for (int i = 0; i < list.Count; i++)
            {
                StopMusic(owner, list[i]);
            }
        }

    }
    public void StopMusicAll()
    {
        List<GameObject> list = new List<GameObject>(bgMusicDic.Keys);
        for (int i = 0; i < list.Count; i++)
        {
            StopMusicOneAll(list[i]);
        }
    }
    public void ReleaseMusic(GameObject owner)
    {
        if (bgMusicDic.ContainsKey(owner))
        {
            StopMusicOneAll(owner);
            List<AudioAsset> list = new List<AudioAsset>(bgMusicDic[owner].Values);
            for (int i = 0; i < list.Count; i++)
            {
                Object.Destroy(list[i].audioSource);
            }
            list.Clear();
        }
        bgMusicDic.Remove(owner);
    }
    public void ReleaseMusicAll()
    {
        List<GameObject> list = new List<GameObject>(sfxDic.Keys);
        for (int i = 0; i < list.Count; i++)
        {
            ReleaseMusic(list[i]);
        }
        bgMusicDic.Clear();
    }

    public void PlaySFX(GameObject owner, string name, float volumeScale = 1f, float delay = 0f)
    {
        AudioAsset au = GetEmptyAudioAssetFromSFXList(owner);
        PlayClip(au, name, false, volumeScale, delay);
        ClearMoreAudioAsset(owner);
    }
    public void PlaySFX(Vector3 position, string name, float volumeScale = 1f, float delay = 0f)
    {
        AudioClip ac = GetAudioClip(name);
        if (ac)
            mono.StartCoroutine(PlaySFXIEnumerator(position, ac, AudioPlayManager.TotleVolume * AudioPlayManager.SFXVolume * volumeScale, delay));
    }
    private IEnumerator PlaySFXIEnumerator(Vector3 position, AudioClip ac, float volume, float delay)
    {
        yield return new WaitForSeconds(delay);
        AudioSource.PlayClipAtPoint(ac, position, volume);
    }
    public void PauseSFXAll(bool isPause)
    {
        List<GameObject> list = new List<GameObject>(sfxDic.Keys);
        for (int j = 0; j < list.Count; j++)
        {
            List<AudioAsset> sfxList = sfxDic[list[j]];
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
    }
    public void ReleaseSFX(GameObject owner)
    {
        if (owner && sfxDic.ContainsKey(owner))
        {
            List<AudioAsset> sfxList = sfxDic[owner];
            for (int i = 0; i < sfxList.Count; i++)
            {
                Object.Destroy(sfxList[i].audioSource);
            }
            sfxList.Clear();
            sfxDic.Remove(owner);
        }
    }
    public void ReleaseSFXAll()
    {
        List<GameObject> list = new List<GameObject>(sfxDic.Keys);
        for (int i = 0; i < list.Count; i++)
        {
            ReleaseSFX(list[i]);
        }
        sfxDic.Clear();
    }
    private AudioAsset GetEmptyAudioAssetFromSFXList(GameObject owner)
    {
        AudioAsset au = null;
        List<AudioAsset> sfxList = null;
        if (sfxDic.ContainsKey(owner))
        {
            sfxList = sfxDic[owner];
        }
        else
        {
            sfxList = new List<AudioAsset>();
            sfxDic.Add(owner, sfxList);

        }
        if (au == null)
        {
            au = CreateAudioAssetByPool(owner, true,  AudioSourceType.SFX);
            sfxList.Add(au);
        }

        return au;
    }

    private List<AudioAsset> tempClearList = new List<AudioAsset>();
    private void ClearMoreAudioAsset(GameObject owner)
    {
        if (sfxDic.ContainsKey(owner))
        {
            List<AudioAsset> sfxList = sfxDic[owner];
           
                for (int i = 0; i < sfxList.Count; i++)
                {
                    sfxList[i].CheckState();
                    if (sfxList[i].PlayState == AudioPlayState.Stop)
                    {
                        tempClearList.Add(sfxList[i]);
                    }
                }

                for (int i = 0; i < tempClearList.Count; i++)
                {
                    AudioAsset asset = tempClearList[i];
                    Object.Destroy(asset.audioSource);
                    DestroyAudioAssetByPool(asset);
                    sfxList.Remove(asset);
                }
                tempClearList.Clear();
            
        }
    }

    private List<GameObject> clearList = new List<GameObject>();
    public void ClearDestroyObjectData()
    {
        if (bgMusicDic.Count > 0)
        {
            clearList.Clear();
            clearList.AddRange(bgMusicDic.Keys);

            for (int i = 0; i < clearList.Count; i++)
            {
                if (clearList[i] == null)
                    bgMusicDic.Remove(clearList[i]);
            }

            foreach (var dic in bgMusicDic)
            {
                foreach (var item in dic.Value)
                {
                    item.Value.CheckState();
                    //if(item.Value.PlayState == AudioPlayState.Stop)
                    //{
                    //    DestroyAudioAssetByPool(bgMusicDic[dic.Key][item.Key]);
                    //    bgMusicDic[dic.Key].Remove(item.Key);
                       
                    //    break;
                    //}
                }
            }
        }
        if (sfxDic.Count > 0)
        {
            clearList.Clear();
            clearList.AddRange(sfxDic.Keys);
            for (int i = 0; i < clearList.Count; i++)
            {
                if (clearList[i] == null)
                {
                    sfxDic.Remove(clearList[i]);
                }
            }
            foreach (var list in sfxDic)
            {
                foreach (var item in list.Value)
                {
                    item.CheckState();
                    if(item.PlayState== AudioPlayState.Stop)
                    {
                        DestroyAudioAssetByPool(item);
                        sfxDic[list.Key].Remove(item);
                        break;
                    }
                }
            }
        }
    }
}


