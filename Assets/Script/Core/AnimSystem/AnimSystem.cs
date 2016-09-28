using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AnimSystem : MonoBehaviour
{

    #region 静态部分

    static AnimSystem instance;

    public static AnimSystem getInstance()
    {
        if (instance == null)
        {
            GameObject animGameObject = new GameObject();
            animGameObject.name = "AnimSystem";
            instance = animGameObject.AddComponent<AnimSystem>();


            ResourceManager.Load("");
        }

        return instance;
    }

    #region 贝塞尔路径方法群
    public static void BezierMove(GameObject animObject, Vector3 from, Vector3 to, float time, PlayType playType , InteType interpolationType = InteType.Default, bool isLocal = true, PathType bezierMoveType = PathType.Bezier2, Vector3[] t_Bezier_contral = null, AnimCallBack callBack = null, params object[] parameter)
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

      


        AnimParnHash animParnHash = new AnimParnHash(
            AnimParamType.GameObj, animObject,
            AnimParamType.AnimType, animType,
            AnimParamType.FromV3, from,
            AnimParamType.ToV3, to,
            AnimParamType.Time, time,
            AnimParamType.InteType, interpolationType,
            AnimParamType.IsLocal, isLocal,
            AnimParamType.PathType, bezierMoveType,
            AnimParamType.V3Control, t_Bezier_contral,
            AnimParamType.PlayType, playType
            );

        


        ValueTo(animParnHash, callBack, parameter);

    }

    public static void BezierMove(GameObject animObject, Vector3 from, Vector3 to, float time, InteType interpolationType = InteType.Default, bool isLocal = true, PathType bezierMoveType = PathType.Bezier2, Vector3[] t_Bezier_contral = null, AnimCallBack callBack = null, params object[] parameter)
    {
        BezierMove(animObject, from, to, time, PlayType.Once, interpolationType, isLocal, bezierMoveType, t_Bezier_contral, callBack, parameter);

    }

    //不传From，传准确控制点
    public static void BezierMove(GameObject animObject, Vector3 to, float time, InteType interpolationType = InteType.Default, PlayType playType = PlayType.Once, bool isLocal = true, PathType bezierMoveType = PathType.Bezier2, Vector3[] t_Bezier_contral = null, AnimCallBack callBack = null, params object[] parameter)
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

        BezierMove(animObject, from, to, time,playType, interpolationType, isLocal,bezierMoveType, t_Bezier_contral, callBack, parameter);
    }

    //传From，传准确控制点随机范围
    public static void BezierMove(GameObject animObject, Vector3 from, Vector3 to, float time, PlayType playType , InteType interpolationType = InteType.Default, bool isLocal = true, PathType bezierMoveType = PathType.Bezier2, float[] t_Bezier_contralRadius = null, AnimCallBack callBack = null, params object[] parameter)
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

        AnimParnHash animParnHash = new AnimParnHash(
            AnimParamType.GameObj, animObject,
            AnimParamType.AnimType, animType,
            AnimParamType.FromV3, from,
            AnimParamType.ToV3, to,
            AnimParamType.Time, time,
            AnimParamType.InteType, interpolationType,
            AnimParamType.IsLocal, isLocal,
            AnimParamType.PathType, bezierMoveType,
            AnimParamType.floatControl, t_Bezier_contralRadius,
            AnimParamType.PlayType, playType
            );

        ValueTo(animParnHash, callBack, parameter);

    }

    public static void BezierMove(GameObject animObject, Vector3 from, Vector3 to, float time, InteType interpolationType = InteType.Default,  bool isLocal = true, PathType bezierMoveType = PathType.Bezier2, float[] t_Bezier_contralRadius = null, AnimCallBack callBack = null, params object[] parameter)
    {

        BezierMove(animObject, from, to, time, PlayType.Once, interpolationType, isLocal, bezierMoveType, t_Bezier_contralRadius, callBack, parameter);
    }


    //不传From，传准确控制点随机范围
    public static void BezierMove(GameObject animObject, Vector3 to, float time, PlayType playType , InteType interpolationType = InteType.Default, bool isLocal = true, PathType bezierMoveType = PathType.Bezier2, float[] t_Bezier_contralRadius = null, AnimCallBack callBack = null, params object[] parameter)
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

        BezierMove(animObject, from, to, time, playType,interpolationType, isLocal,bezierMoveType, t_Bezier_contralRadius, callBack, parameter);
    }




    #endregion

    #region UGUI Alpha
    public static void uguiAlpha(GameObject animObject, float from, float to, float time, PlayType playType, InteType interpolationType = InteType.Default, bool isChild = true, AnimCallBack callBack = null, params object[] parameter)
    {
        AnimParnHash animParnHash = new AnimParnHash(
           AnimParamType.GameObj, animObject,
           AnimParamType.AnimType, AnimType.UGUI_alpha,
           AnimParamType.FromAlpha, from,
           AnimParamType.ToAlpha, to,
           AnimParamType.Time, time,
           AnimParamType.InteType, interpolationType,
           AnimParamType.IsChild, isChild
           );

        ValueTo(animParnHash, callBack, parameter);
    }

    public static void uguiAlpha(GameObject animObject, float from, float to, float time, InteType interpolationType = InteType.Default, bool isChild = true, AnimCallBack callBack = null, params object[] parameter)
    {
        uguiAlpha(animObject, from, to, time, PlayType.Once,interpolationType, isChild, callBack, parameter);
    }


    #endregion

    #region UGUI Move
    public static void UguiMove(GameObject animObject, Vector3 from, Vector3 to, float time, InteType interpolationType = InteType.Default, AnimCallBack callBack = null, params object[] parameter)
    {
        AnimParnHash animParnHash = new AnimParnHash(
          AnimParamType.GameObj, animObject,
          AnimParamType.AnimType, AnimType.UGUI_anchoredPosition,
          AnimParamType.FromV3, from,
          AnimParamType.ToV3, to,
          AnimParamType.Time, time,
          AnimParamType.InteType, interpolationType
          );

        ValueTo(animParnHash, callBack, parameter);

    }



    public static void UguiMove(GameObject animObject, Vector3 to, float time, InteType interpolationType = InteType.Default, AnimCallBack callBack = null, params object[] parameter)
    {
        Vector3 from = animObject.GetComponent<RectTransform>().anchoredPosition;


        UguiMove(animObject, from, to, time, interpolationType, callBack, parameter);
    }
    #endregion


    #region Move方法群
    public static void Move(GameObject animObject, Vector3 from, Vector3 to, float time, InteType interpolationType = InteType.Default, PlayType playType = PlayType.Once, bool isLocal = true, AnimCallBack callBack = null, params object[] parameter)
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

        AnimParnHash animParnHash = new AnimParnHash(
            AnimParamType.GameObj, animObject,
            AnimParamType.AnimType, animType,
            AnimParamType.FromV3, from,
            AnimParamType.ToV3, to,
            AnimParamType.Time, time,
            AnimParamType.InteType, interpolationType,
            AnimParamType.IsLocal, isLocal,
            AnimParamType.PlayType, playType
            );

        ValueTo(animParnHash, callBack, parameter);

    }

    public static void Move(GameObject animObject, Vector3 from, Vector3 to, float time, InteType interpolationType = InteType.Default, bool isLocal = true, AnimCallBack callBack = null, params object[] parameter)
    {
        Move(animObject, from, to, time, interpolationType, PlayType.Once, isLocal, callBack, parameter);
    }

    //不传From
    public static void Move(GameObject animObject, Vector3 to, float time, InteType interpolationType = InteType.Default, bool isLocal = true, AnimCallBack callBack = null, params object[] parameter)
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


        Move(animObject, from, to, time, interpolationType, isLocal, callBack, parameter);

    }

    #endregion


    #region Scale 变换方法群
    public static void Scale(GameObject animObject, Vector3 from, Vector3 to, float time,PlayType playType , InteType interpolationType = InteType.Default , AnimCallBack callBack = null, params object[] parameter)
    {
        AnimParnHash animParnHash = new AnimParnHash(
            AnimParamType.GameObj, animObject,
            AnimParamType.AnimType, AnimType.LocalScale,
            AnimParamType.FromV3, from,
            AnimParamType.ToV3, to,
            AnimParamType.Time, time,
            AnimParamType.InteType, interpolationType,
            AnimParamType.PlayType, playType

            );

        ValueTo(animParnHash, callBack, parameter);
    }

    public static void Scale(GameObject animObject, Vector3 from, Vector3 to, float time, InteType interpolationType = InteType.Default, AnimCallBack callBack = null, params object[] parameter)
    {
        Scale(animObject, from, to, time, PlayType.Once,interpolationType,  callBack, parameter);
    }

    public static void Scale(GameObject animObject, Vector3 to, float time, InteType interpolationType = InteType.Default, AnimCallBack callBack = null, params object[] parameter)
    {
        Vector3 from = animObject.transform.localScale;
        Scale(animObject, from, to, time, interpolationType, callBack, parameter);

    }

    #endregion

    #region SizeDelta
    public static void SizeDelta(GameObject animObject, Vector2 from, Vector2 to, float time, PlayType playType, InteType interpolationType = InteType.Default, AnimCallBack callBack = null, params object[] parameter)
    {
        AnimParnHash animParnHash = new AnimParnHash(
            AnimParamType.GameObj, animObject,
            AnimParamType.AnimType, AnimType.SizeDetal,
            AnimParamType.FromV2, from,
            AnimParamType.ToV2, to,
            AnimParamType.Time, time,
            AnimParamType.InteType, interpolationType,
            AnimParamType.PlayType, playType

            );

        ValueTo(animParnHash, callBack, parameter);
    }

    public static void SizeDelta(GameObject animObject, Vector2 from, Vector2 to, float time, InteType interpolationType = InteType.Default, AnimCallBack callBack = null, params object[] parameter)
    {
        SizeDelta(animObject, from, to, time, PlayType.Once, interpolationType, callBack, parameter);
    }

    public static void SizeDelta(GameObject animObject, Vector2 to, float time, InteType interpolationType = InteType.Default, AnimCallBack callBack = null, params object[] parameter)
    {
        Vector2 from = animObject.GetComponent<RectTransform>().sizeDelta;
        SizeDelta(animObject, from, to, time, interpolationType, callBack, parameter);
    }

    #endregion



    public static void ValueTo(AnimParnHash l_animHash, AnimCallBack l_callBack, params object[] l_objs)
    {

        if (l_animHash.ContainsKey(AnimParamType.CallBack)  )
        {
            if(l_callBack != null)
            {
                l_animHash[AnimParamType.CallBack] = l_callBack;
            }
        }
        else
        {
            l_animHash.Add(AnimParamType.CallBack, l_callBack);
 
        }

        if (l_animHash.ContainsKey(AnimParamType.CallBackParams))
        {
            if (l_objs != null)
            {
                l_animHash[AnimParamType.CallBackParams] = l_objs;
            }
        }
        else
        {
            l_animHash.Add(AnimParamType.CallBackParams, l_objs);
        }

        
        

        AnimData l_tmp = l_animHash.GetAnimData();

        l_tmp.Init();

        getInstance().animList.Add(l_tmp);

    }

    public class AnimParnHash : Dictionary<AnimParamType, object>
    {
        public AnimData DataTmp = new AnimData();

        public AnimParnHash(params object[] l_params)
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
                    case AnimParamType.GameObj: DataTmp.animGameObejct = (GameObject)hash.Value; break;
                    case AnimParamType.FromV3: DataTmp.fromPos = (Vector3)hash.Value; break;
                    case AnimParamType.FromV2: DataTmp.fromV2 = (Vector2)hash.Value; break;
                    case AnimParamType.FromColor: DataTmp.fromColor = (Color)hash.Value; break;
                    case AnimParamType.FromAlpha: DataTmp.fromAlpha = (float)hash.Value; break;

                    case AnimParamType.ToV3: DataTmp.toPos = (Vector3)hash.Value; break;
                    case AnimParamType.ToV2: DataTmp.toV2 = (Vector2)hash.Value; break;
                    case AnimParamType.ToColor: DataTmp.toColor = (Color)hash.Value; break;
                    case AnimParamType.ToAlpha: DataTmp.toAlpha = (float)hash.Value; break;

                    case AnimParamType.AnimType: DataTmp.animType = (AnimType)hash.Value; break;
                    case AnimParamType.Time: DataTmp.totalTime = (float)hash.Value; break;
                    case AnimParamType.InteType: DataTmp.interpolationType = (InteType)hash.Value; break;
                    case AnimParamType.PathType: DataTmp.pathType = (PathType)hash.Value; break;
                    case AnimParamType.V3Control: DataTmp.v3Contral = (Vector3[])hash.Value; break;
                    case AnimParamType.floatControl: DataTmp.floatContral = (float[])hash.Value; break;
                    case AnimParamType.PlayType: DataTmp.playType = (PlayType)hash.Value; break;

                    case AnimParamType.IsChild: DataTmp.b_isChild = (bool)hash.Value; break;
                    case AnimParamType.CallBack: DataTmp.callBack = (AnimCallBack)hash.Value; break;
                    case AnimParamType.CallBackParams: DataTmp.parameter = (object[])hash.Value; break;


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
        FromAlpha,
        FromColor,

        ToV3,
        ToV2,
        ToAlpha,
        ToColor,

        AnimType,
        Time,
        InteType,

        PathType,
        V3Control,
        floatControl,

        RepeatType,

        IsChild,
        IsLocal,

        AllHash,
        PlayType,

        CallBack,
        CallBackParams
    }

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

            if (animList[i].isDone == true)
            {
               
                //执行回调
                animList[i].executeCallBack();

                switch (animList[i].playType)
                {
                    case PlayType.Once: 
                        animList.RemoveAt(i);
                        i--;
                        break;
                    case PlayType.Loop:
                        animList[i].isDone = false;
                        animList[i].currentTime = 0;
                        break;
                    case PlayType.PingPang:

                        ExchangeTwoValue(ref animList[i].fromV2, ref animList[i].toV2);
                        ExchangeTwoValue(ref animList[i].fromPos, ref animList[i].toPos);
                        ExchangeTwoValue(ref animList[i].fromAlpha, ref animList[i].toAlpha);
                        animList[i].isDone = false;
                        animList[i].currentTime = 0;
                        break;
                }
            }
        }
    }

    public void ExchangeTwoValue(ref Vector3 v1, ref Vector3 v2)
    {
        Vector3 v3 = v1;
        v1 = v2;
        v2 = v3;
 
    }
    public void ExchangeTwoValue(ref Vector2 v1, ref Vector2 v2)
    {
        Vector2 v3 = v1;
        v1 = v2;
        v2 = v3;

    }
    public void ExchangeTwoValue(ref float v1, ref float v2)
    {
        float v3 = v1;
        v1 = v2;
        v2 = v3;

    }

    #endregion
}

public delegate void AnimCallBack(params object[] arg);

//动画类型
public enum AnimType
{
    LocalPosition,
    Position,
    UGUI_alpha,
    UGUI_anchoredPosition,
    LocalScale,
    SizeDetal,

}

//插值算法类型
public enum InteType
{
    Default,
    Linear,
    InBack,
    outBack,
    inOutBack,
    outInBack,
    inQuad,
    outQuad,
    inoutQuad,
    inCubic,
    outCubic,
    inoutCubic,
    outInCubic,
    inQuart,
    outQuart,
    inOutQuart,
    outInQuart,
    inQuint,
    outQuint,
    inOutQuint,
    outInQuint,
    inSine,
    outSine,
    inOutSine,
    outInSine,
    inExpo,
    outExpo,
    inOutExpo,
    outInExpo,
}

public enum PathType
{
    Line,
    Bezier2,
    Bezier3,

}

public enum PlayType
{
    Once,
    Loop,
    PingPang,

}
