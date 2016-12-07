using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class AnimSystem : MonoBehaviour
{
    #region 静态部分

    static AnimSystem instance;

    public static AnimSystem GetInstance()
    {
        if (instance == null)
        {
            GameObject animGameObject = new GameObject();
            animGameObject.name = "AnimSystem";
            instance = animGameObject.AddComponent<AnimSystem>();

            DontDestroyOnLoad(instance.gameObject);
        }

        return instance;
    }

    #region UGUI_Color
    /// <summary>
    /// 动画过度到目标颜色
    /// </summary>
    /// <param name="animObject">动画对象</param>
    /// <param name="from">起始颜色(可空)</param>
    /// <param name="to">目标颜色</param>
    /// <param name="time">动画时间</param>
    /// <param name="isChild">是否影响子节点</param>
    /// <param name="interp">插值类型</param>
    /// <param name="IsIgnoreTimeScale">是否忽略时间缩放</param>
    /// <param name="repeatType">重复类型</param>
    /// <param name="repeatCount">重复次数</param>
    /// <param name="callBack">动画完成回调函数</param>
    /// <param name="parameter">动画完成回调函数传参</param>
    /// <returns></returns>
    public static AnimData UguiColor(GameObject animObject, Color? from, Color to, 
        float time = 0.5f,
        float delayTime = 0, 
        InterpType interp = InterpType.Default,
        bool isChild = true,
        bool IsIgnoreTimeScale = false,
        RepeatType repeatType = RepeatType.Once,
        int repeatCount = -1, 
        AnimCallBack callBack = null, object[] parameter = null)
    {

        Color fromTmp = from ?? Color.white;

        if (from == null)
        {
            if (animObject.GetComponent<Graphic>() != null)
            {
                fromTmp = from ?? animObject.GetComponent<Graphic>().color;
            }
        }

        AnimParamHash animParnHash = new AnimParamHash(
          AnimParamType.GameObj, animObject,
          AnimParamType.AnimType, AnimType.UGUI_Color,
          AnimParamType.FromColor, fromTmp,
          AnimParamType.ToColor, to,
          AnimParamType.Time, time,
          AnimParamType.DelayTime, delayTime,
          AnimParamType.InteType, interp,
          AnimParamType.IsIncludeChild, isChild,
          AnimParamType.RepeatCount, repeatCount,
          AnimParamType.IsIgnoreTimeScale, IsIgnoreTimeScale,
          AnimParamType.CallBack, callBack,
          AnimParamType.CallBackParams, parameter
          );

        return ValueTo(animParnHash);
    }

    #endregion

    #region UGUI Alpha

    /// <summary>
    /// 动画过度到目标alpha
    /// </summary>
    /// <param name="animObject">动画对象</param>
    /// <param name="from">起始alpha(可空)</param>
    /// <param name="to">目标alpha</param>
    /// <param name="time">动画时间</param>
    /// <param name="isChild">是否影响子节点</param>
    /// <param name="interp">插值类型</param>
    /// <param name="IsIgnoreTimeScale">是否忽略时间缩放</param>
    /// <param name="repeatType">重复类型</param>
    /// <param name="repeatCount">重复次数</param>
    /// <param name="callBack">动画完成回调函数</param>
    /// <param name="parameter">动画完成回调函数传参</param>
    /// <returns></returns>
    public static AnimData UguiAlpha(GameObject animObject, float? from, float to,
        float time = 0.5f,
        float delayTime = 0, 
        InterpType interp = InterpType.Default, 
        bool isChild = true,
        bool IsIgnoreTimeScale = false,
        RepeatType repeatType = RepeatType.Once,
        int repeatCount = -1, 
        AnimCallBack callBack = null, object[] parameter = null)
    {

        float fromTmp = from ?? 1;

        if (from == null)
        {
            if (animObject.GetComponent<Graphic>() != null)
            {
                fromTmp = from ?? animObject.GetComponent<Graphic>().color.a;
            }
        }

        AnimParamHash animParnHash = new AnimParamHash(
           AnimParamType.GameObj, animObject,
           AnimParamType.AnimType, AnimType.UGUI_Alpha,
           AnimParamType.FromFloat, fromTmp,
           AnimParamType.ToFloat, to,
           AnimParamType.Time, time,
           AnimParamType.DelayTime, delayTime,
           AnimParamType.IsIgnoreTimeScale, IsIgnoreTimeScale,
           AnimParamType.RepeatCount, repeatCount,
           AnimParamType.InteType, interp,
           AnimParamType.IsIncludeChild, isChild,
           AnimParamType.RepeatType, repeatType,
           AnimParamType.CallBack,callBack,
           AnimParamType.CallBackParams, parameter
           );

        return ValueTo(animParnHash);
    }

    #endregion

    #region UGUI Move

    public static AnimData UguiMove(GameObject animObject, Vector3? from, Vector3 to,
        float time = 0.5f,
        float delayTime = 0, 
        InterpType interp = InterpType.Default,
        bool IsIgnoreTimeScale = false,
        RepeatType repeatType = RepeatType.Once,
        int repeatCount = -1, 
        AnimCallBack callBack = null,
        object[] parameter = null)
    {

        Vector3 fromTmp = from ?? animObject.GetComponent<RectTransform>().anchoredPosition;

        AnimParamHash animParnHash = new AnimParamHash(
          AnimParamType.GameObj, animObject,
          AnimParamType.AnimType, AnimType.UGUI_AnchoredPosition,
          AnimParamType.FromV3, fromTmp,
          AnimParamType.ToV3, to,
          AnimParamType.Time, time,
          AnimParamType.DelayTime, delayTime,
          AnimParamType.InteType, interp,
          AnimParamType.IsIgnoreTimeScale, IsIgnoreTimeScale,
          AnimParamType.RepeatCount, repeatCount,
          AnimParamType.CallBack, callBack,
          AnimParamType.CallBackParams, parameter
          );

        return ValueTo(animParnHash);
    }

    #endregion

    #region UGUI_SizeDelta
    public static AnimData UguiSizeDelta(GameObject animObject, Vector2? from, Vector2 to,

        float time = 0.5f,
        float delayTime = 0, 
        InterpType interp = InterpType.Default,
        bool IsIgnoreTimeScale = false,
        RepeatType repeatType = RepeatType.Once,
        int repeatCount = -1, 
        AnimCallBack callBack = null, 
        object[] parameter = null)
    {
        Vector2 fromTmp = from ?? animObject.GetComponent<RectTransform>().sizeDelta;

        AnimParamHash animParnHash = new AnimParamHash(
            AnimParamType.GameObj, animObject,
            AnimParamType.AnimType, AnimType.UGUI_SizeDetal,
            AnimParamType.FromV2, fromTmp,
            AnimParamType.ToV2, to,
            AnimParamType.Time, time,
            AnimParamType.DelayTime, delayTime,
            AnimParamType.InteType, interp,
            AnimParamType.IsIgnoreTimeScale, IsIgnoreTimeScale,
            AnimParamType.RepeatCount, repeatCount,
            AnimParamType.RepeatType, repeatType,
            AnimParamType.CallBack, callBack,
            AnimParamType.CallBackParams, parameter
            );

        return ValueTo(animParnHash);
    }

    #endregion

    #region Color

    public static AnimData ColorTo(GameObject animObject, Color from, Color to,

        float time             = 0.5f,
        float delayTime = 0, 
        InterpType interp      = InterpType.Default,
        bool isChild           = true,
        bool IsIgnoreTimeScale = false,
        RepeatType repeatType  = RepeatType.Once,
        int repeatCount        = -1, 
        AnimCallBack callBack  = null, 
        object[] parameter = null)
    {
        AnimParamHash animParnHash = new AnimParamHash(
            AnimParamType.GameObj, animObject,
            AnimParamType.AnimType, AnimType.Color,
            AnimParamType.FromColor, from,
            AnimParamType.ToColor, to,
            AnimParamType.Time, time,
            AnimParamType.DelayTime, delayTime,
            AnimParamType.InteType, interp,
            AnimParamType.IsIncludeChild, isChild,
            AnimParamType.RepeatType, repeatType,
            AnimParamType.IsIgnoreTimeScale, IsIgnoreTimeScale,
            AnimParamType.RepeatCount, repeatCount,
            AnimParamType.CallBack, callBack,
            AnimParamType.CallBackParams, parameter
            );

        return ValueTo(animParnHash);
    }

    public static AnimData AlphaTo(GameObject animObject, float from, float to, 

      float time = 0.5f,
      float delayTime = 0, 
      InterpType interp = InterpType.Default,
      bool IsIgnoreTimeScale = false,
      RepeatType repeatType = RepeatType.Once,
      int repeatCount = -1, 
      bool isChild = true,
      AnimCallBack callBack = null, object[] parameter = null)
    {

        AnimParamHash animParnHash = new AnimParamHash(
            AnimParamType.GameObj, animObject,
            AnimParamType.AnimType, AnimType.Alpha,
            AnimParamType.FromFloat, from,
            AnimParamType.ToFloat, to,
            AnimParamType.Time, time,
            AnimParamType.DelayTime, delayTime,
            AnimParamType.InteType, interp,
            AnimParamType.IsIncludeChild, isChild,
            AnimParamType.RepeatType, repeatType,
            AnimParamType.IsIgnoreTimeScale, IsIgnoreTimeScale,
            AnimParamType.RepeatCount, repeatCount,
            AnimParamType.CallBack, callBack,
            AnimParamType.CallBackParams, parameter
            );

        return ValueTo(animParnHash);
    }

    #endregion

    #region Move
    /// <summary>
    /// 动画移动到某位置
    /// </summary>
    /// <param name="animObject">动画对象</param>
    /// <param name="from">起点位置(可空，如为空则从当前位置开始)</param>
    /// <param name="to">终点位置</param>
    /// <param name="time">动画时间</param>
    /// <param name="isLocal">是否是用相对位置</param>
    /// <param name="interp">插值类型</param>
    /// <param name="IsIgnoreTimeScale">是否忽略时间缩放</param>
    /// <param name="repeatType">重复类型</param>
    /// <param name="repeatCount">重复次数</param>
    /// <param name="callBack">动画完成回调函数</param>
    /// <param name="parameter">动画完成回调函数传参</param>
    /// <returns></returns>
    public static AnimData Move(GameObject animObject, Vector3? from, Vector3 to,
        float delayTime = 0, 
        float time = 0.5f,
        bool isLocal = true,
        InterpType interp = InterpType.Default,
        bool IsIgnoreTimeScale = false,
        RepeatType repeatType = RepeatType.Once,
        int repeatCount = -1, 
        AnimCallBack callBack = null,
        object[] parameter = null)
    {

        Vector3 fromTmp;
        AnimType animType;
        if (isLocal)
        {
            fromTmp = from ?? animObject.transform.localPosition;
            animType = AnimType.LocalPosition;
        }
        else
        {
            fromTmp = from ?? animObject.transform.position;
            animType = AnimType.Position;
        }

        AnimParamHash animParnHash = new AnimParamHash(
            AnimParamType.GameObj, animObject,
            AnimParamType.AnimType, animType,
            AnimParamType.FromV3, fromTmp,
            AnimParamType.ToV3, to,
            AnimParamType.Time, time,
            AnimParamType.DelayTime, delayTime,
            AnimParamType.InteType, interp,
            AnimParamType.IsLocal, isLocal,
            AnimParamType.RepeatType, repeatType,
            AnimParamType.IsIgnoreTimeScale, IsIgnoreTimeScale,
            AnimParamType.RepeatCount, repeatCount,
            AnimParamType.CallBack, callBack,
            AnimParamType.CallBackParams, parameter
            );

        return ValueTo(animParnHash);
    }

    #endregion

    #region Rotate

    public static void Rotate(GameObject animObject, Vector3? from, Vector3 to, 

        float time = 0.5f,
        float delayTime = 0, 
        bool isLocal = true,
        InterpType interp = InterpType.Default,
        bool IsIgnoreTimeScale = false,
        RepeatType repeatType = RepeatType.Once,
        int repeatCount = -1, 

        AnimCallBack callBack = null, object[] parameter = null)
    {
        AnimType animType;
        Vector3 fromTmp ;

        if (isLocal)
        {
            fromTmp = from ?? animObject.transform.localEulerAngles;
            animType = AnimType.LocalRotate;
        }
        else
        {
            fromTmp = from ?? animObject.transform.eulerAngles;
            animType = AnimType.Rotate;
        }

        AnimParamHash animParnHash = new AnimParamHash(
            AnimParamType.GameObj, animObject,
            AnimParamType.AnimType, animType,
            AnimParamType.FromV3, fromTmp,
            AnimParamType.ToV3, to,
            AnimParamType.Time, time,
            AnimParamType.DelayTime, delayTime,
            AnimParamType.InteType, interp,
            AnimParamType.IsLocal, isLocal,
            AnimParamType.RepeatType, repeatType,
            AnimParamType.IsIgnoreTimeScale, IsIgnoreTimeScale,
            AnimParamType.RepeatCount, repeatCount,
            AnimParamType.CallBack, callBack,
            AnimParamType.CallBackParams, parameter
            );

        ValueTo(animParnHash);

    }


    #endregion

    #region Scale
    public static AnimData Scale(GameObject animObject, Vector3? from, Vector3 to,
        float time = 0.5f,
        InterpType interp = InterpType.Default,
        bool IsIgnoreTimeScale = false,
        RepeatType repeatType = RepeatType.Once,
        int repeatCount = -1,
        float delayTime = 0, 
        AnimCallBack callBack = null, object[] parameter = null)
    {

        Vector3 fromTmp = from ?? animObject.transform.localScale;

        AnimParamHash animParnHash = new AnimParamHash(
            AnimParamType.GameObj, animObject,
            AnimParamType.AnimType, AnimType.LocalScale,
            AnimParamType.FromV3, fromTmp,
            AnimParamType.ToV3, to,
            AnimParamType.Time, time,
            AnimParamType.DelayTime, delayTime,
            AnimParamType.InteType, interp,
            AnimParamType.RepeatType, repeatType,
            AnimParamType.IsIgnoreTimeScale, IsIgnoreTimeScale,
            AnimParamType.RepeatCount, repeatCount,
            AnimParamType.CallBack, callBack,
            AnimParamType.CallBackParams, parameter
            );

        return ValueTo(animParnHash);
    }
    #endregion

    #region CustomMethod

    public static AnimData CustomMethodToFloat(AnimCustomMethodFloat method, float from, float to,
        float time = 0.5f,
        float delayTime = 0, 
        InterpType interp = InterpType.Default,
        bool IsIgnoreTimeScale = false,
        RepeatType repeatType = RepeatType.Once,
        int repeatCount = -1, 
        AnimCallBack callBack = null, object[] parameter = null)
    {
        AnimParamHash animParnHash = new AnimParamHash(
           AnimParamType.AnimType, AnimType.Custom_Float,
           AnimParamType.FromFloat, from,
           AnimParamType.ToFloat, to,
           AnimParamType.Time, time,
           AnimParamType.DelayTime, delayTime,
           AnimParamType.InteType, interp,
           AnimParamType.RepeatType, repeatType,
           AnimParamType.CustomMethodFloat , method,
           AnimParamType.IsIgnoreTimeScale, IsIgnoreTimeScale,
           AnimParamType.RepeatCount, repeatCount,
           AnimParamType.CallBack, callBack,
           AnimParamType.CallBackParams, parameter
           );

        return ValueTo(animParnHash);
    }

    public static AnimData CustomMethodToVector2(AnimCustomMethodVector2 method, Vector2 from, Vector2 to, 
        float time = 0.5f,
        float delayTime = 0, 
        InterpType interp = InterpType.Default,
        bool IsIgnoreTimeScale = false,
        RepeatType repeatType = RepeatType.Once,
        int repeatCount = -1, 
        AnimCallBack callBack = null, object[] parameter = null)
    {
        AnimParamHash animParnHash = new AnimParamHash(
           AnimParamType.AnimType, AnimType.Custom_Vector2,
           AnimParamType.FromV2, from,
           AnimParamType.ToV2, to,
           AnimParamType.Time, time,
           AnimParamType.DelayTime, delayTime,
           AnimParamType.InteType, interp,
           AnimParamType.RepeatType , repeatType,
           AnimParamType.CustomMethodV2, method,
           AnimParamType.IsIgnoreTimeScale, IsIgnoreTimeScale,
           AnimParamType.RepeatCount, repeatCount,
           AnimParamType.CallBack, callBack,
           AnimParamType.CallBackParams, parameter
           );

        return ValueTo(animParnHash);
    }

    public static AnimData CustomMethodToVector3(AnimCustomMethodVector3 method, Vector3 from, Vector3 to,
        float time =0.5f,
        float delayTime = 0, 
        InterpType interp = InterpType.Default,
        bool IsIgnoreTimeScale = false,
        RepeatType repeatType = RepeatType.Once,
        int repeatCount = -1, 
        AnimCallBack callBack = null, object[] parameter = null)
    {
        AnimParamHash animParnHash = new AnimParamHash(
           AnimParamType.AnimType, AnimType.Custom_Vector3,
           AnimParamType.FromV3, from,
           AnimParamType.ToV3, to,
           AnimParamType.Time, time,
           AnimParamType.DelayTime, delayTime,
           AnimParamType.InteType, interp,
           AnimParamType.RepeatType, repeatType,
           AnimParamType.CustomMethodV3, method,
           AnimParamType.IsIgnoreTimeScale, IsIgnoreTimeScale,
           AnimParamType.RepeatCount, repeatCount,
           AnimParamType.CallBack, callBack,
           AnimParamType.CallBackParams, parameter
           );

        return ValueTo(animParnHash);
    }

    #endregion

    #region 贝塞尔
    public static AnimData BezierMove(GameObject animObject, Vector3 from, Vector3 to, 
        float time = 0.5f,
        RepeatType repeatType = RepeatType.Once, 
        InterpType interp = InterpType.Default, 
        bool isLocal = true,
        PathType bezierMoveType = PathType.Bezier2,
        Vector3[] t_Bezier_contral = null,
        AnimCallBack callBack = null, object[] parameter = null)
    {
        AnimType animType;
        if (isLocal)
        {
            animType = AnimType.LocalPosition;
        }
        else
        {
            animType = AnimType.Position;
        }

        AnimParamHash animParnHash = new AnimParamHash(
            AnimParamType.GameObj, animObject,
            AnimParamType.AnimType, animType,
            AnimParamType.FromV3, from,
            AnimParamType.ToV3, to,
            AnimParamType.Time, time,
            AnimParamType.InteType, interp,
            AnimParamType.IsLocal, isLocal,
            AnimParamType.PathType, bezierMoveType,
            AnimParamType.V3Control, t_Bezier_contral,
            AnimParamType.RepeatType, repeatType,
            AnimParamType.CallBack, callBack,
            AnimParamType.CallBackParams, parameter
            );

        return ValueTo(animParnHash);
    }

    public static AnimData BezierMove(GameObject animObject, Vector3 from, Vector3 to,
        float time = 0.5f,
        InterpType interp = InterpType.Default,
        bool isLocal = true, 
        PathType bezierMoveType = PathType.Bezier2, 
        Vector3[] t_Bezier_contral = null, 
        AnimCallBack callBack = null, object[] parameter = null)
    {
        return BezierMove(animObject, from, to, time, RepeatType.Once, interp, isLocal, bezierMoveType, t_Bezier_contral, callBack, parameter);
    }

    //不传From，传准确控制点
    public static AnimData BezierMove(GameObject animObject, Vector3 to, 
        float time = 0.5f,
        InterpType interp = InterpType.Default,
        RepeatType repeatType = RepeatType.Once,
        bool isLocal = true,
        PathType bezierMoveType = PathType.Bezier2,
        Vector3[] t_Bezier_contral = null,
        AnimCallBack callBack = null, object[] parameter = null)
    {
        Vector3 from;
        if (isLocal)
        {
            from = animObject.transform.localPosition;
        }
        else
        {
            from = animObject.transform.position;
        }

        return BezierMove(animObject, from, to, time, repeatType, interp, isLocal, bezierMoveType, t_Bezier_contral, callBack, parameter);
    }

    //传From，传准确控制点随机范围
    public static AnimData BezierMove(GameObject animObject, Vector3 from, Vector3 to, float time, RepeatType repeatType,
        InterpType interp = InterpType.Default,
        bool isLocal = true,
        PathType bezierMoveType = PathType.Bezier2,
        float[] t_Bezier_contralRadius = null,
        AnimCallBack callBack = null, object[] parameter = null)
    {
        AnimType animType;
        if (isLocal)
        {
            animType = AnimType.LocalPosition;
        }
        else
        {
            animType = AnimType.Position;
        }

        AnimParamHash animParnHash = new AnimParamHash(
            AnimParamType.GameObj, animObject,
            AnimParamType.AnimType, animType,
            AnimParamType.FromV3, from,
            AnimParamType.ToV3, to,
            AnimParamType.Time, time,
            AnimParamType.InteType, interp,
            AnimParamType.IsLocal, isLocal,
            AnimParamType.PathType, bezierMoveType,
            AnimParamType.floatControl, t_Bezier_contralRadius,
            AnimParamType.RepeatType, repeatType,
            AnimParamType.CallBack, callBack,
            AnimParamType.CallBackParams, parameter
            );

        return ValueTo(animParnHash);
    }

    public static AnimData BezierMove(GameObject animObject, Vector3 from, Vector3 to, float time,
        InterpType interp = InterpType.Default, bool isLocal = true,
        PathType bezierMoveType = PathType.Bezier2,
        float[] t_Bezier_contralRadius = null,
        AnimCallBack callBack = null, object[] parameter = null)
    {
        return BezierMove(animObject, from, to, time, RepeatType.Once, interp, isLocal, bezierMoveType, t_Bezier_contralRadius, callBack, parameter);
    }


    //不传From，传准确控制点随机范围
    public static AnimData BezierMove(GameObject animObject, Vector3 to, float time, RepeatType repeatType,
        InterpType interp = InterpType.Default,
        bool isLocal = true,
        PathType bezierMoveType = PathType.Bezier2,
        float[] t_Bezier_contralRadius = null,
        AnimCallBack callBack = null, object[] parameter = null)
    {
        Vector3 from;
        if (isLocal)
        {
            from = animObject.transform.localPosition;
        }
        else
        {
            from = animObject.transform.position;
        }

        return BezierMove(animObject, from, to, time, repeatType, interp, isLocal, bezierMoveType, t_Bezier_contralRadius, callBack, parameter);
    }

    #endregion

    #region 闪烁
    public static AnimData Blink(GameObject animObject, float space,

        float time =0.5f,
        float delayTime = 0, 
        bool IsIgnoreTimeScale = false,
        RepeatType repeatType = RepeatType.Once,
        int repeatCount = -1, 
        AnimCallBack callBack = null, 
        object[] parameter = null)
    {
        AnimParamHash animParnHash = new AnimParamHash(
            AnimParamType.GameObj, animObject,
            AnimParamType.AnimType, AnimType.Blink,
            AnimParamType.Space , space,
            AnimParamType.Time, time,
            AnimParamType.DelayTime, delayTime,
            AnimParamType.RepeatType, repeatType,
            AnimParamType.IsIgnoreTimeScale, IsIgnoreTimeScale,
            AnimParamType.RepeatCount, repeatCount,
            AnimParamType.CallBack, callBack,
            AnimParamType.CallBackParams, parameter
            );

        return ValueTo(animParnHash);
    }

    #endregion

    #region ValueTo

    public static AnimData ValueTo(AnimParamHash l_animHash)
    {
       AnimData l_tmp = l_animHash.GetAnimData();

       l_tmp.Init();

       GetInstance().animList.Add(l_tmp);

       return l_tmp;
    }

    public class AnimParamHash : Dictionary<AnimParamType, object>
    {
        public AnimData DataTmp = new AnimData();

        public AnimParamHash(params object[] l_params)
        {
            for (int i = 0; i < l_params.Length; i += 2)
            {
                this[(AnimParamType)l_params[i]] = l_params[i + 1];
            }
        }

        public AnimData GetAnimData()
        {
            foreach (var hash in this)
            {
                AnimParamType l_ParamType = hash.Key;
                switch (l_ParamType)
                {
                    //基础参数
                    case AnimParamType.GameObj: DataTmp.m_animGameObejct = (GameObject)hash.Value; break;
                    case AnimParamType.AnimType: DataTmp.m_animType = (AnimType)hash.Value; break;
                    case AnimParamType.Time: DataTmp.m_totalTime = (float)hash.Value; break;
                    case AnimParamType.InteType: DataTmp.m_interpolationType = (InterpType)hash.Value; break;
                    case AnimParamType.RepeatType: DataTmp.m_repeatType = (RepeatType)hash.Value; break;
                    case AnimParamType.RepeatCount: DataTmp.m_repeatCount = (int)hash.Value; break;
                    case AnimParamType.DelayTime: DataTmp.m_delayTime = (float)hash.Value; break;

                    //From
                    case AnimParamType.FromV3: DataTmp.m_fromV3 = (Vector3)hash.Value; break;
                    case AnimParamType.FromV2: DataTmp.m_fromV2 = (Vector2)hash.Value; break;
                    case AnimParamType.FromColor: DataTmp.m_fromColor = (Color)hash.Value; break;
                    case AnimParamType.FromFloat: DataTmp.m_fromFloat = (float)hash.Value; break;
                    
                    //To
                    case AnimParamType.ToV3: DataTmp.m_toV3 = (Vector3)hash.Value; break;
                    case AnimParamType.ToV2: DataTmp.m_toV2 = (Vector2)hash.Value; break;
                    case AnimParamType.ToColor: DataTmp.m_toColor = (Color)hash.Value; break;
                    case AnimParamType.ToFloat: DataTmp.m_toFloat = (float)hash.Value; break;

                    //动画回调
                    case AnimParamType.CallBack: DataTmp.m_callBack = (AnimCallBack)hash.Value; break;
                    case AnimParamType.CallBackParams: DataTmp.m_parameter = (object[])hash.Value; break;

                    //定制函数
                    case AnimParamType.CustomMethodV3: DataTmp.m_customMethodV3 = (AnimCustomMethodVector3)hash.Value; break;
                    case AnimParamType.CustomMethodV2: DataTmp.m_customMethodV2 = (AnimCustomMethodVector2)hash.Value; break;
                    case AnimParamType.CustomMethodFloat: DataTmp.m_customMethodFloat = (AnimCustomMethodFloat)hash.Value; break;

                    //闪烁
                    case AnimParamType.Space: DataTmp.m_space = (float)hash.Value; break;

                    //贝塞尔控制点
                    case AnimParamType.PathType: DataTmp.m_pathType = (PathType)hash.Value; break;
                    case AnimParamType.V3Control: DataTmp.m_v3Contral = (Vector3[])hash.Value; break;
                    case AnimParamType.floatControl: DataTmp.m_floatContral = (float[])hash.Value; break;

                    //其他设置
                    case AnimParamType.IsIncludeChild: DataTmp.m_isChild = (bool)hash.Value; break;
                    case AnimParamType.IsLocal: DataTmp.m_isLocal = (bool)hash.Value; break;
                    case AnimParamType.IsIgnoreTimeScale: DataTmp.m_ignoreTimeScale = (bool)hash.Value; break;

                }
            }

            return DataTmp;
        }
    }



    #endregion

    #region 功能函数

    /// <summary>
    /// 停止一个对象身上的所有动画
    /// </summary>
    /// <param name="animGameObject">要停止动画的对象</param>
    /// <param name="isCallBack">是否触发回调</param>
    public static void StopAnim(GameObject animGameObject,bool isCallBack = false)
    {
        for (int i = 0; i < GetInstance().animList.Count; i++)
        {
            if (GetInstance().animList[i].m_animGameObejct == animGameObject)
            {
                if(isCallBack)
                {
                    AnimData dataTmp = GetInstance().animList[i];
                    dataTmp.executeCallBack();
                }

                GetInstance().animList.RemoveAt(i);
                i--;
            }
        }
    }

    /// <summary>
    /// 停止一个动画
    /// </summary>
    /// <param name="animGameObject">要停止的动画</param>
    /// <param name="isCallBack">是否触发回调</param>
    public static void StopAnim(AnimData animData, bool isCallBack = false)
    {
        if (isCallBack)
        {
            animData.executeCallBack();
        }

        GetInstance().animList.Remove(animData);
    }

    /// <summary>
    /// 立即完成一个动画
    /// </summary>
    /// <param name="animGameObject">要完成的</param>
    public static void FinishAnim(AnimData animData)
    {
        animData.m_currentTime = animData.m_totalTime;
        animData.executeUpdate();
        animData.executeCallBack();

        GetInstance().animList.Remove(animData);
    }

    #endregion

    #endregion

    #region 实例部分

    public List<AnimData> animList = new List<AnimData>();

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < animList.Count; i++)
        {
            //执行Update
            animList[i].executeUpdate();

            if (animList[i].m_isDone == true)
            {
                AnimData animTmp = animList[i];

                if (!animTmp.AnimReplayLogic())
                {
                    animList.Remove(animTmp);
                    i--;
                }

                //执行回调
                animTmp.executeCallBack();
            }
        }
    }
    #endregion
}

    #region 枚举与代理声明

public delegate void AnimCallBack(params object[] arg);

public delegate void AnimCustomMethodVector3(Vector3 data);
public delegate void AnimCustomMethodVector2(Vector2 data);
public delegate void AnimCustomMethodFloat(float data);

//动画类型
public enum AnimType
{
    LocalPosition,
    Position,
    LocalScale,
    LocalRotate,
    Rotate,
    
    Color,
    Alpha,

    UGUI_Color,
    UGUI_Alpha,
    UGUI_AnchoredPosition,
    UGUI_SizeDetal,

    Custom_Vector3,
    Custom_Vector2,
    Custom_Float,

    Blink,
}

//插值算法类型
public enum InterpType
{
    Default,
    Linear,
    InBack,
    OutBack,
    InOutBack,
    OutInBack,
    InQuad,
    OutQuad,
    InoutQuad,
    InCubic,
    OutCubic,
    InoutCubic,
    OutInCubic,
    InQuart,
    OutQuart,
    InOutQuart,
    OutInQuart,
    InQuint,
    OutQuint,
    InOutQuint,
    OutInQuint,
    InSine,
    OutSine,
    InOutSine,
    OutInSine,
    InExpo,
    OutExpo,
    InOutExpo,
    OutInExpo,
}

public enum AnimParamType
{
    GameObj,

    FromV3,
    FromV2,
    FromFloat,
    FromColor,

    ToV3,
    ToV2,
    ToFloat,
    ToColor,

    DelayTime,

    AnimType,
    Time,
    InteType,

    IsIgnoreTimeScale,

    PathType,
    V3Control,
    floatControl,

    IsIncludeChild,
    IsLocal,

    RepeatType,
    RepeatCount,

    CustomMethodV3,
    CustomMethodV2,
    CustomMethodFloat,

    Space,

    CallBack,
    CallBackParams
}

public enum PathType
{
    Line,
    Bezier2,
    Bezier3,
}

public enum RepeatType
{
    Once,
    Loop,
    PingPang,
}

#endregion
