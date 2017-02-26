using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
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
        //byte[] bs = new byte[4];
        //bs[0] = (byte)(value >> 24);
        //bs[1] = (byte)(value >> 16);
        //bs[2] = (byte)(value >> 8);
        //bs[3] = (byte)(value);

        bytes.Add((byte)(value >> 24));
        bytes.Add((byte)(value >> 16));
        bytes.Add((byte)(value >> 8));
        bytes.Add((byte)(value));
    }
    public void WriteShort(int value)
    {
        //byte[] bs = new byte[2];
        //bs[0] = (byte)(value >> 8);
        //bs[1] = (byte)(value);
        //bytes.AddRange(bs);

        bytes.Add((byte)(value >> 8));
        bytes.Add((byte)(value));
    }
    public int ReadInt()
    {
        //byte[] bs = new byte[4];
        //for (int i = 0; i < 4; i++)
        //{
        //    bs[i] = bytes[i + Postion];
        //}

        int result = (int)bytes[3 + Postion] | ((int)bytes[2 + Postion] << 8) | ((int)bytes[1 + Postion] << 16) | ((int)bytes[0 + Postion] << 24);

        Postion += 4;
        return result;
    }
    public int ReadShort()
    {
        //byte[] bs = new byte[2];
        //for (int i = 0; i < 2; i++)
        //{
        //    bs[i] = bytes[i + Postion];
        //}
        
        int result = (int)bytes[1 + Postion] | ((int)bytes[0 + Postion] << 8);
        Postion += 2;
        return result;
    }

    byte[] b = new byte[8];
    //byte[] Temp = new byte[8];
    public double ReadDouble()
    {
        for (int i = 0; i < 8; i++)
        {
            b[7 - i] = bytes[i + Postion];
        }
        Postion += 8;
        //Array.Reverse(b);
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
        byte[] bs = Encoding.UTF8.GetBytes(content);
        WriteShort(bs.Length);
        WriteALLBytes(bs);
    }
}