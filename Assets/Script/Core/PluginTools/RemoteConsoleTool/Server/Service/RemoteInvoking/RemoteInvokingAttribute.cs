using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace GameConsoleController
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class RemoteInvokingAttribute : Attribute
    {
        public const string MethodType_Custom = "Custom";
        public const string MethodType_System = "System";

        private string m_methodType;
        /// <summary>
        /// 调用方法分类
        /// </summary>
        public string methodType{ get
            {
                if (string.IsNullOrEmpty(m_methodType))
                    return MethodType_Custom;
                else
                    return m_methodType;
            }
            set
            {
                m_methodType = value;
            }
        }
        public string name { get; set; }
        public string description { get; set; }

         //public ParamsDescription[] paramsDescription { get; set; }

        //public RemoteInvokingAttribute()
        //{
        //    this.methodType = MethodType_Custom;
        //}
        //public RemoteInvokingAttribute(string name)
        //{
        //    this.name = name;
        //}
        //public RemoteInvokingAttribute(string m_methodType, string name, string description, ParamsDescription[] paramsDescription)
        //{
        //    this.m_methodType = m_methodType;
        //    this.name = name;
        //    this.description = description;
        //    this.paramsDescription = paramsDescription;
        //}
        //public RemoteInvokingAttribute(string name, ParamsDescription[] paramsDescription)
        //{
        //    this.name = name;
        //    this.paramsDescription = paramsDescription;
        //}

        //public ParamsDescription GetParamsDescription(string paramName)
        //{
        //    if (paramsDescription != null)
        //    {
        //        foreach (var item in paramsDescription)
        //        {
        //            if(item.paramName == paramName)
        //            {
        //                return item;
        //            }
        //        }
        //    }
        //    return null;
        //}
    }
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class ParamsDescriptionAttribute : Attribute
    {
        public string paramName { get; set; }
        /// <summary>
        /// 自定义显示名称
        /// </summary>
        public string paramsDescriptionName { get; set; }
        /// <summary>
        /// 自定义参数默认值（只支持基础类型参数(int，float,bool,等等)）
        /// </summary>
        public string getDefaultValueMethodName { get; set; }
        public string[] selectItemValues { get; set; }

        public static ParamsDescriptionAttribute GetParamsDescription(IEnumerable<ParamsDescriptionAttribute> paramsDescription, string paramName)
        {
            if (paramsDescription != null)
            {
                foreach (var item in paramsDescription)
                {
                    if (item.paramName == paramName)
                    {
                        return item;
                    }
                }
            }
            return null;
        }
        //public ParamsDescription(string paramName,string paramsDescriptionName, string[] selectItemValues=null)
        //{
        //    this.paramName = paramName;
        //    this.paramsDescriptionName = paramsDescriptionName;
        //    this.selectItemValues = selectItemValues;
        //}
        //public ParamsDescription(string paramName, string[] selectItemValues = null)
        //{
        //    this.paramName = paramName;
        //    this.selectItemValues = selectItemValues;
        //}
    }
}
