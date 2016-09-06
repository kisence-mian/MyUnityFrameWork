using UnityEngine;
using System.Collections;
using UnityEditor;

public static class EditorGUIStyleData 
{
    public static GUIStyle s_ErrorMessageLabel = new GUIStyle();
    public static GUIStyle s_WarnMessageLabel = new GUIStyle();

    static EditorGUIStyleData()
    {
        s_ErrorMessageLabel.normal.textColor = Color.red;
        s_WarnMessageLabel.normal.textColor = Color.yellow;
    }
}
