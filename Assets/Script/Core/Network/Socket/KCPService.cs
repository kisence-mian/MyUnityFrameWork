using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KCPService : SocketService
{

    public override void Update()
    {
        base.Update();
    }

    public override void Send(byte[] sendbytes)
    {
        base.Send(sendbytes);
    }

    protected override void DealByte(byte[] data, ref int offset, int length)
    {

   

        base.DealByte(data, ref offset, length);
    }
}
