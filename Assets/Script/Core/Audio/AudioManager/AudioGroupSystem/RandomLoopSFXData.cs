using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class RandomLoopSFXData
{
    public float currentTime;
    public int runTime;

    public SFXRandomLoopData configData;
    public RandomLoopSFXData(SFXRandomLoopData configData)
    {
        this.configData = configData;
        RandomTime();
    }

    public bool IsRunFinished()
    {
        if (configData.loopTimes == -1)
            return false;
        if (runTime >= configData.loopTimes)
            return true;

        return false;
    }
    private void RandomTime()
    {
        float r = UnityEngine.Random.Range(configData.delayRange.x, configData.delayRange.y);
        currentTime = r;
    }
    public SFXPlayData Excute()
    {
        SFXPlayData data = null;
        if (currentTime <= 0)
        {
            if (configData.SFXDatas.Count > 0)
            {
                int r = UnityEngine.Random.Range(0, configData.SFXDatas.Count);
                data= configData.SFXDatas[r];
            }
            runTime++;
            Debug.Log("runTime:" + runTime);
            RandomTime();
        }
        else
        {
            currentTime -= Time.deltaTime;
        }

        return data;
    }
}
