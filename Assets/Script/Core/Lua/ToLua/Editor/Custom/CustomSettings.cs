using System;
using System.Collections.Generic;
using UnityEngine;
#if USE_LUA
using BindType = ToLuaMenu.BindType;
#endif

public static class CustomSettings
{
#if USE_LUA

    public static string saveDir = LuaExportList.saveDir;
    public static string toluaBaseType = LuaExportList.toluaBaseType;

    public static List<Type> staticClassTypes = LuaExportList.staticClassTypes;
    public static DelegateType[] customDelegateList = LuaExportList.customDelegateList;
    public static BindType[] customTypeList = LuaExportList.customTypeList;
    public static List<Type> dynamicList = LuaExportList.dynamicList;
    public static List<Type> outList = LuaExportList.outList;
#endif

    //    public static string saveDir = Application.dataPath + "/Script/LuaGenerate";
    //    public static string toluaBaseType = Application.dataPath + "/Script/LuaGenerate/BaseType/";

    //    public static List<Type> staticClassTypes = new List<Type>();
    //    public static DelegateType[] customDelegateList = { };
    //    public static BindType[] customTypeList = { };
    //    public static List<Type> dynamicList = new List<Type>();
    //    public static List<Type> outList = new List<Type>();
    //#endif
}