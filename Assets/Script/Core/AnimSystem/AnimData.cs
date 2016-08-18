using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class AnimData
{
    public AnimType animType;
    public InteType interpolationType;
    public PathType pathType = PathType.Line;

    public GameObject animGameObejct;
    public bool b_isChild = false;
    public float currentTime = 0;
    public float totalTime = 0;
    public Vector3[] v3Contral = null; //二阶取第一个用，三阶取前两个
    public float[] floatContral = null;

    public bool isDone = false;

    public object[] parameter;
    public AnimCallBack callBack;





    public Vector3 formPos;
    public Vector3 toPos;

    public Vector2 fromV2;
    public Vector2 toV2;

    RectTransform rectRransform;
    Transform transform;



    public void executeUpdate()
    {
        currentTime += Time.deltaTime;

        if (currentTime > totalTime)
        {
            currentTime = totalTime;
            isDone = true;
        }


        switch (animType)
        {
            case AnimType.UGUI_alpha: UguiAlpha(); break;
            case AnimType.UGUI_anchoredPosition: UguiPosition(); break;
            case AnimType.Position: Position(); break;
            case AnimType.LocalPosition: LocalPosition(); break;
            case AnimType.LocalScale: LocalScale(); break;
            case AnimType.SizeDetal: SizeDelta(); break;
        }

    }

    public void executeCallBack()
    {
        try
        {
            if (callBack != null)
            {
                callBack(parameter);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }

    float getInterpolation(float oldValue, float aimValue)
    {
        switch (interpolationType)
        {
            case InteType.Default:
            case InteType.Linear: return Mathf.Lerp(oldValue, aimValue, currentTime / totalTime);
            case InteType.InBack: return InBack(oldValue, aimValue, currentTime, totalTime);
            case InteType.outBack: return OutBack(oldValue, aimValue, currentTime, totalTime);
            case InteType.inOutBack: return InOutBack(oldValue, aimValue, currentTime, totalTime);
            case InteType.outInBack: return OutInBack(oldValue, aimValue, currentTime, totalTime);
            case InteType.inQuad: return InQuad(oldValue, aimValue, currentTime, totalTime);
            case InteType.outQuad: return OutQuad(oldValue, aimValue, currentTime, totalTime);
            case InteType.inoutQuad: return InoutQuad(oldValue, aimValue, currentTime, totalTime);
            case InteType.inCubic: return InCubic(oldValue, aimValue, currentTime, totalTime);
            case InteType.outCubic: return OutCubic(oldValue, aimValue, currentTime, totalTime);
            case InteType.inoutCubic: return InoutCubic(oldValue, aimValue, currentTime, totalTime);
            case InteType.inQuart: return InQuart(oldValue, aimValue, currentTime, totalTime);
            case InteType.outQuart: return OutQuart(oldValue, aimValue, currentTime, totalTime);
            case InteType.inOutQuart: return InOutQuart(oldValue, aimValue, currentTime, totalTime);
            case InteType.outInQuart: return OutInQuart(oldValue, aimValue, currentTime, totalTime);
            case InteType.inQuint: return InQuint(oldValue, aimValue, currentTime, totalTime);
            case InteType.outQuint: return OutQuint(oldValue, aimValue, currentTime, totalTime);
            case InteType.inOutQuint: return InOutQuint(oldValue, aimValue, currentTime, totalTime);
            case InteType.outInQuint: return OutInQuint(oldValue, aimValue, currentTime, totalTime);
            case InteType.inSine: return InSine(oldValue, aimValue, currentTime, totalTime);
            case InteType.outSine: return OutSine(oldValue, aimValue, currentTime, totalTime);
            case InteType.inOutSine: return InOutSine(oldValue, aimValue, currentTime, totalTime);
            case InteType.outInSine: return OutInSine(oldValue, aimValue, currentTime, totalTime);
            case InteType.inExpo: return InExpo(oldValue, aimValue, currentTime, totalTime);
            case InteType.outExpo: return OutExpo(oldValue, aimValue, currentTime, totalTime);
            case InteType.inOutExpo: return InOutExpo(oldValue, aimValue, currentTime, totalTime);
            case InteType.outInExpo: return OutInExpo(oldValue, aimValue, currentTime, totalTime);

        }

        return 0;
    }

    Vector3 getInterpolationV3(Vector3 oldValue, Vector3 aimValue)
    {

        Vector3 result = new Vector3(
            getInterpolation(oldValue.x, aimValue.x),
            getInterpolation(oldValue.y, aimValue.y),
            getInterpolation(oldValue.z, aimValue.z)
        );

        if (pathType != PathType.Line)
        {
            result = getBezierInterpolationV3(oldValue, aimValue);
        }

        return result;


    }


    /// <summary>
    /// 贝塞尔初始化（根据随机范围进行控制点随机）
    /// </summary>
    public void bezierInit()
    {
        if (v3Contral == null)
        {
            if (floatContral != null)
            {
                v3Contral = new Vector3[3];
                v3Contral[0] = UnityEngine.Random.insideUnitSphere * floatContral[0] + formPos;
                v3Contral[1] = UnityEngine.Random.insideUnitSphere * floatContral[1] + toPos;

                //二阶贝塞尔，控制点以起点与终点的中间为随机中心
                if (pathType == PathType.Bezier2)
                {
                    v3Contral[0] = UnityEngine.Random.insideUnitSphere * floatContral[0] + (formPos + toPos) * 0.5f;
                }
            }
            else
            {
                Debug.LogError("bezierInit(): v3Contral && floatContral == null");
            }
        }

        //如果在平面内（z轴相同），控制点也要在该平面内
        if (formPos.z == toPos.z)
        {
            v3Contral[0].z = formPos.z;
            v3Contral[1].z = toPos.z;
        }
    }
    /// <summary>
    /// 贝塞尔专用算法
    /// </summary>
    Vector3 getBezierInterpolationV3(Vector3 oldValue, Vector3 aimValue)
    {

        Vector3 result = new Vector3(
            getInterpolation(oldValue.x, aimValue.x),
            0,
            0
        );
        float n_finishingRate = (result.x - oldValue.x) / (aimValue.x - oldValue.x);
        n_finishingRate = Mathf.Clamp(n_finishingRate, -1, 2);

        switch (pathType)
        {
            case PathType.Bezier2: return Bezier2(oldValue, aimValue, n_finishingRate, v3Contral);
            case PathType.Bezier3: return Bezier3(oldValue, aimValue, n_finishingRate, v3Contral);
            default: return Vector3.zero;
        }

    }

    /// <summary>
    /// 二阶贝塞尔曲线函数
    /// </summary>

    Vector3 Bezier2(Vector3 startPos, Vector3 endPos, float n_time, Vector3[] v3_ControlPoint)
    {
        return (1 - n_time) * (1 - n_time) * startPos + 2 * (1 - n_time) * n_time * v3_ControlPoint[0] + n_time * n_time * endPos;
    }

    /// <summary>
    /// 三阶贝塞尔曲线函数
    /// </summary>

    Vector3 Bezier3(Vector3 startPos, Vector3 endPos, float n_time, Vector3[] t_ControlPoint)
    {
        return (1 - n_time) * (1 - n_time) * (1 - n_time) * startPos + 3 * (1 - n_time) * (1 - n_time) * n_time * t_ControlPoint[0] + 3 * (1 - n_time) * n_time * n_time * t_ControlPoint[1] + n_time * n_time * n_time * endPos;
    }



    public void Init()
    {
        switch (animType)
        {
            case AnimType.UGUI_alpha: UguiAlphaInit(b_isChild); break;
            case AnimType.UGUI_anchoredPosition: UguiPositionInit(); break;
            case AnimType.SizeDetal: UguiPositionInit(); break;
            case AnimType.Position: TransfromInit(); break;
            case AnimType.LocalPosition: TransfromInit(); break;
            case AnimType.LocalScale: TransfromInit(); break;
        }

        if (pathType != PathType.Line)
        {
            bezierInit();
        }



    }

    #region UGUI



    #region UGUI_alpha

    List<Image> animObjectList_Image = new List<Image>();
    List<Text> animObjectList_Text = new List<Text>();

    List<Color> oldColor = new List<Color>();

    public float formAlpha = 0;
    public float toAlpha = 0;

    public void UguiAlphaInit(bool isChild)
    {
        animObjectList_Image = new List<Image>();
        oldColor = new List<Color>();

        if (isChild)
        {
            Image[] images = animGameObejct.GetComponentsInChildren<Image>();

            for (int i = 0; i < images.Length; i++)
            {
                animObjectList_Image.Add(images[i]);
                oldColor.Add(images[i].color);
            }

            Text[] texts = animGameObejct.GetComponentsInChildren<Text>();

            for (int i = 0; i < texts.Length; i++)
            {
                animObjectList_Text.Add(texts[i]);
                oldColor.Add(texts[i].color);
            }
        }
        else
        {
            animObjectList_Image.Add(animGameObejct.GetComponent<Image>());
            oldColor.Add(animGameObejct.GetComponent<Image>().color);
        }

        setUGUIAlpha(formAlpha);
    }

    void UguiAlpha()
    {
        //Debug.Log("UguiAlpha " + currentTime +"  " + totalTime);

        setUGUIAlpha(getInterpolation(formAlpha, toAlpha));
    }

    public void setUGUIAlpha(float a)
    {
        Color newColor = new Color();

        int index = 0;
        for (int i = 0; i < animObjectList_Image.Count; i++)
        {
            newColor = oldColor[index];
            newColor.a = a;
            animObjectList_Image[i].color = newColor;

            index++;
        }

        for (int i = 0; i < animObjectList_Text.Count; i++)
        {
            newColor = oldColor[index];
            newColor.a = a;
            animObjectList_Text[i].color = newColor;

            index++;
        }
    }



    void SizeDelta()
    {
        if (rectRransform == null)
        {
            Debug.LogError(transform.name + "缺少RectTransform组件，不能进行sizeDelta变换！！");
            return;
        }
        rectRransform.sizeDelta = getInterpolationV3(fromV2, toV2);
    }

    #endregion

    #region UGUI_position

    public void UguiPositionInit()
    {
        rectRransform = animGameObejct.GetComponent<RectTransform>();
    }

    void UguiPosition()
    {
        rectRransform.anchoredPosition3D = getInterpolationV3(formPos, toPos);
    }



    #endregion
    #endregion

    #region Transfrom


    public void TransfromInit()
    {
        transform = animGameObejct.transform;
    }

    void Position()
    {
        transform.position = getInterpolationV3(formPos, toPos);
    }

    void LocalPosition()
    {
        transform.localPosition = getInterpolationV3(formPos, toPos);
    }

    void LocalScale()
    {
        transform.localScale = getInterpolationV3(formPos, toPos);
    }





    #endregion

    #region color
    public Color fromColor;
    public Color toColor;


    #endregion



    #region 插值算法

    public float InBack(float b, float to, float t, float d)
    {
        float s = 1.70158f;
        float c = to - b;
        t = t / d;

        return c * t * t * ((s + 1) * t - s) + b;
    }

    public float OutBack(float b, float to, float t, float d, float s = 1.70158f)
    {
        float c = to - b;

        t = t / d - 1;

        return c * (t * t * ((s + 1) * t + s) + 1) + b;

    }

    public float InOutBack(float b, float to, float t, float d, float s = 1.70158f)
    {
        float c = to - b;
        s = s * 1.525f;
        t = t / d * 2;
        if (t < 1)
            return c / 2 * (t * t * ((s + 1) * t - s)) + b;
        else
        {
            t = t - 2;
            return c / 2 * (t * t * ((s + 1) * t + s) + 2) + b;
        }

    }

    public float OutInBack(float b, float to, float t, float d, float s = 1.70158f)
    {
        float c = to - b;
        if (t < d / 2)
        {
            t *= 2;
            c *= 0.5f;
            t = t / d * 2;

            t = t / d - 1;
            return c * (t * t * ((s + 1) * t + s) + 1) + b;

        }

        else
        {
            t = t * 2 - d;
            b += c * 0.5f;
            c *= 0.5f;


            if (t < 1)
                return c / 2 * (t * t * ((s + 1) * t - s)) + b;
            else
            {
                t = t - 2;
                return c / 2 * (t * t * ((s + 1) * t + s) + 2) + b;
            }
        }


    }

    public float InQuad(float b, float to, float t, float d)
    {
        float c = to - b;
        t = t / d;
        return (float)(c * Math.Pow(t, 2) + b);
    }

    public float OutQuad(float b, float to, float t, float d)
    {
        float c = to - b;
        t = t / d;
        return (float)(-c * t * (t - 2) + b);
    }

    public float InoutQuad(float b, float to, float t, float d)
    {
        float c = to - b;
        t = t / d * 2;
        if (t < 1)
            return (float)(c / 2 * Math.Pow(t, 2) + b);
        else
            return -c / 2 * ((t - 1) * (t - 3) - 1) + b;

    }
    public float InCubic(float b, float to, float t, float d)
    {
        float c = to - b;
        t = t / d;
        return (float)(c * Math.Pow(t, 3) + b);

    }
    public float OutCubic(float b, float to, float t, float d)
    {
        float c = to - b;
        t = t / d - 1;
        return (float)(c * (Math.Pow(t, 3) + 1) + b);

    }
    public float InoutCubic(float b, float to, float t, float d)
    {
        float c = to - b;
        t = t / d * 2;
        if (t < 1)
            return c / 2 * t * t * t + b;
        else
        {
            t = t - 2;
            return c / 2 * (t * t * t + 2) + b;
        }
    }
    public float InQuart(float b, float to, float t, float d)
    {
        float c = to - b;
        t = t / d;
        return (float)(c * Math.Pow(t, 4) + b);

    }
    public float OutQuart(float b, float to, float t, float d)
    {
        float c = to - b;
        t = t / d - 1;
        return (float)(-c * (Math.Pow(t, 4) - 1) + b);

    }

    public float InOutQuart(float b, float to, float t, float d)
    {
        float c = to - b;
        t = t / d * 2;
        if (t < 1)
            return (float)(c / 2 * Math.Pow(t, 4) + b);
        else
        {
            t = t - 2;
            return (float)(-c / 2 * (Math.Pow(t, 4) - 2) + b);
        }

    }
    public float OutInQuart(float b, float to, float t, float d)
    {
        if (t < d / 2)
        {
            float c = to - b;
            t *= 2;
            c *= 0.5f;
            t = t / d - 1;

            return (float)(-c * (Math.Pow(t, 4) - 1) + b);
        }
        else
        {
            float c = to - b;
            t = t * 2 - d;
            b = b + c * 0.5f;
            c *= 0.5f;
            t = t / d;


            return (float)(c * Math.Pow(t, 4) + b);

        }
    }

    public float InQuint(float b, float to, float t, float d)
    {
        float c = to - b;
        t = t / d;
        return (float)(c * Math.Pow(t, 5) + b);

    }

    public float OutQuint(float b, float to, float t, float d)
    {
        float c = to - b;
        t = t / d - 1;
        return (float)(c * (Math.Pow(t, 5) + 1) + b);
    }

    public float InOutQuint(float b, float to, float t, float d)
    {
        float c = to - b;
        t = t / d * 2;
        if (t < 1)
            return (float)(c / 2 * Math.Pow(t, 5) + b);
        else
        {
            t = t - 2;
            return (float)(c / 2 * (Math.Pow(t, 5) + 2) + b);

        }

    }

    public float OutInQuint(float b, float to, float t, float d)
    {
        float c = to - b;
        if (t < d / 2)
        {
            t *= 2;
            c *= 0.5f;
            t = t / d - 1;
            return (float)(c * (Math.Pow(t, 5) + 1) + b);
        }
        else
        {
            t = t * 2 - d;
            b = b + c * 0.5f;
            c *= 0.5f;

            t = t / d;
            return (float)(c * Math.Pow(t, 5) + b);
        }
    }

    public float InSine(float b, float to, float t, float d)
    {
        float c = to - b;
        return (float)(-c * Math.Cos(t / d * (Math.PI / 2)) + c + b);

    }

    public float OutSine(float b, float to, float t, float d)
    {
        float c = to - b;
        return (float)(c * Math.Sin(t / d * (Math.PI / 2)) + b);
    }

    public float InOutSine(float b, float to, float t, float d)
    {
        float c = to - b;
        return (float)(-c / 2 * (Math.Cos(Math.PI * t / d) - 1) + b);

    }
    public float OutInSine(float b, float to, float t, float d)
    {
        float c = to - b;
        if (t < d / 2)
        {
            t *= 2;
            c *= 0.5f;
            return (float)(c * Math.Sin(t / d * (Math.PI / 2)) + b);
        }
        else
        {
            t = t * 2 - d;
            b += c * 0.5f;
            c *= 0.5f;
            return (float)(-c * Math.Cos(t / d * (Math.PI / 2)) + c + b);

        }
    }
    public float InExpo(float b, float to, float t, float d)
    {
        float c = to - b;
        if (t == 0)
            return b;
        else
            return (float)(c * Math.Pow(2, 10 * (t / d - 1)) + b - c * 0.001f);
    }
    public float OutExpo(float b, float to, float t, float d)
    {
        float c = to - b;
        if (t == d)
            return b + c;
        else
            return (float)(c * 1.001 * (-Math.Pow(2, -10 * t / d) + 1) + b);

    }
    public float InOutExpo(float b, float to, float t, float d)
    {
        float c = to - b;
        if (t == 0)
            return b;
        if (t == d)
            return (b + c);

        t = t / d * 2;

        if (t < 1)
            return (float)(c / 2 * Math.Pow(2, 10 * (t - 1)) + b - c * 0.0005f);
        else
        {
            t = t - 1;
            return (float)(c / 2 * 1.0005 * (-Math.Pow(2, -10 * t) + 2) + b);

        }


    }

    public float OutInExpo(float b, float to, float t, float d)
    {
        float c = to - b;
        if (t < d / 2)
        {
            t *= 2;
            c *= 0.5f;
            if (t == d)
                return b + c;
            else
                return (float)(c * 1.001 * (-Math.Pow(2, -10 * t / d) + 1) + b);
        }
        else
        {
            t = t * 2 - d;
            b += c * 0.5f;
            c *= 0.5f;
            if (t == 0)
                return b;
            else
                return (float)(c * Math.Pow(2, 10 * (t / d - 1)) + b - c * 0.001f);

        }
    }




    //outInExpo,
    //inBack,
    //outBack,
    //inOutBack,
    //outInBack,




    #endregion
}


