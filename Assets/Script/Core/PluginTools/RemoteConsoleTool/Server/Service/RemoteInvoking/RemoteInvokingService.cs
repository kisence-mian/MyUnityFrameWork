using UnityEngine;
using System.Collections;
using System.Reflection;
using System;
using System.Collections.Generic;
using LiteNetLibManager;

namespace GameConsoleController
{
    public class RemoteInvokingService : CustomServiceBase
    {
        public override string FunctionName
        {
            get
            {
                return "RemoteInvoking";
            }
        }

        private Dictionary<Type, List<MethodInfo>> invokeMothodsInfos = new Dictionary<Type, List<MethodInfo>>();
        public override void OnStart()
        {
            invokeMothodsInfos.Clear();

            Assembly asm = Assembly.GetAssembly(typeof(RemoteInvokingAttribute));
            Type[] types = asm.GetExportedTypes();

            foreach (var t in types)
            {
                MethodInfo[] infos = t.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                foreach (var m in infos)
                {
                    RemoteInvokingAttribute attribute = GetCustomAttribute<RemoteInvokingAttribute>(m);
                    if (attribute != null)
                    {
                        //Debug.Log("RemoteInvokingAttribute=========>" + t.FullName+"."+m.Name);
                        AddMethodInfo(t, m);
                    }
                }

            }

            msgManager.RegisterMessage<UseMethod2Server>(OnRemoteInvokingEvent);
        }

        private void OnRemoteInvokingEvent(NetMessageHandler msgHandler)
        {
            UseMethod2Server msg = msgHandler.GetMessage<UseMethod2Server>();
            //Debug.Log("Server接收到UseMethod2Server：" + JsonUtils.ToJson(msg));
            int code = 0;
            string error="";
            try
            {
                MethodInfo mInfo = null;
                foreach (var mData in invokeMothodsInfos)
                {
                    if (mData.Key.FullName == msg.classFullName)
                    {
                        List<MethodInfo> methods = mData.Value;

                        foreach (var m in methods)
                        {
                            if (m.Name == msg.methodName)
                            {
                                mInfo = m;
                                break;
                            }
                        }
                    }
                }

                if (mInfo != null)
                {
                    List<object> pValues = new List<object>();
                    ParameterInfo[] parameters = mInfo.GetParameters();
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        ParameterInfo p = parameters[i];
                        object v = SimpleJsonUtils.FromJson(p.ParameterType,msg.paramNameValues[p.Name]);
                        pValues.Add(v);
                    }
                    mInfo.Invoke(null, pValues.ToArray());
                }
                else
                {
                    code = -2;
                }
            }
            catch (Exception e)
            {
                code = -1;
                error = e.ToString();
                Debug.LogError(e);
            }
            UseMethod2Client toMsg = new UseMethod2Client();
            toMsg.code = code;
            toMsg.error = error;
            netManager.Send(msgHandler.player, toMsg);
            //Debug.Log("发送UseMethod2Client：" + JsonUtils.ToJson(toMsg));
        }

        private void AddMethodInfo(Type classType, MethodInfo method)
        {
            if (invokeMothodsInfos.ContainsKey(classType))
            {
                invokeMothodsInfos[classType].Add(method);
            }
            else
            {
                invokeMothodsInfos.Add(classType, new List<MethodInfo>() { method });
            }
        }

        protected override void OnFunctionClose()
        {
            
        }

        protected override void OnFunctionOpen()
        {
            
        }

        private T GetCustomAttribute<T>(MethodInfo info) where T : Attribute
        {
            List<T> list = GetCustomAttributes<T>(info);
            if (list.Count > 0)
                return list[0];
            return null;
        }
            private List<T> GetCustomAttributes<T>(MethodInfo info) where T: Attribute
        {
            object[] tempArr = info.GetCustomAttributes(typeof(T), false);
            List<T> list = new List<T>();
            if (tempArr == null)
                return list;
            foreach (var item in tempArr)
            {
                list.Add((T)item);
            }
            return list;
        }
        protected override void OnPlayerLogin(LiteNetLibManager.Player player)
        {
            foreach (var mData in invokeMothodsInfos)
            {
                List<MethodInfo> methods = mData.Value;

                foreach (MethodInfo m in methods)
                {
                    RemoteInvokingAttribute attribute = GetCustomAttribute<RemoteInvokingAttribute>(m);
                    MethodData2Client msg = new MethodData2Client();
                   
                   
                    msg.data.methodType = attribute.methodType;
                    
                    msg.data.showName = attribute.name;
                    msg.data.description = attribute.description;
                    msg.data.classFullName = mData.Key.FullName;
                    msg.data.methodName = m.Name;

                    ParameterInfo[] parameters= m.GetParameters();

                    IEnumerable< ParamsDescriptionAttribute> paramsDescriptions= GetCustomAttributes<ParamsDescriptionAttribute>(m);
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        ParameterInfo p = parameters[i];
                        ParamsData paramsData = new ParamsData();

                        ParamsDescriptionAttribute paramsDescription = ParamsDescriptionAttribute.GetParamsDescription(paramsDescriptions,p.Name); ;
                        if(paramsDescription != null)
                        {
                            paramsData.descriptionName = paramsDescription.paramsDescriptionName;
                        }

                        paramsData.paraName = p.Name;
                        paramsData.paraTypeFullName = p.ParameterType.FullName;
                        try
                        {
                            paramsData.defaultValueStr = GetDefaultValueString(mData.Key, p.ParameterType, paramsDescription);
                        }
                        catch (Exception e)
                        {

                            Debug.LogError(mData.Key.FullName + "." + m.Name + "\n" + e);
                        }
                       
                        paramsData.selectItemValues = GetTypeSelectItemValues(p.ParameterType,paramsDescription);
                        msg.data.paramsDatas.Add(paramsData);
                    }
                    Debug.Log("Send MethodData2Client:" + JsonUtility.ToJson(msg));
                    netManager.Send(player, msg);
                }
            }
        }

        private string GetDefaultValueString(Type classType, Type parameterType, ParamsDescriptionAttribute paramsDescription)
        {
            object value = null;
            if(paramsDescription!=null &&!string.IsNullOrEmpty( paramsDescription.getDefaultValueMethodName))
            {
                MethodInfo info = classType.GetMethod(paramsDescription.getDefaultValueMethodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

                if (info != null)
                {
                    Type returnType = info.ReturnType;
                    if(returnType != parameterType)
                    {
                        Debug.LogError("GetDefaultValueString Method:" + classType.FullName + "." + paramsDescription.getDefaultValueMethodName + " ReturnType(" + returnType.FullName + ") is different from parameterType(" + parameterType + ")");
                    }
                    else
                    {
                        try
                        {
                           value= info.Invoke(null,new object[0]);
                        }
                        catch (Exception e)
                        {
                            Debug.LogError(e);
                        }
                    }
                }
            }
            if(value==null)
            {
                Debug.Log("CreateDefultInstance Type:" + parameterType);
                value = ReflectionTool.CreateDefultInstance(parameterType);
            }

            return SimpleJsonUtils.ToJson(value);
        }

        private string[] GetTypeSelectItemValues(Type type, ParamsDescriptionAttribute paramsDescription)
        {
           if(type.IsArray || type.Name == typeof(List<>).Name)
            {
                Type itemType = null;
                if (type.IsArray)
                {
                    itemType= type.GetElementType();
                }
                else
                {
                    itemType = type.GetGenericArguments()[0];
                }
                return GetSimpleTypeSelectItem(itemType, paramsDescription);
            }
            else
            {
                return GetSimpleTypeSelectItem(type, paramsDescription);
            }
        }
        private string[] GetSimpleTypeSelectItem(Type type, ParamsDescriptionAttribute paramsDescription)
        {
            if (type == typeof(bool))
            {
                return new string[] { "true", "false" };
            }
            else if (type.IsEnum)
            {
                return Enum.GetNames(type);
            }
            else if (type == typeof(string))
            {
                if (paramsDescription != null && paramsDescription.selectItemValues != null)
                {
                    return paramsDescription.selectItemValues;
                }
            }

            return new string[0];
        }
    }
}
