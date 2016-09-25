using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class UIBase : MonoBehaviour
{

    #region 重载方法

    //当UI第一次打开时调用OnInit方法，调用时机在OnOpen之前
    public virtual void OnInit()
    {

    }

    public virtual void OnDestroy()
    {

    }

    #endregion

    #region 继承方法

    public void Init()
    {
        CreateObjectTable();
        OnInit();
    }

    public void Destroy()
    {
        OnDestroy();
    }

    public List<GameObject> m_objectList = new List<GameObject>();

    //生成对象表，便于快速获取对象，并忽略层级
    void CreateObjectTable()
    {
        for(int i=0;i < m_objectList.Count;i++)
        {
            m_objects.Add(m_objectList[i].name, m_objectList[i]);
        }
    }

    Dictionary<string, GameObject> m_objects          = new Dictionary<string, GameObject>();
    Dictionary<string, Image> m_images                = new Dictionary<string, Image>();
    Dictionary<string, Text> m_texts                  = new Dictionary<string, Text>();
    Dictionary<string, Button> m_buttons              = new Dictionary<string, Button>();
    Dictionary<string, ScrollRect> m_scrollRects      = new Dictionary<string, ScrollRect>();
    Dictionary<string, RawImage> m_rawImage           = new Dictionary<string, RawImage>();
    Dictionary<string, RectTransform> m_rectTransform = new Dictionary<string, RectTransform>();

    public GameObject GetGameObject(string name)
    {
        if (m_objects.ContainsKey(name))
        {
            return m_objects[name];
        }
        else
        {
            throw new Exception("UIWindowBase GetGameObject error: dont find " + name);
        }
    }

    public RectTransform GetRectTransform(string name)
    {
        if (m_rectTransform.ContainsKey(name))
        {
            return m_rectTransform[name];
        }

        if (m_objects.ContainsKey(name))
        {
            RectTransform tmp = m_objects[name].GetComponent<RectTransform>();

            m_rectTransform.Add(name, tmp);

            return tmp;
        }
        else
        {
            throw new Exception("UIWindowBase GetRectTransform error: dont find " + name);
        }
    }

    public Image GetImage(string name)
    {
        if (m_images.ContainsKey(name))
        {
            return m_images[name];
        }

        if (m_objects.ContainsKey(name))
        {
            Image tmp = m_objects[name].GetComponent<Image>();

            m_images.Add(name, tmp);

            return tmp;
        }
        else
        {
            throw new Exception("UIWindowBase GetImage error: dont find " + name);
        }
    }

    public Text GetText(string name)
    {
        if (m_texts.ContainsKey(name))
        {
            return m_texts[name];
        }

        if (m_objects.ContainsKey(name))
        {
            Text tmp = m_objects[name].GetComponent<Text>();

            m_texts.Add(name, tmp);

            return tmp;
        }
        else
        {
            throw new Exception("UIWindowBase GetText error: dont find " + name);
        }
    }

    public Button GetButton(string name)
    {
        if (m_buttons.ContainsKey(name))
        {
            return m_buttons[name];
        }

        if (m_objects.ContainsKey(name))
        {
            Button tmp = m_objects[name].GetComponent<Button>();

            m_buttons.Add(name, tmp);
            return tmp;
        }
        else
        {
            throw new Exception("UIWindowBase GetText error: dont find " + name);
        }
    }

    public ScrollRect GetScrollRect(string name)
    {
        if (m_scrollRects.ContainsKey(name))
        {
            return m_scrollRects[name];
        }

        if (m_objects.ContainsKey(name))
        {
            ScrollRect tmp = m_objects[name].GetComponent<ScrollRect>();

            m_scrollRects.Add(name, tmp);
            return tmp;
        }
        else
        {
            throw new Exception("UIWindowBase GetScrollRect error: dont find " + name);
        }
    }

    public RawImage GetRawImage(string name)
    {
        if (m_rawImage.ContainsKey(name))
        {
            return m_rawImage[name];
        }

        if (m_objects.ContainsKey(name))
        {
            RawImage tmp = m_objects[name].GetComponent<RawImage>();

            m_rawImage.Add(name, tmp);
            return tmp;
        }
        else
        {
            throw new Exception("UIWindowBase GetRawImage error: dont find " + name);
        }
    }
    #endregion
}
