using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 字体选择器，当字体为繁体时自动选择另一套字体
/// </summary>
public class FontChooserComponent : MonoBehaviour {

    public Font m_Traditional;
    public Font m_Simplified;

    Text m_text;

	void Start () 
    {
        if (m_text == null)
        {
            m_text = GetComponent<Text>();
        }

        ResetLanguage();
        GlobalEvent.AddEvent(LanguageEventEnum.LanguageChange, ReceviceLanguageChange);
	}

    void ResetLanguage()
    {
        if (m_text != null)
        {
            if (LanguageManager.s_currentLanguage == SystemLanguage.ChineseTraditional)
            {
                m_text.font = m_Traditional;
            }
            else
            {
                m_text.font = m_Simplified;
            }
        }
    }
	
    void ReceviceLanguageChange(params object[] objs)
    {
        ResetLanguage();
    }
}
