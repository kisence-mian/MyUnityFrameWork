using UnityEngine;
using System.Collections;

public class MianWindow : UIWindowBase 
{

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	 
	}

    public void OnClickStart()
    {
        AnimSystem.uguiAlpha(gameObject, 1, 0, 1, InteType.linear, null, true);
    }
}
