using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        return JsonUtils.FromJson<T>(content);
    }
}

