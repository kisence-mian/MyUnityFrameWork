using UnityEngine;
using System.Collections;

public class GUIUtil 
{
    static bool s_isInit = false;

    static int s_fontSize = 20;

    public static int FontSize
    {
        get { return GUIUtil.s_fontSize; }
        set { GUIUtil.s_fontSize = value; }
    }

    public static void SetGUIStyle()
    {
        if (!s_isInit)
        {
            s_isInit = true;

            s_fontSize = (int)(Screen.dpi * 0.13f);

            #if UNITY_EDITOR
            s_fontSize *= 3;

            #endif


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

    const int maxContent = 15000;
    public static void SafeTextArea(string content)
    {
        if(content.Length > maxContent)
        {
            GUILayout.TextArea(content.Substring(0, maxContent));
            SafeTextArea(content.Substring(maxContent+ 1,content.Length - maxContent - 1));
        }
        else
        {
            GUILayout.TextArea(content);
        }
    }
}
