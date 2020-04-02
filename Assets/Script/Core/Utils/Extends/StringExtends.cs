using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public static class StringExtends
{
    /// <summary>
    /// 分割特定开始字符串和结束字符串包裹的字符串
    /// </summary>
    /// <param name="value"></param>
    /// <param name="startSign"></param>
    /// <param name="endSign"></param>
    /// <returns></returns>
    public static string[] SplitExtend(this string value,string startSign,string endSign)
    {
        List<string> results = new List<string>();

        string content = value;
        int startIndex = content.IndexOf(startSign);
        while (startIndex != -1)
        {
            int tempInt = startIndex + startSign.Length;

            int endIndex = content.IndexOf(endSign, tempInt);

            if (endIndex == -1)
                break;
            else
            {
                results.Add(content.Substring(tempInt, endIndex - tempInt));
                content = content.Remove(0, endIndex + endSign.Length);
                startIndex = content.IndexOf(startSign); 
            }
        }

        return results.ToArray();
    }
}

