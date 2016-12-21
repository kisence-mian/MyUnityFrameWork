using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DataGenerateManager<T> where T : DataGenerateBase, new()
{
    static Dictionary<string, T> s_dict = new Dictionary<string, T>();

    public static T GetData(string key) 
    {
        if (s_dict.ContainsKey(key))
        {
            return s_dict[key];
        }
        else
        {
            T data = new T();
            data.LoadData(key);
            s_dict.Add(key,data);
            return data;
        }
    }
}


public abstract class DataGenerateBase
{
    public virtual void LoadData(string key)
    {

    }
}

