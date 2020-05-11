using System;
using UnityEngine;

namespace LiteNetLib.Utils
{
    public static class NetDataReaderExtension
    {
        public static object GetValue(this NetDataReader reader, Type type)
        {
            #region Generic Values
            if (type == typeof(bool))
                return reader.GetBool();

            if (type == typeof(bool[]))
                return reader.GetBoolArray();

            if (type == typeof(byte))
                return reader.GetByte();

            if (type == typeof(byte[]))
                return reader.GetBytesWithLength();

            if (type == typeof(char))
                return reader.GetChar();

            if (type == typeof(double))
                return reader.GetDouble();

            if (type == typeof(double[]))
                return reader.GetDoubleArray();

            if (type == typeof(float))
                return reader.GetFloat();

            if (type == typeof(float[]))
                return reader.GetFloatArray();

            if (type == typeof(int))
                return reader.GetInt();

            if (type == typeof(int[]))
                return reader.GetIntArray();

            if (type == typeof(long))
                return reader.GetLong();

            if (type == typeof(long[]))
                return reader.GetLongArray();

            if (type == typeof(sbyte))
                return reader.GetSByte();

            if (type == typeof(short))
                return reader.GetShort();

            if (type == typeof(short[]))
                return reader.GetShortArray();

            if (type == typeof(string))
                return reader.GetString();

            if (type == typeof(uint))
                return reader.GetUInt();

            if (type == typeof(uint[]))
                return reader.GetUIntArray();

            if (type == typeof(ulong))
                return reader.GetULong();

            if (type == typeof(ulong[]))
                return reader.GetULongArray();

            if (type == typeof(ushort))
                return reader.GetUShort();

            if (type == typeof(ushort[]))
                return reader.GetUShortArray();
            #endregion

            #region Unity Values
            if (type == typeof(Color))
                return reader.GetColor();

            if (type == typeof(Quaternion))
                return reader.GetQuaternion();

            if (type == typeof(Vector2))
                return reader.GetVector2();

            if (type == typeof(Vector2Int))
                return reader.GetVector2Int();

            if (type == typeof(Vector3))
                return reader.GetVector3();

            if (type == typeof(Vector3Int))
                return reader.GetVector3Int();

            if (type == typeof(Vector4))
                return reader.GetVector4();
            #endregion

            if (typeof(INetSerializable).IsAssignableFrom(type))
            {
                object instance = Activator.CreateInstance(type);
                (instance as INetSerializable).Deserialize(reader);
                return instance;
            }

            throw new ArgumentException("NetDataReader cannot read type " + type.Name);
        }

        public static Color GetColor(this NetDataReader reader)
        {
            float r = reader.GetByte() * 0.01f;
            float g = reader.GetByte() * 0.01f;
            float b = reader.GetByte() * 0.01f;
            float a = reader.GetByte() * 0.01f;
            return new Color(r, g, b, a);
        }

        public static Quaternion GetQuaternion(this NetDataReader reader)
        {
            Vector3 vector3 = new Vector3(reader.GetFloat(), reader.GetFloat(), reader.GetFloat());
            return vector3.magnitude <= 0 ? Quaternion.identity : Quaternion.Euler(vector3);
        }

        public static Vector2 GetVector2(this NetDataReader reader)
        {
            return new Vector2(reader.GetFloat(), reader.GetFloat());
        }

        public static Vector2Int GetVector2Int(this NetDataReader reader)
        {
            return new Vector2Int(reader.GetInt(), reader.GetInt());
        }

        public static Vector3 GetVector3(this NetDataReader reader)
        {
            return new Vector3(reader.GetFloat(), reader.GetFloat(), reader.GetFloat());
        }

        public static Vector3Int GetVector3Int(this NetDataReader reader)
        {
            return new Vector3Int(reader.GetInt(), reader.GetInt(), reader.GetInt());
        }

        public static Vector4 GetVector4(this NetDataReader reader)
        {
            return new Vector4(reader.GetFloat(), reader.GetFloat(), reader.GetFloat(), reader.GetFloat());
        }

        #region Packed Unsigned Int (Credit: https://sqlite.org/src4/doc/trunk/www/varint.wiki)
        public static ushort GetPackedUShort(this NetDataReader reader)
        {
            return (ushort)GetPackedULong(reader);
        }

        public static uint GetPackedUInt(this NetDataReader reader)
        {
            return (uint)GetPackedULong(reader);
        }

        public static ulong GetPackedULong(this NetDataReader reader)
        {
            byte a0 = reader.GetByte();
            if (a0 < 241)
            {
                return a0;
            }

            byte a1 = reader.GetByte();
            if (a0 >= 241 && a0 <= 248)
            {
                return 240 + 256 * (a0 - ((ulong)241)) + a1;
            }

            byte a2 = reader.GetByte();
            if (a0 == 249)
            {
                return 2288 + (((ulong)256) * a1) + a2;
            }

            byte a3 = reader.GetByte();
            if (a0 == 250)
            {
                return a1 + (((ulong)a2) << 8) + (((ulong)a3) << 16);
            }

            byte a4 = reader.GetByte();
            if (a0 == 251)
            {
                return a1 + (((ulong)a2) << 8) + (((ulong)a3) << 16) + (((ulong)a4) << 24);
            }

            byte a5 = reader.GetByte();
            if (a0 == 252)
            {
                return a1 + (((ulong)a2) << 8) + (((ulong)a3) << 16) + (((ulong)a4) << 24) + (((ulong)a5) << 32);
            }

            byte a6 = reader.GetByte();
            if (a0 == 253)
            {
                return a1 + (((ulong)a2) << 8) + (((ulong)a3) << 16) + (((ulong)a4) << 24) + (((ulong)a5) << 32) + (((ulong)a6) << 40);
            }

            byte a7 = reader.GetByte();
            if (a0 == 254)
            {
                return a1 + (((ulong)a2) << 8) + (((ulong)a3) << 16) + (((ulong)a4) << 24) + (((ulong)a5) << 32) + (((ulong)a6) << 40) + (((ulong)a7) << 48);
            }

            byte a8 = reader.GetByte();
            if (a0 == 255)
            {
                return a1 + (((ulong)a2) << 8) + (((ulong)a3) << 16) + (((ulong)a4) << 24) + (((ulong)a5) << 32) + (((ulong)a6) << 40) + (((ulong)a7) << 48) + (((ulong)a8) << 56);
            }
            throw new System.IndexOutOfRangeException("ReadPackedULong() failure: " + a0);
        }
        #endregion
    }
}
