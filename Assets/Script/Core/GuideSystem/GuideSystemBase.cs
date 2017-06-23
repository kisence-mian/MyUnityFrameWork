using UnityEngine;
using System.Collections.Generic;

namespace FrameWork.GuideSystem
{
    /// <summary>
    /// 新手引导类
    /// 建议使用Tools -> 新手引导 ->初始化 自动生成的GuideSystem单例类
    /// </summary>
    public abstract class GuideSystemBase
    {
        public const string c_guideWindowName = "GuideWindow"; //引导界面名称
        public const string c_guideDataName = "GuideData";     //引导数据名

        public const string c_guideStartPoint = "StartPoint";  //引导开始点
        public const string c_guideEndPoint   = "EndPoint";    //引导结束点

        public const string c_PremiseKey = "Premise";          //前提条件
        public const string c_NextGuideNameKey = "NextGuide";  //下一步引导,如果为空,则为下一条记录

        public const string c_CallToNextKey = "CallToNext";    //是否调用去下一步引导
        public const string c_ClickToNextKey = "ClickToNext";  //是否点击去下一步引导

        public const string c_GuideWindowNameKey = "GuideWindowName";  //引导的界面名字
        public const string c_GuideObjectNameKey = "GuideObjectName";  //高亮显示的对象名字
        public const string c_GuideItemNameKey = "GuideItemName";      //高亮的Item名字

        public const string c_TipContentKey = "TipContent";           //提示文本内容
        public const string c_TipContentPosKey = "TipContentPos";     //提示文本位置

        public const string c_MaskAlphaKey = "MaskAlpha";             //遮罩Alpha

        public const string c_guideRecordName = "GuideRecord";        //引导记录名
        public const string c_guideSwitchName = "GuideSwitch";        //引导开关
        public const string c_guideCurrentKeyName = "CurrentGuide";   //当前执行完毕的引导

        bool m_isInit = false;
        bool m_isStart = false;

        bool m_isOperationUI = false;  //是否已经操作了UI
        protected GuideWindowBase m_guideWindowBase; //当前引导界面

        UIWindowBase m_currentOperationWindow; //当前操作的界面

        /// <summary>
        /// 新手引导记录表
        /// </summary>
        Dictionary<string, string> m_guideRecord = new Dictionary<string, string>();

        DataTable m_guideData;
        protected SingleData m_currentGuideData;
        int m_currentGuideIndex = 0;
        string m_currentGuideKey = "";

        #region 外部调用

        /// <summary>
        /// 关闭新手引导
        /// </summary>
        public void Dispose()
        {
            if (m_isInit)
            {
                m_isInit = false;
                m_guideData = null;

                //清除操作
                ClearGuideLogic();
                EndGuide();
            }
        }

        /// <summary>
        /// 调用新手引导去下一步
        /// </summary>
        public void Next()
        {
            if (GetCallToNext(m_currentGuideData) && GuideCallFilter())
            {
                NextGuide();
            }
        }

        /// <summary>
        /// 新手引导开始点
        /// </summary>
        public void Start()
        {
            SingleData guideData = LoadFirstGuide();

            if (!m_isStart 
                && guideData != null
                && GuideStartCondition(guideData)
                && GetGuideSwitch())
            {
                StartGuide();
            }
        }

        #endregion

        #region 重载方法

        /// <summary>
        /// 新手引导启动时调用
        /// </summary>
        protected virtual void OnStart()
        {

        }

        /// <summary>
        /// 请求引导记录
        /// 可以根据情况选择是从本地读取还是从服务器请求
        /// </summary>
        protected virtual void GetGuideRecord()
        {
            m_currentGuideKey = RecordManager.GetStringRecord(c_guideRecordName, c_guideCurrentKeyName, "");
        }

        /// <summary>
        /// 保存引导记录
        /// 可以根据情况选择是保存在本地还是发往服务器
        /// </summary>
        protected virtual void SaveGuideRecord(SingleData data)
        {
            RecordManager.SaveRecord(c_guideRecordName, data.m_SingleDataKey, true);
            RecordManager.SaveRecord(c_guideRecordName, c_guideCurrentKeyName, data.m_SingleDataKey);
        }

        /// <summary>
        /// 判断是否满足引导开始条件
        /// </summary>
        /// <returns></returns>
        protected virtual bool GuideStartCondition(SingleData data)
        {
            return GetGuideStartPoint(data);
        }

        /// <summary>
        /// 引导退出条件
        /// </summary>
        /// <returns></returns>
        protected virtual bool GuideEndCondition()
        {
            return GetGuideEndPoint(m_currentGuideData);
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
            m_guideWindowBase.ShowTips(GetTipContent(m_currentGuideData)
                                  , GetTipContentPos(m_currentGuideData));

            //调整背景遮罩Alpha
            m_guideWindowBase.SetMaskAlpha(GetMaskAlpha(m_currentGuideData));

            //创建特效

            //移动手指到目标位置

        }

        /// <summary>
        /// 清除对UI的操作
        /// </summary>
        /// <param name="ui"></param>
        protected virtual void ClearGuideBehaveByUI(UIWindowBase ui)
        {
            //清除高亮
            ui.ClearGuideModel();

            //清除特效
            m_guideWindowBase.ClearEffect();
            
            //清除手指
            m_guideWindowBase.HideAllGuideUI();
        }

        /// <summary>
        /// 清除非UI操作
        /// </summary>
        protected virtual void ClearGuideBehave()
        {

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
            if (GetClickToNext(m_currentGuideData) && GuideClickFilter(e))
            {
                NextGuide();
            }
        }

        void ReceviceUIOpenEvent(UIWindowBase UI, params object[] objs)
        {
            if (!m_isOperationUI && UI.UIName.Equals(GetGuideWindowName(m_currentGuideData)))
            {
                m_isOperationUI = true;
                m_currentOperationWindow = UI;
                GuideBehaveByUI(UI);
            }
        }

        void ReceviceUIShowEvent(UIWindowBase UI, params object[] objs)
        {
            if (!m_isOperationUI && UI.UIName.Equals(GetGuideWindowName(m_currentGuideData)))
            {
                m_isOperationUI = true;
                m_currentOperationWindow = UI;
                GuideBehaveByUI(UI);
            }
        }

        void ReceviceUICloseEvent(UIWindowBase UI, params object[] objs)
        {

        }

        void ReceviceGuideRecord(Dictionary<string, string> record)
        {
            m_guideRecord = record;
        }

        #endregion

        #region 引导逻辑

        protected void Init()
        {
            if (!m_isInit)
            {
                m_isInit = true;
                LoadGuideData();
                GetGuideRecord();
            }
        }

        void StartGuide()
        {
            m_isStart = true;

            m_guideWindowBase = (GuideWindowBase)UIManager.OpenUIWindow(c_guideWindowName);

            InputManager.AddAllEventListener<InputUIOnClickEvent>(ReceviceClickEvent);
            UISystemEvent.RegisterAllUIEvent(UIEvent.OnOpen, ReceviceUIOpenEvent);
            UISystemEvent.RegisterAllUIEvent(UIEvent.OnShow, ReceviceUIShowEvent);
            UISystemEvent.RegisterAllUIEvent(UIEvent.OnClose, ReceviceUICloseEvent);

            ApplicationManager.s_OnApplicationUpdate += Update;

            SetCurrent(LoadFirstGuide());
            GuideLogic();

            OnStart();
        }

        void EndGuide()
        {
            m_isStart = false;

            UIManager.CloseUIWindow(m_guideWindowBase);
            m_guideWindowBase = null;

            InputManager.RemoveAllEventListener<InputUIOnClickEvent>(ReceviceClickEvent);
            UISystemEvent.RemoveAllUIEvent(UIEvent.OnOpen, ReceviceUIOpenEvent);
            UISystemEvent.RemoveAllUIEvent(UIEvent.OnShow, ReceviceUIShowEvent);
            UISystemEvent.RemoveAllUIEvent(UIEvent.OnClose, ReceviceUICloseEvent);

            ApplicationManager.s_OnApplicationUpdate -= Update;
        }

        void NextGuide()
        {
            //判断是否满足进行下一步的条件
            if (GuideNextCondition())
            {
                //清除上一步的操作
                ClearGuideLogic();

                //保存这一步
                SaveGuideRecord(m_currentGuideData);

                SingleData nextGuideData = GetNextGuideData(m_currentGuideData);

                //退出判断
                if (!GuideEndCondition() 
                    && nextGuideData != null)
                {
                    //读取下一步引导
                    SetCurrent(nextGuideData);

                    //引导逻辑
                    GuideLogic();
                }
                else
                {
                    EndGuide();
                }
            }
        }

        //引导逻辑
        void GuideLogic()
        {
            if (m_currentGuideData != null)
            {
                //处理非UI逻辑
                GuideBehave();

                string uiName = GetGuideWindowName(m_currentGuideData);

                if(uiName != null 
                    && uiName != ""
                    && uiName != "Null"
                    && uiName != "null")
                {
                    //获取UI进行表现
                    UIWindowBase ui = UIManager.GetUI(uiName);
                    if (ui != null)
                    {
                        m_isOperationUI = true;
                        m_currentOperationWindow = ui;
                        GuideBehaveByUI(ui);
                    }
                    else
                    {
                        m_isOperationUI = false;
                    }
                }
            }
        }

        void ClearGuideLogic()
        {
            ClearGuideBehave();
            if(m_currentOperationWindow != null)
            {
                ClearGuideBehaveByUI(m_currentOperationWindow);
                m_currentOperationWindow = null;
            }
        }

        void LoadGuideData()
        {
            if (m_guideData == null)
            {
                m_guideData = DataManager.GetData(c_guideDataName);
            }
        }

        //读取第一条引导
        protected SingleData LoadFirstGuide()
        {
            if(m_guideData.TableIDs.Count == 0)
            {
                Dispose();
                throw new System.Exception("LoadFirstGuide :新手引导无记录！");
            }

            SingleData guideData = null;

            //如果新手引导启动时没有为m_currentGuideKey赋值
            //则认为从第一条记录开始
            if (m_currentGuideKey == "" 
                || m_currentGuideKey == null
                || m_currentGuideKey == "Null"
                || m_currentGuideKey == "null")
            {
                guideData = m_guideData[m_guideData.TableIDs[0]];
            }
            else
            {
                guideData = GetNextGuideData(m_guideData[m_currentGuideKey]);
            }

            return guideData;
        }

        //将一条记录设为当前要执行的引导记录
        void SetCurrent(SingleData data)
        {
            if (data != null)
            {
                m_currentGuideIndex = m_guideData.TableIDs.IndexOf(data.m_SingleDataKey);
                m_currentGuideData = data;
                m_currentGuideKey = m_currentGuideData.m_SingleDataKey;
            }
            else
            {
                m_currentGuideIndex = -1;
                m_currentGuideData = null;
                m_currentGuideKey = "";
            }
        }

        #endregion

        #region 读取数据

        bool GetGuideSwitch()
        {
            return RecordManager.GetBoolRecord(c_guideRecordName, c_guideSwitchName,true);
        }

        string GetPremise(SingleData data)
        {
            return data.GetString(c_PremiseKey);
        }

        string GetNextGuideNeme(SingleData data)
        {
            return data.GetString(c_NextGuideNameKey);
        }

        bool GetGuideStartPoint(SingleData data)
        {
            return data.GetBool(c_guideStartPoint);
        }

        bool GetGuideEndPoint(SingleData data)
        {
            return data.GetBool(c_guideEndPoint);
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

        float GetMaskAlpha(SingleData data)
        {
            return data.GetFloat(c_MaskAlphaKey);
        }

        SingleData GetNextGuideData(SingleData data)
        {
            string next = GetNextGuideNeme(data);

            if (next == null
                || next == "null"
                || next == "Null"
                || next == "")
            {
                int newIndex = m_guideData.TableIDs.IndexOf(data.m_SingleDataKey) + 1;
                return GetGuideDataByIndex(newIndex);
            }
            else
            {
                return GetGuideDataByName(next);
            }
        }

        SingleData GetGuideDataByIndex(int index)
        {
            if (m_guideData.TableIDs.Count > index)
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

        #region Update

        void Update()
        {
            if(!DevelopReplayManager.IsReplay 
                && ApplicationManager.AppMode == AppMode.Developing)
            {
                if(Input.GetKeyDown(KeyCode.F3))
                {
                    Dispose();
                }
            }
        }

        #endregion
    }
}
