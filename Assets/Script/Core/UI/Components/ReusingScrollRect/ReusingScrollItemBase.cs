using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReusingScrollItemBase : UIBase 
{
    public int m_index = 0;

    public float m_height = 0;
    public float m_width = 0;

    private RectTransform m_rectTransform;
    public RectTransform m_RectTransform
    {
        get {

            if (m_rectTransform == null)
            {
                m_rectTransform = GetComponent<RectTransform>();
            }

            return m_rectTransform;
        }
        set { m_rectTransform = value; }
    }

    private Bounds m_bounds;
    public Bounds m_Bounds
    {
        get {
            if (m_bounds == null)
            {
                m_bounds = new Bounds(m_RectTransform.rect.center, m_RectTransform.rect.size);
            }
            return m_bounds; 
        }
        set { m_bounds = value; }
    }


    public void SetConetnt(int index,Dictionary<string, object> data)
    {

    }

}
