using UnityEngine;
using System.Collections;

public class RuntimeTest : MonoBehaviour 
{

	// Use this for initialization
	void Start ()
    {
        BundleConfigManager.Initialize();
        ResourceManager.gameLoadType = ResLoadType.Streaming;
	}
	
	// Update is called once per frame
	void Update () 
    {
	    if(Input.GetKey(KeyCode.A))
        {
            GameObject testTmp = (GameObject)ResourceManager.Load("GameObject");

            Instantiate(testTmp);
        }
	}
}
