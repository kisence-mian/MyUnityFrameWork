
using System.Collections;
using UnityEngine;

public class MsgCompressGzip : MsgCompressBase
{
    public override byte[] CompressBytes(byte[] data)
    {
       
        return ZipUtils.CompressBytes(data);
    }

    public override byte[] DecompressBytes(byte[] data)
    {
        //Debug.Log("DecompressBytes");
        return ZipUtils.DecompressBytes(data);
    }

    public override string GetCompressType()
    {
        return "gzip";
    }
 

    
}
