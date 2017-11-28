using NUnit.Framework;
using Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ProtocolMsgTest
{
    [Test(Description = "Protocol 正确性测试")]
    public void LegitimateTest()
    {
        List<Type> ModuleList = new List<Type>();
        List<Type> msgList = new List<Type>();
        List<Type> StructList = new List<Type>();


        msgList.Clear();

        Type[] types = Assembly.Load("Assembly-CSharp").GetTypes();

        for (int i = 0; i < types.Length; i++)
        {
            if (typeof(IProtocolMessageInterface).IsAssignableFrom(types[i])
                && types[i] != typeof(IProtocolMessageInterface)
                && types[i] != typeof(CsharpProtocolInterface)
                && !types[i].IsAbstract
                )
            {
                msgList.Add(types[i]);
            }

            if (typeof(IProtocolMessageInterface).IsAssignableFrom(types[i])
                && types[i].IsAbstract

                )
            {
                ModuleList.Add(types[i]);
            }
        }

        StructList.Clear();

        for (int i = 0; i < types.Length; i++)
        {
            if (typeof(IProtocolStructInterface).IsAssignableFrom(types[i])
                && types[i] != typeof(IProtocolStructInterface))
            {
                StructList.Add(types[i]);
            }
        }


        for (int i = 0; i < msgList.Count; i++)
        {
            if(!Verify(msgList[i], new List<Type>()))
            {
                Assert.Fail(" >" + msgList[i].FullName + "< is not legitimate Protocol Type !");
            }

            //Debug.Log("--------------------------");
        }

        for (int i = 0; i < StructList.Count; i++)
        {
            if (!Verify(StructList[i], new List<Type>()))
            {
                Assert.Fail(" >" + StructList[i].FullName + "< is not legitimate Protocol Struct Type !");
            }

            //Debug.Log("--------------------------");
        }
    }

    public bool Verify(Type type ,List<Type> list)
    {
        if(isBaseType(type))
        {
            return true;
        }

        if (type.Name == typeof(List<>).Name)
        {
            type = type.GetGenericArguments()[0];
        }

        if (list.Contains(type))
        {
            Debug.Log("repetition Type is " + type.FullName);
            return false;
        }

        List<Type> listTmp = new List<Type>();
        for (int i = 0; i < list.Count; i++)
        {
            listTmp.Add(list[i]);
        }

        listTmp.Add(type);

        FieldInfo[] fields = type.GetFields();

        for (int i = 0; i < fields.Length; i++)
        {
            if (!fields[i].IsStatic &&!Verify(fields[i].FieldType, listTmp))
            {
                return false;
            }
        }

        return true;
    }

    bool isBaseType(Type type)
    {
        if (type == typeof(int))
        {
            return true;
        }
        else if (type == typeof(bool))
        {
            return true;
        }

        else if (type == typeof(float))
        {
            return true;
        }

        else if (type.IsSubclassOf(typeof(Enum)))
        {
            return true;
        }

        else if (type == typeof(string))
        {
            return true;
        }

        return false;
    }
}
