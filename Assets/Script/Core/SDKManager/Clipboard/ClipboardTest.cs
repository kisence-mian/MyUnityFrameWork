using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClipboardTest : MonoBehaviour {


    public Text text;

    public void Copy()
    {

        ClipboardManager.ToClipboard("1111111111");
    }

    public void Paste()
    {
        text.text = ClipboardManager.GetClipboard();
    }
}
