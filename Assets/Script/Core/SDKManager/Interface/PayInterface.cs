using FrameWork.SDKManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FrameWork.SDKManager
{
    public abstract class PayInterface : SDKInterfaceBase
    {
        public PayCallBack m_PayResultCallBack;
        public PayCallBack m_ConfirmCallBack;

        virtual public void Pay(string goodsID, string tag, GoodsType goodsType = GoodsType.NORMAL, string orderID = null)
        {

        }

        virtual public void ConfirmPay(string orderID, string tag)
        {

        }

        public override void Init()
        {

        }
    }
}
