using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageComponent : MonoBehaviour
{
    public string m_moduleName = LanguageManager.c_defaultModuleKey;
    public string m_languageID = "";

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
        m_text.text = LanguageManager.GetContent(m_moduleName,m_languageID);
    }

    void ReceviceLanguageChange(params object[] objs)
    {
        ResetLanguage();
    }
}
