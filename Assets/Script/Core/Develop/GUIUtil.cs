using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    #region Tips

    static List<string> tiplist = new List<string>();

    public static void ShowTips(string content)
    {
        if(tiplist.Count == 0)
        {
            ApplicationManager.s_OnApplicationOnGUI += TipsGUI;
        }

        tiplist.Add(content);
    }

    const int tipWindowStartID = 1000;

    static float tipWidth = Screen.width/2;
    static float tipHeight = Screen.height / 2;
    static void TipsGUI()
    {
        int lastID = 0;
        for (int i = 0; i < tiplist.Count; i++)
        {
            lastID = tipWindowStartID + i;

            Rect rect = new Rect(GetTipPos(i) ,new Vector2(tipWidth, tipHeight));
            GUILayout.Window(lastID, rect, TipsWindow, "TipsWindow");
        }

        GUI.BringWindowToFront(lastID);
    }

    static Vector2 GetTipPos(int i)
    {
        Vector3 pos = new Vector2(Screen.width / 2 - tipWidth / 2, Screen.height / 2 - tipHeight / 2) + new Vector2(50, 50) * i;

        while(pos.x + tipWidth > Screen.width)
        {
            pos.x -= Screen.width/2;
        }

        while (pos.y + tipHeight > Screen.height)
        {
            pos.y -= Screen.height/2;
        }

        return pos;
    }

    static Vector2 TipsScrollPos = Vector2.zero;

    static void TipsWindow(int windowID)
    {
        TipsScrollPos = GUILayout.BeginScrollView(TipsScrollPos);
        GUILayout.Label(tiplist[windowID - tipWindowStartID], GUILayout.ExpandHeight(true));
        GUILayout.EndScrollView();

        if ( GUILayout.Button("OK"))
        {
            tiplist.RemoveAt(windowID - tipWindowStartID);
            if(tiplist.Count == 0)
            {
                ApplicationManager.s_OnApplicationOnGUI -= TipsGUI;
            }
        }
    }

    #endregion
}
