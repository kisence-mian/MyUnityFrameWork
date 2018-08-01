using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageComponent : MonoBehaviour
{
    public string languageKey = "";

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
        GlobalEvent.AddEvent(LanguageEventEnum.LanguageChange, ReceviceLanguageChange);
        ResetLanguage();
    }

    public void ResetLanguage()
    {
        if (string.IsNullOrEmpty(languageKey))
            return;
        m_text.text = LanguageManager.GetContentByKey(languageKey);
    }

    void ReceviceLanguageChange(params object[] objs)
    {
        ResetLanguage();
    }
}
