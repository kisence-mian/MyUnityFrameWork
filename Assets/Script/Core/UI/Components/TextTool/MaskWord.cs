using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//输入检测屏蔽字组件
public class MaskWord : MonoBehaviour {

    public char splitChar = ','; //分割字符串
    public string textName = "";//字库资源名


    string[] SentiWords = null;//定义一个接受文件内容的字符串数组
    InputField inputField;
    CallBack<bool> callBack; // true 表示有屏蔽字，需要重新输入

    public void Init(CallBack<bool> l_callBack)
    {
        callBack = null;
        callBack += l_callBack;
    }

    // Use this for initialization
    void Start () {

        transform.GetComponent<InputField>().onValueChanged.AddListener(OnValueChanged);

        if (String.IsNullOrEmpty(textName))
        {
            Debug.LogError("MaskWord textName error = " + textName);
            //无屏蔽字库
            return;
        }
        SentiWords = ResourceManager.LoadText(textName).Split(splitChar);
        ResourceManager.DestoryAssetsCounter(textName);
        for (int i =0;i< SentiWords.Length;i++)
        {
            if (SentiWords[i].Contains("\n"))
            {
                SentiWords[i] = SentiWords[i].Replace("\r", "");

                SentiWords[i] = SentiWords[i].Replace("\n", "");
            }
        }
            inputField = transform.GetComponent<InputField>() ;
    }

    private void OnValueChanged(string t)
    {
        bool needReInput = false;
        if (SentiWords == null)
            return;


        if (string.IsNullOrEmpty(t))
        {
            return;
        }
        foreach (string ssr in SentiWords)
        {
            if (t.Contains(ssr) )
            {
                if (!ssr.Equals(""))
                {
                    needReInput = true;
                    Debug.Log("包含敏感词汇:" + ssr + ",需要进行替换");
                    //string stt = inputField.text;
                    //int length = ssr.ToCharArray().Length;
                    //string s = "";
                    //for (int i = 0; i < length; i++)
                    //    s += "*";
                    //Debug.Log(stt.Replace(ssr, s));
                    //stt = stt.Replace(ssr, s);
                    //inputField.text = stt;
                    break;
                }
            }
        }

        if (needReInput)
        {
            inputField.text = null;

            if (callBack != null)
            {
                callBack(needReInput);
            }

        }



        

    }

    // Update is called once per frame
    void Update () {
		
	}
}
