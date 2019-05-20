using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class AudioGroupData
{
    /// <summary>
    /// 用于区别，启动的key
    /// </summary>
    public string keyName = "";
    /// <summary>
    /// 描述
    /// </summary>
    public string description = "";
    /// <summary>
    /// 固定循环的音乐
    /// </summary>
    public List<MusicPlayData> fixedMusicDatas = new List<MusicPlayData>();
    /// <summary>
    /// 固定播放一次的音效
    /// </summary>
    public List<SFXPlayData> fixedSFXDatas = new List<SFXPlayData>();

    /// <summary>
    /// 随机时间触发循环音效
    /// </summary>
    public List<SFXRandomLoopData> sFXRandomLoopDatas = new List<SFXRandomLoopData>();
}

public class SFXRandomLoopData
{
    /// <summary>
    /// 循环次数，-1无限次
    /// </summary>
    public int loopTimes = -1;
    public Vector2 delayRange;
    public List<SFXPlayData> SFXDatas = new List<SFXPlayData>();
}

public class MusicPlayData
{
    public string name="";
    public float volume=1f;
    public int channel;
    public float delay;
    public float fadeTime=1f;
    public bool isLoop=true;
}

public class SFXPlayData
{
    public string name="";
    public float volume = 1f;
    public float delay;
    public float pitch = 1 ;
}
