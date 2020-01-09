using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

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
        public const string c_guideClosePoint = "ClosePoint";  //引导关闭点

        public const string c_PremiseKey = "Premise";          //前提条件
        public const string c_NextGuideNameKey = "NextGuide";  //下一步引导,如果为空,则为下一条记录

        public const string c_CallToNextKey = "CallToNext";    //是否调用去下一步引导
        public const string c_ClickToNextKey = "ClickToNext";  //是否点击去下一步引导
        public const string c_CustomEventKey = "CustomEvent";    //自定义事件名称
        public const string c_ConditionToNextKey = "ConditionToNextKey";    //是否自动判断条件去下一步引导

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
        bool m_isRegister = false;

        bool m_isOperationUI = false;  //是否已经操作了UI
        protected GuideWindowBase m_guideWindowBase; //当前引导界面

        protected UIWindowBase m_currentOperationWindow; //当前操作的界面

        /// <summary>
        /// 新手引导记录表
        /// </summary>
        //Dictionary<string, string> m_guideRecord = new Dictionary<string, string>();

        DataTable m_guideData;
        protected SingleData m_currentGuideData;
        //int m_currentGuideIndex = 0;
       protected string m_currentGuideKey = "";
       protected string m_startGuideKey = "";

        public bool IsStart
        {
            get
            {
                return m_isStart;
            }
        }

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
            if ( IsStart && GetCallToNext(m_currentGuideData) && GuideCallFilter())
            {
                NextGuide();
            }
        }

        /// <summary>
        /// 新手引导开始点
        /// </summary>
        public void Start(string guideKey = null)
        {
            SingleData guideData;
            // Debug.Log("Guide Start!!!");
            if (!string.IsNullOrEmpty( guideKey ))
            {
                guideData = GetGuideDataByName(guideKey);
            }
            else
            {
                guideData = LoadFirstGuide();
                guideKey = guideData.m_SingleDataKey;
            }

            if (!IsStart
                && guideData != null
                && GuideStartCondition(guideKey,guideData)
                && GetGuideSwitch())
            {
                StartGuide(guideData);
            }
        }
        /// <summary>
        /// 检查是否满足启动引导的条件
        /// </summary>
        /// <param name="guideKey"></param>
        /// <returns></returns>
        public bool CanStartGuide(string guideKey)
        {
            if (string.IsNullOrEmpty(guideKey))
                return false;
           SingleData guideData = GetGuideDataByName(guideKey);
            if (!IsStart
                && guideData != null
                && GuideStartCondition(guideKey, guideData)
                && GetGuideSwitch())
            {
                return true;
            }
            return false;
        }

        #endregion

        #region 重载方法

        protected virtual void OnInit() { }
        /// <summary>
        /// 新手引导启动时调用
        /// </summary>
        protected virtual void OnStart()
        {

        }

        protected virtual void OnCloseGuide()
        {

        }

        protected virtual void OnEndGuide()
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
        protected virtual void SaveGuideRecord(string startKey,string currentKey)
        {
            RecordManager.SaveRecord(c_guideRecordName, startKey, true);
            RecordManager.SaveRecord(c_guideRecordName, c_guideCurrentKeyName, currentKey);
        }

        /// <summary>
        /// 判断是否满足引导开始条件,默认判断是不是开始点
        /// </summary>
        /// <returns></returns>
        protected virtual bool GuideStartCondition(string currentGuideKey ,SingleData data)
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

        protected virtual bool GuideCloseCondition()
        {
            return GetGuideClosePoint(m_currentGuideData);
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
            //读取配置 设置摄像机

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

            //清除文本
            m_guideWindowBase.ClearTips();
        }

        /// <summary>
        /// 清除非UI操作
        /// </summary>
        protected virtual void ClearGuideBehave()
        {

        }

        protected virtual GuideWindowBase OpenGuideWindow()
        {
            return (GuideWindowBase)UIManager.OpenUIWindow(c_guideWindowName);
        }

        /// <summary>
        /// 引导点击过滤器,返回true通过
        /// </summary>
        protected virtual bool GuideClickFilter(InputUIOnClickEvent e)
        {
            string winName = GetGuideWindowName(m_currentGuideData);
            Debug.Log("e.EventKey :" + e.EventKey + "   winName:" + winName);
            if (!string.IsNullOrEmpty(winName))
            {
                if (!e.EventKey.Contains(winName))
                {
                    return false;
                }
            }

            string[] objnames = GetGuideObjectNames(m_currentGuideData);
            if (objnames.Length > 0)
            {

                bool isExist = false;
                for (int i = 0; i < objnames.Length; i++)
                {
                    string objName = objnames[i];
                    if (objName.Contains("."))
                    {
                        string[] tempArr = objName.Split('.');
                        objName = tempArr[tempArr.Length - 1];
                    }
                    string endStr ="."+ objName+".";
                  
                    if (e.EventKey.Contains(objName))
                    {
                        isExist = true;
                    }
                    Debug.Log("e.EventKey :" + e.EventKey + "   objName:" + objName + " endStr :" + endStr+ "  isExist :"+ isExist);
                }

                if (!isExist)
                {
                    return false;
                }
            }
            string[] itemNames = GetGuideItemNames(m_currentGuideData);
            //Debug.Log(" itemNames :" + itemNames.Length);
            if (itemNames.Length>0)
            {
               
                bool isExist = false;
                for (int i = 0; i < itemNames.Length; i++)
                {
                    string itemName = GetObjName(itemNames[i]);

                    if (e.EventKey.Contains(itemName))
                    {
                        isExist = true;
                    }
                }

                if (!isExist)
                {
                    return false;
                }
            }

            return true;
        }

        string GetObjName(string objName)
        {
            if(objName.Contains("."))
            {
                string[] temp = objName.Split('.');

                objName = temp[temp.Length - 1];
            }

            if(objName.Contains("["))
            {
                string[] temp = objName.Split('[');

                objName = temp[0];
            }

            return objName;
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
            Debug.Log(" ReceviceClickEvent ");

            if (IsStart && GetClickToNext(m_currentGuideData) && GuideClickFilter(e))
            {
                NextGuide();
            }
        }

        void ReceviceUIOpenEvent(UIWindowBase UI, params object[] objs)
        {
            if (IsStart && !m_isOperationUI && UI.UIName.Equals(GetGuideWindowName(m_currentGuideData)))
            {
                m_isOperationUI = true;
                m_currentOperationWindow = UI;
                try
                {
                    GuideBehaveByUI(UI);
                }
                catch(Exception e)
                {
                    Debug.LogError("ReceviceUIOpenEvent exception -> " + e.ToString());
                }

            }
        }

        void ReceviceUIShowEvent(UIWindowBase UI, params object[] objs)
        {
            if (IsStart && !m_isOperationUI && UI.UIName.Equals(GetGuideWindowName(m_currentGuideData)))
            {
                m_isOperationUI = true;
                m_currentOperationWindow = UI;

                try
                {
                    GuideBehaveByUI(UI);
                }
                catch (Exception e)
                {
                    Debug.LogError("ReceviceUIShowEvent exception -> " + e.ToString());
                }

            }
        }

        void ReceviceUICloseEvent(UIWindowBase UI, params object[] objs)
        {

        }

        void ReceviceGuideRecord(Dictionary<string, string> record)
        {
            //m_guideRecord = record;
        }

        protected virtual void ReceviceCustomEvent(IInputEventBase e)
        {

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
                OnInit();
            }
        }

        void StartGuide(SingleData guideData)
        {
           
            m_isStart = true;

            m_startGuideKey = guideData.m_SingleDataKey;
            Debug.Log(" 启动新手引导 : " + m_startGuideKey);
            SetCurrent(guideData);

            m_guideWindowBase = OpenGuideWindow();

            OnStart();

            GuideLogic();

            if(!m_isRegister)
            {
               // Debug.Log("StartGuide");
                m_isRegister = true;
                InputManager.AddAllEventListener<InputUIOnClickEvent>(ReceviceClickEvent);
                UISystemEvent.RegisterAllUIEvent(UIEvent.OnOpened, ReceviceUIOpenEvent);
                UISystemEvent.RegisterAllUIEvent(UIEvent.OnShow, ReceviceUIShowEvent);
                UISystemEvent.RegisterAllUIEvent(UIEvent.OnClose, ReceviceUICloseEvent);

                ApplicationManager.s_OnApplicationUpdate += Update;
            }
        }

      protected  void EndGuide()
        {
            Debug.Log("EndGuide ");

            CloseGuide();

            OnCloseGuide();
        }
        /// <summary>
        /// 关闭引导逻辑，不调用OnCloseGuide
        /// </summary>
        protected void CloseGuide()
        {
            CloseGuideWindow(m_guideWindowBase);

            m_isStart = false;
            m_isOperationUI = false;

            m_guideWindowBase = null;

            if (m_isRegister)
            {
                Debug.Log("RemoveAllEventListener");

                m_isRegister = false;
                InputManager.RemoveAllEventListener<InputUIOnClickEvent>(ReceviceClickEvent);
                UISystemEvent.RemoveAllUIEvent(UIEvent.OnOpened, ReceviceUIOpenEvent);
                UISystemEvent.RemoveAllUIEvent(UIEvent.OnShow, ReceviceUIShowEvent);
                UISystemEvent.RemoveAllUIEvent(UIEvent.OnClose, ReceviceUICloseEvent);

                ApplicationManager.s_OnApplicationUpdate -= Update;
            }
        }
        protected virtual void CloseGuideWindow( GuideWindowBase m_guideWindowBase)
        {
            Debug.Log("guide window =>" + m_guideWindowBase);
            //Debug.Log("==>>" + UIManager.GetUI<GuideWindow>());
            if (m_guideWindowBase != null)
                UIManager.CloseUIWindow(m_guideWindowBase);
            else
            {
                Debug.LogError("Guide Window is null");
            }
        }
      protected  void NextGuide()
        {
            Debug.Log("NextGuide m_currentGuideData " + m_currentGuideData.m_SingleDataKey + "");

            //
            OnEndGuide();

            //清除上一步的操作
            ClearGuideLogic();

            //如果是开始点点则讲开始点设为自身
            if (GetGuideStartPoint(m_currentGuideData))
                m_startGuideKey = m_currentGuideData.m_SingleDataKey;

            //如果是结束点则保存这一步
            if (GetGuideEndPoint(m_currentGuideData))
                SaveGuideRecord(m_startGuideKey,m_currentGuideData.m_SingleDataKey);

            SingleData nextGuideData = GetNextGuideData(m_currentGuideData);

            //Debug.Log("NextGuide m_currentGuideData " + m_currentGuideData.m_SingleDataKey + " " + GuideCloseCondition() + " nextGuideData " + nextGuideData);

            //退出判断
            if (!GuideCloseCondition()
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

        //引导逻辑
        void GuideLogic()
        {
            //Debug.Log("GuideLogic " + m_currentGuideData.m_SingleDataKey);

            if (m_currentGuideData != null)
            {
                //注册自定义事件监听
                AddCustomEventListener(GetCustomEvent(m_currentGuideData));

                //处理非UI逻辑
                GuideBehave();

                string uiName = GetGuideWindowName(m_currentGuideData);

                if(uiName != null)
                {
                    //获取UI进行表现
                    UIWindowBase ui = UIManager.GetUI(uiName);
                    if (ui != null)
                    {
                        m_isOperationUI = true;
                        m_currentOperationWindow = ui;

                        try
                        {
                            GuideBehaveByUI(ui);
                        }
                        catch(Exception e)
                        {
                            Debug.LogError("GuideLogic GuideBehaveByUI Exception " + e.ToString());
                        }
                    }
                    else
                    {
                        m_isOperationUI = false;
                    }
                }
            }
        }

      protected  void ClearGuideLogic()
        {
            if(m_currentOperationWindow != null)
            {
                ClearGuideBehaveByUI(m_currentOperationWindow);
                m_currentOperationWindow = null;
            }

            //取消自定义事件监听
            RemoveCustomEventListener(GetCustomEvent(m_currentGuideData));

            ClearGuideBehave();
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
                Debug.LogError("LoadFirstGuide :新手引导无记录！");
                return null;
            }

            SingleData guideData = null;

            //如果新手引导启动时没有为m_currentGuideKey赋值
            //则认为从第一条记录开始
            if (m_currentGuideKey == "")
            {
                guideData = m_guideData[m_guideData.TableIDs[0]];
            }
            else
            {
                //guideData = GetNextGuideData(m_guideData[m_currentGuideKey]);
                guideData = m_guideData[m_currentGuideKey];
            }

            return guideData;
        }

        //将一条记录设为当前要执行的引导记录
        void SetCurrent(SingleData data)
        {
            if (data != null)
            {
                //m_currentGuideIndex = m_guideData.TableIDs.IndexOf(data.m_SingleDataKey);
                m_currentGuideData = data;
                m_currentGuideKey = m_currentGuideData.m_SingleDataKey;
            }
            else
            {
                //m_currentGuideIndex = -1;
                m_currentGuideData = null;
                m_currentGuideKey = "";
            }
        }

        void AddCustomEventListener(string[] eventKey)
        {
            if (eventKey == null)
            {
                return;
            }

            for (int i = 0; i < eventKey.Length; i++)
            {
                Debug.Log("AddCustomEventListener");
                InputManager.AddListener(eventKey[i], eventKey[i], ReceviceCustomEvent);
            }
        }

        void RemoveCustomEventListener(string[] eventKey)
        {
            if(eventKey == null)
            {
                return;
            }

            for (int i = 0; i < eventKey.Length; i++)
            {
                InputManager.RemoveListener(eventKey[i], eventKey[i], ReceviceCustomEvent);
            }
        }

        #endregion

        #region 读取数据

        protected bool GetGuideSwitch()
        {
            return RecordManager.GetBoolRecord(c_guideRecordName, c_guideSwitchName,true);
        }

        protected string GetPremise(SingleData data)
        {
            return data.GetString(c_PremiseKey);
        }

        protected string GetNextGuideNeme(SingleData data)
        {
            return data.GetString(c_NextGuideNameKey);
        }

        protected bool GetGuideStartPoint(SingleData data)
        {
            return data.GetBool(c_guideStartPoint);
        }

        protected bool GetGuideEndPoint(SingleData data)
        {
            return data.GetBool(c_guideEndPoint);
        }

        protected bool GetGuideClosePoint(SingleData data)
        {
            //对旧项目做兼容
            if (!data.ContainsKey(c_guideClosePoint)
                && !data.data.m_defaultValue.ContainsKey(c_guideClosePoint))
            {
                return false;
            }


            return data.GetBool(c_guideClosePoint);
        }

        protected bool GetCallToNext(SingleData data)
        {
            return data.GetBool(c_CallToNextKey);
        }

        protected bool GetClickToNext(SingleData data)
        {
            return data.GetBool(c_ClickToNextKey);
        }

        protected string[] GetCustomEvent(SingleData data)
        {
            return data.GetStringArray(c_CustomEventKey);
        }

        protected bool GetConditionToNext(SingleData data)
        {
            //对旧项目做兼容
            if (!data.ContainsKey(c_ConditionToNextKey)
                && !data.data.m_defaultValue.ContainsKey(c_ConditionToNextKey))
            {
                return false;
            }

            return data.GetBool(c_ConditionToNextKey);
        }

        protected string GetGuideWindowName(SingleData data)
        {
            return data.GetString(c_GuideWindowNameKey);
        }

        protected string[] GetGuideObjectNames(SingleData data)
        {
            return data.GetStringArray(c_GuideObjectNameKey);
        }

        protected string[] GetGuideItemNames(SingleData data)
        {
            return data.GetStringArray(c_GuideItemNameKey);
        }

        protected virtual string GetTipContent(SingleData data)
        {
            return data.GetString(c_TipContentKey);
        }

        protected Vector3 GetTipContentPos(SingleData data)
        {
            return data.GetVector3(c_TipContentPosKey);
        }

        protected float GetMaskAlpha(SingleData data)
        {
            return data.GetFloat(c_MaskAlphaKey);
        }

        protected SingleData GetNextGuideData(SingleData data)
        {
            string next = GetNextGuideNeme(data);

            if (next == null)
            {
                int newIndex = m_guideData.TableIDs.IndexOf(data.m_SingleDataKey) + 1;
                return GetGuideDataByIndex(newIndex);
            }
            else
            {
                return GetGuideDataByName(next);
            }
        }

        protected SingleData GetGuideDataByIndex(int index)
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

        protected SingleData GetGuideDataByName(string key)
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
            if (IsStart)
            {
                if (!DevelopReplayManager.IsReplay
                    && ApplicationManager.AppMode == AppMode.Developing)
                {
                    if (Input.GetKeyDown(KeyCode.F3))
                    {
                        Dispose();
                    }
                }

                if (GetConditionToNext(m_currentGuideData))
                {
                    //判断是否满足进行下一步的条件
                    if (GuideNextCondition())
                    {
                        NextGuide();
                    }
                }
            }
        }

        #endregion


    }
}
