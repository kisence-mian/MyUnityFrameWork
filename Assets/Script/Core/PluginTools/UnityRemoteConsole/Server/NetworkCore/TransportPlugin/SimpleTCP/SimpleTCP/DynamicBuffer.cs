using UnityEngine;
using System.Collections;
using System;

namespace SimpleTCP
{
    public class DynamicBuffer
    {
        public int Length { get; internal set; }
        protected byte[] _data;
        private int originalSize;
        public DynamicBuffer(int size)
        {
            originalSize = size;
            _data = new byte[size];
        }
        public void WriteBuffer(byte[] data, int offset, int count)
        {
            try
            {
                ResizeIfNeed(count+ Length);
            }
            catch (Exception e)
            {
                Logger.LogError(" ResizeIfNeed DynamicBuffer.Length:" + Length + " _data.Lengh:" + _data.Length + " offset:" + offset + " count:" + count + " data.Lengh:" + data.Length + "\n" + e.ToString());
            }
           
            try
            {
              
                Buffer.BlockCopy(data, offset, _data, Length, count);
                Length += count;

            }
            catch (Exception e)
            {
                Logger.LogError("DynamicBuffer.Length:" + Length + " _data.Lengh:" + _data.Length + " offset:" + offset + " count:" + count + " data.Lengh:" + data.Length + "\n" + e.ToString());
            }
        }
        public  bool ReadBuffer(byte[] data, int offset, int size)
        {
            try
            {
                Buffer.BlockCopy(_data, offset, data, 0, size);

            }
            catch (Exception e)
            {
                Logger.LogError("DynamicBuffer.Length:"+ Length+ " _data.Lengh:"+_data.Length+ " offset:"+ offset+ " size:"+ size+ " data.Lengh:"+data.Length+"\n"+ e.ToString());
                return false;
            }
            return true;
        }

        public void Clear()
        {
            Length = 0;
        }

        public void Remove(int size)
        {
            if (Length <= size)
            {
                Length = 0;
            }
            else
            {
                byte[] tempArr = new byte[Length - size];
                ReadBuffer(tempArr, size, tempArr.Length);
                _data = tempArr;
                Length = Length - size;
            }
        }

        private void ResizeIfNeed(int newSize)
        {
            int len = _data.Length;
            if (len < newSize)
            {
                while (len < newSize)
                    len *= 2;
                Array.Resize(ref _data, len);
            }
        }
    }
}
