using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimSystem : MonoBehaviour
{

    #region 静态部分

    static AnimSystem instance;

    public static AnimSystem getInstance()
    {
        if(instance == null)
        {
            GameObject animGameObject = new GameObject();
            animGameObject.name = "AnimSystem";
            instance = animGameObject.AddComponent<AnimSystem>();
        }

        return instance;
    }

    public static void uguiAlpha(GameObject animObject,float fromAlpha,float toAlpha,float time,InteType interpolationType ,AnimCallBack callBack = null,bool isChild = true)
    {
        AnimData DataTmp = getAnimData(animObject,  time,  interpolationType,callBack );

        DataTmp.animType = AnimType.UGUI_alpha;
        DataTmp.formAlpha = fromAlpha;
        DataTmp.toAlpha   = toAlpha;
        DataTmp.UguiAlphaInit(isChild);

        getInstance().animList.Add(DataTmp);
    }



    public static void UguiMove(GameObject animObject, Vector3 from, Vector3 to, float time, InteType interpolationType, AnimCallBack callBack = null)
    {
        AnimData DataTmp = getAnimData(animObject, time, interpolationType, callBack);

        DataTmp.animType = AnimType.UGUI_anchoredPosition;
        DataTmp.formPos = from;
        DataTmp.toPos = to;
        DataTmp.UguiPositionInit();

        getInstance().animList.Add(DataTmp);
    }

    static AnimData getAnimData(GameObject animObject, float time, InteType interpolationType, AnimCallBack callBack = null)
    {
        AnimData DataTmp = new AnimData();

        DataTmp.interpolationType = interpolationType;
        DataTmp.animGameObejct = animObject;
        DataTmp.currentTime = 0;
        DataTmp.totalTime = time;
        DataTmp.callBack = callBack;

        return DataTmp;
    }

    #endregion

    #region 实例部分

    public List<AnimData> animList = new List<AnimData>();
	
	// Update is called once per frame
	void Update () 
    {
        for (int i = 0; i < animList.Count;i++ )
        {
            //执行Update
            animList[i].executeUpdate();

            if(animList[i].isDone == true)
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
    UGUI_alpha,
    UGUI_anchoredPosition
}

//插值算法类型
public enum InteType
{
    linear
}
