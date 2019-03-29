using UnityEngine;
using UnityEditor;

/// <summary>
/// 通用数据修改窗体
/// </summary>
public class GeneralDataModificationWindow : EditorWindow
{
    string m_Title;
    //object value;
    object modifi_Value;
    CallBackR<object, object> customDrawGUI;
    CallBack<object> modificationCompleteCallBack;
    CallBackR<bool, object> checkCanOkButtonCallBack;

   static  GeneralDataModificationWindow win;
    public static GeneralDataModificationWindow GetInstance()
    {
        return win;
    }
    EditorWindow otherWindow;
    /// <summary>
    /// 用于储存其他要使用的参数
    /// </summary>
    public static object otherParameter;
    /// <summary>
    /// 打开窗口
    /// </summary>
    /// <param name="title">标题</param>
    /// <param name="value">希望修改的数据</param>
    /// <param name="customDrawGUI">自定义GUI绘制</param>
    /// <param name="checkCanOkButtonCallBack">检查是否能使用"OK"按钮</param>
    /// <param name="modificationCompleteCallBack">完成修改回调</param>
    public static void OpenWindow(EditorWindow otherWindow, string title,object value,CallBackR<object,object> customDrawGUI,CallBackR<bool,object> checkCanOkButtonCallBack, CallBack<object> modificationCompleteCallBack)
    {
       win = GetWindow<GeneralDataModificationWindow>();
        win.wantsMouseMove = true;
        win.autoRepaintOnSceneChange = true;
        win.otherWindow = otherWindow;
        FocusWindowIfItsOpen<GeneralDataModificationWindow>();

        win.m_Title = title;
        //win.value = value;
        win.modifi_Value = value;//.DeepCopySelf();

        win.customDrawGUI = customDrawGUI;
        win.modificationCompleteCallBack = modificationCompleteCallBack;
        win.checkCanOkButtonCallBack = checkCanOkButtonCallBack;
    }

    private void OnGUI()
    {
        if (modifi_Value == null)
            return;
        EditorDrawGUIUtil.DrawHorizontalCenter(() =>
        {
            GUILayout.Label(m_Title);
        });

        EditorDrawGUIUtil.DrawScrollView(modifi_Value, () =>
         {
             if (customDrawGUI != null)
                 modifi_Value = customDrawGUI(modifi_Value);
             else
             {
                 modifi_Value = EditorDrawGUIUtil.DrawBaseValue("Value", modifi_Value);
             }
             GUILayout.FlexibleSpace();
         });

        GUILayout.FlexibleSpace();
        bool isClose = false;
        EditorDrawGUIUtil.DrawHorizontalCenter(() =>
        {
            if (GUILayout.Button("OK",GUILayout.Width( position.width/4)))
            {
                if (checkCanOkButtonCallBack != null)
                {
                    if (!checkCanOkButtonCallBack(modifi_Value))
                        return;
                }
               if(modificationCompleteCallBack != null)
                {
                    modificationCompleteCallBack(modifi_Value);
                }
                if (otherWindow)
                    otherWindow.Repaint();
                isClose = true;
            }

            if (GUILayout.Button("Cancel", GUILayout.Width(position.width / 4)))
            {
                isClose = true;
            }
        });
        GUILayout.Space(6);

        if (isClose)
            Close();
    }
}