using UnityEngine;
using System.Collections;
using System;

public class CameraFade : MonoBehaviour {
    private static CameraFade instance = null;
    public  delegate void CallBack();
    public static CameraFade Instance
    {
        get {
            if (instance == null)
            {
                GameObject obj = new GameObject("[CameraFade]");
                instance = obj.AddComponent<CameraFade>();
            }
            return CameraFade.instance; }
        
    }
  //  AfterFadeFunction fun;
    public void KillAll()
    {
        instance = null;
        Destroy(gameObject);
    }

    float alpha = 0; 
    private float fadeTime = 0.5f;
    private Texture2D crossfadeTexture;

    //淡入
    public void FadeIn(float _fadeInTime, CallBack _fun = null)
    {
        if (!isFading)
        {
            StartCoroutine(FadeAction(true,_fadeInTime, _fun));
        }
    }
  //淡出
    public void FadeOut(float _fadeOutTime, CallBack _fun = null)
    {
        if (!isFading)
        {
            StartCoroutine(FadeAction(false,_fadeOutTime, _fun));
        }

    }
    //从淡入到淡出
    public void FadeInToOut(float _fadeInTime, float afterInDelayTime, float _fadeOutTime, CallBack afterFadeInCallback=null, CallBack afterFadeOutCallback = null)
    {
        if (!isFading)
        {
            StartCoroutine(FadeInToOutAction(_fadeInTime, afterInDelayTime, _fadeOutTime, afterFadeInCallback, afterFadeOutCallback));
        }
    }
    IEnumerator FadeInToOutAction(float _fadeInTime, float afterInDelayTime, float _fadeOutTime, CallBack afterFadeInCallback = null, CallBack afterFadeOutCallback = null)
    {
        yield return StartCoroutine(FadeAction(true,_fadeInTime, afterFadeInCallback));
        isFading = true;
        yield return new WaitForSeconds(afterInDelayTime);
        isFading = false;
        FadeOut(_fadeOutTime, afterFadeOutCallback);
    }
    public bool isFading = false;

    IEnumerator FadeAction(bool isFadeIn,float tempFadeTime,  CallBack _fun)
    {
        tempColor = GUI.color;
        GUI.depth = 100;
        fadeTime = tempFadeTime;

        if (isFadeIn) alpha = 0;
        else alpha = 1;

        if (crossfadeTexture == null)
        {
            crossfadeTexture = new Texture2D(1, 1, TextureFormat.RGB24, false);
            crossfadeTexture.SetPixel(0, 0, Color.black);
            crossfadeTexture.Apply();
        }

        isFading = true;
        float tempTime = fadeTime + Time.unscaledTime; ;

        while (true)
        {
                if (!isFadeIn)
                {
                    alpha = (tempTime - Time.unscaledTime) / fadeTime;

                    if (alpha < 0.05f)
                    {
                        alpha = 0;
                        break;
                    }
                }
                else
                {
                    alpha = Mathf.Clamp(1 - ((tempTime - Time.unscaledTime) / fadeTime), 0f, 1f);

                    if (alpha >= 0.98f)
                    {
                        alpha = 1;
                        break;
                    }
                }
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForEndOfFrame();
        isFading = false;
        Debug.Log("Camera fade alpha: " + alpha);
        try
        {
            if (_fun != null)
            {
                _fun();
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Camera Fade Call back Exception :" + e);
        }
      //  yield return new WaitForEndOfFrame();
    }

    // Update is called once per frame
    Color tempColor ;
    void OnGUI()
    {           
        tempColor.a = alpha;
        GUI.color = tempColor;
        if (crossfadeTexture != null)
        {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), crossfadeTexture);
        }
    }
}




