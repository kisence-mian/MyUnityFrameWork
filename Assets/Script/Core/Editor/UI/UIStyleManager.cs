using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

public class UIStyleManager 
{
    Dictionary<string, UIStyleInfo> m_styleData ;

    public void OnEnable()
    {
        m_styleData = UIStyleConfigManager.GetData();
    }

    public void GUI()
    {
        UIStyleList();
        CreateUIStyleGUI();
        ButtonsGUI();

        DeleteLogic();
    }

    bool isFoldCreateUIStyle = false;

    string m_CreateStyleName = "";

    void CreateUIStyleGUI()
    {
        EditorGUI.indentLevel = 1;
        isFoldCreateUIStyle = EditorGUILayout.Foldout(isFoldCreateUIStyle, "创建Style:");
        if (isFoldCreateUIStyle)
        {
            EditorGUI.indentLevel = 2;
            m_CreateStyleName = EditorGUILayout.TextField("Style名称:",m_CreateStyleName);

            if (m_CreateStyleName != "")
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();

                if (UIStyleConfigManager.GetData(m_CreateStyleName)== null)
                {
                    if (GUILayout.Button("创建新UIStyle", GUILayout.Width(EditorGUIStyleData.s_ButtonWidth_large)))
                    {
                        CreateUIStyle(m_CreateStyleName);

                        m_CreateStyleName = "";
                    }
                }
                else
                {
                    if (GUILayout.Button("覆盖UIStyle", GUILayout.Width(EditorGUIStyleData.s_ButtonWidth_large)))
                    {
                        if (EditorUtility.DisplayDialog("警告", "该模板已存在，是否覆盖？", "是", "否"))
                        {
                            CreateUIStyle(m_CreateStyleName);

                            m_CreateStyleName = "";
                        }
                    }
                }

                EditorGUILayout.Space();
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();
        }
    }

    void ButtonsGUI()
    {
        if (GUILayout.Button("保存"))
        {
            UIStyleConfigManager.SaveData();
        }

        if(GUILayout.Button("应用全部Style"))
        {
            ApplyAllStyle();
        }
    }

    void CreateUIStyle(string l_uiStyleName)
    {
        UIStyleInfo l_tmp = new UIStyleInfo();

        l_tmp.m_StyleInfoName = l_uiStyleName;

        UIStyleConfigManager.AddData(l_uiStyleName, l_tmp);
    }

    bool isFoldUIStyleList = false;
    Vector2 m_styleListScroll = new Vector2();
    /// <summary>
    /// UIStyle列表
    /// </summary>
    void UIStyleList()
    {
        EditorGUI.indentLevel = 1;
        isFoldUIStyleList = EditorGUILayout.Foldout(isFoldUIStyleList, "Style列表:");
        
        if (isFoldUIStyleList)
        {
            m_styleListScroll = EditorGUILayout.BeginScrollView(m_styleListScroll,GUILayout.ExpandHeight(false));
            foreach (var item in m_styleData.Values)
            {
                SingleUIStyleInfo(item);
            }
            EditorGUILayout.EndScrollView();
        }

    }
    string m_deleteKey = "";
    UIStyleComponentType m_compType = UIStyleComponentType.Text;
    void SingleUIStyleInfo(UIStyleInfo data)
    {
        EditorGUI.indentLevel = 2;
        data.isFold = EditorGUILayout.Foldout(data.isFold, data.m_StyleInfoName+ ":");

        if (data.isFold)
        {
            if (data.m_TextInfo != null && data.m_TextInfo.isActive)
            {
                TextStyleGUI(data.m_TextInfo, data);
            }

            if (data.m_ImageInfo != null && data.m_ImageInfo.isActive)
            {
                ImageStyleGUI(data.m_ImageInfo, data);
            }

            if (data.m_RectTransformInfo != null && data.m_RectTransformInfo.isActive)
            {
                RectTransformGUI(data.m_RectTransformInfo, data);
            }

            EditorGUILayout.Space();

            EditorGUI.indentLevel = 3;
            EditorGUILayout.BeginHorizontal();

            m_compType = (UIStyleComponentType)EditorGUILayout.EnumPopup("组件类型：", m_compType);
            if(GUILayout.Button("添加一个组件"))
            {
                AddComp(data, m_compType);
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();

            if (GUILayout.Button("应用", GUILayout.Width(EditorGUIStyleData.s_ButtonWidth_large)))
            {
                ApplySingleStyle(data);
            }

            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            if (GUILayout.Button("删除", GUILayout.Width(EditorGUIStyleData.s_ButtonWidth_large)))
            {
                if (EditorUtility.DisplayDialog("警告", "该操作不可逆，是否删除？", "是", "否"))
                {
                    m_deleteKey = data.m_StyleInfoName;
                }
            }
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
        }
    }

    void AddComp(UIStyleInfo style, UIStyleComponentType type)
    {
        switch (type)
        {
            case UIStyleComponentType.Text:
                style.m_TextInfo.isActive = true;
                break;
            case UIStyleComponentType.Image:
                style.m_ImageInfo.isActive = true;
                break;
            case UIStyleComponentType.RectTransform:
                style.m_RectTransformInfo.isActive = true;
                break;
        }
    }

#pragma warning disable
    void GraphicGUI(UIGraphicInfo data)
    {
        data.color         = EditorGUILayout.ColorField("Color:", data.color);
        data.material      = (Material)EditorGUILayout.ObjectField("Material:", data.material, typeof(Material));
        data.raycastTarget = EditorGUILayout.Toggle("Raycast Target:", data.raycastTarget);
    }

    void TextStyleGUI(UITextStyleInfo style,UIStyleInfo info)
    {
        EditorGUI.indentLevel = 3;

        EditorGUILayout.BeginHorizontal();
        style.isFold = EditorGUILayout.Foldout(style.isFold, "Text:");

        if (GUILayout.Button("删除"))
        {
            info.m_TextInfo.isActive = false;
        }

        EditorGUILayout.EndHorizontal();

        if (style.isFold)
        {
            EditorGUI.indentLevel = 4;
            EditorGUILayout.LabelField("Character:");

            EditorGUI.indentLevel = 5;

            style.font        = (Font)EditorGUILayout.ObjectField("Font:", style.font, typeof(Font));
            style.fontStyle   = (FontStyle)EditorGUILayout.EnumPopup("FontStyle", style.fontStyle);
            style.fontSize    = EditorGUILayout.IntField("FontSize",style.fontSize);
            style.lineSpacing = EditorGUILayout.FloatField("LineSpacing", style.lineSpacing);
            style.richText    = EditorGUILayout.Toggle("RichText", style.richText);

            EditorGUI.indentLevel = 4;
            EditorGUILayout.LabelField("Paragraph:");

            EditorGUI.indentLevel = 5;
            style.alignment          = (TextAnchor)EditorGUILayout.EnumPopup("FontStyle", style.alignment);
            style.alignByGeometry    = EditorGUILayout.Toggle("AlignByGeometry", style.alignByGeometry);
            style.horizontalOverflow = (HorizontalWrapMode)EditorGUILayout.EnumPopup("HorizontalOverflow", style.horizontalOverflow);
            style.verticalOverflow   = (VerticalWrapMode)EditorGUILayout.EnumPopup("VerticalOverflow", style.verticalOverflow);
            style.bestFit            = EditorGUILayout.Toggle("BestFit", style.bestFit);

            EditorGUI.indentLevel = 4;
            GraphicGUI(style);
        }
    }

    void ImageStyleGUI(UIImageInfo style, UIStyleInfo info)
    {
        EditorGUI.indentLevel = 3;

        EditorGUILayout.BeginHorizontal();
        style.isFold = EditorGUILayout.Foldout(style.isFold, "Image:");

        if (GUILayout.Button("删除"))
        {
            info.m_ImageInfo.isActive = false;
        }

        EditorGUILayout.EndHorizontal();

        if (style.isFold)
        {
            EditorGUI.indentLevel = 4;
            GraphicGUI(style);
        }
    }

    void RectTransformGUI(UIRectTransformInfo style, UIStyleInfo info)
    {
        EditorGUI.indentLevel = 3;
        EditorGUILayout.BeginHorizontal();
        style.isFold = EditorGUILayout.Foldout(style.isFold, "RectTransform:");

        if (GUILayout.Button("删除"))
        {
            info.m_RectTransformInfo.isActive = false;
        }

        EditorGUILayout.EndHorizontal();

        if (style.isFold)
        {
            EditorGUI.indentLevel = 4;

            style.anchoredPosition3D = EditorGUILayout.Vector3Field("AnchoredPosition3D", style.anchoredPosition3D);

            style.anchorMin = EditorGUILayout.Vector2Field("AnchorMin", style.anchorMin);
            style.anchorMax = EditorGUILayout.Vector2Field("AnchorMax", style.anchorMax);

            style.pivot = EditorGUILayout.Vector2Field("Pivot", style.pivot);

            style.localRotation = EditorGUILayout.Vector3Field("LocalRotation", style.localRotation);
            style.localScale    = EditorGUILayout.Vector3Field("LocalScale", style.localScale);
        }
    }

    /// <summary>
    /// 应用单个Style
    /// </summary>
    /// <param name="style">UIStyle</param>
    void ApplySingleStyle(UIStyleInfo style)
    {
        foreach (var item in UIEditorWindow.allUIPrefab.Values)
        {
            UIStyleComponent[] comps = item.GetComponentsInChildren<UIStyleComponent>();

            for (int i = 0; i < comps.Length; i++)
            {
                if (comps[i].m_styleID.Equals(style.m_StyleInfoName))
                {
                    style.ApplyStyle(comps[i].gameObject );
                }
            }
        }
    }

    void ApplyAllStyle()
    {
        foreach (var item in m_styleData.Values)
        {
            ApplySingleStyle(item);
        }
    }

    void DeleteLogic()
    {
        if(m_deleteKey != "")
        {
            UIStyleConfigManager.DeleteData(m_deleteKey);
            m_deleteKey = "";
        }
    }
}

public enum UIStyleComponentType
{
    Text,
    Image,
    RectTransform,
}