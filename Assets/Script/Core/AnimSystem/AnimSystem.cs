using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    UGUI_anchoredPosition
}

//插值算法类型
public enum InteType
{
    Default,
    Linear,
    InBack,
}
