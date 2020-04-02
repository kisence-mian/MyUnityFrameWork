using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class AnimSystem : MonoBehaviour
{
    #region 静态部分

    static AnimSystem instance;

    // static AnimParamHash HashTemp = new AnimParamHash(); 
    public static AnimSystem GetInstance()
    {
        if (instance == null)
        {
            GameObject animGameObject = GameObject.Find("AnimSystem");
            if (animGameObject == null)
            {
                animGameObject = new GameObject();
                animGameObject.name = "AnimSystem";
                instance = animGameObject.AddComponent<AnimSystem>();
            }

            if(instance == null)
               instance = animGameObject.GetComponent<AnimSystem>();

            if (Application.isPlaying)
            {
                DontDestroyOnLoad(instance.gameObject);
            }
            else
            {
                //不加编译器宏会导致打包失败
#if UNITY_EDITOR

                EditorApplication.update += instance.Update;
#endif
            }
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

        AnimData l_tmp = new AnimData();

        l_tmp.m_animGameObejct = animObject;
        l_tmp.m_animType = AnimType.UGUI_Color;
        l_tmp.m_fromColor = fromTmp;
        l_tmp.m_toColor = to;
        l_tmp.m_isChild = isChild;

        l_tmp.m_delayTime = delayTime;
        l_tmp.m_totalTime = time;
        l_tmp.m_interpolationType = interp;
        l_tmp.m_repeatType = repeatType;
        l_tmp.m_repeatCount = repeatCount;
        l_tmp.m_ignoreTimeScale = IsIgnoreTimeScale;
        l_tmp.m_callBack = callBack;
        l_tmp.m_parameter = parameter;

        l_tmp.Init();

        GetInstance().animList.Add(l_tmp);
        return l_tmp;
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

        AnimData l_tmp = new AnimData();

        l_tmp.m_animGameObejct = animObject;
        l_tmp.m_animType = AnimType.UGUI_Alpha;
        l_tmp.m_fromFloat = fromTmp;
        l_tmp.m_toFloat = to;
        l_tmp.m_isChild = isChild;

        l_tmp.m_delayTime = delayTime;
        l_tmp.m_totalTime = time;
        l_tmp.m_interpolationType = interp;
        l_tmp.m_repeatType = repeatType;
        l_tmp.m_repeatCount = repeatCount;
        l_tmp.m_ignoreTimeScale = IsIgnoreTimeScale;
        l_tmp.m_callBack = callBack;
        l_tmp.m_parameter = parameter;

        l_tmp.Init();

        GetInstance().animList.Add(l_tmp);
        return l_tmp;
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

        AnimData l_tmp = new AnimData(); ;

        l_tmp.m_animGameObejct = animObject;
        l_tmp.m_animType = AnimType.UGUI_AnchoredPosition;
        l_tmp.m_fromV3 = fromTmp;
        l_tmp.m_toV3 = to;

        l_tmp.m_delayTime = delayTime;
        l_tmp.m_totalTime = time;
        l_tmp.m_interpolationType = interp;
        l_tmp.m_repeatType = repeatType;
        l_tmp.m_repeatCount = repeatCount;
        l_tmp.m_ignoreTimeScale = IsIgnoreTimeScale;
        l_tmp.m_callBack = callBack;
        l_tmp.m_parameter = parameter;

        l_tmp.Init();

        GetInstance().animList.Add(l_tmp);
        return l_tmp;
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

        AnimData l_tmp = new AnimData(); ;

        l_tmp.m_animGameObejct = animObject;
        l_tmp.m_animType = AnimType.UGUI_SizeDetal;
        l_tmp.m_fromV2 = fromTmp;
        l_tmp.m_toV2 = to;

        l_tmp.m_delayTime = delayTime;
        l_tmp.m_totalTime = time;
        l_tmp.m_interpolationType = interp;
        l_tmp.m_repeatType = repeatType;
        l_tmp.m_repeatCount = repeatCount;
        l_tmp.m_ignoreTimeScale = IsIgnoreTimeScale;
        l_tmp.m_callBack = callBack;
        l_tmp.m_parameter = parameter;

        l_tmp.Init();

        GetInstance().animList.Add(l_tmp);
        return l_tmp;
    }

    #endregion

    #region Color

    public static AnimData ColorTo(GameObject animObject, Color from, Color to,

        float time = 0.5f,
        float delayTime = 0,
        InterpType interp = InterpType.Default,
        bool isChild = true,
        bool IsIgnoreTimeScale = false,
        RepeatType repeatType = RepeatType.Once,
        int repeatCount = -1,
        AnimCallBack callBack = null,
        object[] parameter = null)
    {
        AnimData l_tmp = new AnimData(); ;

        l_tmp.m_animGameObejct = animObject;
        l_tmp.m_animType = AnimType.Color;
        l_tmp.m_fromColor = from;
        l_tmp.m_toColor = to;
        l_tmp.m_isChild = isChild;

        l_tmp.m_delayTime = delayTime;
        l_tmp.m_totalTime = time;
        l_tmp.m_interpolationType = interp;
        l_tmp.m_repeatType = repeatType;
        l_tmp.m_repeatCount = repeatCount;
        l_tmp.m_ignoreTimeScale = IsIgnoreTimeScale;
        l_tmp.m_callBack = callBack;
        l_tmp.m_parameter = parameter;

        l_tmp.Init();

        GetInstance().animList.Add(l_tmp);
        return l_tmp;
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

        AnimData l_tmp = new AnimData(); ;

        l_tmp.m_animGameObejct = animObject;
        l_tmp.m_animType = AnimType.Alpha;
        l_tmp.m_fromFloat = from;
        l_tmp.m_toFloat = to;
        l_tmp.m_isChild = isChild;

        l_tmp.m_delayTime = delayTime;
        l_tmp.m_totalTime = time;
        l_tmp.m_interpolationType = interp;
        l_tmp.m_repeatType = repeatType;
        l_tmp.m_repeatCount = repeatCount;
        l_tmp.m_ignoreTimeScale = IsIgnoreTimeScale;
        l_tmp.m_callBack = callBack;
        l_tmp.m_parameter = parameter;

        l_tmp.Init();

        GetInstance().animList.Add(l_tmp);
        return l_tmp;
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
        Transform toTransform = null,
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

        AnimData l_tmp = new AnimData(); ;

        l_tmp.m_animGameObejct = animObject;
        l_tmp.m_animType = animType;
        l_tmp.m_fromV3 = fromTmp;
        l_tmp.m_toV3 = to;
        l_tmp.m_isLocal = isLocal;
        l_tmp.m_toTransform = toTransform;

        l_tmp.m_delayTime = delayTime;
        l_tmp.m_totalTime = time;
        l_tmp.m_interpolationType = interp;
        l_tmp.m_repeatType = repeatType;
        l_tmp.m_repeatCount = repeatCount;
        l_tmp.m_ignoreTimeScale = IsIgnoreTimeScale;
        l_tmp.m_callBack = callBack;
        l_tmp.m_parameter = parameter;

        l_tmp.Init();

        GetInstance().animList.Add(l_tmp);
        return l_tmp;
    }

    #endregion

    #region Rotate

    public static AnimData Rotate(GameObject animObject, Vector3? from, Vector3 to,

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
        Vector3 fromTmp;

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

        AnimData l_tmp = new AnimData(); ;

        l_tmp.m_animGameObejct = animObject;
        l_tmp.m_animType = animType;
        l_tmp.m_fromV3 = fromTmp;
        l_tmp.m_toV3 = to;

        l_tmp.m_isLocal = isLocal;

        l_tmp.m_delayTime = delayTime;
        l_tmp.m_totalTime = time;
        l_tmp.m_interpolationType = interp;
        l_tmp.m_repeatType = repeatType;
        l_tmp.m_repeatCount = repeatCount;
        l_tmp.m_ignoreTimeScale = IsIgnoreTimeScale;
        l_tmp.m_callBack = callBack;
        l_tmp.m_parameter = parameter;

        l_tmp.Init();

        GetInstance().animList.Add(l_tmp);
        return l_tmp;

    }


    #endregion

    #region Rotate_Quaternion

    public static AnimData Rotation(GameObject animObject, Quaternion? from, Quaternion to,

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
        Quaternion fromTmp;

        if (isLocal)
        {
            fromTmp = from ?? animObject.transform.localRotation;
            animType = AnimType.LocalRotation;
        }
        else
        {
            fromTmp = from ?? animObject.transform.rotation;
            animType = AnimType.Rotation;
        }

        AnimData l_tmp = new AnimData(); ;

        l_tmp.m_animGameObejct = animObject;
        l_tmp.m_animType = animType;
        l_tmp.m_fromQ4 = fromTmp;
        l_tmp.m_toQ4 = to;

        l_tmp.m_isLocal = isLocal;

        l_tmp.m_delayTime = delayTime;
        l_tmp.m_totalTime = time;
        l_tmp.m_interpolationType = interp;
        l_tmp.m_repeatType = repeatType;
        l_tmp.m_repeatCount = repeatCount;
        l_tmp.m_ignoreTimeScale = IsIgnoreTimeScale;
        l_tmp.m_callBack = callBack;
        l_tmp.m_parameter = parameter;

        l_tmp.Init();

        GetInstance().animList.Add(l_tmp);
        return l_tmp;

    }


    public static AnimData Rotation(GameObject animObject, Vector3? from, Vector3 to,

     float time = 0.5f,
     float delayTime = 0,
     bool isLocal = true,
     InterpType interp = InterpType.Default,
     bool IsIgnoreTimeScale = false,
     RepeatType repeatType = RepeatType.Once,
     int repeatCount = -1,

     AnimCallBack callBack = null, object[] parameter = null)
    {

        Quaternion? quaternion = null;
        if (from != null)
        {
            quaternion = Quaternion.Euler((Vector3)from);
        }
        return Rotation( animObject, quaternion, Quaternion.Euler(to),

          time ,
          delayTime,
          isLocal ,
          interp ,
          IsIgnoreTimeScale,
          repeatType ,
          repeatCount,

          callBack , parameter );

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

        AnimData l_tmp = new AnimData(); ;

        l_tmp.m_animGameObejct = animObject;
        l_tmp.m_animType = AnimType.LocalScale;
        l_tmp.m_fromV3 = fromTmp;
        l_tmp.m_toV3 = to;

        l_tmp.m_delayTime = delayTime;
        l_tmp.m_totalTime = time;
        l_tmp.m_interpolationType = interp;
        l_tmp.m_repeatType = repeatType;
        l_tmp.m_repeatCount = repeatCount;
        l_tmp.m_ignoreTimeScale = IsIgnoreTimeScale;
        l_tmp.m_callBack = callBack;
        l_tmp.m_parameter = parameter;

        l_tmp.Init();

        GetInstance().animList.Add(l_tmp);
        return l_tmp;
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
        AnimData l_tmp = new AnimData(); ;

        l_tmp.m_animType = AnimType.Custom_Float;
        l_tmp.m_fromFloat = from;
        l_tmp.m_toFloat = to;
        l_tmp.m_customMethodFloat = method;

        l_tmp.m_delayTime = delayTime;
        l_tmp.m_totalTime = time;
        l_tmp.m_interpolationType = interp;
        l_tmp.m_repeatType = repeatType;
        l_tmp.m_repeatCount = repeatCount;
        l_tmp.m_ignoreTimeScale = IsIgnoreTimeScale;
        l_tmp.m_callBack = callBack;
        l_tmp.m_parameter = parameter;

        l_tmp.Init();

        GetInstance().animList.Add(l_tmp);
        return l_tmp;
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
        AnimData l_tmp = new AnimData(); ;

        l_tmp.m_animType = AnimType.Custom_Vector2;
        l_tmp.m_fromV2 = from;
        l_tmp.m_toV2 = to;
        l_tmp.m_customMethodV2 = method;

        l_tmp.m_delayTime = delayTime;
        l_tmp.m_totalTime = time;
        l_tmp.m_interpolationType = interp;
        l_tmp.m_repeatType = repeatType;
        l_tmp.m_repeatCount = repeatCount;
        l_tmp.m_ignoreTimeScale = IsIgnoreTimeScale;
        l_tmp.m_callBack = callBack;
        l_tmp.m_parameter = parameter;

        l_tmp.Init();

        GetInstance().animList.Add(l_tmp);
        return l_tmp;
    }

    public static AnimData CustomMethodToVector3(AnimCustomMethodVector3 method, Vector3 from, Vector3 to,
        float time = 0.5f,
        float delayTime = 0,
        InterpType interp = InterpType.Default,
        bool IsIgnoreTimeScale = false,
        RepeatType repeatType = RepeatType.Once,
        int repeatCount = -1,
        AnimCallBack callBack = null, object[] parameter = null)
    {
        AnimData l_tmp = new AnimData(); ;

        l_tmp.m_animType = AnimType.Custom_Vector3;
        l_tmp.m_fromV3 = from;
        l_tmp.m_toV3 = to;
        l_tmp.m_customMethodV3 = method;

        l_tmp.m_delayTime = delayTime;
        l_tmp.m_totalTime = time;
        l_tmp.m_interpolationType = interp;
        l_tmp.m_repeatType = repeatType;
        l_tmp.m_repeatCount = repeatCount;
        l_tmp.m_ignoreTimeScale = IsIgnoreTimeScale;
        l_tmp.m_callBack = callBack;
        l_tmp.m_parameter = parameter;

        l_tmp.Init();

        GetInstance().animList.Add(l_tmp);
        return l_tmp;
    }

    public static AnimData CustomMethodToVector4(AnimCustomMethodVector4 method, Vector4 from, Vector4 to,
    float time = 0.5f,
    float delayTime = 0,
    InterpType interp = InterpType.Default,
    bool IsIgnoreTimeScale = false,
    RepeatType repeatType = RepeatType.Once,
    int repeatCount = -1,
    AnimCallBack callBack = null, object[] parameter = null)
    {
        AnimData l_tmp = new AnimData(); ;

        l_tmp.m_animType = AnimType.Custom_Vector4;
        l_tmp.m_fromV4 = from;
        l_tmp.m_toV4 = to;
        l_tmp.m_customMethodV4 = method;

        l_tmp.m_delayTime = delayTime;
        l_tmp.m_totalTime = time;
        l_tmp.m_interpolationType = interp;
        l_tmp.m_repeatType = repeatType;
        l_tmp.m_repeatCount = repeatCount;
        l_tmp.m_ignoreTimeScale = IsIgnoreTimeScale;
        l_tmp.m_callBack = callBack;
        l_tmp.m_parameter = parameter;

        l_tmp.Init();

        GetInstance().animList.Add(l_tmp);
        return l_tmp;
    }

    #endregion

    #region 贝塞尔
    public static AnimData BezierMove(GameObject animObject, Vector3? from, Vector3 to,
        Vector3[] bezier_contral,
        float time = 0.5f,
        float delayTime = 0,
        RepeatType repeatType = RepeatType.Once,
        int repeatCount = -1,
        InterpType interp = InterpType.Default,
        bool isLocal = true,
        PathType bezierMoveType = PathType.Bezier2,

        AnimCallBack callBack = null, object[] parameter = null)
    {

        AnimData l_tmp = new AnimData(); ;

        if (isLocal)
        {
            l_tmp.m_animType = AnimType.LocalPosition;
            l_tmp.m_fromV3 = from ?? animObject.transform.localPosition;
        }
        else
        {
            l_tmp.m_animType = AnimType.Position;
            l_tmp.m_fromV3 = from ?? animObject.transform.position;
        }

        l_tmp.m_animGameObejct = animObject;


        l_tmp.m_toV3 = to;
        l_tmp.m_isLocal = isLocal;
        l_tmp.m_pathType = bezierMoveType;
        l_tmp.m_v3Contral = bezier_contral;

        l_tmp.m_delayTime = delayTime;
        l_tmp.m_totalTime = time;
        l_tmp.m_interpolationType = interp;
        l_tmp.m_repeatType = repeatType;
        l_tmp.m_repeatCount = repeatCount;
        l_tmp.m_callBack = callBack;
        l_tmp.m_parameter = parameter;

        l_tmp.Init();

        GetInstance().animList.Add(l_tmp);
        return l_tmp;
    }

    public static AnimData BezierMove(GameObject animObject, Vector3? from, Vector3 to,
        Vector3[] t_Bezier_contral,
        float time = 0.5f,
        InterpType interp = InterpType.Default,
        bool isLocal = true,
        PathType bezierMoveType = PathType.Bezier2,

        AnimCallBack callBack = null, object[] parameter = null)
    {
        return BezierMove(animObject, from, to, t_Bezier_contral, time, 0, RepeatType.Once, -1, interp, isLocal, bezierMoveType, callBack, parameter);
    }

    //不传From，传准确控制点
    public static AnimData BezierMove(GameObject animObject, Vector3 to,
        Vector3[] t_Bezier_contral,
        float time = 0.5f,
        InterpType interp = InterpType.Default,
        RepeatType repeatType = RepeatType.Once,
        bool isLocal = true,
        PathType bezierMoveType = PathType.Bezier2,

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

        return BezierMove(animObject, from, to, t_Bezier_contral, time, 0, repeatType, -1, interp, isLocal, bezierMoveType, callBack, parameter);
    }

    //传From，传准确控制点随机范围
    public static AnimData BezierMove(GameObject animObject, Vector3? from, Vector3 to, float time,
        float[] t_Bezier_contralRadius,
        RepeatType repeatType,
        int repeatCount = -1,
        float delayTime = 0,
        InterpType interp = InterpType.Default,
        bool isLocal = true,
        PathType bezierMoveType = PathType.Bezier2,

        AnimCallBack callBack = null, object[] parameter = null)
    {

        AnimData l_tmp = new AnimData(); ;

        if (isLocal)
        {
            l_tmp.m_animType = AnimType.LocalPosition;
            l_tmp.m_fromV3 = from ?? animObject.transform.localPosition;
        }
        else
        {
            l_tmp.m_animType = AnimType.Position;
            l_tmp.m_fromV3 = from ?? animObject.transform.position;
        }

        l_tmp.m_animGameObejct = animObject;

        l_tmp.m_toV3 = to;
        l_tmp.m_isLocal = isLocal;
        l_tmp.m_pathType = bezierMoveType;
        l_tmp.m_floatContral = t_Bezier_contralRadius;

        l_tmp.m_delayTime = delayTime;
        l_tmp.m_totalTime = time;
        l_tmp.m_interpolationType = interp;
        l_tmp.m_repeatType = repeatType;
        l_tmp.m_repeatCount = repeatCount;
        l_tmp.m_callBack = callBack;
        l_tmp.m_parameter = parameter;

        l_tmp.Init();

        GetInstance().animList.Add(l_tmp);

        return l_tmp;
    }

    public static AnimData BezierMove(GameObject animObject, Vector3 from, Vector3 to, float time,
        float[] t_Bezier_contralRadius,
        float delayTime = 0,
        InterpType interp = InterpType.Default, bool isLocal = true,
        PathType bezierMoveType = PathType.Bezier2,
        AnimCallBack callBack = null, object[] parameter = null)
    {
        return BezierMove(animObject, from, to, time, t_Bezier_contralRadius, RepeatType.Once, 1, delayTime, interp, isLocal, bezierMoveType, callBack, parameter);
    }


    //不传From，传准确控制点随机范围
    public static AnimData BezierMove(GameObject animObject, Vector3 to, float time, RepeatType repeatType,
        float[] t_Bezier_contralRadius,
        InterpType interp = InterpType.Default,
        float delayTime = 0,
        bool isLocal = true,
        PathType bezierMoveType = PathType.Bezier2,

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

        return BezierMove(animObject, from, to, time, t_Bezier_contralRadius, repeatType, 1, delayTime, interp, isLocal, bezierMoveType, callBack, parameter);
    }

    #endregion

    #region 闪烁
    public static AnimData Blink(GameObject animObject, float space,

        float time = 0.5f,
        float delayTime = 0,
        bool IsIgnoreTimeScale = false,
        RepeatType repeatType = RepeatType.Once,
        int repeatCount = -1,
        AnimCallBack callBack = null,
        object[] parameter = null)
    {

        AnimData l_tmp = new AnimData(); ;

        l_tmp.m_animType = AnimType.Blink;
        l_tmp.m_animGameObejct = animObject;
        l_tmp.m_space = space;

        l_tmp.m_delayTime = delayTime;
        l_tmp.m_totalTime = time;
        l_tmp.m_repeatType = repeatType;
        l_tmp.m_repeatCount = repeatCount;
        l_tmp.m_callBack = callBack;
        l_tmp.m_parameter = parameter;
        l_tmp.m_ignoreTimeScale = IsIgnoreTimeScale;

        l_tmp.Init();

        GetInstance().animList.Add(l_tmp);
        return l_tmp;

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
        public AnimParamHash(params object[] l_params)
        {
            for (int i = 0; i < l_params.Length; i += 2)
            {
                this[(AnimParamType)l_params[i]] = l_params[i + 1];
            }
        }

        public AnimParamHash SetData(params object[] l_params)
        {
            Clear();

            for (int i = 0; i < l_params.Length; i += 2)
            {
                this[(AnimParamType)l_params[i]] = l_params[i + 1];
            }

            return this;
        }

        public AnimData GetAnimData()
        {
            AnimData DataTmp = HeapObjectPool<AnimData>.GetObject();

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
    public static void StopAnim(GameObject animGameObject, bool isCallBack = false)
    {
        for (int i = 0; i < GetInstance().animList.Count; i++)
        {
            if (GetInstance().animList[i].m_animGameObejct == animGameObject)
            {
                AnimData animData = GetInstance().animList[i];

                if (isCallBack)
                {
                    animData.ExecuteCallBack();
                }

                GetInstance().removeList.Add(animData);
            }
        }

        for (int i = 0; i < GetInstance().removeList.Count; i++)
        {
            GetInstance().animList.Remove(GetInstance().removeList[i]);
        }
        GetInstance().removeList.Clear();
    }

    /// <summary>
    /// 停止一个动画
    /// </summary>
    /// <param name="animGameObject">要停止的动画</param>
    /// <param name="isCallBack">是否触发回调</param>
    public static void StopAnim(AnimData animData, bool isCallBack = false)
    {
        if(GetInstance().animList.Contains(animData))
        {
            if (isCallBack)
            {
                animData.ExecuteCallBack();
            }

            GetInstance().animList.Remove(animData);
        }
    }

    /// <summary>
    /// 立即完成一个动画
    /// </summary>
    /// <param name="animGameObject">要完成的</param>
    public static void FinishAnim(AnimData animData)
    {
        animData.m_currentTime = animData.m_totalTime;
        animData.ExecuteUpdate();
        animData.ExecuteCallBack();

        GetInstance().animList.Remove(animData);
    }

    public static void ClearAllAnim(bool isCallBack = false)
    {
        for (int i = 0; i < GetInstance().animList.Count; i++)
        {
            AnimData animTmp = GetInstance().animList[i];
            if (isCallBack)
            {
                animTmp.ExecuteCallBack();
            }
        }

        GetInstance().animList.Clear();
    }
    #endregion

    #endregion

    #region 实例部分

    public List<AnimData> animList = new List<AnimData>();
    public List<AnimData> removeList = new List<AnimData>();

    // Update is called once per frame
    public void Update()
    {
        for (int i = 0; i < animList.Count; i++)
        {
            //执行Update
            bool isError =  animList[i].ExecuteUpdate();

            if(isError)
            {
                removeList.Add(animList[i]);
                continue;
            }

            if (animList[i].m_isDone == true)
            {
                AnimData animTmp = animList[i];

                //执行回调
                animTmp.ExecuteCallBack();  

                if (!animTmp.AnimReplayLogic())
                {
                    removeList.Add(animTmp);
                }
            }
        }

        for (int i = 0; i < removeList.Count; i++)
        {
            animList.Remove(removeList[i]);
        }

        removeList.Clear();
    }
    #endregion
}

    #region 枚举与代理声明

public delegate void AnimCallBack(params object[] arg);

public delegate void AnimCustomMethodVector4(Vector4 data);
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
    LocalRotation,
    Rotation,
    
    Color,
    Alpha,

    UGUI_Color,
    UGUI_Alpha,
    UGUI_AnchoredPosition,
    UGUI_SizeDetal,

    Custom_Vector4,
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

    OutBounce,
    InBounce,
    InOutBounce,
    OutInBounce,
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
