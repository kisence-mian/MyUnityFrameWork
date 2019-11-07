using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FrameWork;

public class EncryptionService : MonoBehaviour
{
    public const string c_EncryptionConfig = "EncryptionConfig";
    public const string c_publickey = "publickey";
    public const string c_isSecretKey  = "isSecret";

    static bool isInit = false;
    static bool isSecret = false;
    static string publickey = "";

    public static bool IsSecret
    {
        get
        {
            Init();
            return isSecret;
        }

        set
        {
            isSecret = value;
        }
    }

    //加密方法  
    public static string Encrypt(string msg)
    {
        //随机生成key
        string key = Random.Range(10000000, 99999999).ToString().Substring(0, 8);

        //用key加密报文
        string securityData = DESservice.Encode(msg, key);

        //加密key
        string securityKey = RSAService.encriptByPublicKey(key, publickey);

        //Debug.Log("-----------------------------------------------");
        //Debug.Log(key + "  length： " + key.Length);
        //Debug.Log(publickey);

        //Debug.Log(securityKey);
        //Debug.Log("-----------------------------------------------");

        //返回string
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("securityKey", securityKey);
        data.Add("securityData", securityData);

        return Json.Serialize(data);
    }

    //解密方法  
    public static string Decrypt(string msg)
    {
        //解析json
        Dictionary<string, object> json = (Dictionary<string, object>)Json.Deserialize(msg);

        //如果未加密直接返回
        if (!json.ContainsKey("sign"))
        {
            return msg;
        }

        string sign = (string)json["sign"];
        string conetnt = (string)json["msg"];

        //验证签名
        if (RSAService.VerifySignedHash(conetnt, sign, publickey))
        {
            return conetnt;
        }
        else
        {
            throw new System.Exception("签名验证失败 " + msg);
        }
    }

    public static void Init()
    {
        if (!isInit)
        {
            isInit = true;

            if(ConfigManager.GetIsExistConfig(c_EncryptionConfig))
            {
                isSecret = ConfigManager.GetData(c_EncryptionConfig, c_isSecretKey).GetBool();
                publickey = ConfigManager.GetData(c_EncryptionConfig, c_publickey).GetString();
            }
            else
            {
                isSecret = false;
            }
        }
    }
}
