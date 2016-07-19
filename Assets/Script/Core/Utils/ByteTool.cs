using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

public class ByteTool
{
    public static byte[] Object2Bytes(object obj)
    {
        byte[] buff;
        using (MemoryStream ms = new MemoryStream())
        {
            IFormatter iFormatter = new BinaryFormatter();
            iFormatter.Serialize(ms, obj);
            buff = ms.GetBuffer();
        }
        return buff;
    }
}
