using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
public class ByteArray
{
    private List<byte> bytes = new List<byte>();
    //private MemoryStream m_Stream = new MemoryStream();
    //private BinaryReader m_Reader = null;
    //private BinaryWriter m_Writer = null;
    public ByteArray()
    {
    }
    public ByteArray(byte[] buffer)
    {
        for (int i = 0; i < buffer.Length; i++)
        {
            bytes.Add(buffer[i]);
        }
    }

    public ByteArray(MemoryStream ms)
    {
        //m_Stream = ms;
        Init();
    }
    private void Init()
    {
        //m_Writer = new BinaryWriter(m_Stream);
        //m_Reader = new BinaryReader(m_Stream);
    }

    public int Length
    {
        get {
            return bytes.Count;
           }
    }
    public void clear()
    {
        bytes.Clear();
    }
    public int Postion
    {
        get;
        set;
    }
    public byte[] Buffer
    {
        get { return bytes.ToArray(); }
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
    public void addpos(int len)
    {
        Postion += len;
    }
    public int getpos()
    {
        return Postion;
    }
    public void WriteInt(int value)
    {
        byte[] bs = new byte[4];
        bs[0] = (byte)(value >> 24);
        bs[1] = (byte)(value >> 16);
        bs[2] = (byte)(value >> 8);
        bs[3] = (byte)(value);
        bytes.AddRange(bs);
    }
    public void WriteShort(int value)
    {
        byte[] bs = new byte[2];
        bs[0] = (byte)(value >> 8);
        bs[1] = (byte)(value);
        bytes.AddRange(bs);
    }
    public int ReadInt()
    {
        byte[] bs = new byte[4];
        for (int i = 0; i < 4; i++)
        {
            bs[i] = bytes[i + Postion];
        }
        Postion += 4;
        int result = (int)bs[3] | ((int)bs[2] << 8) | ((int)bs[1] << 16) | ((int)bs[0] << 24);
        return result;
    }
    public int ReadShort()
    {
        byte[] bs = new byte[2];
        for (int i = 0; i < 2; i++)
        {
            bs[i] = bytes[i + Postion];
        }
        Postion += 2;
        int result = (int)bs[1] | ((int)bs[0] << 8);
        return result;
    }
    public double ReadDouble()
    {
        byte[] b = new byte[8];
        for (int i = 0; i < 8; i++)
        {
            b[i] = bytes[i + Postion];
        }
        Postion += 8;
        Array.Reverse(b);
        double dbl = BitConverter.ToDouble(b, 0);
        return dbl;
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
    public void WriteUTFBytes(string value)
    {
        byte[] bs = Encoding.UTF8.GetBytes(value);
        bytes.AddRange(bs);
    }
    public void WriteALLBytes(byte[] bs)
    {
        bytes.AddRange(bs);
    }
    public void WriteNullBytes(int num)
    {
        byte[] bs = new byte[num];
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
    public void WriteBytes(byte[] value, int offset, int length)
    {
        for (int i = 0; i < length; i++)
        {
            bytes.Add(value[i + offset]);
        }
    }
    public void WriteBytes(byte[] value)
    {
        bytes.AddRange(value);
    }
    public void WriteDouble(double v)
    {
        byte[] temp = BitConverter.GetBytes(v);
        Array.Reverse(temp);
        bytes.AddRange(temp);
    }
}