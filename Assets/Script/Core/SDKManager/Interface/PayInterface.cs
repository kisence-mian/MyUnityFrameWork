using FrameWork.SDKManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FrameWork.SDKManager
{
    public abstract class PayInterface : SDKInterfaceBase
    {
        /// <summary>
        /// 验证回调，参数：string（商店名）,string（订单凭据，base64加密串）
        /// </summary>
        //public CallBack<StoreName, string> m_ConfirmCallBack;
        protected List<LocalizedGoodsInfo> productDefinitions = new List<LocalizedGoodsInfo>();
        public virtual StoreName GetStoreName()
        {
            return StoreName.None;
        }
        public override void ExtraInit(string tag)
        {
            if (!string.IsNullOrEmpty(tag))
            {
                //Debug.Log("PayInterface:" + tag);
                productDefinitions = JsonUtils.FromJson<List<LocalizedGoodsInfo>>(tag);
                //Debug.Log("After PayInterface:" + JsonUtils.ToJson(productDefinitions));
            }
            ExtraInit();
        }
        protected virtual void ExtraInit()
        {

        }

        virtual public void Pay(PayInfo payInfo)
        {

        }

        /// <summary>
        /// 适用于多种store的方式
        /// </summary>
        /// <param name="goodsID"></param>
        /// <param name="tag"></param>
        /// <param name="StoreName"></param>
        virtual public void ConfirmPay(string goodsID, string tag,string StoreName)
        {

        }

        //virtual public void ConfirmPay(string goodsID, string tag)
        //{

        //}

        virtual public LocalizedGoodsInfo GetGoodsInfo(string goodsID)
        {
            for (int i = 0; i < productDefinitions.Count; i++)
            {
                if(productDefinitions[i].goodsID == goodsID)
                {
                    Debug.LogWarning("======goodsID========" + productDefinitions[i].localizedPriceString);
                    return productDefinitions[i];
                }
            }

            return null;
        }
        virtual public string GetUserID()
        {
            return "userID";
        }

        virtual public List<LocalizedGoodsInfo> GetAllGoodsInfo()
        {
            return productDefinitions;
        }

        public override void Init()
        {

        }

        protected void PayCallBack(OnPayInfo info)
        {
            //info.storeName = GetStoreName();
            if (SDKManager.PayCallBack != null)
                SDKManager.PayCallBack(info);
        }
        /// <summary>
        /// 获取商品类型
        /// </summary>
        /// <param name="goodID"></param>
        /// <returns></returns>
        public GoodsType GetGoodType(string goodID)
        {

            for (int i = 0; i < productDefinitions.Count; i++)
            {
                if (productDefinitions[i].goodsID == goodID)
                {
                    return productDefinitions[i].goodsType;
                }
            }

            Debug.LogError(" pay productDefinitions goodID is not found" + "id: " + goodID + " count: " + productDefinitions.Count);

            return GoodsType.NORMAL;

        }
    }
    /// <summary>
    /// 本地化商品信息，来自SDK平台的后台，如：
    /// localizedPriceString :¥6.00
    ///localizedTitle :小包钻石
    ///localizedDescription :钻石可以在游戏内用于购买各项商品
    ///isoCurrencyCode :CNY
    ///localizedPrice :6
    /// </summary>
    public class LocalizedGoodsInfo
    {
        public LocalizedGoodsInfo()
        {
        }
        public LocalizedGoodsInfo(string goodsID, GoodsType goodsType, float price,string isoCurrencyCode = "CNY",string goodName= "")
        {
            this.goodsID = goodsID;
            this.goodsType = goodsType;
            this.localizedPrice = price;
            this.localizedTitle = goodName;
            this.isoCurrencyCode = isoCurrencyCode; //默认人民币
        }

        /// <summary>
        /// id
        /// </summary>
        public string goodsID;
        /// <summary>
        /// 描述串
        /// </summary>
        public string localizedPriceString;
        /// <summary>
        /// 标题
        /// </summary>
        public string localizedTitle;
        /// <summary>
        /// 商店描述
        /// </summary>
        public string localizedDescription;
        /// <summary>
        /// 货币类型
        /// </summary>
        public string isoCurrencyCode;
        /// <summary>
        /// 价格
        /// </summary>
        public float localizedPrice;

        /// <summary>
        /// 商品类型
        /// </summary>
        public GoodsType goodsType;
    }
}
