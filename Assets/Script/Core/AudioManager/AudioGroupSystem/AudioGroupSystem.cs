using HDJ.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public enum AudioGroupPlayState
{
    Playing,
    Pause,
    Stoped,
}
public class AudioGroupSystem :MonoBehaviour
{
    public const string ConfigName = "AudioGroupConfig";

    private static Dictionary<string, AudioGroupData> audioGroupDataDic = new Dictionary<string, AudioGroupData>();

    private static AudioGroupData currentAudioGroupData;
    private static AudioGroupPlayState audioGroupPlayState = AudioGroupPlayState.Stoped;
    public static void Play(string keyName,float fadeTime =0.6f)
    {
        Init();
        if (currentAudioGroupData != null && keyName == currentAudioGroupData.keyName)
            return;

        audioGroupPlayState = AudioGroupPlayState.Playing;
        currentAudioGroupData = audioGroupDataDic[keyName];

        Dictionary<int, AudioAsset> playingMusics = AudioPlayManager.a2DPlayer.bgMusicDic;

        foreach (var item in playingMusics)
        {
            if (item.Key > currentAudioGroupData.fixedMusicDatas.Count - 1)
            {
                AudioPlayManager.StopMusic2D(item.Key, fadeTime);
            }
        }
        for (int i = 0; i < currentAudioGroupData.fixedMusicDatas.Count; i++)
        {
            MusicPlayData data = currentAudioGroupData.fixedMusicDatas[i];
            PlayMusicData(data);
        }

        foreach (var item in currentAudioGroupData.fixedSFXDatas)
        {
            PlaySFXData(item);
        }

        instance.randomLoopSFXDatas.Clear();
        foreach (var item in currentAudioGroupData.sFXRandomLoopDatas)
        {
           instance.randomLoopSFXDatas.Add( AddLoopSFX(item));
        }
    }

    public static void Pause(bool isPause,float fadeTime=0.5f)
    {
        if (audioGroupPlayState == AudioGroupPlayState.Playing && isPause)
            audioGroupPlayState = AudioGroupPlayState.Pause;
        if (audioGroupPlayState == AudioGroupPlayState.Pause && !isPause)
            audioGroupPlayState = AudioGroupPlayState.Playing;

        for (int i = 0; i < currentAudioGroupData.fixedMusicDatas.Count; i++)
        {
            MusicPlayData data = currentAudioGroupData.fixedMusicDatas[i];
            AudioPlayManager.PauseMusic2D(data.channel, isPause, fadeTime);
        }
        AudioPlayManager.PauseSFXAll2D(isPause);
    }

    private static void PlayMusicData(MusicPlayData data)
    {
        AudioPlayManager.PlayMusic2D(data.name, data.channel, data.volume, data.isLoop, data.fadeTime, data.delay);
    }

    private static void PlaySFXData(SFXPlayData data)
    {
        AudioPlayManager.PlaySFX2D(data.name, data.volume, data.delay, data.pitch);
    }
    private static bool isInit = false; 
    private static AudioGroupSystem instance;
    private static void Init()
    {
        if (isInit)
            return;
        isInit = true;

        GameObject obj = new GameObject("[AudioGroupSystem]");
        instance = obj.AddComponent<AudioGroupSystem>();

        TextAsset asset = ResourceManager.Load<TextAsset>(ConfigName);

        List<AudioGroupData> datas = JsonUtils.FromJson<List<AudioGroupData>>(asset.text);
        audioGroupDataDic.Clear();
        foreach (var item in datas)
        {
            audioGroupDataDic.Add(item.keyName, item);
        }
    }

    private static RandomLoopSFXData AddLoopSFX(SFXRandomLoopData data)
    {
        RandomLoopSFXData r = new RandomLoopSFXData();
        r.configData = data;
        RandomTime(r);

        return r;
    }
    private static void RandomTime(RandomLoopSFXData data)
    {
        float r = UnityEngine.Random.Range(data.configData.delayRange.x, data.configData.delayRange.y);
        data.currentTime = r;
    }
    List<RandomLoopSFXData> randomLoopSFXDatas = new List<RandomLoopSFXData>();
    List<RandomLoopSFXData> clearRandomList = new List<RandomLoopSFXData>();
    private void Update()
    {
        if (audioGroupPlayState != AudioGroupPlayState.Playing)
            return;

        foreach (var item in randomLoopSFXDatas)
        {
            if(item.IsRunFinished())
            {
                clearRandomList.Add(item);
            }
            else
            {
                if (item.currentTime <= 0)
                {
                    if (item.configData.SFXDatas.Count > 0)
                    {
                        int r = UnityEngine.Random.Range(0, item.configData.SFXDatas.Count);
                        PlaySFXData(item.configData.SFXDatas[r]);
                    }
                    item.runTime++;
                    RandomTime(item);
                }
                else
                {
                    item.currentTime -= Time.deltaTime;
                }
            }
        }

        foreach (var item in clearRandomList)
        {
            randomLoopSFXDatas.Remove(item);
        }
        clearRandomList.Clear();
    }


    private class RandomLoopSFXData
    {
        public float currentTime;
        public int runTime;

        public SFXRandomLoopData configData;

        public bool IsRunFinished()
        {
            if (configData.loopTimes == -1)
                return false;
            if (runTime >= configData.loopTimes)
                return true;

            return false;
        }
    }

}

