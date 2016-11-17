using UnityEngine;
using System.Collections;

public class GUIUtil 
{
    static int s_fontSize = 20;

    public static int FontSize
    {
        get { return GUIUtil.s_fontSize; }
        set { GUIUtil.s_fontSize = value; }
    }

    public static void SetGUIStyle()
    {
        s_fontSize = (int)(Screen.dpi * 0.13f);

        //#if UNITY_ANDROID || UNITY_IOS
        GUI.skin.label.fontSize = s_fontSize;
        //GUI.skin.button.fixedHeight = 0;
        GUI.skin.button.fontSize = s_fontSize;

        GUI.skin.verticalScrollbar.fixedWidth = s_fontSize;
        GUI.skin.verticalScrollbarUpButton.fixedWidth = s_fontSize;
        GUI.skin.verticalScrollbarThumb.fixedWidth = s_fontSize;

        //GUI.skin.horizontalScrollbar.fixedWidth = 0;
        //GUI.skin.horizontalScrollbarLeftButton.fixedWidth = 0;
        //GUI.skin.horizontalScrollbarThumb.fixedWidth = 0;

        GUI.skin.horizontalScrollbar.fixedHeight = s_fontSize;
        GUI.skin.horizontalScrollbarLeftButton.fixedHeight = s_fontSize;
        GUI.skin.horizontalScrollbarThumb.fixedHeight = s_fontSize;

        GUI.skin.toggle.fontSize = s_fontSize;
        GUI.skin.textField.fontSize = s_fontSize;
        GUI.skin.textField.wordWrap = true;

    }
}
