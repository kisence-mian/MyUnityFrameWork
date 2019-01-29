using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageComponent : MonoBehaviour
{
    public string languageKey = "";
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
        GlobalEvent.AddEvent(LanguageEventEnum.LanguageChange, ReceviceLanguageChange);
        ResetLanguage();
    }

    public void ResetLanguage()
    {
        if (string.IsNullOrEmpty(languageKey))
            return;

        string value = LanguageManager.GetContentByKey(languageKey).ToString();
        value = value.Replace("\\n", "\n");
        m_text.text = value;
    }

    void ReceviceLanguageChange(params object[] objs)
    {
        ResetLanguage();
    }
}
