using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonLuaHelper 
{
	public static string Dict2Json(object data)
    {
        return FrameWork.Json.Serialize(data);
    }

    public static Dictionary<string,object> Json2Dict(string content)
    {
        return (Dictionary<string, object>)FrameWork.Json.Deserialize(content);
    }

    public static void SendMessage(string content)
    {
        NetworkManager.SendMessage(Json2Dict(content));
    }
}
