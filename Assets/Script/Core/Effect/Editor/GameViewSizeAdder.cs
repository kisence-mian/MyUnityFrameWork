using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameViewSizeAdder
{
    [InitializeOnLoadMethod]
    static void Init()
    {
        var wide = new GameViewSizeHelper.GameViewSize
        {
            type = GameViewSizeHelper.GameViewSizeType.FixedResolution,
            width = 2436,
            height = 1125,
            baseText = "iPhone X/XS Landscape"
        };
        var tall = new GameViewSizeHelper.GameViewSize
        {
            type = GameViewSizeHelper.GameViewSizeType.FixedResolution,
            width = 1125,
            height = 2436,
            baseText = "iPhone X/XS Portrait"
        };
        GameViewSizeHelper.AddCustomSize(GameViewSizeGroupType.Standalone, wide);
        GameViewSizeHelper.AddCustomSize(GameViewSizeGroupType.Standalone, tall);
        GameViewSizeHelper.AddCustomSize(GameViewSizeGroupType.Android, wide);
        GameViewSizeHelper.AddCustomSize(GameViewSizeGroupType.Android, tall);
    }
}
