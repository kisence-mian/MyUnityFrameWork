using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class RandomLoopMusicData
{
    public string flag = "";
    private int runTime;
    private bool isPlaying = false;
    private int currentPlayIndex = -1;

    public MusicRandomLoopData configData;
    public RandomLoopMusicData(MusicRandomLoopData configData)
    {
        this.configData = configData;
        flag = GetHashCode().ToString();
        AudioPlayManager.OnMusicStopCallBack += OnMusicPlayComplete;
    }

    private void OnMusicPlayComplete(string name, int channal, string flag)
    {
        if (this.flag == flag)
        {
            //Debug.Log("OnMusicStopCallBack :" + name + " flag:" + flag);
            foreach (var item in configData.musicDatas)
            {
                if (item.name == name)
                {
                    isPlaying = false;
                    break;
                }
            }
        }
        

    }

    public bool IsRunFinished()
    {
        if (configData.loopTimes == -1)
            return false;
        if (runTime >= configData.loopTimes)
            return true;

        return false;
    }

    public MusicPlayData Excute()
    {
        if (isPlaying)
            return null;
        if (configData.musicDatas.Count == 0)
            return null;

        isPlaying = true;
        //Debug.Log("currentPlayIndex:" + currentPlayIndex + " flag :"+flag);
        if (configData.isRandom)
        {
            List<MusicPlayData> musicDatas = new List<MusicPlayData>();
            musicDatas.AddRange(configData.musicDatas);
            if (currentPlayIndex != -1)
                musicDatas.RemoveAt(currentPlayIndex);
            if (musicDatas.Count > 0)
            {
                int r = UnityEngine.Random.Range(0, configData.musicDatas.Count);
                currentPlayIndex = r;
            }
            else
            {
                currentPlayIndex = 0;
            }

        }
        else
        {
            currentPlayIndex++;
            if (currentPlayIndex >= configData.musicDatas.Count)
            {
                runTime++;
                currentPlayIndex = 0;
            }
        }


        return configData.musicDatas[currentPlayIndex];
    }

    public void Close()
    {
        AudioPlayManager.OnMusicStopCallBack -= OnMusicPlayComplete;
    }
}

