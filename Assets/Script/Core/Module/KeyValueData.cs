using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class KeyValueData
{
    public string key="";
    public string value="";

    public KeyValueData() { }

    public KeyValueData(string key,string value)
    {
        this.key = key;
        this.value = value;
    }

    public static Dictionary<string,string> KeyValueDataList2Dictionary(List<KeyValueData> list)
    {
        Dictionary<string, string> dic = new Dictionary<string, string>();
        if (list != null)
        {
            foreach (var item in list)
            {
                dic.Add(item.key, item.value);
            }
        }

        return dic;
    }

    public static List<KeyValueData> Dictionary2KeyValueDataList(Dictionary<string,string> dic)
    {
        List<KeyValueData> list = new List<KeyValueData>();
        if (dic != null)
        {
            foreach (var item in dic)
            {
                KeyValueData d = new KeyValueData(item.Key, item.Value);
                list.Add(d);
            }
        }
        return list;
    }
}

