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
        }

        return instance;
    }

    public static void uguiAlpha(GameObject animObject, float fromAlpha, float toAlpha, float time, InteType interpolationType = InteType.Default, bool isChild = true, AnimCallBack callBack = null, params object[] parameter)
    {
        AnimData DataTmp = getAnimData(animObject, time, interpolationType, callBack, parameter);

        DataTmp.animType = AnimType.UGUI_alpha;
        DataTmp.formAlpha = fromAlpha;
        DataTmp.toAlpha = toAlpha;
        DataTmp.UguiAlphaInit(isChild);

        getInstance().animList.Add(DataTmp);
    }

    public static void UguiMove(GameObject animObject, Vector3 from, Vector3 to, float time, InteType interpolationType = InteType.Default, AnimCallBack callBack = null, params object[] parameter)
    {
        AnimData DataTmp = getAnimData(animObject, time, interpolationType, callBack, parameter);

        DataTmp.animType = AnimType.UGUI_anchoredPosition;
        DataTmp.formPos = from;
        DataTmp.toPos = to;
        DataTmp.UguiPositionInit();

        getInstance().animList.Add(DataTmp);
    }



    public static void Move(GameObject animObject, Vector3 from, Vector3 to, float time, InteType interpolationType = InteType.Default, bool isLocal = true, AnimCallBack callBack = null, params object[] parameter)
    {
        AnimData DataTmp = getAnimData(animObject, time, interpolationType, callBack, parameter);

        if (isLocal)
        {
            DataTmp.animType = AnimType.LocalPosition;
        }
        else
        {
            DataTmp.animType = AnimType.Position;
        }


        DataTmp.formPos = from;
        DataTmp.toPos = to;
        DataTmp.TransfromInit();

        getInstance().animList.Add(DataTmp);
    }

    #region 贝塞尔移动方法群
    public static void BezierMove(GameObject animObject, Vector3 from, Vector3 to, float time, InteType interpolationType = InteType.Default, bool isLocal = true, BezierType bezierMoveType = BezierType.Bezier2, AnimCallBack callBack = null, Vector3[] t_Bezier_contral = null, params object[] parameter)
    {
        AnimData DataTmp = getAnimData(animObject, time, interpolationType, callBack, parameter);

        if (isLocal)
        {
            DataTmp.animType = AnimType.LocalPosition;
        }
        else
        {
            DataTmp.animType = AnimType.Position;
        }
        DataTmp.bezierType = bezierMoveType;
        DataTmp.BezierContral = t_Bezier_contral;
        DataTmp.formPos = from;
        DataTmp.toPos = to;
        DataTmp.TransfromInit();

        getInstance().animList.Add(DataTmp);

    }

    public static void BezierMove(GameObject animObject, Vector3 from, Vector3 to, float time, InteType interpolationType = InteType.Default, bool isLocal = true, BezierType bezierMoveType = BezierType.Bezier2, AnimCallBack callBack = null, float[] t_Bezier_contralRadius = null, params object[] parameter)
    {

        //三阶贝塞尔，两个控制点分别以起点和终点为中心进行随机
        Vector3[] t_Bezier_contral = new Vector3[3];
        t_Bezier_contral[0] = UnityEngine.Random.insideUnitSphere * t_Bezier_contralRadius[0] + from;
        t_Bezier_contral[1] = UnityEngine.Random.insideUnitSphere * t_Bezier_contralRadius[1] + to;

        //二阶贝塞尔，控制点以起点与终点的中间为随机中心
        if (bezierMoveType == BezierType.Bezier2)
        {
            t_Bezier_contral[0] = UnityEngine.Random.insideUnitSphere * t_Bezier_contralRadius[0] + (from + to) * 0.5f;
        }

        //如果在平面内（z轴相同），控制点也要在该平面内
        if (from.z == to.z)
        {
            t_Bezier_contral[0].z = from.z;
            t_Bezier_contral[1].z = from.z;
        }


        BezierMove(animObject, from, to, time, interpolationType, isLocal, bezierMoveType, callBack, t_Bezier_contral, parameter);

    }
    public static void BezierMove(GameObject animObject, Vector3 to, float time, InteType interpolationType = InteType.Default, bool isLocal = true, BezierType bezierMoveType = BezierType.Bezier2, AnimCallBack callBack = null, float[] t_Bezier_contralRadius = null, params object[] parameter)
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

        BezierMove(animObject, from, to, time, interpolationType, isLocal, bezierMoveType, callBack, t_Bezier_contralRadius, parameter);
    }


    #endregion

    public static void Scale(GameObject animObject, Vector3 from, Vector3 to, float time, InteType interpolationType = InteType.Default, bool isLocal = true, AnimCallBack callBack = null, params object[] parameter)
    {
        AnimData DataTmp = getAnimData(animObject, time, interpolationType, callBack, parameter);


        DataTmp.animType = AnimType.LocalScale;


        DataTmp.formPos = from;
        DataTmp.toPos = to;
        DataTmp.TransfromInit();

        getInstance().animList.Add(DataTmp);
    }

    public static void SizeDelta(GameObject animObject, Vector3 from, Vector3 to, float time, InteType interpolationType = InteType.Default, bool isLocal = true, AnimCallBack callBack = null, params object[] parameter)
    {
        AnimData DataTmp = getAnimData(animObject, time, interpolationType, callBack, parameter);


        DataTmp.animType = AnimType.SizeDetal;


        DataTmp.formPos = from;
        DataTmp.toPos = to;
        DataTmp.TransfromInit();

        getInstance().animList.Add(DataTmp);
    }

    static AnimData getAnimData(GameObject animObject, float time, InteType interpolationType = InteType.Default, AnimCallBack callBack = null, params object[] parameter)
    {
        AnimData DataTmp = new AnimData();

        if (interpolationType != InteType.Default)
        {
            DataTmp.interpolationType = interpolationType;
        }
        else
        {
            DataTmp.interpolationType = InteType.Linear;
        }

        DataTmp.animGameObejct = animObject;
        DataTmp.currentTime = 0;
        DataTmp.totalTime = time;
        DataTmp.callBack = callBack;
        DataTmp.parameter = parameter;

        return DataTmp;
    }

    public static void ValueTo(GameObject l_gameObject, params object[] l_params)
    {
        AnimData DataTmp = new AnimData();

        for (int i = 0; i < l_params.Length; i += 2)
        {
            AnimParamType l_ParamType = (AnimParamType)l_params[i];
            switch (l_ParamType)
            {
                case AnimParamType.FromV3: DataTmp.formPos = (Vector3)l_params[i + 1]; break;
                case AnimParamType.FromV2: DataTmp.formPos = (Vector2)l_params[i + 1]; break;
                case AnimParamType.FromColor: DataTmp.formPos = (Color)l_params[i + 1]; break;
                case AnimParamType.FromFloat: DataTmp.formPos = (float)l_params[i + 1]; break;

                case AnimParamType.ToV3: DataTmp.formPos = (Vector3)l_params[i + 1]; break;
                case AnimParamType.ToV2: DataTmp.formPos = (Vector3)l_params[i + 1]; break;
                case AnimParamType.ToFloat: DataTmp.formPos = (Vector3)l_params[i + 1]; break;
                case AnimParamType.ToColor: DataTmp.formPos = (Vector3)l_params[i + 1]; break;

                case AnimParamType.AnimType: DataTmp.formPos = (Vector3)l_params[i + 1]; break;
                case AnimParamType.Time: DataTmp.formPos = (Vector3)l_params[i + 1]; break;
                case AnimParamType.InteType: DataTmp.formPos = (Vector3)l_params[i + 1]; break;
                case AnimParamType.RoadType: DataTmp.formPos = (Vector3)l_params[i + 1]; break;







            }
        }
    }

    public enum AnimParamType
    {
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
        RoadType,

        RepeatType,

        IsChild,
        IsLocal,

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

                animList.RemoveAt(i);
                i--;
            }
        }
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

public enum BezierType
{
    Bezier1,
    Bezier2,
    Bezier3,

}
