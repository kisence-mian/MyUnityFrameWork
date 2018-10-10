using UnityEngine;
using System.Collections;
using System;
using UnityEngine.EventSystems;
namespace HDJ.Framework.Tools
{

    public class CameraFade : MonoBehaviour
    {

        private static CameraFade instance;
        public static CameraFade Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameObject("[CameraFade]").AddComponent<CameraFade>();
                    
                }
                return instance;
            }

            set
            {
                instance = value;
            }
        }
        private float alpha = 0;
        private Texture2D crossfadeTexture;
         void OnAwake()
        {
            crossfadeTexture = new Texture2D(1, 1, TextureFormat.RGB24, false);
            crossfadeTexture.SetPixel(0, 0, Color.black);
            crossfadeTexture.Apply();
            DontDestroyOnLoad(this);
        }


        //淡入
        public void FadeIn(float _fadeInTime, CallBack _fun = null)
        {
            if (!isFading)
            {
                StartCoroutine(FadeAction(true, _fadeInTime, _fun));
            }
        }
        //淡出
        public void FadeOut(float _fadeOutTime, CallBack _fun = null)
        {
            if (!isFading)
            {
                StartCoroutine(FadeAction(false, _fadeOutTime, _fun));
            }

        }
        //从淡入到淡出
        public void FadeInToOut(float _fadeInTime, float afterInDelayTime, float _fadeOutTime, CallBack afterFadeInCallback = null, CallBack afterFadeOutCallback = null)
        {
            if (!isFading)
            {
                StartCoroutine(FadeInToOutAction(_fadeInTime, afterInDelayTime, _fadeOutTime, afterFadeInCallback, afterFadeOutCallback));
            }
        }
        IEnumerator FadeInToOutAction(float _fadeInTime, float afterInDelayTime, float _fadeOutTime, CallBack afterFadeInCallback = null, CallBack afterFadeOutCallback = null)
        {
            yield return StartCoroutine(FadeAction(true, _fadeInTime, afterFadeInCallback));
            isFading = true;
            yield return new WaitForSeconds(afterInDelayTime);
            yield return StartCoroutine(FadeAction(false, _fadeOutTime, afterFadeOutCallback));
        }
        public bool isFading = false;

        IEnumerator FadeAction(bool isFadeIn, float fadeTime, CallBack _fun)
        {
            isFading = true;
            tempColor = GUI.color;
            GUI.depth = 100;
            if (isFadeIn)
                alpha = 0;
            else
                alpha = 1;

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
            //yield return new WaitForEndOfFrame();
            isFading = false;
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
        }

        // Update is called once per frame
        Color tempColor;


        void OnGUI()
        {
            if (alpha <= 0)
                return;
            tempColor.a = alpha;
            GUI.color = tempColor;
            if (crossfadeTexture != null)
            {
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), crossfadeTexture, ScaleMode.StretchToFill);

            }
        }
    }
}


