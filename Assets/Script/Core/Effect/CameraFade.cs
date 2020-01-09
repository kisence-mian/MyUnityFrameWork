using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class CameraFade : MonoBehaviour
{
    private static CameraFade instance = null;
    public static CameraFade Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("[CameraFade]");
                instance = obj.AddComponent<CameraFade>();
                instance.Init();
            }
            return CameraFade.instance;
        }

    }
    public static CallBack FadeCompleteCallBack;
    private static bool isFading = false;
    public static bool IsFading
    {
        get
        {
            return isFading;
        }
    }

    private void Init()
    {
        alpha = 0;
        tempColor = GUI.color;
        // GUI.depth = 100;
        if (crossfadeTexture == null)
        {
            crossfadeTexture = new Texture2D(1, 1, TextureFormat.RGB24, false);
            crossfadeTexture.SetPixel(0, 0, Color.black);
            crossfadeTexture.Apply();
        }
    }

    #region 外部调用
    //淡入
    public static void FadeIn(float fadeTime, CallBack completeCallBack = null, bool isForceAlpha = false, float delay = 0)
    {
        CameraFadeData data = GetCameraFadeData(CameraFadeType.FadeIn, isForceAlpha, delay, fadeTime, completeCallBack);
        Instance.StartFadeFunc(data);
    }
    //淡出
    public static void FadeOut(float fadeTime, CallBack completeCallBack = null, bool isForceAlpha = false, float delay = 0)
    {
        CameraFadeData data = GetCameraFadeData(CameraFadeType.FadeOut, isForceAlpha, delay, fadeTime, completeCallBack);
        Instance.StartFadeFunc(data);
    }
    //从淡入到淡出
    public static void FadeInToOut(float _fadeInTime, float afterInDelayTime, float _fadeOutTime, CallBack afterFadeInCallback = null, CallBack afterFadeOutCallback = null, float delay = 0, bool isForceAlpha = false)
    {
        CameraFadeData data = GetCameraFadeData(CameraFadeType.FadeIn, isForceAlpha, delay, _fadeInTime, afterFadeInCallback);
        CameraFadeData data2 = GetCameraFadeData(CameraFadeType.FadeOut, isForceAlpha, afterInDelayTime, _fadeOutTime, afterFadeOutCallback);
        Instance.StartFadeFunc(data, data2);
    }
    #endregion

    private List<CameraFadeData> currentFadeDatas = new List<CameraFadeData>();
    private void StartFadeFunc(params CameraFadeData[] paras)
    {
        if (currentFadeDatas.Count > 0)
        {
            foreach (var item in currentFadeDatas)
            {
                if (item.completeCallBack != null)
                    item.completeCallBack();

                cacheData.Add(item);
            }
            currentFadeDatas.Clear();
        }

        for (int i = 0; i < paras.Length; i++)
        {
            currentFadeDatas.Add(paras[i]);
        }

        CameraFadeData data = paras[0];
        if (data.isForceAlpha)
        {
            if (data.fadeType == CameraFadeType.FadeIn)
            {
                alpha = 0;
            }
            else
            {
                alpha = 1;
            }
        }
        else
        {
            if (isFading)
            {
                data.delay = 0;
            }
        }
        isFading = true;

        float tempTime = data.fadeTime * alpha;
        data.tempFadeCaculateTime = tempTime;

    }

    // Update is called once per frame
    private void Update()
    {
        if (currentFadeDatas.Count == 0)
        {
            isFading = false;
            return;
        }

        CameraFadeData data = currentFadeDatas[0];

        if (data.delay > 0)
        {
            data.delay -= Time.unscaledDeltaTime;
            return;
        }

        if (data.fadeType == CameraFadeType.FadeIn)
        {
            if (data.tempFadeCaculateTime >= data.fadeTime)
            {
                alpha = 1;
                RunComplete(data);
                return;
            }
            data.tempFadeCaculateTime += Time.unscaledDeltaTime;


        }
        else
        {
            if (data.tempFadeCaculateTime <= 0)
            {
                alpha = 0;
                RunComplete(data);
                return;
            }


            data.tempFadeCaculateTime -= Time.unscaledDeltaTime;
        }
        alpha = data.tempFadeCaculateTime / data.fadeTime;


    }

    private void RunComplete(CameraFadeData data)
    {
        if (currentFadeDatas.Contains(data))
        {
            currentFadeDatas.Remove(data);
            cacheData.Add(data);
        }
        if (data.completeCallBack != null)
        {
            data.completeCallBack();
        }
       
        if (FadeCompleteCallBack != null)
        {
            FadeCompleteCallBack();
        }
    }

    float alpha = 0;
    private Texture2D crossfadeTexture;
    Color tempColor;
    void OnGUI()
    {
        tempColor.a = alpha;
        GUI.color = tempColor;
        if (crossfadeTexture != null && alpha!=0)
        {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), crossfadeTexture);
        }
    }

    private static List<CameraFadeData> cacheData = new List<CameraFadeData>();
    private static CameraFadeData GetCameraFadeData(CameraFadeType fadeType, bool isForceAlpha, float delay, float fadeTime, CallBack completeCallBack)
    {
        CameraFadeData data = null;
        if (cacheData.Count > 0)
        {
            data = cacheData[0];
            cacheData.RemoveAt(0);
        }
        else
        {
            data = new CameraFadeData();
        }
        data.SetCameraFadeData(fadeType, isForceAlpha, delay, fadeTime, completeCallBack);
        return data;
    }
    public class CameraFadeData
    {
        public CameraFadeType fadeType;
        /// <summary>
        /// 是否硬性切换透明值到1,或0，再渐变
        /// </summary>
        public bool isForceAlpha = false;
        public float delay;
        public float fadeTime;
        public CallBack completeCallBack;

        public float tempFadeCaculateTime;

        public CameraFadeData() { }
        public CameraFadeData(CameraFadeType fadeType, bool isForceAlpha, float delay, float fadeTime, CallBack completeCallBack)
        {
            SetCameraFadeData(fadeType, isForceAlpha, delay, fadeTime, completeCallBack);
        }
        public void SetCameraFadeData(CameraFadeType fadeType, bool isForceAlpha, float delay, float fadeTime, CallBack completeCallBack)
        {
            this.fadeType = fadeType;
            this.isForceAlpha = isForceAlpha;
            this.delay = delay;
            this.fadeTime = fadeTime;
            this.completeCallBack = completeCallBack;

            tempFadeCaculateTime = fadeTime;
        }
    }
    public enum CameraFadeType
    {
        FadeIn,
        FadeOut,
    }
}




