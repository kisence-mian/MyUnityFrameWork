using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 简易版  运行后，需要点一下unity外，再点回来，触发重新加载文本，即可查看结果
/// </summary>
public class MaskWordMakeTool :MonoBehaviour{

    public string maskDataBaseName = "MaskWordData";//原屏蔽字库文本
    public string savePath = "/Resources/Data/NameWorld/MaskWordData3.txt";//存储路径

    string maskDataBase; //原屏蔽字文本

    private void Awake()
    {
        maskDataBase = ResourceManager.Load<TextAsset>(maskDataBaseName).text;

        maskDataBase = maskDataBase.Replace(',', '，');

        string[] words = maskDataBase.Split('，');

        string newMaskData = "";


        for (int i = 0; i < words.Length; i++)
        {
            newMaskData += words[i] + "," + "\n";
        }

        ResourceIOTool.WriteStringByFile(Application.dataPath + savePath, newMaskData);

    }


}
