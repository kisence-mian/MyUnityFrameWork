using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using UnityEngine;

public class ByteArray
{
    private List<byte> bytes;

    public List<byte> Bytes
    {
        get
        {
            if(bytes == null)
            {
                bytes = new List<byte>();
            }

            return bytes;
        }

        set
        {
            bytes = value;
        }
    }

    public void Add(byte[] buffer)
    {
        for (int i = 0; i < buffer.Length; i++)
        {
            Bytes.Add(buffer[i]);
        }
    }

    public int Length
    {
        get {
            return Bytes.Count;
           }
    }
    public void clear()
    {

        Postion = 0;
        Bytes.Clear();
    }
    public int Postion
    {
        get;
        set;
    }
    public byte[] Buffer
    {
        get 
        { 
            return Bytes.ToArray(); 
        }
    }



    public bool ReadBoolean()
    {
        byte b = Bytes[Postion];
        Postion += 1;
        return b == (byte)0 ? false : true;
    }
    public byte ReadByte()
    {
        byte result = Bytes[Postion];
        Postion += 1;
        return result;
    }
    public byte[] ReadBytes(int len)
    {
        byte[] result = new byte[len];
        for (int i = 0; i < len; i++)
        {
            result[i] = Bytes[i + Postion];
        }
        Postion += len;
        return result;
    }

    public void WriteInt(int value)
    {
        Bytes.Add((byte)(value >> 24));
        Bytes.Add((byte)(value >> 16));
        Bytes.Add((byte)(value >> 8));
        Bytes.Add((byte)(value));
    }
    public void WriteShort(int value)
    {
        short tmp = (short)value;

        Bytes.Add((byte)(tmp >> 8));
        Bytes.Add((byte)(tmp));
    }

    public void WriteInt8(int value)
    {
        Bytes.Add((byte)(value));
    }

    public int ReadUInt()
    {
        int result = Bytes[3 + Postion] | (Bytes[2 + Postion] << 8) | (Bytes[1 + Postion] << 16) | (Bytes[0 + Postion] << 24);
        Postion += 4;
        return result;
    }
    public int ReadUShort()
    {
        int result = Bytes[1 + Postion] | Bytes[Postion] << 8;
        Postion += 2;

        return result;
    }

    static byte[] int32Cache = new byte[4];
    public int ReadInt32()
    {
        int32Cache[3] = Bytes[Postion];
        int32Cache[2] = Bytes[Postion + 1];
        int32Cache[1] = Bytes[Postion + 2];
        int32Cache[0] = Bytes[Postion + 3];

        int result = BitConverter.ToInt32(int32Cache, 0);
        Postion += 4;
        return result;
    }

    static byte[] int16Catch = new byte[2];
    public int ReadInt16()
    {
        int16Catch[1] = Bytes[Postion];
        int16Catch[0] = Bytes[Postion + 1];

        int result = BitConverter.ToInt16(int16Catch, 0);
        Postion += 2;
        return result;
    }

    public int ReadInt8()
    {
        int result = Bytes[Postion];
        Postion += 1;
        return result;
    }

    static byte[] b = new byte[8];
    public double ReadDouble()
    {
        for (int i = 0; i < 8; i++)
        {
            b[7 - i] = Bytes[i + Postion];
        }
        Postion += 8;
        return  BitConverter.ToDouble(b, 0);
    }
    public string ReadUTFBytes(uint length)
    {
        if (length == 0)
            return string.Empty;

        byte[] b = new byte[length];
        for (int i = 0; i < length; i++)
        {
            b[i] = Bytes[i + Postion];
        }
        Postion += (int)length;

        string decodedString = Encoding.UTF8.GetString(b);
        return decodedString;
    }

    public void WriteALLBytes(byte[] bs)
    {
        Bytes.AddRange(bs);
    }

    public void WriteBoolean(bool value)
    {
        Bytes.Add(value ? ((byte)1) : ((byte)0));
    }

    public void WriteByte(byte value)
    {
        Bytes.Add(value);
    }

    public void WriteDouble(double v)
    {
        byte[] temp = BitConverter.GetBytes(v);

        for (int i = 0; i < 8; i++)
        {
            b[7 - i] = temp[i];
        }

        Bytes.AddRange(b);
    }

    public void WriteString(string content)
    {
        if(content == null)
        {
            content = "";
        }

        byte[] bs = Encoding.UTF8.GetBytes(content);
        WriteShort(bs.Length);
        WriteALLBytes(bs);
    }
}