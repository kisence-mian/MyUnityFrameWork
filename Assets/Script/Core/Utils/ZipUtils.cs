
using System;
using System.Collections;
using System.IO;
using System.IO.Compression;
using System.Text;

public class ZipUtils 
{
    public static string CompressString(string str)
    {
        var compressBeforeByte = Encoding.GetEncoding("UTF-8").GetBytes(str);
        var compressAfterByte = CompressBytes(compressBeforeByte);
        string compressString = Convert.ToBase64String(compressAfterByte);
        return compressString;
    }

    public static string DecompressString(string str)
    {
        var compressBeforeByte = Convert.FromBase64String(str);
        var compressAfterByte = DecompressBytes(compressBeforeByte);
        string compressString = Encoding.GetEncoding("UTF-8").GetString(compressAfterByte);
        return compressString;
    }

    /// <summary>
    /// Compress
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static byte[] CompressBytes(byte[] data)
    {
        try
        {
            var ms = new MemoryStream();
            var zip = new GZipStream(ms, CompressionMode.Compress, true);
            zip.Write(data, 0, data.Length);
            zip.Close();
            var buffer = new byte[ms.Length];
            ms.Position = 0;
            ms.Read(buffer, 0, buffer.Length);
            ms.Close();
            return buffer;

        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    /// <summary>
    /// Decompress
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static byte[] DecompressBytes(byte[] data)
    {
        try
        {
            var ms = new MemoryStream(data);
            var zip = new GZipStream(ms, CompressionMode.Decompress, true);
            var msreader = new MemoryStream();
            var buffer = new byte[0x1000];
            while (true)
            {
                var reader = zip.Read(buffer, 0, buffer.Length);
                if (reader <= 0)
                {
                    break;
                }
                msreader.Write(buffer, 0, reader);
            }
            zip.Close();
            ms.Close();
            msreader.Position = 0;
            buffer = msreader.ToArray();
            msreader.Close();
            return buffer;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
}
