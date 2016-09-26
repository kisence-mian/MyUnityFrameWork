using UnityEngine;
using System.Collections;

public class UITools : MonoBehaviour 
{

    /// <summary>
    /// 加载一个对象并把它实例化
    /// </summary>
    /// <param name="l_gameObjectName">对象名</param>
    /// <param name="l_parent">对象的父节点,可空</param>
    /// <returns></returns>
    public static GameObject CreateUiItem(string l_gameObjectName, GameObject l_parent = null)
    {
        GameObject l_goTmp = (GameObject)ResourceManager.Load(l_gameObjectName);

        if (l_goTmp == null)
        {
            Debug.LogError("CreateItem error dont find :" + l_gameObjectName);
            return null;
        }

        GameObject l_instanceTmp = Instantiate(l_goTmp);
        l_instanceTmp.name = l_gameObjectName;

        if (l_parent != null)
        {
            l_instanceTmp.transform.SetParent(l_parent.transform);
            l_instanceTmp.transform.localScale = Vector3.one;
        }

        return l_instanceTmp;
    }
}
