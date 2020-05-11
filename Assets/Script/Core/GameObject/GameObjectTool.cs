using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectTool 
{
    /// <summary>
    /// 重置位置、旋转 、缩放
    /// </summary>
    /// <param name="go"></param>
    /// <param name="isLocal"></param>
    public static void ResetTransform(GameObject go, bool isLocal = true)
    {
        if (isLocal)
        {
            go.transform.localPosition = Vector3.zero;
            go.transform.localEulerAngles = Vector3.zero;
        }
        else
        {
            go.transform.position = Vector3.zero;
            go.transform.eulerAngles = Vector3.zero;
        }

        go.transform.localScale = Vector3.one;
    }

    /// <summary>
    /// 设置父节点并重置
    /// </summary>
    /// <param name="go_child"></param>
    /// <param name="go_parent"></param>
    public static void SetParentAndReset(GameObject go_child,GameObject go_parent)
    {
        go_child.transform.SetParent(go_parent.transform);
        ResetTransform(go_child);
    }

}
