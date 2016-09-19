using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using UnityEditor;

public class UIStyleInfo
{
    public string m_StyleInfoName;

    public UITextStyleInfo m_TextInfo;
    public UIRawImageInfo m_RawImageInfo;
    public UIImageInfo m_ImageInfo;
    public UIRectTransformInfo m_RectTransformInfo;
    public UIButtonInfo m_ButtonInfo;

    public void GetStyle(GameObject go)
    {
        //m_StyleInfoName = go.name;

        //m_TextInfo = UITextStyleInfo();
        //Text[] texts = go.GetComponentsInChildren<Text>();
        //UITextStyleInfo l_textInfo = new UITextStyleInfo();;
        //for (int i = 0; i < texts.Length;i++)
        //{
        //    m_TextInfo.Add((UITextStyleInfo)l_textInfo.GetStyle(texts[i]));
        //}

        //m_Images = new List<UIImageInfo>();
        //Image[] Images = go.GetComponentsInChildren<Image>();
        //UIImageInfo l_imageInfo = new UIImageInfo(); ;
        //for (int i = 0; i < Images.Length; i++)
        //{
        //    m_Images.Add((UIImageInfo)l_imageInfo.GetStyle(Images[i]));
        //}


        //m_RawImages = new List<UIRawImageInfo>();
        //RawImage[] rawImagess = go.GetComponentsInChildren<RawImage>();
        //UIRawImageInfo l_rawImageInfo = new UIRawImageInfo(); ;
        //for (int i = 0; i < rawImagess.Length; i++)
        //{
        //    m_RawImages.Add((UIRawImageInfo)l_rawImageInfo.GetStyle(rawImagess[i]));
        //}


        //m_Buttons = new List<UIButtonInfo>();
        //Button[] Buttons = go.GetComponentsInChildren<Button>();
        //UIButtonInfo l_ButtonInfo = new UIButtonInfo(); ;
        //for (int i = 0; i < Buttons.Length; i++)
        //{
        //    m_Buttons.Add((UIButtonInfo)l_ButtonInfo.GetStyle(Buttons[i]));
        //}

        //m_RectTransforms = new List<UIRectTransformInfo>();
        //RectTransform[] rects = go.GetComponentsInChildren<RectTransform>();
        //UIRectTransformInfo l_rectInfo = new UIRectTransformInfo(); ;
        //for (int i = 0; i < rects.Length; i++)
        //{
        //    m_RectTransforms.Add((UIRectTransformInfo)l_rectInfo.GetStyle(rects[i]));
        //}
    }

    //public void ApplyStyle(GameObject go)
    //{
    //    Text[] cmops = go.GetComponentsInChildren<Text>();
    //    for (int i = 0; i < m_Texts.Count; i++)
    //    {
    //        for (int j = 0; j < cmops.Length; j++)
    //        {
    //            if (m_Texts[i].IsFits(cmops[j].name))
    //            {
    //                m_Texts[i].ApplyStyle(cmops[j]);
    //            }
    //        }
    //    }

    //    Image[] Images = go.GetComponentsInChildren<Image>();
    //    for (int i = 0; i < m_Images.Count; i++)
    //    {
    //        for (int j = 0; j < Images.Length; j++)
    //        {
    //            if (m_Images[i].IsFits(Images[j].name))
    //            {
    //                m_Images[i].ApplyStyle(Images[j]);
    //            }
    //        }
    //    }

    //    RawImage[] rawImagess = go.GetComponentsInChildren<RawImage>();
    //    for (int i = 0; i < m_RawImages.Count; i++)
    //    {
    //        for (int j = 0; j < rawImagess.Length; j++)
    //        {
    //            if (m_RawImages[i].IsFits(cmops[j].name))
    //            {
    //                m_RawImages[i].ApplyStyle(cmops[j]);
    //            }
    //        }
    //    }

    //    Button[] Buttons = go.GetComponentsInChildren<Button>();
    //    for (int i = 0; i < m_Buttons.Count; i++)
    //    {
    //        for (int j = 0; j < Buttons.Length; j++)
    //        {
    //            if (m_Buttons[i].IsFits(Buttons[j].name))
    //            {
    //                m_Buttons[i].ApplyStyle(Buttons[j]);
    //            }
    //        }
    //    }

    //    RectTransform[] rects = go.GetComponentsInChildren<RectTransform>();
    //    for (int i = 0; i < m_RectTransforms.Count; i++)
    //    {
    //        for (int j = 0; j < rects.Length; j++)
    //        {
    //            if (m_RectTransforms[i].IsFits(rects[j].name))
    //            {
    //                m_RectTransforms[i].ApplyStyle(rects[j]);
    //            }
    //        }
    //    }
    //}

    //public static string StlyleData2String(UIStyleInfo l_styleData)
    //{
    //    Dictionary<string, object> result = new Dictionary<string, object>();

    //    result.Add("StyleInfoName",     l_styleData.m_StyleInfoName);
    //    result.Add("m_Texts",           JsonTool.List2Json<UITextStyleInfo>(l_styleData.m_Texts));
    //    result.Add("m_RawImages",       JsonTool.List2Json<UIRawImageInfo>(l_styleData.m_RawImages));
    //    result.Add("m_Images",          JsonTool.List2Json<UIImageInfo>(l_styleData.m_Images));
    //    result.Add("m_RectTransforms",  JsonTool.List2Json<UIRectTransformInfo>(l_styleData.m_RectTransforms));
    //    result.Add("m_Buttons",         JsonTool.List2Json<UIButtonInfo>(l_styleData.m_Buttons));

    //    return MiniJSON.Json.Serialize(result);
    //}

    //public static UIStyleInfo String2StlyleData(string l_content)
    //{
    //    Dictionary<string, object> dict = (Dictionary<string, object>)MiniJSON.Json.Deserialize(l_content);

    //    UIStyleInfo result = new UIStyleInfo();

    //    result.m_StyleInfoName = (string)dict["StyleInfoName"];

    //    result.m_Texts          = JsonTool.Json2List<UITextStyleInfo>(      (string)dict["m_Texts"]);
    //    result.m_RawImages      = JsonTool.Json2List<UIRawImageInfo>(       (string)dict["m_RawImages"]);
    //    result.m_Images         = JsonTool.Json2List<UIImageInfo>(          (string)dict["m_Images"]);
    //    result.m_RectTransforms = JsonTool.Json2List<UIRectTransformInfo>(  (string)dict["m_RectTransforms"]);
    //    result.m_Buttons        = JsonTool.Json2List<UIButtonInfo>(         (string)dict["m_Buttons"]);

    //    return result;
    //}
}

public class UIInfoInterface
{
    /// <summary>
    /// 正则表达式匹配规则
    /// </summary>

    public string REfilter;

    public bool IsFits(string l_UIname)
    {
        //return Regex.IsMatch(l_UIname, REfilter);

        return (l_UIname == REfilter);
    }

    public virtual void ApplyStyle(Component component)
    {

    }

    public virtual UIInfoInterface GetStyle(Component component)
    {
        //UIInfoInterface tmp = new UIInfoInterface();

        REfilter = component.name;

        return this;
    }
}

public class UIGraphicInfo : UIInfoInterface
{
    bool raycastTarget;
    public Color color;
    Material material;


    public override void ApplyStyle(Component component)
    {
        base.ApplyStyle(component);

        Graphic comp = (Graphic)component;

        comp.color = color;
        comp.material = material;
        comp.raycastTarget = raycastTarget;
    }

    public override UIInfoInterface GetStyle(Component component)
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
    public string fontPath = "";
    public FontStyle fontStyle;
    public int fontSize;
    public TextAnchor alignment;
    public HorizontalWrapMode horizontalOverflow;
    public VerticalWrapMode verticalOverflow;
    public bool alignByGeometry;

    public override void ApplyStyle(Component component)
    {
        base.ApplyStyle(component);
        Text comp = (Text)component;

        comp.font               = AssetDatabase.LoadAssetAtPath<Font>(fontPath); 
        comp.fontStyle          = fontStyle;
        comp.fontSize           = fontSize;
        comp.alignment          = alignment;
        comp.horizontalOverflow = horizontalOverflow;
        comp.verticalOverflow   = verticalOverflow;
        comp.alignByGeometry    = alignByGeometry;

    }

    public override UIInfoInterface GetStyle(Component component)
    {
        Text comp = (Text)component;
        UITextStyleInfo style = (UITextStyleInfo)base.GetStyle(component); ;

        style.fontPath           = AssetDatabase.GetAssetPath(comp.font);
        style.fontStyle          = comp.fontStyle;
        style.fontSize           = comp.fontSize;
        style.alignment          = comp.alignment;
        style.horizontalOverflow = comp.horizontalOverflow;
        style.verticalOverflow   = comp.verticalOverflow;
        style.alignByGeometry    = comp.alignByGeometry;

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

    public override UIInfoInterface GetStyle(Component component)
    {
        RawImage comp = (RawImage)component;
        UIRawImageInfo style = (UIRawImageInfo)base.GetStyle(component);

        style.uvRect = comp.uvRect;
        return style;
    }
}
[System.Serializable]
public class UIButtonInfo : UIInfoInterface
{
    bool interactable;

    public override void ApplyStyle(Component component)
    {
        base.ApplyStyle(component);
        Button comp = (Button)component;

        comp.interactable = interactable;
    }

    public override UIInfoInterface GetStyle(Component component)
    {
        Button comp = (Button)component;
        UIButtonInfo style = (UIButtonInfo)base.GetStyle(component);

        style.interactable = comp.interactable;
        return style;
    }
}

[System.Serializable]
public class UIRectTransformInfo : UIInfoInterface
{
    public Vector2 anchorMin;
    public Vector2 anchorMax;
    public Vector2 sizeDelta;
    public Vector2 pivot;

    public Vector3 anchoredPosition3D;
    public Quaternion localRotation;
    public Vector3 localScale;

    public override void ApplyStyle(Component component)
    {
        base.ApplyStyle(component);
        RectTransform comp = (RectTransform)component;

        comp.anchorMin          = anchorMin;
        comp.anchorMax          = anchorMax;
        comp.localRotation      = localRotation;
        comp.localScale         = localScale;
        comp.pivot              = pivot;
        comp.anchoredPosition3D = anchoredPosition3D;
        comp.sizeDelta          = sizeDelta;
    }

    public override UIInfoInterface GetStyle(Component component)
    {
        RectTransform comp = (RectTransform)component;
        UIRectTransformInfo style = (UIRectTransformInfo)base.GetStyle(component);

        style.anchorMin          = comp.anchorMin;
        style.anchorMax          = comp.anchorMax;
        style.localRotation      = comp.localRotation;
        style.localScale         = comp.localScale;
        style.pivot              = comp.pivot;
        style.anchoredPosition3D = comp.anchoredPosition3D;
        style.sizeDelta          = comp.sizeDelta;

        return style;
    }
}
