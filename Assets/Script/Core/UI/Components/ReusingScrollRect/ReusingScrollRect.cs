using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class ReusingScrollRect : ScrollRectInput
{
    public string m_ItemName = "";

    //默认是从上到下，从左到右，勾上此选项则反向
    public bool m_isInversion;

    public List<Dictionary<string, object>> m_datas = new List<Dictionary<string, object>>();
    public List<ReusingScrollItemBase> m_items = new List<ReusingScrollItemBase>();
    public List<ReusingScrollItemBase> m_itemCatchs = new List<ReusingScrollItemBase>();

    private Bounds m_viewBounds;
    GameObject m_itemPrefab;

    Vector3 m_itemSize;

    #region 公共方法

    public  void Init(string UIEventKey,string itemName)
    {
        base.Init(UIEventKey);

        m_ItemName = itemName;

        Rebuild(CanvasUpdate.Layout);

        UpdateBounds();
        SetLayout();

        m_itemPrefab = ResourceManager.Load<GameObject>(m_ItemName);
        m_itemSize = m_itemPrefab.GetComponent<RectTransform>().sizeDelta;
    }


    public void SetData(List<Dictionary<string, object>> data)
    {
        m_datas = data;
        
        UpdateIndexList(data.Count);
        UpdateConetntSize(data.Count);

        SetItemDisplay();
    }

    public ReusingScrollItemBase GetItem(int index)
    {
        for (int i = 0; i < m_items.Count; i++)
        {
            if(m_items[i].m_index == index)
            {
                return m_items[i];
            }
        }

        return null;
    }

    public Vector3 GetItemAnchorPos(int index)
    {
        return GetItemPos(index) + GetRealItemOffset() + content.localPosition;
    }

    public void SetPos(Vector3 pos)
    {
        content.anchoredPosition3D = pos;

        SetItemDisplay();
    }

    #endregion 

    #region 继承方法
    //protected override void SetContentAnchoredPosition(Vector2 position)
    //{
    //    InputUIEventProxy.DispatchScrollEvent("", "", position);

    //    //base.SetContentAnchoredPosition(position);
    //    //SetItemDisplay();
    //}

    protected override void OnSetContentAnchoredPosition(InputUIOnScrollEvent e)
    {
        base.OnSetContentAnchoredPosition(e);
        SetItemDisplay();
    }

    public override void Rebuild(CanvasUpdate executing)
    {
        base.Rebuild(executing);

        UpdateBounds();
        SetItemDisplay();
    }

    #endregion

    #region 私有方法

    void SetLayout()
    {
        content.anchorMin = GetMinAchors();
        content.anchorMax = GetMaxAchors();
        content.pivot = GetPivot();
        content.anchoredPosition3D = Vector3.zero;
    }

    void UpdateBounds()
    {
        m_viewBounds = new Bounds(viewRect.rect.center, viewRect.rect.size);
    }

    void UpdateConetntSize(int count)
    {
        Vector3 size = m_itemSize;

        if(horizontal)
        {
            size.x *= count;
        }

        if(vertical)
        {
            size.y *= count;
        }

        content.sizeDelta = size;
    }

    void UpdateIndexList(int count)
    {
        m_indexList = new List<ReusingData>();
        for (int i = 0; i < count; i++)
        {
           ReusingData reusingTmp = null;
           if (m_indexList.Count > i)
           {
               reusingTmp = m_indexList[i];
           }
           else
           {
                reusingTmp = new ReusingData();
                m_indexList.Add(reusingTmp);
           }

           reusingTmp.index = i;
           reusingTmp.status = ReusingStatus.Hide;
        }
    }

    List<ReusingData> m_indexList = new List<ReusingData>();
    bool m_clip = false;   //剪枝标志位，性能优化使用
    int m_startPos = 0;
    void SetItemDisplay()
    {
        m_clip = false;
        //计算已显示的哪些需要隐藏
        for (int i = 0; i < m_items.Count; i++)
        {
            if (!m_clip)
            {
                m_startPos = m_items[i].m_index;
                m_clip = true;
            }
            else
            {
                if (m_startPos > m_items[i].m_index)
                {
                    m_startPos = m_items[i].m_index;
                }
            }

            //已经完全离开了显示区域
            if (!m_viewBounds.Intersects(GetItemBounds(m_items[i].m_index)))
            {
                ReusingScrollItemBase itemTmp = m_items[i];
                m_items.Remove(itemTmp);
                itemTmp.OnHide();

                //隐藏并移到缓存
                itemTmp.gameObject.SetActive(false);
                m_itemCatchs.Add(itemTmp);

                m_indexList[itemTmp.m_index].status =  ReusingStatus.Hide;
            }
        }

        if (m_startPos > 0)
        {
            m_startPos--;
        }

        //Debug.Log(m_startPos);

        m_clip = false;

        //计算出哪些需要显示
        //如果有未显示的则显示出来，从对象池取出对象
        for (int i = m_startPos; i < m_indexList.Count; i++)
        {
            if (m_indexList[i].status == ReusingStatus.Hide)
            {
                if (m_viewBounds.Intersects(GetItemBounds(m_indexList[i].index)))
                {
                    ShowItem(i, m_datas[i]);
                    m_indexList[i].status = ReusingStatus.Show;
                    m_clip = true;
                }
                else
                {
                    if (m_clip)
                    {
                        break;
                    }
                }
            }
            else
            {
                m_clip = true;
            }
        }
    }

    void ShowItem(int index,Dictionary<string, object> data)
    {
        ReusingScrollItemBase itemTmp = GetItem();
        itemTmp.transform.SetParent(content);
        itemTmp.transform.localScale = Vector3.one;

        itemTmp.SetConetnt(index,data);

        itemTmp.m_RectTransform.pivot = GetPivot();
        itemTmp.m_RectTransform.anchorMin = GetMinAchors();
        itemTmp.m_RectTransform.anchorMax = GetMaxAchors();

        itemTmp.m_RectTransform.anchoredPosition3D = GetItemPos(index);

        itemTmp.m_index = index;
    }

    ReusingScrollItemBase GetItem()
    {
        ReusingScrollItemBase result = null;

        if (m_itemCatchs.Count>0)
        {
            result = m_itemCatchs[0];
            result.gameObject.SetActive(true);
            result.OnShow();
            m_itemCatchs.RemoveAt(0);

            m_items.Add(result);
            return result;
        }

        result = GameObjectManager.CreatGameObjectByPool(m_itemPrefab).GetComponent<ReusingScrollItemBase>();
        result.Init(m_items.Count);
        m_items.Add(result);

        return result;
    }

    Bounds GetItemBounds(int index)
    {
        return new Bounds(GetItemPos(index) + GetRealItemOffset() + content.localPosition, m_itemSize);
    }


    Vector3 GetItemPos(int index)
    {
        Vector3 offset = Vector3.zero;
        if (vertical)
        {
            offset = new Vector3(0, -m_itemSize.y, 0);
        }
        else
        {
            offset = new Vector3(m_itemSize.x, 0, 0);
        }

        if (m_isInversion)
        {
            offset *= -1;
        }

        offset *= index;
        return offset;
    }

    Vector3 GetRealItemOffset()
    {
        Vector3 offset;
        if (vertical)
        {

            offset = new Vector3(0, -m_itemSize.y / 2, 0);
        }
        else
        {
            offset = new Vector3(m_itemSize.x/2, 0, 0);
        }

        if(m_isInversion)
        {
            offset *= -1;
        }

        return offset;
    }

    Vector2 GetPivot()
    {
        Vector2 pivot = new Vector2(0.5f, 0.5f);

        if (horizontal)
        {
            if (!m_isInversion)
            {
                pivot.x = 0;
            }
            else
            {
                pivot.x = 1;
            }
        }

        if (vertical)
        {
            if (!m_isInversion)
            {
                pivot.y = 1;
            }
            else
            {
                pivot.y = 0;
            }
        }

        return pivot;
    }

    Vector2 GetMinAchors()
    {
        Vector2 minAchors = new Vector2(0.5f, 0.5f);

        if (horizontal)
        {
            if (!m_isInversion)
            {
                minAchors.x = 0;
            }
            else
            {
                minAchors.x = 1;
            }
        }

        if (vertical)
        {
            if (!m_isInversion)
            {
                minAchors.y = 1;
            }
            else
            {
                minAchors.y = 0;
            }
        }

        return minAchors;
    }

    Vector2 GetMaxAchors()
    {
        Vector2 maxAchors = new Vector2(0.5f, 0.5f);

        if (horizontal)
        {
            if (!m_isInversion)
            {
                maxAchors.x = 0;
            }
            else
            {
                maxAchors.x = 1;
            }
        }

        if (vertical)
        {
            if (!m_isInversion)
            {
                maxAchors.y = 1;
            }
            else
            {
                maxAchors.y = 0;
            }
        }

        return maxAchors;
    }

    //void OnDrawGizmos()
    //{
    //    return;

    //    Gizmos.color = Color.red;
    //    Gizmos.DrawCube(m_viewBounds.center, m_viewBounds.size);

    //    Gizmos.color = Color.green;
    //    Gizmos.DrawCube(GetItemBounds(0).center, GetItemBounds(0).size);

    //    Gizmos.color = new Color(1, 1, 0, 0.5f);

    //    for (int i = 0; i < 100; i++)
    //    {
    //        Gizmos.color -= new Color(0.01f, 0, 0, 0);
    //        Gizmos.DrawCube(GetItemBounds(i).center, GetItemBounds(i).size);

    //    }

    //}

    #endregion

    #region 事件监听与转发
    


    #endregion

    #region 私有类和枚举

    class ReusingData
    {
        public int index;
        public ReusingStatus status;
    }

    enum ReusingStatus
    {
        Show,
        Hide
    }

    #endregion
}
