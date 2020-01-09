using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PreLoadResFlowItem : FlowItemBase
{
    /// <summary>
    /// 预加载进度显示 ：当前数量：最大数量，是否加载完毕
    /// </summary>
    public  CallBack<int, int, bool> OnPreLoadProgressCallBack;
    private  List<PreloadResourcesDataGenerate> otherResList = new List<PreloadResourcesDataGenerate>();
    /// <summary>
    /// 添加其他需要预加载的配置
    /// </summary>
    /// <param name="resList"></param>
    public  void AddOtherPreLoadResources(List<PreloadResourcesDataGenerate> resList)
    {
        otherResList = resList;
    }


    private  void PreLoadProgress(int currentNum, int count)
    {
        bool isFinish = false;
        if (count == 0 || currentNum >= count)
        {
            isFinish = true;
        }

        if (OnPreLoadProgressCallBack != null)
            OnPreLoadProgressCallBack(currentNum, count, isFinish);

        //Debug.Log("PreLoadProgress:" + currentNum + "/" + count);
        if (isFinish)
        {
            Finish(null);
        }
    }
    protected override void OnFlowStart(params object[] paras)
    {
        Debug.Log("开始预加载");
        PreloadManager.progressCallBack += PreLoadProgress;
        PreloadManager.StartLoad(otherResList);
       
    }

    protected override void OnFlowFinished()
    {
        PreloadManager.progressCallBack -= PreLoadProgress;

    }
}
