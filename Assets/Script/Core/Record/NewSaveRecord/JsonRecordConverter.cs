using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class JsonRecordConverter : IRecordConverter
{
    public string GetFileExtend()
    {
        return ".json";
    }

    public string GetSaveDirectoryName()
    {
        return "Record";
    }

    public string Object2String(object obj)
    {
        return JsonUtils.ToJson(obj);
    }

    public T String2Object<T>(string content)
    {
        T t = default(T);
        JsonUtils.TryFromJson(out t, content);
        //Debug.Log(state+ " old:" + content + "\n" + "new:" + JsonUtils.ToJson(t)+"\n def:"+JsonUtils.ToJson(default(T)));
        return t;
    }
}

