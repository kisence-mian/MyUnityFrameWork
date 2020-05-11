using UnityEngine;
using System.Collections;
namespace LiteNetLibManager
{
    public abstract class PlayerLoginHandlerBase 
    {
        public abstract uint LoginLogic(Login2Server msg, long connectId, out Player player);

    }
}
