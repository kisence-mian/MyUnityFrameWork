using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ImageLoadComponent : MonoBehaviour {

    public string iconName;
	// Use this for initialization
	void Start () {
        LoadImage();

    }

    public Image LoadImage()
    {
        Image image = GetComponent<Image>();
        if (image)
        {
            UGUITool.SetImageSprite(image, iconName);
        }
        else
        {
            Debug.LogError(" Dont have Image!!!");
        }
        return image;
    }

}
