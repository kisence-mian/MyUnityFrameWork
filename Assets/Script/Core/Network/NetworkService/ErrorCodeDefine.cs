

public class ErrorCodeDefine
{

    public const int Success = 0;

    //==============登录================//
    /**
	 * 错误的登录密码
	 */
    public const int Login_WrongAccountOrPassword = 20000;
    /**
	 * 登录验证失败
	 */
    public const int Login_VerificationFailed = 20001;
    /**
	 * 异地重复登录
	 */
    public const int Login_OtherPlaceLogin = 20002;

    //=================账号合并=============//
    /**
	 * 不能绑定自己
	 */
    public const int AccountMerge_CantBindSelf = 20100;
    /**
	 * 该账号已经绑定了
	 */
    public const int AccountMerge_AccountAlreadyBind = 20101;
    /**
	 * 该账号已经绑定当前登录类型
	 */
    public const int AccountMerge_LoginTypeAlreadyBind = 20102;
    /**
	 * 对方账号已经绑定当前登录类型
	 */
    public const int AccountMerge_LoginTypeAlreadyBeBind = 20104;
    /**
	 * 对应要绑定的账户不存在
	 */
    public const int AccountMerge_NoUser = 20103;

    //=================商店功能===========//

    /**
	 * 服务端支付成功游戏逻辑未实现
	 */
    public const int StroePay_NoGameLogic = 20200;

    /**
	 * 商店商品在配置中找不到
	 */
    public const int StroePay_ConfigError = 20201;
    /**
	 * 商店出现未知错误
	 */
    public const int StroePay_StoreError = 20202;
    /**
	 * 订单重复
	 */
    public const int StorePay_RepeatReceipt = 20203; //订单重复
                                                            /**
                                                             * 错误的商品ID
                                                             */
    public const int StorePay_ErrorGoodsID = 20204; //错误的商品ID
                                                           /**
                                                            * 商店验证失败
                                                            */
    public const int StroePay_VerificationFailed = 20205;
    /// <summary>
    /// 没有登录不能支付
    /// </summary>
    public const int StroePay_NoLogin = 20206;

    //===============兑换码功能===========//

    /**
	 * 没有兑换码
	 */
    public const int RedeemCode_DontHave = 30000;
    /**
     * 兑换作用日期还没开始
     */
    public const int RedeemCode_NotStart = 30001;
    /**
	 * 兑换码过期
	 */
    public const int RedeemCode_Overdue = 30002;
    /**
	 * 兑换码失效
	 */
    public const int RedeemCode_CantUse = 30003;
    /**
	 * 兑换码不能重复使用
	 */
    public const int RedeemCode_CantRepeatUse = 30004;

    /**
	 * 该兑换码功能没实现
	 */
    public const int RedeemCode_FunctionClassNoFound = 30005;
    /**
	 * 该兑换码功能出错
	 */
    public const int RedeemCode_Error = 30006;
    /**
	 * 兑换码是空的
	 */
    public const int RedeemCode_CodeIsNull = 30007;
    /**
	 * 该兑换码不是激活码功能
	 */
    public const int RedeemCode_NotActivationCode = 30008;

    //=================通用商店===============//
    /***
	 * 没有商店逻辑
	 */
    public const int GeneralGameShop_NoLogic = 30100;
    /***
	 * 达到购买数量上限
	 */
    public const int GeneralGameShop_NumberLimit = 30101;
    /***
	 * 当前时间段不能购买(不在能购买的时间段内)
	 */
    public const int GeneralGameShop_TimeRangeLimit = 30102;

    /***
	 * 货币数目不足不能购买
	 */
    public const int GeneralGameShop_CoinNotEnough = 30103;

    /***
	 * 商店购买出错
	 */
    public const int GeneralGameShop_Error = 30104;
}
