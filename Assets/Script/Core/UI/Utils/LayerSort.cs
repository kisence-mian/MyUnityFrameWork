using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerSort : MonoBehaviour {

    public int order;
    void Start()
    {
        Canvas canvas = GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.overrideSorting = true;
            canvas.sortingOrder = order;
        }
        else
        {
            Renderer[] renders = GetComponentsInChildren<Renderer>();
            foreach (Renderer render in renders)
            {
                render.sortingOrder = order;
            }
        }
    }

}
