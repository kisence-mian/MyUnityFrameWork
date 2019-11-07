using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.Xml;
using UnityEditor;
using UnityEngine;

public class RSAConvertWindow : EditorWindow
{

    [MenuItem("Tools/将密钥转换成.NET格式")]

    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(RSAConvertWindow));
    }

    string res = "";
    string result = "";

    void OnGUI()
    {
        titleContent.text = "密钥转换器";
        GUILayout.Label("原文");
        res = TextArea(res);
        GUILayout.Label("转换结果");
        result = TextArea(result);

        try
        {
            if (GUILayout.Button("转换公钥 Java -> net"))
            {
                result = RSAPublicKeyJava2DotNet(res);
            }

            if (GUILayout.Button("转换私钥 Java -> net"))
            {
                result = RSAPrivateKeyJava2DotNet(res);
            }

            if (GUILayout.Button("转换公钥 net -> Java"))
            {
                result = RSAPublicKeyDotNet2Java(res);
            }

            if (GUILayout.Button("转换私钥 net -> Java"))
            {
                result = RSAPrivateKeyDotNet2Java(res);
            }

            GUILayout.Space(20);

            if (GUILayout.Button("生成加密配置"))
            {
                SaveConfig();
            }
        }
        catch
        {
            result = "转换错误";
        }
    }

    void SaveConfig()
    {
        Dictionary<string, SingleField> data = new Dictionary<string, SingleField>();

        SingleField isSecret = new SingleField(true);
        SingleField publicKey = new SingleField("publickey");

        data.Add(EncryptionService.c_isSecretKey, isSecret);
        data.Add(EncryptionService.c_publickey, publicKey);

        ConfigEditorWindow.SaveData(EncryptionService.c_EncryptionConfig, data);
    }


    /// <summary>    
    /// RSA私钥格式转换，java->.net    
    /// </summary>    
    /// <param name="privateKey">java生成的RSA私钥</param>    
    /// <returns></returns>   
    public static string RSAPrivateKeyJava2DotNet(string privateKey)
    {

        RsaPrivateCrtKeyParameters privateKeyParam = (RsaPrivateCrtKeyParameters)PrivateKeyFactory.CreateKey(Convert.FromBase64String(privateKey));
        return string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent><P>{2}</P><Q>{3}</Q><DP>{4}</DP><DQ>{5}</DQ><InverseQ>{6}</InverseQ><D>{7}</D></RSAKeyValue>",
        Convert.ToBase64String(privateKeyParam.Modulus.ToByteArrayUnsigned()),
        Convert.ToBase64String(privateKeyParam.PublicExponent.ToByteArrayUnsigned()),
        Convert.ToBase64String(privateKeyParam.P.ToByteArrayUnsigned()),
        Convert.ToBase64String(privateKeyParam.Q.ToByteArrayUnsigned()),
        Convert.ToBase64String(privateKeyParam.DP.ToByteArrayUnsigned()),
        Convert.ToBase64String(privateKeyParam.DQ.ToByteArrayUnsigned()),
        Convert.ToBase64String(privateKeyParam.QInv.ToByteArrayUnsigned()),
         Convert.ToBase64String(privateKeyParam.Exponent.ToByteArrayUnsigned()));
    }
    /// <summary>    
    /// RSA私钥格式转换，.net->java    
    /// </summary>    
    /// <param name="privateKey">.net生成的私钥</param>    
    /// <returns></returns>   
    public static string RSAPrivateKeyDotNet2Java(string privateKey)
    {
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(privateKey);
        BigInteger m = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("Modulus")[0].InnerText));
        BigInteger exp = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("Exponent")[0].InnerText));
        BigInteger d = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("D")[0].InnerText));
        BigInteger p = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("P")[0].InnerText));
        BigInteger q = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("Q")[0].InnerText));
        BigInteger dp = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("DP")[0].InnerText));
        BigInteger dq = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("DQ")[0].InnerText));
        BigInteger qinv = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("InverseQ")[0].InnerText));
        RsaPrivateCrtKeyParameters privateKeyParam = new RsaPrivateCrtKeyParameters(m, exp, d, p, q, dp, dq, qinv);
        PrivateKeyInfo privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(privateKeyParam);
        byte[] serializedPrivateBytes = privateKeyInfo.ToAsn1Object().GetEncoded();
        return Convert.ToBase64String(serializedPrivateBytes);
    }
    /// <summary>    
    /// RSA公钥格式转换，java->.net    
    /// </summary>    
    /// <param name="publicKey">java生成的公钥</param>    
    /// <returns></returns>    
    public static string RSAPublicKeyJava2DotNet(string publicKey)
    {
        RsaKeyParameters publicKeyParam = (RsaKeyParameters)PublicKeyFactory.CreateKey(Convert.FromBase64String(publicKey));
        return string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent></RSAKeyValue>",
            Convert.ToBase64String(publicKeyParam.Modulus.ToByteArrayUnsigned()),
            Convert.ToBase64String(publicKeyParam.Exponent.ToByteArrayUnsigned()));
    }
    /// <summary>    
    /// RSA公钥格式转换，.net->java    
    /// </summary>    
    /// <param name="publicKey">.net生成的公钥</param>    
    /// <returns></returns>   
    public static string RSAPublicKeyDotNet2Java(string publicKey)
    {
        XmlDocument doc = new XmlDocument(); doc.LoadXml(publicKey);
        BigInteger m = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("Modulus")[0].InnerText));
        BigInteger p = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("Exponent")[0].InnerText));
        RsaKeyParameters pub = new RsaKeyParameters(false, m, p);
        SubjectPublicKeyInfo publicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(pub);
        byte[] serializedPublicBytes = publicKeyInfo.ToAsn1Object().GetDerEncoded();
        return Convert.ToBase64String(serializedPublicBytes);
    }

    public static string HandleCopyPaste(int controlID)
    {
        if (controlID == GUIUtility.keyboardControl)
        {
            if (Event.current.type == UnityEngine.EventType.KeyUp && (Event.current.modifiers == EventModifiers.Control || Event.current.modifiers == EventModifiers.Command))
            {
                if (Event.current.keyCode == KeyCode.C)
                {
                    Event.current.Use();
                    TextEditor editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
                    editor.Copy();
                }
                else if (Event.current.keyCode == KeyCode.V)
                {
                    Event.current.Use();
                    TextEditor editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
                    editor.Paste();
#if UNITY_5_3_OR_NEWER || UNITY_5_3
                    return editor.text; //以及更高的unity版本中editor.content.text已经被废弃，需使用editor.text代替
#else
                    return editor.content.text;
#endif
                }
                else if (Event.current.keyCode == KeyCode.A)
                {
                    Event.current.Use();
                    TextEditor editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
                    editor.SelectAll();
                }
            }
        }
        return null;
    }
    /// <summary>
    /// TextField复制粘贴的实现
    /// </summary>
    public static string TextField(string value, params GUILayoutOption[] options)
    {
        int textFieldID = GUIUtility.GetControlID("TextField".GetHashCode(), FocusType.Keyboard) + 1;
        if (textFieldID == 0)
            return value;

        //处理复制粘贴的操作
        value = HandleCopyPaste(textFieldID) ?? value;

        return GUILayout.TextField(value, options);
    }

    public static string TextArea(string value, params GUILayoutOption[] options)
    {
        int textFieldID = GUIUtility.GetControlID("TextField".GetHashCode(), FocusType.Keyboard) + 1;
        if (textFieldID == 0)
            return value;

        //处理复制粘贴的操作
        value = HandleCopyPaste(textFieldID) ?? value;

        return GUILayout.TextArea(value, options);
    }
}

