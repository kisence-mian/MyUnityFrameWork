using UnityEngine;
using System.Collections;
using UnityEditor;

public static class EditorGUIStyleData 
{
    public static GUIStyle s_ErrorMessageLabel;
    public static GUIStyle s_WarnMessageLabel ;

    public static int s_ButtonWidth_large = 200;
    public static int s_ButtonWidth_small = 100;

    static EditorGUIStyleData()
    {
        //Init();
    }

    public static void Init()
    {
        if (s_ErrorMessageLabel == null)
        {
            s_ErrorMessageLabel = new GUIStyle();
            s_WarnMessageLabel = new GUIStyle();

            s_ErrorMessageLabel.normal.textColor = Color.red;
            s_WarnMessageLabel.normal.textColor = Color.yellow;
        }
    }
}
