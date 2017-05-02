using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using UnityEngine;

public class ByteArray
{
    public List<byte> bytes = new List<byte>();

    public void Add(byte[] buffer)
    {
        for (int i = 0; i < buffer.Length; i++)
        {
            bytes.Add(buffer[i]);
        }
    }

    public int Length
    {
        get {
            return bytes.Count;
           }
    }
    public void clear()
    {
        Postion = 0;
        bytes.Clear();
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
            return bytes.ToArray(); 
        }
    }
    public bool ReadBoolean()
    {
        byte b = bytes[Postion];
        Postion += 1;
        return b == (byte)0 ? false : true;
    }
    public byte ReadByte()
    {
        byte result = bytes[Postion];
        Postion += 1;
        return result;
    }
    public byte[] ReadBytes(int len)
    {
        byte[] result = new byte[len];
        for (int i = 0; i < len; i++)
        {
            result[i] = bytes[i + Postion];
        }
        Postion += len;
        return result;
    }

    public void WriteInt(int value)
    {
        bytes.Add((byte)(value >> 24));
        bytes.Add((byte)(value >> 16));
        bytes.Add((byte)(value >> 8));
        bytes.Add((byte)(value));
    }
    public void WriteShort(int value)
    {
        short tmp = (short)value;

        bytes.Add((byte)(tmp >> 8));
        bytes.Add((byte)(tmp));
    }

    public void WriteInt8(int value)
    {
        bytes.Add((byte)(value));
    }

    public int ReadUInt()
    {
        int result = bytes[3 + Postion] | (bytes[2 + Postion] << 8) | (bytes[1 + Postion] << 16) | (bytes[0 + Postion] << 24);
        Postion += 4;
        return result;
    }
    public int ReadUShort()
    {
        int result = bytes[1 + Postion] | bytes[Postion] << 8;
        Postion += 2;

        return result;
    }

    byte[] int32Catch = new byte[4];
    public int ReadInt32()
    {
        int32Catch[3] = bytes[Postion];
        int32Catch[2] = bytes[Postion + 1];
        int32Catch[1] = bytes[Postion + 2];
        int32Catch[0] = bytes[Postion + 3];

        int result = BitConverter.ToInt32(int32Catch, 0);
        Postion += 4;
        return result;
    }

    byte[] int16Catch = new byte[2];
    public int ReadInt16()
    {
        int16Catch[1] = bytes[Postion];
        int16Catch[0] = bytes[Postion + 1];

        int result = BitConverter.ToInt16(int16Catch, 0);
        Postion += 2;
        return result;
    }

    public int ReadInt8()
    {
        int result = bytes[Postion];
        Postion += 1;
        return result;
    }

    byte[] b = new byte[8];
    public double ReadDouble()
    {
        for (int i = 0; i < 8; i++)
        {
            b[7 - i] = bytes[i + Postion];
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
            b[i] = bytes[i + Postion];
        }
        Postion += (int)length;

        string decodedString = Encoding.UTF8.GetString(b);
        return decodedString;
    }

    public void WriteALLBytes(byte[] bs)
    {
        bytes.AddRange(bs);
    }

    public void WriteBoolean(bool value)
    {
        bytes.Add(value ? ((byte)1) : ((byte)0));
    }

    public void WriteByte(byte value)
    {
        bytes.Add(value);
    }

    public void WriteDouble(double v)
    {
        byte[] temp = BitConverter.GetBytes(v);

        for (int i = 0; i < 8; i++)
        {
            b[7 - i] = temp[i];
        }

        bytes.AddRange(b);
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