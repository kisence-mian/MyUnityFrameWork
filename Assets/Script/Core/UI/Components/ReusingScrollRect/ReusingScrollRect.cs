using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class ReusingScrollRect : ScrollRect
{
    public string ItemName = "";

    public List<Dictionary<string, object>> m_datas = new List<Dictionary<string, object>>();
    public List<ReusingScrollItemBase> m_items = new List<ReusingScrollItemBase>();
    public List<ReusingScrollItemBase> m_itemCatchs = new List<ReusingScrollItemBase>();

    private Bounds m_viewBounds;
    GameObject m_itemPrefab;

    Vector3 m_size;

    public void Init()
    {
        UpdateBounds();
        m_itemPrefab = (GameObject)ResourceManager.Load(ItemName);

        m_size = m_itemPrefab.GetComponent<RectTransform>().sizeDelta;
    }

    protected override void SetContentAnchoredPosition(Vector2 position)
    {
        base.SetContentAnchoredPosition(position);

        Debug.Log(position);

        SetItemDisplay(position);
    }

    void UpdateBounds()
    {
        m_viewBounds    = new Bounds(viewRect.rect.center, viewRect.rect.size);
    }

    void SetItemDisplay(Vector2 position)
    {
        //计算已显示的哪些需要隐藏
        for (int i = 0; i < m_items.Count; i++)
        {
            //已经完全离开了显示区域
            if (!m_viewBounds.Intersects(m_items[i].m_Bounds))
            {
                ReusingScrollItemBase itemTmp = m_items[i];
                m_items.Remove(itemTmp);

                //隐藏并移到缓存
                itemTmp.gameObject.SetActive(false);
                m_itemCatchs.Add(itemTmp);
            }
        }

        //计算出哪些需要显示
        //如果有未显示的则显示出来，从对象池取出对象
        for (int i = 0; i < m_datas.Count; i++)
        {
            if (m_viewBounds.Intersects(GetBounds(i)))
            {
                ShowItem(i,m_datas[i]);
            }
        }
    }

    void ShowItem(int index,Dictionary<string, object> data)
    {
        ReusingScrollItemBase itemTmp = GetItem();
        itemTmp.transform.SetParent(content);

        itemTmp.SetConetnt(index,data);
        itemTmp.m_RectTransform.anchoredPosition = GetPos(index);
    }

    ReusingScrollItemBase GetItem()
    {
        ReusingScrollItemBase result = null;

        if (m_itemCatchs.Count>0)
        {
            result = m_itemCatchs[0];
            m_itemCatchs.RemoveAt(0);

            return result;
        }

        result = GameObjectManager.CreatGameObjectByPool(m_itemPrefab).GetComponent<ReusingScrollItemBase>();
        result.Init();
        m_items.Add(result);

        return result;
    }

    Bounds GetBounds(int index)
    {
        return new Bounds(GetPos(index), m_size);
    }

    Vector3 GetPos(int index)
    {
        Vector3 pos = Vector3.zero;
        if (vertical)
        {
            pos = new Vector3(0, m_size.y, 0);
        }
        else
        {
            pos = new Vector3(m_size.x, 0, 0);
        }

        return pos * index;
    }
}
