using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using UnityEditor;

public class UIStyleInfo
{
    public string m_StyleInfoName;

    public UITextStyleInfo m_TextInfo = new UITextStyleInfo();
    public UIRawImageInfo m_RawImageInfo = new UIRawImageInfo();
    public UIImageInfo m_ImageInfo = new UIImageInfo();
    public UIRectTransformInfo m_RectTransformInfo = new UIRectTransformInfo();
    public UIButtonInfo m_ButtonInfo = new UIButtonInfo();

    public bool isFold = false;

    public UIStyleInfo GetStyle(GameObject go)
    {
        Text compText = go.GetComponent<Text>();
        if (compText != null)
        {
            m_TextInfo.isActive = true;
            m_TextInfo.GetStyle(compText);
        }

        Image compImage = go.GetComponent<Image>();
        if (compImage != null)
        {
            m_ImageInfo.isActive = true;
            m_ImageInfo.GetStyle(compImage);
        }

        RectTransform compRectTransform = go.GetComponent<RectTransform>();
        if (compRectTransform != null)
        {
            m_RectTransformInfo.isActive = true;
            m_RectTransformInfo.GetStyle(compRectTransform);
        }

        return this;
    }

    public void ApplyStyle(GameObject go)
    {
        if (m_TextInfo.isActive)
        {
            Text compText = go.GetComponent<Text>();
            if (compText!= null)
            {
                m_TextInfo.ApplyStyle(compText);
            }
        }

        if (m_ImageInfo.isActive)
        {
            Image compText = go.GetComponent<Image>();
            if (compText != null)
            {
                m_ImageInfo.ApplyStyle(compText);
            }
        }

        if (m_RectTransformInfo.isActive)
        {
            RectTransform compText = go.GetComponent<RectTransform>();
            if (compText != null)
            {
                m_RectTransformInfo.ApplyStyle(compText);
            }
        }
    }

}

public class UIStyleInfoInterface
{
    /// <summary>
    /// 正则表达式匹配规则
    /// </summary>

    public string REfilter;
    public bool isFold = false;
    public bool isActive = false;

    public bool IsFits(string l_UIname)
    {
        //return Regex.IsMatch(l_UIname, REfilter);

        return (l_UIname == REfilter);
    }

    public virtual void ApplyStyle(Component component)
    {

    }

    public virtual UIStyleInfoInterface GetStyle(Component component)
    {
        //UIInfoInterface tmp = new UIInfoInterface();

        REfilter = component.name;

        return this;
    }
}

public class UIGraphicInfo : UIStyleInfoInterface
{
    public bool raycastTarget;
    public Color color;
    public Material material;


    public override void ApplyStyle(Component component)
    {
        base.ApplyStyle(component);

        Graphic comp = (Graphic)component;

        comp.color = color;
        comp.material = material;
        comp.raycastTarget = raycastTarget;

    }

    public override UIStyleInfoInterface GetStyle(Component component)
    {
        Graphic comp = (Graphic)component;
        UIGraphicInfo style = (UIGraphicInfo)base.GetStyle(component);

        style.color         = comp.color;
        style.material      = comp.material;
        style.raycastTarget = comp.raycastTarget;

        return style;
    }
}
[System.Serializable]
public class UITextStyleInfo : UIGraphicInfo
{
    public Font font;
    public FontStyle fontStyle;
    public int fontSize;
    public float lineSpacing;
    public TextAnchor alignment;
    public HorizontalWrapMode horizontalOverflow;
    public VerticalWrapMode verticalOverflow;
    public bool alignByGeometry;

    public bool richText;
    public bool bestFit;

    public override void ApplyStyle(Component component)
    {
        base.ApplyStyle(component);
        Text comp = (Text)component;

        comp.font                 = font; 
        comp.fontStyle            = fontStyle;
        comp.fontSize             = fontSize;
        comp.lineSpacing          = lineSpacing;
        comp.alignment            = alignment;
        comp.horizontalOverflow   = horizontalOverflow;
        comp.verticalOverflow     = verticalOverflow;
        comp.alignByGeometry      = alignByGeometry;
        comp.supportRichText      = richText;
        comp.resizeTextForBestFit = bestFit;
    }

    public override UIStyleInfoInterface GetStyle(Component component)
    {
        Text comp = (Text)component;
        UITextStyleInfo style = (UITextStyleInfo)base.GetStyle(component); ;

        style.font               = comp.font;
        style.fontStyle          = comp.fontStyle;
        style.fontSize           = comp.fontSize;
        style.lineSpacing        = comp.lineSpacing;
        style.alignment          = comp.alignment;
        style.horizontalOverflow = comp.horizontalOverflow;
        style.verticalOverflow   = comp.verticalOverflow;
        style.alignByGeometry    = comp.alignByGeometry;

        style.richText           = comp.supportRichText;
        style.bestFit            = comp.resizeTextForBestFit;
        return style;
    }
}
[System.Serializable]
public class UIImageInfo : UIGraphicInfo
{

}
[System.Serializable]
public class UIRawImageInfo : UIGraphicInfo
{
    Rect uvRect;
    public override void ApplyStyle(Component component)
    {
        base.ApplyStyle(component);
        RawImage comp = (RawImage)component;
        comp.uvRect = uvRect;
    }

    public override UIStyleInfoInterface GetStyle(Component component)
    {
        RawImage comp = (RawImage)component;
        UIRawImageInfo style = (UIRawImageInfo)base.GetStyle(component);

        style.uvRect = comp.uvRect;
        return style;
    }
}
[System.Serializable]
public class UIButtonInfo : UIStyleInfoInterface
{
    bool interactable;

    public override void ApplyStyle(Component component)
    {
        base.ApplyStyle(component);
        Button comp = (Button)component;

        comp.interactable = interactable;
    }

    public override UIStyleInfoInterface GetStyle(Component component)
    {
        Button comp = (Button)component;
        UIButtonInfo style = (UIButtonInfo)base.GetStyle(component);

        style.interactable = comp.interactable;
        return style;
    }
}

[System.Serializable]
public class UIRectTransformInfo : UIStyleInfoInterface
{
    public Vector2 anchorMin = Vector2.zero;
    public Vector2 anchorMax = Vector2.one;
    public Vector2 sizeDelta;
    public Vector2 pivot;

    public Vector3 anchoredPosition3D = Vector3.zero;
    public Vector3 localRotation      = Vector3.zero;
    public Vector3 localScale         = Vector3.one;

    public override void ApplyStyle(Component component)
    {
        base.ApplyStyle(component);
        RectTransform comp = (RectTransform)component;

        comp.anchorMin          = anchorMin;
        comp.anchorMax          = anchorMax;
        comp.localEulerAngles   = localRotation;
        comp.localScale         = localScale;
        comp.pivot              = pivot;
        comp.anchoredPosition3D = anchoredPosition3D;
        comp.sizeDelta          = sizeDelta;
    }

    public override UIStyleInfoInterface GetStyle(Component component)
    {
        RectTransform comp = (RectTransform)component;
        UIRectTransformInfo style = (UIRectTransformInfo)base.GetStyle(component);

        style.anchorMin          = comp.anchorMin;
        style.anchorMax          = comp.anchorMax;
        style.localRotation      = comp.localEulerAngles;
        style.localScale         = comp.localScale;
        style.pivot              = comp.pivot;
        style.anchoredPosition3D = comp.anchoredPosition3D;
        style.sizeDelta          = comp.sizeDelta;

        return style;
    }
}
