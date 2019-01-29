using FrameWork.SDKManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FrameWork.SDKManager
{
    public abstract class PayInterface : SDKInterfaceBase
    {
        public PayCallBack m_PayResultCallBack;
        /// <summary>
        /// 验证回调，参数：string（商店名）,string（订单凭据，base64加密串）
        /// </summary>
        public CallBack<StoreName, string> m_ConfirmCallBack;

        virtual public void Pay(string goodsID, string tag, GoodsType goodsType = GoodsType.NORMAL, string orderID = null)
        {

        }

        virtual public void ConfirmPay(string goodsID, string tag)
        {

        }

        public override void Init()
        {

        }
    }
}
