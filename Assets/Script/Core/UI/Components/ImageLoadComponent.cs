using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ImageLoadComponent : MonoBehaviour {

    public string iconName;
    private Image loadImage;
    // Use this for initialization
    void Awake () {
        loadImage = LoadImage();
    }

    private void OnEnable()
    {
        if (loadImage == null || loadImage.sprite == null)
        {
            loadImage =  LoadImage();
        }
    }
    public Image LoadImage()
    {
        Image image = GetComponent<Image>();
        if (image)
        {
            if (!string.IsNullOrEmpty(iconName))
                UGUITool.SetImageSprite(image, iconName);
        }
        else
        {
            Debug.LogError(" Dont have Image!!!");
        }
        return image;
    }
    private void OnDestroy()
    {
        if (!string.IsNullOrEmpty(iconName))
            ResourceManager.DestoryAssetsCounter(iconName);
    }

}
