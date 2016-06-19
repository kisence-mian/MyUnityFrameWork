using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour
{
    #region 静态部分

    static UIManager instance;

    public static UIManager getInstance()
    {
        if (instance == null)
        {
            GameObject instanceGO = new GameObject();
            instanceGO.name = "UIManager";

            instance = instanceGO.AddComponent<UIManager>();
        }

        return instance;
    }

    public static void openUI()
    {

    }

    public static void closeUI()
    {

    }


    #endregion 

    #region 实例部分

    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

    }

    #endregion
}
