using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

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

    public static AnimData UguiColor(GameObject animObject, Color from, Color to, float time, 
        RepeatType repeatType = RepeatType.Once,
        InteType interpolationType = InteType.Default,
        bool isChild = true,
        AnimCallBack callBack = null, params object[] parameter)
    {
        AnimParamHash animParnHash = new AnimParamHash(
          AnimParamType.GameObj, animObject,
          AnimParamType.AnimType, AnimType.UGUI_Color,
          AnimParamType.FromColor, from,
          AnimParamType.ToColor, to,
          AnimParamType.Time, time,
          AnimParamType.InteType, interpolationType,
          AnimParamType.IsIncludeChild, isChild,
          AnimParamType.CallBack, callBack,
          AnimParamType.CallBackParams, parameter
          );

        return ValueTo(animParnHash);
    }

    #endregion

    #region UGUI Alpha
    public static AnimData UguiAlpha(GameObject animObject, float from, float to, float time,
        RepeatType repeatType = RepeatType.Once, 
        InteType interpolationType = InteType.Default, 
        bool isChild = true,
        AnimCallBack callBack = null, params object[] parameter)
    {
        AnimParamHash animParnHash = new AnimParamHash(
           AnimParamType.GameObj, animObject,
           AnimParamType.AnimType, AnimType.UGUI_Alpha,
           AnimParamType.FromFloat, from,
           AnimParamType.ToFloat, to,
           AnimParamType.Time, time,
           AnimParamType.InteType, interpolationType,
           AnimParamType.IsIncludeChild, isChild,
           AnimParamType.RepeatType, repeatType,
           AnimParamType.CallBack,callBack,
           AnimParamType.CallBackParams, parameter
           );

        return ValueTo(animParnHash);
    }

    public static AnimData UguiAlpha(GameObject animObject, float from, float to, float time, 
        InteType interpolationType = InteType.Default, bool isChild = true, AnimCallBack callBack = null, params object[] parameter)
    {
        return UguiAlpha(animObject, from, to, time, RepeatType.Once,interpolationType, isChild, callBack, parameter);
    }


    #endregion

    #region UGUI Move

    public static AnimData UguiMove(GameObject animObject, Vector3 from, Vector3 to, float time, 
        InteType interpolationType = InteType.Default, AnimCallBack callBack = null, params object[] parameter)
    {
        AnimParamHash animParnHash = new AnimParamHash(
          AnimParamType.GameObj, animObject,
          AnimParamType.AnimType, AnimType.UGUI_AnchoredPosition,
          AnimParamType.FromV3, from,
          AnimParamType.ToV3, to,
          AnimParamType.Time, time,
          AnimParamType.InteType, interpolationType,
          AnimParamType.CallBack, callBack,
          AnimParamType.CallBackParams, parameter
          );

        return ValueTo(animParnHash);
    }

    public static AnimData UguiMove(GameObject animObject, Vector3 to, float time,
        InteType interpolationType = InteType.Default, AnimCallBack callBack = null, params object[] parameter)
    {
        Vector3 from = animObject.GetComponent<RectTransform>().anchoredPosition;

        return UguiMove(animObject, from, to, time, interpolationType, callBack, parameter);
    }
    #endregion

    #region UGUI_SizeDelta
    public static AnimData UguiSizeDelta(GameObject animObject, Vector2 from, Vector2 to, float time,
        RepeatType repeatType, InteType interpolationType = InteType.Default, AnimCallBack callBack = null, params object[] parameter)
    {
        AnimParamHash animParnHash = new AnimParamHash(
            AnimParamType.GameObj, animObject,
            AnimParamType.AnimType, AnimType.UGUI_SizeDetal,
            AnimParamType.FromV2, from,
            AnimParamType.ToV2, to,
            AnimParamType.Time, time,
            AnimParamType.InteType, interpolationType,
            AnimParamType.RepeatType, repeatType,
            AnimParamType.CallBack, callBack,
            AnimParamType.CallBackParams, parameter
            );

        return ValueTo(animParnHash);
    }

    public static AnimData UguiSizeDelta(GameObject animObject, Vector2 from, Vector2 to, float time,
        InteType interpolationType = InteType.Default, AnimCallBack callBack = null, params object[] parameter)
    {
        return UguiSizeDelta(animObject, from, to, time, RepeatType.Once, interpolationType, callBack, parameter);
    }

    public static AnimData UguiSizeDelta(GameObject animObject, Vector2 to, float time,
        InteType interpolationType = InteType.Default, AnimCallBack callBack = null, params object[] parameter)
    {
        Vector2 from = animObject.GetComponent<RectTransform>().sizeDelta;
        return UguiSizeDelta(animObject, from, to, time, interpolationType, callBack, parameter);
    }

    #endregion

    #region Color

    public static AnimData ColorTo(GameObject animObject, Color from, Color to, float time,
       InteType interpolationType = InteType.Default,
        RepeatType repeatType = RepeatType.Once,
        bool isChild = true,
       AnimCallBack callBack = null, params object[] parameter)
    {

        AnimParamHash animParnHash = new AnimParamHash(
            AnimParamType.GameObj, animObject,
            AnimParamType.AnimType, AnimType.Color,
            AnimParamType.FromColor, from,
            AnimParamType.ToColor, to,
            AnimParamType.Time, time,
            AnimParamType.InteType, interpolationType,
            AnimParamType.IsIncludeChild, isChild,
            AnimParamType.RepeatType, repeatType,
            AnimParamType.CallBack, callBack,
            AnimParamType.CallBackParams, parameter
            );

        return ValueTo(animParnHash);
    }

    public static AnimData AlphaTo(GameObject animObject, float from, float to, float time,
      InteType interpolationType = InteType.Default,
      RepeatType repeatType = RepeatType.Once,
      bool isChild = true,
      AnimCallBack callBack = null, params object[] parameter)
    {

        AnimParamHash animParnHash = new AnimParamHash(
            AnimParamType.GameObj, animObject,
            AnimParamType.AnimType, AnimType.Alpha,
            AnimParamType.FromFloat, from,
            AnimParamType.ToFloat, to,
            AnimParamType.Time, time,
            AnimParamType.InteType, interpolationType,
            AnimParamType.IsIncludeChild, isChild,
            AnimParamType.RepeatType, repeatType,
            AnimParamType.CallBack, callBack,
            AnimParamType.CallBackParams, parameter
            );

        return ValueTo(animParnHash);
    }

    #endregion

    #region Move
    public static AnimData Move(GameObject animObject, Vector3 from, Vector3 to, float time,
        InteType interpolationType = InteType.Default, RepeatType repeatType = RepeatType.Once, bool isLocal = true,
        AnimCallBack callBack = null, params object[] parameter)
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
            AnimParamType.InteType, interpolationType,
            AnimParamType.IsLocal, isLocal,
            AnimParamType.RepeatType, repeatType,
            AnimParamType.CallBack, callBack,
            AnimParamType.CallBackParams, parameter
            );

        return ValueTo(animParnHash);
    }

    public static AnimData Move(GameObject animObject, Vector3 from, Vector3 to, float time, 
        InteType interpolationType = InteType.Default, bool isLocal = true, AnimCallBack callBack = null, params object[] parameter)
    {
        return Move(animObject, from, to, time, interpolationType, RepeatType.Once, isLocal, callBack, parameter);
    }

    //不传From
    public static AnimData Move(GameObject animObject, Vector3 to, float time,
        InteType interpolationType = InteType.Default, bool isLocal = true, AnimCallBack callBack = null, params object[] parameter)
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

        return Move(animObject, from, to, time, interpolationType, isLocal, callBack, parameter);
    }

    #endregion

    #region Rotate

    public static void Rotate(GameObject animObject, Vector3 from, Vector3 to, float time, 
        InteType interpolationType = InteType.Default,
        RepeatType repeatType = RepeatType.Once,
        bool isLocal = true,
        AnimCallBack callBack = null, params object[] parameter)
    {
        AnimType animType;
        if (isLocal)
        {
            animType = AnimType.LocalRotate;
        }
        else
        {
            animType = AnimType.Rotate;
        }

        AnimParamHash animParnHash = new AnimParamHash(
            AnimParamType.GameObj, animObject,
            AnimParamType.AnimType, animType,
            AnimParamType.FromV3, from,
            AnimParamType.ToV3, to,
            AnimParamType.Time, time,
            AnimParamType.InteType, interpolationType,
            AnimParamType.IsLocal, isLocal,
            AnimParamType.RepeatType, repeatType,
            AnimParamType.CallBack, callBack,
            AnimParamType.CallBackParams, parameter
            );

        ValueTo(animParnHash);

    }


    #endregion

    #region Scale
    public static AnimData Scale(GameObject animObject, Vector3 from, Vector3 to, float time, RepeatType repeatType, 
        InteType interpolationType = InteType.Default , AnimCallBack callBack = null, params object[] parameter)
    {
        AnimParamHash animParnHash = new AnimParamHash(
            AnimParamType.GameObj, animObject,
            AnimParamType.AnimType, AnimType.LocalScale,
            AnimParamType.FromV3, from,
            AnimParamType.ToV3, to,
            AnimParamType.Time, time,
            AnimParamType.InteType, interpolationType,
            AnimParamType.RepeatType, repeatType,
            AnimParamType.CallBack, callBack,
            AnimParamType.CallBackParams, parameter
            );

        return ValueTo(animParnHash);
    }

    public static AnimData Scale(GameObject animObject, Vector3 from, Vector3 to, float time,
        InteType interpolationType = InteType.Default, AnimCallBack callBack = null, params object[] parameter)
    {
        return Scale(animObject, from, to, time, RepeatType.Once,interpolationType,  callBack, parameter);
    }

    public static AnimData Scale(GameObject animObject, Vector3 to, float time,
        InteType interpolationType = InteType.Default, AnimCallBack callBack = null, params object[] parameter)
    {
        Vector3 from = animObject.transform.localScale;
        return Scale(animObject, from, to, time, interpolationType, callBack, parameter);
    }

    #endregion

    #region CustomMethod

    public static AnimData CustomMethodToFloat(AnimCustomMethodFloat method, float from, float to, float time, 
        InteType interpolationType = InteType.Default,
        RepeatType repeatType = RepeatType.Once,
        AnimCallBack callBack = null, params object[] parameter)
    {
        AnimParamHash animParnHash = new AnimParamHash(
           AnimParamType.AnimType, AnimType.Custom_Float,
           AnimParamType.FromFloat, from,
           AnimParamType.ToFloat, to,
           AnimParamType.Time, time,
           AnimParamType.InteType, interpolationType,
           AnimParamType.RepeatType, repeatType,
           AnimParamType.CustomMethodFloat , method,
           AnimParamType.CallBack, callBack,
           AnimParamType.CallBackParams, parameter
           );

        return ValueTo(animParnHash);
    }

    public static AnimData CustomMethodToVector2(AnimCustomMethodVector2 method, Vector2 from, Vector2 to, float time,
        InteType interpolationType = InteType.Default,
        RepeatType repeatType = RepeatType.Once,
        AnimCallBack callBack = null, params object[] parameter)
    {
        AnimParamHash animParnHash = new AnimParamHash(
           AnimParamType.AnimType, AnimType.Custom_Vector2,
           AnimParamType.FromV2, from,
           AnimParamType.ToV2, to,
           AnimParamType.Time, time,
           AnimParamType.InteType, interpolationType,
           AnimParamType.RepeatType , repeatType,
           AnimParamType.CustomMethodV2, method,
           AnimParamType.CallBack, callBack,
           AnimParamType.CallBackParams, parameter
           );

        return ValueTo(animParnHash);
    }

    public static AnimData CustomMethodToVector3(AnimCustomMethodVector3 method, Vector3 from, Vector3 to, float time,
       InteType interpolationType = InteType.Default,
        RepeatType repeatType = RepeatType.Once,
        AnimCallBack callBack = null, params object[] parameter)
    {
        AnimParamHash animParnHash = new AnimParamHash(
           AnimParamType.AnimType, AnimType.Custom_Vector3,
           AnimParamType.FromV3, from,
           AnimParamType.ToV3, to,
           AnimParamType.Time, time,
           AnimParamType.InteType, interpolationType,
           AnimParamType.RepeatType, repeatType,
           AnimParamType.CustomMethodV3, method,
           AnimParamType.CallBack, callBack,
           AnimParamType.CallBackParams, parameter
           );

        return ValueTo(animParnHash);
    }

    #endregion

    #region 贝塞尔
    public static AnimData BezierMove(GameObject animObject, Vector3 from, Vector3 to, float time,
        RepeatType repeatType, InteType interpolationType = InteType.Default, bool isLocal = true,
        PathType bezierMoveType = PathType.Bezier2, Vector3[] t_Bezier_contral = null,
        AnimCallBack callBack = null, params object[] parameter)
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
            AnimParamType.InteType, interpolationType,
            AnimParamType.IsLocal, isLocal,
            AnimParamType.PathType, bezierMoveType,
            AnimParamType.V3Control, t_Bezier_contral,
            AnimParamType.RepeatType, repeatType,
            AnimParamType.CallBack, callBack,
            AnimParamType.CallBackParams, parameter
            );

        return ValueTo(animParnHash);
    }

    public static AnimData BezierMove(GameObject animObject, Vector3 from, Vector3 to, float time,
        InteType interpolationType = InteType.Default, bool isLocal = true, PathType bezierMoveType = PathType.Bezier2, Vector3[] t_Bezier_contral = null, AnimCallBack callBack = null, params object[] parameter)
    {
        return BezierMove(animObject, from, to, time, RepeatType.Once, interpolationType, isLocal, bezierMoveType, t_Bezier_contral, callBack, parameter);
    }

    //不传From，传准确控制点
    public static AnimData BezierMove(GameObject animObject, Vector3 to, float time,
        InteType interpolationType = InteType.Default,
        RepeatType repeatType = RepeatType.Once,
        bool isLocal = true,
        PathType bezierMoveType = PathType.Bezier2,
        Vector3[] t_Bezier_contral = null,
        AnimCallBack callBack = null, params object[] parameter)
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

        return BezierMove(animObject, from, to, time, repeatType, interpolationType, isLocal, bezierMoveType, t_Bezier_contral, callBack, parameter);
    }

    //传From，传准确控制点随机范围
    public static AnimData BezierMove(GameObject animObject, Vector3 from, Vector3 to, float time, RepeatType repeatType,
        InteType interpolationType = InteType.Default,
        bool isLocal = true,
        PathType bezierMoveType = PathType.Bezier2,
        float[] t_Bezier_contralRadius = null,
        AnimCallBack callBack = null, params object[] parameter)
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
            AnimParamType.InteType, interpolationType,
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
        InteType interpolationType = InteType.Default, bool isLocal = true,
        PathType bezierMoveType = PathType.Bezier2,
        float[] t_Bezier_contralRadius = null,
        AnimCallBack callBack = null, params object[] parameter)
    {
        return BezierMove(animObject, from, to, time, RepeatType.Once, interpolationType, isLocal, bezierMoveType, t_Bezier_contralRadius, callBack, parameter);
    }


    //不传From，传准确控制点随机范围
    public static AnimData BezierMove(GameObject animObject, Vector3 to, float time, RepeatType repeatType,
        InteType interpolationType = InteType.Default,
        bool isLocal = true,
        PathType bezierMoveType = PathType.Bezier2,
        float[] t_Bezier_contralRadius = null,
        AnimCallBack callBack = null, params object[] parameter)
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

        return BezierMove(animObject, from, to, time, repeatType, interpolationType, isLocal, bezierMoveType, t_Bezier_contralRadius, callBack, parameter);
    }

    #endregion

    #region 闪烁
    public static AnimData Blink(GameObject animObject, float space, float time, RepeatType repeatType, AnimCallBack callBack = null, params object[] parameter)
    {
        AnimParamHash animParnHash = new AnimParamHash(
            AnimParamType.GameObj, animObject,
            AnimParamType.AnimType, AnimType.Blink,
            AnimParamType.Space , space,
            AnimParamType.Time, time,
            AnimParamType.RepeatType, repeatType,
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
                    case AnimParamType.InteType: DataTmp.m_interpolationType = (InteType)hash.Value; break;
                    case AnimParamType.RepeatType: DataTmp.m_repeatType = (RepeatType)hash.Value; break;

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
                }
            }

            return DataTmp;
        }
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

        AnimType,
        Time,
        InteType,

        PathType,
        V3Control,
        floatControl,

        IsIncludeChild,
        IsLocal,

        RepeatType,

        CustomMethodV3,
        CustomMethodV2,
        CustomMethodFloat,

        Space,


        CallBack,
        CallBackParams
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
                //执行回调
                animList[i].executeCallBack();

                if (!animList[i].AnimReplayLogic())
                {
                    animList.RemoveAt(i);
                    i--;
                }
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
public enum InteType
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
