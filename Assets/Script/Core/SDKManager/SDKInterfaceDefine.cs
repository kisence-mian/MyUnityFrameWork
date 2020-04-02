using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FrameWork.SDKManager
{
    public class SDKInterfaceDefine
    {
        public const string ModuleName   = "ModuleName";
        public const string FunctionName = "FunctionName";
        public const string ListenerName = "ListenerName";

        public const string SDKName  = "SDKName";
        public const string SDKIndex = "SDKIndex";
        public const string Tag = "Tag";

        public const string ParameterName_IsSuccess = "IsSuccess";
        public const string ParameterName_Error     = "Error";
        public const string ParameterName_Content   = "Content";
        public const string ParameterName_UserID    = "UserID";

        public const string ModuleName_Init     = "Init";
        public const string ModuleName_Dispose  = "Dispose";
        public const string ModuleName_Debug    = "Debug";

        public const string ModuleName_Login    = "Login";
        public const string ModuleName_Pay      = "Pay";
        public const string ModuleName_AD       = "AD";
        public const string ModuleName_Log      = "Log";
        public const string ModuleName_Other    = "Other";
        public const string ModuleName_LifeCycle = "LifeCycle";
        public const string ModuleName_RealName = "RealName";

        //回调方法
        public const string FunctionName_OnError = "OnError";
        public const string FunctionName_OnLog   = "OnLog";

        public const string FunctionName_OnInit = "OnInit";
        public const string FunctionName_OnLogin = "OnLogin";
        public const string FunctionName_OnLogout = "OnLogout";
        public const string FunctionName_OnPay = "OnPay";
        public const string FunctionName_OnOther = "OnOther";

        //LifeCycle 相关参数
        public const string LifeCycle_FunctionName_OnApplicationQuit = "OnApplicationQuit";

        //Login相关参数
        public const string Login_FunctionName_Login    = "Login";
        public const string Login_FunctionName_LoginOut = "LoginOut";

        public const string Login_ParameterName_Device = "Device";
        public const string Login_ParameterName_AccountId = "AccountId";
        public const string Login_ParameterName_loginPlatform = "loginPlatform";
        public const string Login_ParameterName_NickName = "NickName";
        public const string Login_ParameterName_HeadPortrait = "HeadPortrait";

        //Pay相关参数
        public const string Pay_FunctionName_OnPay = "OnPay";//支付回调
        public const string Pay_FunctionName_GetGoodsInfo = "GetGoodsInfo";//获取商品信息回调
        public const string Pay_FunctionName_ClearPurchase = "ClearPurchase";//擦除购买记录（已正常发货）
        public const string Pay_ParameterName_GoodsID     = "GoodsID";
        public const string Pay_ParameterName_GoodsType   = "GoodsType";
        public const string Pay_ParameterName_Count       = "Count";
        public const string Pay_ParameterName_GoodsName   = "GoodsName";
        public const string Pay_ParameterName_GoodsDescription = "GoodsDescription";
        public const string Pay_ParameterName_CallBackUrl = "CallBackUrl";
        public const string Pay_ParameterName_CpOrderID   = "CpOrderID";//第三方支付ID
        public const string Pay_ParameterName_OrderID     = "OrderID";  //我们自己的支付ID
        public const string Pay_ParameterName_PrepayID    = "PrepayID "; //预支付订单id
        public const string Pay_ParameterName_Price       = "Price";  //价格
        public const string Pay_ParameterName_Currency    = "Currency";  //货币
        public const string Pay_ParameterName_Payment     = "Payment";   //支付途径
        public const string Pay_ParameterName_Receipt     = "Receipt";   //支付回执
        public const string Pay_ParameterName_LocalizedPriceString = "localizedPriceString";//本地货币类型与数目

        public const string Pay_GoodsTypeEnum_ONCE_ONLY = "ONCE_ONLY";
        public const string Pay_GoodsTypeEnum_NORMAL    = "NORMAL";
        public const string Pay_GoodsTypeEnum_RIGHTS    = "RIGHTS";

        //AD相关参数
        public const string AD_ParameterName_ADType = "ADType";
        public const string AD_ParameterName_ADResult = "ADResult";   //广告播放结果

        public const string AD_FunctionName_LoadAD = "LoadAD";
        public const string AD_FunctionName_PlayAD = "PlayAD";
        public const string AD_FunctionName_CloseAD = "CloseAD";
        public const string AD_FunctionName_ADIsLoaded = "ADIsLoaded";
        public const string AD_FunctionName_OnAD = "OnAD";


        //Log相关参数
        public const string Log_FunctionName_Login    = "LogLogin";
        public const string Log_FunctionName_LoginOut = "LogLoginOut";
        public const string Log_FunctionName_Event    = "LogEvent";
        public const string Log_FunctionName_LogPay   = "LogPay";
        public const string Log_FunctionName_LogPaySuccess = "LogPaySuccess";
        public const string Log_FunctionName_RewardVirtualCurrency = "LogRewardVirtualCurrency"; //奖励虚拟币
        public const string Log_FunctionName_PurchaseVirtualCurrency = "LogPurchaseVirtualCurrency";//消费虚拟币
        public const string Log_FunctionName_UseItem = "LogUseItem";//使用虚拟物品（通过虚拟币购买的）

        //Log Login相关
        public const string Log_ParameterName_AccountId = "AccountId";

        //Log VirtualCurrency相关
        public const string Log_ParameterName_RewardReason = "RewardReason";

        //Log Event相关
        public const string Log_ParameterName_EventID    = "EventID";
        public const string Log_ParameterName_EventLabel = "EventLabel";
        public const string Log_ParameterName_EventMap   = "EventMap";

        //realName相关
        public const string RealName_FunctionName_GetRealNameType = "GetRealNameType";//获得实名认证状态
        public const string RealName_FunctionName_IsAdult ="IsAdult";//是否成年
        public const string RealName_FunctionName_LogPayAmount ="LogPayAmount";//上报支付金额
        public const string RealName_FunctionName_CheckPayLimit ="CheckPayLimit";//是否支付受限制
        public const string RealName_FunctionName_GetTodayOnlineTime ="GetTodayOnlineTime";//获取今日在线时长
        public const string RealName_FunctionName_StartRealNameAttestation = "StartRealNameAttestation";//开始实名认证
        public const string RealName_FunctionName_RealNameCallBack="RealNameCallBack";//实名认证完成后的回调
        public const string RealName_ParameterName_RealNameStatus ="RealNameStatus";//实名认证的状态
        public const string RealName_ParameterName_IsAdult = "IsAdult";//是否是成年人
        public const string RealName_ParameterName_PayAmount = "PayAmount";//支付金额

        //Other相关参数
        public const string Other_FunctionName_Exit = "Exit";

        //Other -> 剪贴板
        public const string Other_FunctionName_CopyToClipboard = "CopyToClipboard";
        public const string Other_FunctionName_CopyFromClipboard = "CopyFromClipboard";
        public const string Other_ParameterName_Content = "Content";

        //Other -> 热更新安装包
        public const string Other_FunctionName_DownloadAPK = "DownloadAPK";
        public const string Other_FunctionName_GetAPKSize = "GetAPKSize";
        public const string Other_ParameterName_DownloadURL = "DownloadURL";
        public const string Other_ParameterName_Progress = "Progress";
        public const string Other_ParameterName_TotalProgress = "TotalProgress";
        public const string Other_ParameterName_Size = "Size";

        //Properties
        public const string FileName_ChannelProperties = "Channel";
        public const string PropertiesKey_IsLog = "IsLog";                           //是否输出日志
        public const string PropertiesKey_SelectNetworkPath = "SelectNetworkPath";  //选服配置下载地址
        public const string PropertiesKey_UpdateDownLoadPath = "UpdateDownLoadPath"; //热更新下载地址(最后没有斜线)
        public const string PropertiesKey_TestUpdateDownLoadPath = "TestUpdateDownLoadPath"; //热更新下载测试地址(最后没有斜线)
        public const string PropertiesKey_ChannelName = "ChannelName";               //渠道名称
        public const string PropertiesKey_StoreName   = "StoreName";                 //商店名称，如果有支付方式用@进行分割
        public const string PropertiesKey_LoginPlatform = "LoginPlatform";           //登录平台
        public const string PropertiesKey_ADPlatform = "ADPlatform";                 //广告平台
        public const string PropertiesKey_NetworkID = "NetworkID";                   //服务器选择
        public const string PropertiesKey_QQGroup   = "QQGroup";                     //玩家QQ群
        public const string PropertiesKey_DirectlyLogin = "DirectlyLogin";           //是否直接登录，不选择登录模式
        public const string PropertiesKey_CloseHealthGamePact = "CloseHealthGamePact";//关闭健康游戏公约
        public const string PropertiesKey_OpenRealName = "OpenRealName";              // 开启实名认证
        public const string PropertiesKey_IsExamine = "IsExamine";                    //审核版本
        public const string PropertiesKey_RedeemCode = "RedeemCode";                  //兑换码

        public const string PropertiesKey_SelectServerURL = "SelectServerURL";       //选服服务器地址(废弃)
    }
}
