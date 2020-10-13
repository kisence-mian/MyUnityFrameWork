using UnityEngine;
using System.Collections;
using SimpleNetCore;

namespace SimpleNetManager
{
    public abstract class PlayerLoginHandlerBase 
    {
        public abstract uint LoginLogic(Login2Server msg, Session session, out Player player);

    }
}
