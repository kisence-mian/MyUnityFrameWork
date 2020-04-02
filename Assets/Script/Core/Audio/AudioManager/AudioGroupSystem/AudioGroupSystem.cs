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
        if (currentAudioGroupData != null && keyName == currentAudioGroupData.keyName && audioGroupPlayState == AudioGroupPlayState.Playing)
            return;

        //Debug.Log("AudioGroupSystem.Play :" + keyName);
        audioGroupPlayState = AudioGroupPlayState.Playing;


        // Dictionary<int, AudioAsset> playingMusics = AudioPlayManager.a2DPlayer.bgMusicDic;
        if (currentAudioGroupData != null)
        {
            foreach (var item in currentAudioGroupData.fixedMusicDatas)
            {
                AudioPlayManager.StopMusic2D(item.channel, fadeTime);
            }
            foreach (var ss in currentAudioGroupData.loopMusicDatas)
            {
                foreach (var item in ss.musicDatas)
                {
                    AudioPlayManager.StopMusic2D(item.channel, fadeTime);
                }
            }
        }
        currentAudioGroupData = audioGroupDataDic[keyName];
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
           instance.randomLoopSFXDatas.Add( new RandomLoopSFXData(item));
        }

        foreach (var item in instance.randomLoopMusicDatas)
        {
            item.Close();
        }
        instance.randomLoopMusicDatas.Clear();
        foreach (var item in currentAudioGroupData.loopMusicDatas)
        {
            instance.randomLoopMusicDatas.Add(new RandomLoopMusicData(item));
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

    private static void PlayMusicData(MusicPlayData data,string flag="")
    {
        AudioPlayManager.PlayMusic2D(data.name, data.channel, data.volume, data.isLoop, data.fadeTime, data.delay,flag:flag);
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
        ResourceManager.DestoryAssetsCounter(ConfigName);
        audioGroupDataDic.Clear();
        foreach (var item in datas)
        {
            audioGroupDataDic.Add(item.keyName, item);
        }
    }
 
    List<RandomLoopSFXData> randomLoopSFXDatas = new List<RandomLoopSFXData>();
    List<RandomLoopSFXData> clearRandomList = new List<RandomLoopSFXData>();

    List<RandomLoopMusicData> randomLoopMusicDatas = new List<RandomLoopMusicData>();
    List<RandomLoopMusicData> clearLoopMusicDatas = new List<RandomLoopMusicData>();
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
              SFXPlayData sFXPlayData=  item.Excute();
                if (sFXPlayData != null)
                {
                    PlaySFXData(sFXPlayData);
                }
            }
        }
        if (clearRandomList.Count > 0)
        {
            foreach (var item in clearRandomList)
            {
                randomLoopSFXDatas.Remove(item);
            }
            clearRandomList.Clear();
        }

        foreach (var item in randomLoopMusicDatas)
        {
            if (item.IsRunFinished())
            {
                clearLoopMusicDatas.Add(item);
            }
            else
            {
                MusicPlayData musicPlayData = item.Excute();
                if (musicPlayData != null)
                {
                    //Debug.Log("currentAudioGroupData: " + currentAudioGroupData.keyName+ " Play MusicPlayData: " + musicPlayData.name);
                    PlayMusicData(musicPlayData,item.flag);
                }
            }
        }

        if (clearLoopMusicDatas.Count > 0)
        {
            foreach (var item in clearLoopMusicDatas)
            {
                item.Close();
                randomLoopMusicDatas.Remove(item);
            }
            clearLoopMusicDatas.Clear();
        }
    }


   

}

