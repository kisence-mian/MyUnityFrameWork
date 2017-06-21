using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class GuideSystemBase
{
    public const string c_guideDataName = "GuideData"; //引导数据名

    public const string c_PremiseKey   = "Premise";        //前提条件
    public const string c_NextGuideNameKey = "NextGuide";  //下一步引导,如果为空,则为下一条记录

    public const string c_CallToNextKey  = "CallToNext";   //是否调用去下一步引导
    public const string c_ClickToNextKey = "ClickToNext";  //是否点击去下一步引导

    public const string c_GuideWindowNameKey = "GuideWindowName";  //引导的界面名字
    public const string c_GuideObjectNameKey = "GuideObjectName";  //高亮显示的对象名字
    public const string c_GuideItemNameKey   = "GuideItemName";    //高亮的Item名字

    public const string c_TipContentKey   = "TipContent";        //提示文本内容
    public const string c_TipContentPosKey = "TipContentPos";    //提示文本位置

    bool m_isInit = false;
    bool m_isStart = false;

    bool m_isOperationUI = false;

    string m_guideWindowName = "";
    GuideWindowBase m_guideWindow;

    /// <summary>
    /// 新手引导记录表
    /// </summary>
    Dictionary<string, string> m_guideRecord = new Dictionary<string, string>();

    DataTable m_guideData;
    SingleData m_currentGuideData;
    int m_currentGuideIndex = 0;

    #region 单例

    private static GuideSystemBase s_instance;

    public static T GetInstance<T>() where T : GuideSystemBase,new()
    {
        if(s_instance == null)
        {
            s_instance = new T();
            s_instance.Init();
        }

        return (T)s_instance;
    }

    #endregion

    #region 外部调用

    public void Init(string guideWiindow)
    {
        if (!m_isInit)
        {
            m_isInit = true;
            m_guideWindowName = guideWiindow;
            LoadGuideData();
            GetGuideRecord();
        }
    }

    public void Dispose()
    {
        if (m_isInit)
        {
            m_isInit = false;
            m_guideData = null;
        }
    }

    /// <summary>
    /// 调用新手引导去下一步
    /// </summary>
    public void CallToNext()
    {
        if(m_isCallToNext && GuideCallFilter())
        {
            NextGuide();
        }
    }

    public void CallToStart()
    {
        if (!m_isStart && GuideStartCondition())
        {
            StartGuide();
        }
    }

    #endregion

    #region 重载方法

    public virtual void Init()
    {

    }

    /// <summary>
    /// 请求引导记录
    /// 根据情况选择是从本地读取还是从服务器请求
    /// </summary>
    protected virtual void GetGuideRecord()
    {

    }

    /// <summary>
    /// 保存引导记录
    /// 根据情况选择是保存在本地还是发往服务器
    /// </summary>
    protected virtual void SaveGuideRecord()
    {

    }

    /// <summary>
    /// 判断是否满足引导开始条件
    /// </summary>
    /// <returns></returns>
    protected virtual bool GuideStartCondition()
    {
        return true;
    }

    /// <summary>
    /// 引导退出条件
    /// </summary>
    /// <returns></returns>
    protected virtual bool GuideEndCondition()
    {
        return false;
    }

    /// <summary>
    /// 判断是否满足引导的下一步条件
    /// </summary>
    /// <returns></returns>
    protected virtual bool GuideNextCondition()
    {
        return true;
    }

    /// <summary>
    /// 引导每步的表现(非UI的操作)
    /// </summary>
    protected virtual void GuideBehave()
    {

    }

    /// <summary>
    /// 引导表现 (对UI的操作)
    /// </summary>
    protected virtual void GuideBehaveByUI(UIWindowBase ui)
    {
        //高亮ObjectName
        string[] objNames = GetGuideObjectNames(m_currentGuideData);

        for (int i = 0; i < objNames.Length; i++)
        {
            ui.SetGuideMode(objNames[i]);
        }

        string[] items = GetGuideItemNames(m_currentGuideData);

        //高亮Item
        for (int i = 0; i < items.Length; i++)
        {
            ui.SetItemGuideMode(items[i]);
        }

        //显示文本
        m_guideWindow.ShowTips(GetTipContent(m_currentGuideData)
                              ,GetTipContentPos(m_currentGuideData));

        //创建特效

        //移动手指到目标位置

    }

    /// <summary>
    /// 引导点击过滤器,返回true通过
    /// </summary>
    protected virtual bool GuideClickFilter(InputUIOnClickEvent e)
    {
        return true;
    }

    /// <summary>
    /// 引导调用过滤器,返回true通过
    /// </summary>
    protected virtual bool GuideCallFilter()
    {
        return true;
    }

    #endregion

    #region 事件接收

    void ReceviceClickEvent(InputUIOnClickEvent e)
    {
        if(m_isClickToNext && GuideClickFilter(e))
        {
            NextGuide();
        }
    }

    void ReceviceUIOpenEvent(UIWindowBase UI, params object[] objs)
    {
        if(!m_isOperationUI && UI.UIName.Equals(GetGuideWindowName(m_currentGuideData)))
        {
            m_isOperationUI = true;
            GuideBehaveByUI(UI);
        }
    }

    void ReceviceUIShowEvent(UIWindowBase UI, params object[] objs)
    {
        if (!m_isOperationUI && UI.UIName.Equals(GetGuideWindowName(m_currentGuideData)))
        {
            m_isOperationUI = true;
            GuideBehaveByUI(UI);
        }
    }

    void ReceviceUICloseEvent(UIWindowBase UI, params object[] objs)
    {

    }

    void ReceviceGuideRecord(Dictionary<string,string> record)
    {
        m_guideRecord = record;
    }

    #endregion

    #region 引导逻辑

    protected bool m_isClickToNext = false;
    protected bool m_isCallToNext = false;

    void StartGuide()
    {
        m_isStart = true;

        m_guideWindow = (GuideWindowBase)UIManager.OpenUIWindow(m_guideWindowName);

        InputManager.AddAllEventListener<InputUIOnClickEvent>(ReceviceClickEvent);
        UISystemEvent.RegisterAllUIEvent(UIEvent.OnOpen, ReceviceUIOpenEvent);
        UISystemEvent.RegisterAllUIEvent(UIEvent.OnShow, ReceviceUIOpenEvent);
        UISystemEvent.RegisterAllUIEvent(UIEvent.OnClose, ReceviceUICloseEvent);
    }

    void EndGuide()
    {
        m_isStart = false;

        UIManager.CloseUIWindow(m_guideWindow);
        m_guideWindow = null;

        InputManager.RemoveAllEventListener<InputUIOnClickEvent>(ReceviceClickEvent);
        UISystemEvent.RemoveAllUIEvent(UIEvent.OnOpen, ReceviceUIOpenEvent);
        UISystemEvent.RemoveAllUIEvent(UIEvent.OnShow, ReceviceUIOpenEvent);
        UISystemEvent.RemoveAllUIEvent(UIEvent.OnClose, ReceviceUICloseEvent);
    }

    void NextGuide()
    {
        if(GuideNextCondition())
        {
            //读取下一步引导
            MoveToNextGuide();

            if(m_currentGuideData != null)
            {
                //进行表现
                GuideBehave();

                //获取UI进行表现
                UIWindowBase ui = UIManager.GetUI(GetGuideWindowName(m_currentGuideData));
                if (ui != null)
                {
                    m_isOperationUI = true;
                    GuideBehaveByUI(ui);
                }
                else
                {
                    m_isOperationUI = false;
                }

                if (GuideEndCondition())
                {
                    EndGuide();
                }
            }
            else
            {
                EndGuide();
            }
        }
    }

    void LoadGuideData()
    {
        if (m_guideData == null)
        {
            m_guideData = DataManager.GetData(c_guideDataName);
        }
    }

    void MoveToNextGuide()
    {
        SingleData nextGuideData = GetNextGuideData();

        m_currentGuideData = nextGuideData;
        if(m_currentGuideData != null)
        {
            m_currentGuideIndex = m_guideData.TableIDs.IndexOf(nextGuideData.m_SingleDataKey);
        }
        else
        {
            m_currentGuideIndex = -1;
        }
    }

    #endregion

    #region 读取数据

    string GetPremise(SingleData data)
    {
        return data.GetString(c_PremiseKey);
    }

    string GetNextGuideNeme(SingleData data)
    {
        return data.GetString(c_NextGuideNameKey);
    }

    bool GetCallToNext(SingleData data)
    {
        return data.GetBool(c_CallToNextKey);
    }

    bool GetClickToNext(SingleData data)
    {
        return data.GetBool(c_ClickToNextKey);
    }

    string GetGuideWindowName(SingleData data)
    {
        return data.GetString(c_GuideWindowNameKey);
    }

    string[] GetGuideObjectNames(SingleData data)
    {
        return data.GetStringArray(c_GuideObjectNameKey);
    }

    string[] GetGuideItemNames(SingleData data)
    {
        return data.GetStringArray(c_GuideItemNameKey);
    }

    string GetTipContent(SingleData data)
    {
        return data.GetString(c_TipContentKey);
    }

    Vector3 GetTipContentPos(SingleData data)
    {
        return data.GetVector3(c_TipContentPosKey);
    }

    SingleData GetNextGuideData()
    {
        string next = GetNextGuideNeme(m_currentGuideData);

        if (   next == null
            || next == "null" 
            || next == "Null"
            || next == "")
        {
            int newIndex = m_currentGuideIndex + 1;
            return GetGuideDataByIndex(newIndex);
        }
        else
        {
            return GetGuideDataByName(next);
        }
    }

    SingleData GetGuideDataByIndex(int index)
    {
        if(m_guideData.TableIDs.Count > index)
        {
            string key = m_guideData.TableIDs[index];
            return GetGuideDataByName(key);
        }
        else
        {
            return null;
        }
    }

    SingleData GetGuideDataByName(string key)
    {
        if (!m_guideData.ContainsKey(key))
        {
            throw new System.Exception("GetGuideDataByName Exception: 没有找到 ->" + key + "<- 记录 ，请检查 " + c_guideDataName + " !");
        }

        return m_guideData[key];
    }

    #endregion
}
