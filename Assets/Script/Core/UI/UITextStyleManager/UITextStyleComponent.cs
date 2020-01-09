using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class UITextStyleComponent : MonoBehaviour {

    public string styleName;
    private Text text;
	// Use this for initialization
	void Start () {
       
        SystemLanguage language = LanguageManager.CurrentLanguage;

        SetTextStyleData(language);

    }
	
	// Update is called once per frame
	public void SetTextStyleData (SystemLanguage language)
    {
		if(text==null)
            text = GetComponent<Text>();

        if (string.IsNullOrEmpty(styleName))
            return;

        UITextStyleManager.SetText(text, styleName, language);
    }
}
