using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FrameWork.SDKInterface
{
    public class SDKInterfaceDefine
    {
        public const string ModuleName   = "ModuleName";
        public const string FunctionName = "FunctionName";
        public const string ListenerName = "ListenerName";

        public const string SDKName  = "SDKName";
        public const string SDKIndex = "SDKIndex";

        public const string ParameterName_IsSuccess = "IsSuccess";
        public const string ParameterName_Error     = "Error";
        public const string ParameterName_Content   = "Content";

        public const string ModuleName_Init     = "Init";
        public const string ModuleName_Dispose  = "Dispose";
        public const string ModuleName_Debug    = "Debug";

        public const string ModuleName_Login    = "Login";
        public const string ModuleName_Pay      = "Pay";
        public const string ModuleName_AD       = "AD";
        public const string ModuleName_Log      = "Log";
        public const string ModuleName_Other    = "Other";

        //回调方法
        public const string FunctionName_OnError = "OnError";
        public const string FunctionName_OnLog   = "OnLog";

        public const string FunctionName_OnInit = "OnInit";
        public const string FunctionName_OnLogin = "OnLogin";
        public const string FunctionName_OnLogout = "OnLogout";
        public const string FunctionName_OnPay = "OnPay";
        public const string FunctionName_OnOther = "OnOther";

        //Login相关参数
        public const string FunctionName_Logout = "Logout";

        public const string Login_ParameterName_Device = "Device";
        public const string Login_ParameterName_AccountId = "AccountId";

        //Pay相关参数
        public const string Pay_ParameterName_GoodsID     = "GoodsID";
        public const string Pay_ParameterName_GoodsType   = "GoodsType";
        public const string Pay_ParameterName_Count       = "Count";
        public const string Pay_ParameterName_GoodsName   = "GoodsName";
        public const string Pay_ParameterName_CallBackUrl = "CallBackUrl";
        public const string Pay_ParameterName_CpOrderID   = "CpOrderID";

        public const string Pay_GoodsTypeEnum_ONCE_ONLY = "ONCE_ONLY";
        public const string Pay_GoodsTypeEnum_NORMAL    = "NORMAL";
        public const string Pay_GoodsTypeEnum_RIGHTS    = "RIGHTS";

        //AD相关参数

        //Log相关参数
        public const string Log_FunctionName_Login    = "Login";
        public const string Log_FunctionName_LoginOut = "LoginOut";
        public const string Log_FunctionName_Event    = "Event";

        //Log Login相关
        public const string Log_ParameterName_AccountId = "AccountId";

        //Log Event相关
        public const string Log_ParameterName_EventID    = "EventID";
        public const string Log_ParameterName_EventLabel = "EventLabel";
        public const string Log_ParameterName_EventMap   = "EventMap";

        //Other相关参数
        public const string Other_FunctionName_Exit = "Exit";
    }
}
