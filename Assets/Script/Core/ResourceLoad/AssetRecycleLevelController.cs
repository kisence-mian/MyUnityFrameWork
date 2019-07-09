using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class AssetRecycleLevelController
{
    private static AssetRecycleLevel nowRecycleLevel = AssetRecycleLevel.None;
    public static AssetRecycleLevel NowRecycleLevel
    {
        get
        {
            if (nowRecycleLevel == AssetRecycleLevel.None)
            {
                nowRecycleLevel= GetAssetRecycleLevel();
            }
            return nowRecycleLevel;
        }
    }
    /// <summary>
    /// 根据设备内存大小开启对应的回收设置
    /// </summary>

    private static AssetRecycleLevel GetAssetRecycleLevel()
    {
        int memorySize = SystemInfo.systemMemorySize;
        AssetRecycleLevel assetRecycleLevel = AssetRecycleLevel.Level1000;
        if (memorySize <= 1124)
        {
            assetRecycleLevel = AssetRecycleLevel.Level1000;
        }
        else if (memorySize <= 1600)
        {
            assetRecycleLevel = AssetRecycleLevel.Level1500;
        }
        else if (memorySize <= 2148)
        {
            assetRecycleLevel = AssetRecycleLevel.Level2000;
        }
        else if (memorySize <= 3196)
        {
            assetRecycleLevel = AssetRecycleLevel.Level3000;
        }
        else if (memorySize <= 4196)
        {
            assetRecycleLevel = AssetRecycleLevel.Level4000;
        }
        else
        {
            assetRecycleLevel = AssetRecycleLevel.Level4000Plus;
        }
        return assetRecycleLevel;
    }
}

public enum AssetRecycleLevel
{
    None=-1,
    /// <summary>
    /// 小于等于1G
    /// </summary>
    Level1000 =0,
    Level1500 =1,
    Level2000 =2,
    Level3000 =3,
    Level4000 =4,
    /// <summary>
    /// 大于4G
    /// </summary>
    Level4000Plus =5,
}

