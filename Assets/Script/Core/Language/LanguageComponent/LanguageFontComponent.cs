using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class LanguageFontComponent : MonoBehaviour {

    public string languageFontKey = "FontData/Font/Normal";
    [System.NonSerialized]
    public Text m_text;
    public void Start()
    {
        if (m_text == null)
        {
            m_text = GetComponent<Text>();
        }
        Init();
    }

    public void Init()
    {
        ResetLanguage();
        LanguageManager.OnChangeLanguage += OnChangeLanguage;
    }

    private void OnChangeLanguage(SystemLanguage t)
    {
        ResetLanguage();
    }
    private void OnDestroy()
    {
        LanguageManager.OnChangeLanguage -= OnChangeLanguage;
    }
    public void ResetLanguage()
    {
        try
        {
            Font font = ResourceManager.Load<Font>(LanguageManager.GetContentByKey(languageFontKey));
            m_text.font = font;
        }
        catch (System.Exception e)
        {
            Debug.LogError("设置语言出错！m_text：" + m_text + "\n" + e);
        }
    }
}
