using UnityEngine;
using System.Collections;
using UnityEditor;

public static class EditorGUIStyleData 
{
    static bool isInit = false;

    private static GUIStyle s_ErrorMessageLabel;
    private static GUIStyle s_WarnMessageLabel;

    private static GUIStyle s_RichText;

    public static int s_ButtonWidth_large = 200;
    public static int s_ButtonWidth_small = 100;

    public static GUIStyle ErrorMessageLabel
    {
        get
        {
            if(!isInit)Init();
            return s_ErrorMessageLabel;
        }
    }

    public static GUIStyle WarnMessageLabel
    {
        get
        {
            if (!isInit) Init();
            return s_WarnMessageLabel;
        }
    }

    public static GUIStyle RichText
    {
        get
        {
            if (!isInit) Init();
            return s_RichText;
        }
    }

    static EditorGUIStyleData()
    {
        //Init();
    }

    public static void Init()
    {
        isInit = true;

        s_ErrorMessageLabel = new GUIStyle();
        s_WarnMessageLabel = new GUIStyle();

        ErrorMessageLabel.normal.textColor = Color.red;
        WarnMessageLabel.normal.textColor = Color.yellow;

        s_RichText = new GUIStyle();
        RichText.richText = true;
        RichText.fontSize = 15;
        //RichText.font =/* Font.*/
    }
}
