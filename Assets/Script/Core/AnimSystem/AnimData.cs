using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class AnimData
{
    #region 参数

    //基本变量
    public GameObject m_animGameObejct;
    public AnimType m_animType;
    public InterpType m_interpolationType = InterpType.Default;
    public PathType m_pathType = PathType.Line;
    public RepeatType m_repeatType = RepeatType.Once;

    public bool m_ignoreTimeScale = false;

    //进度控制变量
    public float m_delayTime = 0;
    public bool m_isDone = false;
    public float m_currentTime = 0;
    public float m_totalTime = 0;
    public int m_repeatCount = -1;

    //Q4
    public Quaternion m_fromQ4;
    public Quaternion m_toQ4;

    //V3
    public Vector4 m_fromV4;
    public Vector4 m_toV4;

    //V3
    public Vector3 m_fromV3;
    public Vector3 m_toV3;

    //V2
    public Vector2 m_fromV2;
    public Vector2 m_toV2;

    //Float
    public float m_fromFloat = 0;
    public float m_toFloat = 0;

    //Move
    public Transform m_toTransform;

    //Color
    public Color m_fromColor;
    public Color m_toColor;
    List<Color> m_oldColor = new List<Color>();

    //动画回调
    public object[] m_parameter;
    public AnimCallBack m_callBack;

    //闪烁
    public float m_space = 0;
    float m_timer = 0;

    //其他设置
    public bool m_isChild = false;
    public bool m_isLocal = false;

    //控制点
    public Vector3[] m_v3Contral = null; //二阶取第一个用，三阶取前两个
    public float[] m_floatContral = null;

    //自定义函数
    public AnimCustomMethodVector4 m_customMethodV4;
    public AnimCustomMethodVector3 m_customMethodV3;
    public AnimCustomMethodVector2 m_customMethodV2;
    public AnimCustomMethodFloat m_customMethodFloat;

    //缓存变量
    RectTransform m_rectRransform;
    Transform m_transform;
    string gameObjectName;

    #endregion

    #region 核心函数

    public bool ExecuteUpdate()
    {
        if (m_delayTime <= 0)
        {
            if (m_ignoreTimeScale)
            {
                m_currentTime += Time.unscaledDeltaTime;
            }
            else
            {
                m_currentTime += Time.deltaTime;
            }
        }
        else
        {
            if (m_ignoreTimeScale)
            {
                m_delayTime -= Time.unscaledDeltaTime;
            }
            else
            {
                m_delayTime -= Time.deltaTime;
            }
        }

        if (m_currentTime >= m_totalTime)
        {
            m_currentTime = m_totalTime;
            m_isDone = true;
        }

        try
        {
            switch (m_animType)
            {
                case AnimType.UGUI_Color: UguiColor(); break;
                case AnimType.UGUI_Alpha: UguiAlpha(); break;
                case AnimType.UGUI_AnchoredPosition: UguiPosition(); break;
                case AnimType.UGUI_SizeDetal: SizeDelta(); break;

                case AnimType.Position: Position(); break;
                case AnimType.LocalPosition: LocalPosition(); break;
                case AnimType.LocalScale: LocalScale(); break;
                case AnimType.LocalRotate: LocalRotate(); break;
                case AnimType.Rotate: Rotate(); break;
                case AnimType.LocalRotation:LocalRotation();break;
                case AnimType.Rotation:Rotation();break;

                case AnimType.Color: UpdateColor(); break;
                case AnimType.Alpha: UpdateAlpha(); break;
                    
                case AnimType.Custom_Vector4: CustomMethodVector4(); break;
                case AnimType.Custom_Vector3: CustomMethodVector3(); break;
                case AnimType.Custom_Vector2: CustomMethodVector2(); break;
                case AnimType.Custom_Float: CustomMethodFloat(); break;

                case AnimType.Blink: Blink(); break;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("AnimSystem Error ! GameObjectName: " + gameObjectName + " Exception: " + e.ToString());
            return true;
        }
        return false;
    }

    //动画播放完毕执行回调
    public void ExecuteCallBack()
    {
        try
        {
            if (m_callBack != null)
            {
                m_callBack(m_parameter);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }

    /// <summary>
    /// 动画循环逻辑
    /// </summary>

    public bool AnimReplayLogic()
    {
        bool result = false;

        switch (m_repeatType)
        {
            case RepeatType.Once:
                return false;

            case RepeatType.Loop:
                m_isDone = false;
                m_currentTime = 0;
                break;
            case RepeatType.PingPang:

                ExchangeV2();
                ExchangeAlpha();
                ExchangePos();
                m_isDone = false;
                m_currentTime = 0;
                break;
        }

        if (m_repeatCount == -1)
        {
            result = true;
        }
        else
        {
            m_repeatCount--;
            result = (m_repeatCount > 0);
        }

        return result;
    }

    #region 循环逻辑

    public void ExchangeV2()
    {
        Vector2 Vtmp = m_fromV2;
        m_fromV2 = m_toV2;
        m_toV2 = Vtmp;

    }
    public void ExchangePos()
    {
        Vector3 Vtmp = m_fromV3;
        m_fromV3 = m_toV3;
        m_toV3 = Vtmp;

    }
    public void ExchangeAlpha()
    {
        float alphaTmp = m_fromFloat;
        m_fromFloat = m_toFloat;
        m_toFloat = alphaTmp;
    }

    #endregion

    #endregion

    #region 初始化

    public void Init()
    {
        if (m_animGameObejct != null)
        {
            gameObjectName = m_animGameObejct.name;
        }
        else
        {
            gameObjectName = "customMethod";
        }


        switch (m_animType)
        {
            case AnimType.UGUI_Color: UguiColorInit(m_isChild); break;
            case AnimType.UGUI_Alpha: UguiAlphaInit(m_isChild); break;
            case AnimType.UGUI_AnchoredPosition: UguiPositionInit(); break;
            case AnimType.UGUI_SizeDetal: UguiPositionInit(); break;

            case AnimType.Color: ColorInit(m_isChild); break;
            case AnimType.Alpha: AlphaInit(m_isChild); break;

            case AnimType.Position: TransfromInit(); break;
            case AnimType.LocalPosition: TransfromInit(); break;
            case AnimType.LocalScale: TransfromInit(); break;
            case AnimType.LocalRotate: TransfromInit(); break;
            case AnimType.Rotate: TransfromInit(); break;
            case AnimType.LocalRotation: TransfromInit(); break;
            case AnimType.Rotation: TransfromInit(); break;

        }

        if (m_pathType != PathType.Line)
        {
            BezierInit();
        }
    }

    public void OnInit() { }

    #endregion

    #region CustomMethod

    public void CustomMethodFloat()
    {
        try
        {
            m_customMethodFloat(GetInterpolation(m_fromFloat, m_toFloat));
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }

    public void CustomMethodVector2()
    {
        try
        {
            m_customMethodV2(GetInterpolationV3(m_fromV2, m_toV2));
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }

    public void CustomMethodVector3()
    {
        try
        {
            m_customMethodV3(GetInterpolationV3(m_fromV3, m_toV3));
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }

    public void CustomMethodVector4()
    {
        try
        {
            m_customMethodV4(GetInterpolationV4(m_fromV4, m_toV4));
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }

    #endregion

    #region 贝塞尔曲线

    /// <summary>
    /// 贝塞尔初始化（根据随机范围进行控制点随机）
    /// </summary>
    public void BezierInit()
    {
        if (m_v3Contral == null)
        {
            if (m_floatContral != null)
            {
                m_v3Contral = new Vector3[3];
                m_v3Contral[0] = UnityEngine.Random.insideUnitSphere * m_floatContral[0] + m_fromV3;
                m_v3Contral[1] = UnityEngine.Random.insideUnitSphere * m_floatContral[1] + m_toV3;

                //二阶贝塞尔，控制点以起点与终点的中间为随机中心
                if (m_pathType == PathType.Bezier2)
                {
                    m_v3Contral[0] = UnityEngine.Random.insideUnitSphere * m_floatContral[0] + (m_fromV3 + m_toV3) * 0.5f;
                }
            }
            else
            {
                Debug.LogError("bezierInit(): v3Contral && floatContral == null");
            }
        }

        //如果在平面内（z轴相同），控制点也要在该平面内
        if (m_fromV3.z == m_toV3.z)
        {
            m_v3Contral[0].z = m_fromV3.z;
            if (m_v3Contral.Length > 1)
            {
                m_v3Contral[1].z = m_toV3.z;
            }

        }
    }
    /// <summary>
    /// 贝塞尔专用算法
    /// </summary>
    Vector3 GetBezierInterpolationV3(Vector3 oldValue, Vector3 aimValue)
    {
        float n_finishingRate = m_currentTime / m_totalTime;

        switch (m_pathType)
        {
            case PathType.Bezier2: return Bezier2(oldValue, aimValue, n_finishingRate, m_v3Contral);
            case PathType.Bezier3: return Bezier3(oldValue, aimValue, n_finishingRate, m_v3Contral);
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
        return (1 - n_time) * (1 - n_time) * (1 - n_time) * startPos
            + 3 * (1 - n_time) * (1 - n_time) * n_time * t_ControlPoint[0]
            + 3 * (1 - n_time) * n_time * n_time * t_ControlPoint[1]
            + n_time * n_time * n_time * endPos;
    }

    #endregion

    #region UGUI

    #region UGUI_Color

    List<Graphic> m_graphicList_Image = new List<Graphic>();
    //List<RawImage> m_rawImageList_Text = new List<RawImage>();
    //List<Text> m_animObjectList_Text = new List<Text>();

    #region ALpha

    public void UguiAlphaInit(bool isChild)
    {
        m_graphicList_Image.Clear();
        m_oldColor.Clear();

        if (isChild)
        {
            Graphic[] images = m_animGameObejct.GetComponentsInChildren<Graphic>(true);
            for (int i = 0; i < images.Length; i++)
            {
                if (images[i].transform.GetComponent<Mask>() == null)
                {
                    m_graphicList_Image.Add(images[i]);
                    m_oldColor.Add(images[i].color);
                }
            }

            //Text[] texts = m_animGameObejct.GetComponentsInChildren<Text>(true);

            //for (int i = 0; i < texts.Length; i++)
            //{
            //    m_animObjectList_Text.Add(texts[i]);
            //    m_oldColor.Add(texts[i].color);
            //}
        }
        else
        {
            Graphic gra = m_animGameObejct.GetComponent<Graphic>();
            if (gra != null)
            {
                m_graphicList_Image.Add(gra);
                m_oldColor.Add(gra.color);
            }
        }

        SetUGUIAlpha(m_fromFloat);
    }

    void UguiAlpha()
    {
        SetUGUIAlpha(GetInterpolation(m_fromFloat, m_toFloat));
    }

    Color colTmp = new Color();
    public void SetUGUIAlpha(float a)
    {
        Color newColor = colTmp;

        int index = 0;
        for (int i = 0; i < m_graphicList_Image.Count; i++)
        {
            newColor = m_oldColor[index];
            newColor.a = a;
            m_graphicList_Image[i].color = newColor;

            index++;
        }

        //for (int i = 0; i < m_graphicList_Image.Count; i++)
        //{
        //    newColor = m_oldColor[index];
        //    newColor.a = a;
        //    m_graphicList_Image[i].color = newColor;

        //    index++;
        //}
    }

    #endregion

    #region Color

    void UguiColor()
    {
        SetUGUIColor(GetInterpolationColor(m_fromColor, m_toColor));
    }

    public void UguiColorInit(bool isChild)
    {
        m_graphicList_Image.Clear();
        if (isChild)
        {
            Graphic[] images = m_animGameObejct.GetComponentsInChildren<Graphic>();
            for (int i = 0; i < images.Length; i++)
            {
                if (images[i].transform.GetComponent<Mask>() == null)
                {
                    m_graphicList_Image.Add(images[i]);
                }
            }
        }
        else
        {
            Graphic[] images = m_animGameObejct.GetComponents<Graphic>();
            for (int i = 0; i < images.Length; i++)
            {
                if (images[i].transform.GetComponent<Mask>() == null)
                {
                    m_graphicList_Image.Add(images[i]);
                }
            }
        }
        SetUGUIColor(m_fromColor);
    }

    void SetUGUIColor(Color color)
    {
        for (int i = 0; i < m_graphicList_Image.Count; i++)
        {
            m_graphicList_Image[i].color = color;
        }

        //for (int i = 0; i < m_graphicList_Image.Count; i++)
        //{
        //    m_graphicList_Image[i].color = color;
        //}
    }


    #endregion

    #endregion

    #region UGUI_SizeDelta
    void SizeDelta()
    {
        if (m_rectRransform == null)
        {
            Debug.LogError(m_transform.name + "缺少RectTransform组件，不能进行sizeDelta变换！！");
            return;
        }
        m_rectRransform.sizeDelta = GetInterpolationV2(m_fromV2, m_toV2);
    }

    #endregion

    #region UGUI_Position

    public void UguiPositionInit()
    {
        m_rectRransform = m_animGameObejct.GetComponent<RectTransform>();
    }

    void UguiPosition()
    {
        m_rectRransform.anchoredPosition3D = GetInterpolationV3(m_fromV3, m_toV3);
    }


    #endregion

    #endregion

    #region Transfrom

    public void TransfromInit()
    {
        m_transform = m_animGameObejct.transform;
    }

    void Position()
    {
        if (m_toTransform != null)
        {
            m_transform.position = GetInterpolationV3(m_fromV3, m_toTransform.position);
        }
        else
        {
            m_transform.position = GetInterpolationV3(m_fromV3, m_toV3);
        }
    }

    void LocalPosition()
    {
        m_transform.localPosition = GetInterpolationV3(m_fromV3, m_toV3);
    }

    void LocalRotate()
    {
        m_transform.localEulerAngles = GetInterpolationV3(m_fromV3, m_toV3);
    }


    void LocalRotation()
    {
        m_transform.localRotation = GetInterpolationQ4(m_fromQ4, m_toQ4);

    }

    void Rotate()
    {
        m_transform.eulerAngles = GetInterpolationV3(m_fromV3, m_toV3);
    }

    void Rotation()
    {
        m_transform.rotation = GetInterpolationQ4(m_fromQ4, m_toQ4);

    }
    void LocalScale()
    {
        m_transform.localScale = GetInterpolationV3(m_fromV3, m_toV3);
    }

    #endregion

    #region Color

    List<SpriteRenderer> m_animObjectList_Sprite = new List<SpriteRenderer>();
    List<TextMesh> m_animObjectList_TextMeshList = new List<TextMesh>();

    #region ALPHA

    public void AlphaInit(bool isChild)
    {
        m_animObjectList_Sprite.Clear();
        m_oldColor.Clear();
        if (isChild)
        {
            SpriteRenderer[] images = m_animGameObejct.GetComponentsInChildren<SpriteRenderer>();
            for (int i = 0; i < images.Length; i++)
            {
                m_animObjectList_Sprite.Add(images[i]);
                m_oldColor.Add(images[i].color);
            }

            TextMesh[] meshs = m_animGameObejct.GetComponentsInChildren<TextMesh>();
            for (int i = 0; i < meshs.Length; i++)
            {
                m_animObjectList_TextMeshList.Add(meshs[i]);
                m_oldColor.Add(meshs[i].color);
            }
        }
        else
        {
            SpriteRenderer image = m_animGameObejct.GetComponent<SpriteRenderer>();
            if (image != null)
            {
                m_animObjectList_Sprite.Add(image);
                m_oldColor.Add(image.color);
            }

            TextMesh mesh = m_animGameObejct.GetComponent<TextMesh>();
            if (mesh != null)
            {
                m_animObjectList_TextMeshList.Add(mesh);
                m_oldColor.Add(mesh.color);
            }
        }

        SetAlpha(m_fromFloat);
    }

    void UpdateAlpha()
    {
        SetAlpha(GetInterpolation(m_fromFloat, m_toFloat));
    }

    public void SetAlpha(float a)
    {
        Color newColor = new Color();

        int index = 0;
        for (int i = 0; i < m_animObjectList_Sprite.Count; i++)
        {
            newColor = m_oldColor[index];
            newColor.a = a;
            m_animObjectList_Sprite[i].color = newColor;

            index++;
        }

        for (int i = 0; i < m_animObjectList_TextMeshList.Count; i++)
        {
            newColor = m_oldColor[index];
            newColor.a = a;
            m_animObjectList_TextMeshList[i].color = newColor;

            index++;
        }
    }

    #endregion

    #region Color

    public void ColorInit(bool isChild)
    {
        m_animObjectList_Sprite.Clear();
        m_oldColor.Clear();

        if (isChild)
        {
            SpriteRenderer[] images = m_animGameObejct.GetComponentsInChildren<SpriteRenderer>();

            for (int i = 0; i < images.Length; i++)
            {
                m_animObjectList_Sprite.Add(images[i]);
            }
        }
        else
        {
            SpriteRenderer image = m_animGameObejct.GetComponent<SpriteRenderer>();
            if (image != null)
            {
                m_animObjectList_Sprite.Add(image);
            }
        }

        SetColor(m_fromColor);
    }

    void UpdateColor()
    {
        SetColor(GetInterpolationColor(m_fromColor, m_toColor));
    }

    public void SetColor(Color color)
    {
        for (int i = 0; i < m_animObjectList_Sprite.Count; i++)
        {
            m_animObjectList_Sprite[i].color = color;
        }
    }

    #endregion

    #endregion

    #region 闪烁

    void Blink()
    {
        if (m_timer < 0)
        {
            m_timer = m_space;
            m_animGameObejct.SetActive(!m_animGameObejct.activeSelf);
        }
        else
        {
            m_timer -= Time.deltaTime;
        }

    }

    #endregion

    #region 插值算法

    #region 总入口

    float GetInterpolation(float oldValue, float aimValue)
    {
        switch (m_interpolationType)
        {
            case InterpType.Default:
            case InterpType.Linear: return Mathf.Lerp(oldValue, aimValue, m_currentTime / m_totalTime);

            case InterpType.InBack: return InBack(oldValue, aimValue, m_currentTime, m_totalTime);
            case InterpType.OutBack: return OutBack(oldValue, aimValue, m_currentTime, m_totalTime);
            case InterpType.InOutBack: return InOutBack(oldValue, aimValue, m_currentTime, m_totalTime);
            case InterpType.OutInBack: return OutInBack(oldValue, aimValue, m_currentTime, m_totalTime);

            case InterpType.InQuad: return InQuad(oldValue, aimValue, m_currentTime, m_totalTime);
            case InterpType.OutQuad: return OutQuad(oldValue, aimValue, m_currentTime, m_totalTime);
            case InterpType.InoutQuad: return InoutQuad(oldValue, aimValue, m_currentTime, m_totalTime);

            case InterpType.InCubic: return InCubic(oldValue, aimValue, m_currentTime, m_totalTime);
            case InterpType.OutCubic: return OutCubic(oldValue, aimValue, m_currentTime, m_totalTime);
            case InterpType.InoutCubic: return InoutCubic(oldValue, aimValue, m_currentTime, m_totalTime);
            case InterpType.OutInCubic: return OutinCubic(oldValue, aimValue, m_currentTime, m_totalTime);

            case InterpType.InQuart: return InQuart(oldValue, aimValue, m_currentTime, m_totalTime);
            case InterpType.OutQuart: return OutQuart(oldValue, aimValue, m_currentTime, m_totalTime);
            case InterpType.InOutQuart: return InOutQuart(oldValue, aimValue, m_currentTime, m_totalTime);
            case InterpType.OutInQuart: return OutInQuart(oldValue, aimValue, m_currentTime, m_totalTime);

            case InterpType.InQuint: return InQuint(oldValue, aimValue, m_currentTime, m_totalTime);
            case InterpType.OutQuint: return OutQuint(oldValue, aimValue, m_currentTime, m_totalTime);
            case InterpType.InOutQuint: return InOutQuint(oldValue, aimValue, m_currentTime, m_totalTime);
            case InterpType.OutInQuint: return OutInQuint(oldValue, aimValue, m_currentTime, m_totalTime);

            case InterpType.InSine: return InSine(oldValue, aimValue, m_currentTime, m_totalTime);
            case InterpType.OutSine: return OutSine(oldValue, aimValue, m_currentTime, m_totalTime);
            case InterpType.InOutSine: return InOutSine(oldValue, aimValue, m_currentTime, m_totalTime);
            case InterpType.OutInSine: return OutInSine(oldValue, aimValue, m_currentTime, m_totalTime);

            case InterpType.InExpo: return InExpo(oldValue, aimValue, m_currentTime, m_totalTime);
            case InterpType.OutExpo: return OutExpo(oldValue, aimValue, m_currentTime, m_totalTime);
            case InterpType.InOutExpo: return InOutExpo(oldValue, aimValue, m_currentTime, m_totalTime);
            case InterpType.OutInExpo: return OutInExpo(oldValue, aimValue, m_currentTime, m_totalTime);

            case InterpType.InBounce: return InBounce(oldValue, aimValue, m_currentTime, m_totalTime);
            case InterpType.OutBounce: return OutBounce(oldValue, aimValue, m_currentTime, m_totalTime);
            case InterpType.InOutBounce: return InOutBounce(oldValue, aimValue, m_currentTime, m_totalTime);
            case InterpType.OutInBounce: return OutInBounce(oldValue, aimValue, m_currentTime, m_totalTime);
        }

        return 0;
    }

    Vector3 GetInterpolationV4(Vector4 oldValue, Vector4 aimValue)
    {
        Vector3 result = Vector3.zero;

        //暂不支持贝塞尔曲线
        //if (m_pathType == PathType.Line)
        //{
            result = new Vector4(
                GetInterpolation(oldValue.x, aimValue.x),
                GetInterpolation(oldValue.y, aimValue.y),
                GetInterpolation(oldValue.z, aimValue.z),
                GetInterpolation(oldValue.w, aimValue.w)
            );
        //}
        //else
        //{
        //    result = GetBezierInterpolationV3(oldValue, aimValue);
        //}

        return result;
    }

    Vector3 GetInterpolationV2(Vector2 oldValue, Vector2 aimValue)
    {
        Vector2 result = Vector2.zero;

        if (m_pathType == PathType.Line)
        {
            result = new Vector2(
                GetInterpolation(oldValue.x, aimValue.x),
                GetInterpolation(oldValue.y, aimValue.y)
            );
        }
        else
        {
            result = GetBezierInterpolationV3(oldValue, aimValue);
        }

        return result;
    }

    Vector3 GetInterpolationV3(Vector3 oldValue, Vector3 aimValue)
    {
        Vector3 result = Vector3.zero;

        if (m_pathType == PathType.Line)
        {
            result = new Vector3(
                GetInterpolation(oldValue.x, aimValue.x),
                GetInterpolation(oldValue.y, aimValue.y),
                GetInterpolation(oldValue.z, aimValue.z)
            );
        }
        else
        {
            result = GetBezierInterpolationV3(oldValue, aimValue);
        }

        return result;
    }


    Quaternion GetInterpolationQ4(Quaternion oldValue, Quaternion aimValue)
    {
        Quaternion result = Quaternion.Euler(Vector3.zero);

        if (m_pathType == PathType.Line)
        {
            result = new Quaternion(
                GetInterpolation(oldValue.x, aimValue.x),
                GetInterpolation(oldValue.y, aimValue.y),
                GetInterpolation(oldValue.z, aimValue.z),
                GetInterpolation(oldValue.w, aimValue.w)
            );
        }
        else
        {
            //result = GetBezierInterpolationV3(oldValue, aimValue);
        }

        return result;
    }

    Color GetInterpolationColor(Color oldValue, Color aimValue)
    {
        Color result = new Color(GetInterpolation(oldValue.r, aimValue.r),
                                  GetInterpolation(oldValue.g, aimValue.g),
                                  GetInterpolation(oldValue.b, aimValue.b),
                                  GetInterpolation(oldValue.a, aimValue.a));
        return result;
    }


    #endregion

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
        //Debug.LogWarning(c * (t * t * ((s + 1) * t + s) + 1) + b);
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

    public float OutinCubic(float b, float to, float t, float d)
    {
        float c = to - b;

        if (t < d / 2)
        {
            return OutCubic( b, b + c / 2, t * 2, d);
        }
        else
        {
            return InCubic( b + c / 2, to, (t * 2) - d, d);
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

    public float OutBounce(float b, float to, float t, float d)
    {
        float c = to - b;
        t = t / d;
        if (t < 1 / 2.75)
        {
            return c * (7.5625f * t * t) + b;
        }
        else if (t < 2 / 2.75)
        {
            t = t - (1.5f / 2.75f);

            return c * (7.5625f * t * t + 0.75f) + b;
        }
        else if (t < 2.5 / 2.75)
        {

            t = t - (2.25f / 2.75f);
            return c * (7.5625f * t * t + 0.9375f) + b;
        }
        else
        {
          t = t - (2.625f / 2.75f);
          return c * (7.5625f * t * t + 0.984375f) + b;
        }
    }

    public float InBounce(float b, float to, float t, float d)
    {
        float c = to - b;
        return c - OutBounce(0 , to, d - t, d) + b;
    }

    public float InOutBounce(float b, float to, float t, float d)
    {
        float c = to - b;
        if (t < d / 2)
        {
            return InBounce(0, to, t * 2f, d) * 0.5f + b;
        }
        else
        {
            return OutBounce(0, to, t * 2f - d, d) * 0.5f + c * 0.5f + b;
        }
    }

    public float OutInBounce(float b, float to, float t, float d)
    {
        float c = to - b;
        if (t < d / 2)
        {
            return OutBounce( b,b +  c / 2, t * 2, d);
        }
        else
        {
            return InBounce(b + c / 2, to, t * 2f - d, d);
            
        }
    }


    //outInExpo,
    //inBack,
    //outBack,
    //inOutBack,
    //outInBack,

    #endregion

    #region Debug


    public void ShowDebug()
    {
        AnimData data = new AnimData();

        Type type = data.GetType();

        System.Reflection.FieldInfo[] infos = type.GetFields();

        for (int i = 0; i < infos.Length; i++)
        {
            if(
                (infos[i].GetValue(this) == null && infos[i].GetValue(data) != null)
                || (infos[i].GetValue(this) != null && infos[i].GetValue(data) == null)
                || (infos[i].GetValue(this) != null && !infos[i].GetValue(this).Equals(infos[i].GetValue(data)))
                )
            {
                Debug.Log(" " + infos[i].Name + " is not equal ! this value : " + infos[i].GetValue(this) + " data Value :" + infos[i].GetValue(data));

            }


        }
    }

    #endregion
}