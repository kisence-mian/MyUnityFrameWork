using FrameWork.SDKManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
#if UnityIAP
using UnityEngine.Purchasing;
#endif

public class PayUnityIAPImplement : PayInterface
{
    public StoreName storeName;
    public override StoreName GetStoreName()
    {
        return storeName;
    }

#if UnityIAP
    private IAPStoreListener listener;

    protected override void ExtraInit()
    {
        listener = new IAPStoreListener();
       

        List<ProductDefinition> products = new List<ProductDefinition>();

       
        foreach (var item in productDefinitions)
        {
            ProductType productType = GoodsType2ProductType(item.goodsType);
            ProductDefinition p = new ProductDefinition(item.goodsID, productType);
            products.Add(p);
        }
        AppStore appStore = (AppStore)Enum.Parse(typeof(AppStore), storeName.ToString());
        listener.Initialize(appStore, products);
        listener.onInitialized = OnInitialized;
        listener.onInitializeFailed = OnInitializeFailed;
        listener.onPurchaseFailed = OnPurchaseFailed;
        listener.onPurchaseSuccess = OnPurchaseSuccess;
    }

    private ProductType GoodsType2ProductType(FrameWork.SDKManager.GoodsType m_ProductType)
    {
        ProductType productType = ProductType.Consumable;
        switch (m_ProductType)
        {
            case FrameWork.SDKManager.GoodsType.NORMAL:
                productType = ProductType.Consumable;
                break;
            case FrameWork.SDKManager.GoodsType.ONCE_ONLY:
                productType = ProductType.NonConsumable;
                break;
            case FrameWork.SDKManager.GoodsType.RIGHTS:
                productType = ProductType.Subscription;
                break;
        }
        return productType;
    }
    private FrameWork.SDKManager.GoodsType ProductType2GoodsType(ProductType m_ProductType)
    {
        FrameWork.SDKManager.GoodsType productType = FrameWork.SDKManager.GoodsType.NORMAL;
        switch (m_ProductType)
        {
            case ProductType.Consumable:
                productType = FrameWork.SDKManager.GoodsType.NORMAL;
                break;
            case ProductType.NonConsumable:
                productType = FrameWork.SDKManager.GoodsType.ONCE_ONLY;
                break;
            case ProductType.Subscription:
                productType = FrameWork.SDKManager.GoodsType.RIGHTS;
                break;
        }
        return productType;
    }
    private void OnPurchaseSuccess(Product t,string receipt)
    {
        OnPayInfo payInfo = new OnPayInfo();
        payInfo.isSuccess = true;
        payInfo.goodsId = t.definition.id;
        payInfo.goodsType = ProductType2GoodsType(t.definition.type);
        payInfo.receipt = receipt;
        payInfo.storeName = storeName;

        PayCallBack(payInfo);      
    }

    private void OnPurchaseFailed(Product t, PurchaseFailureReason t1)
    {
        Debug.LogError("OnPurchaseFailed ! t:"+t+ " PurchaseFailureReason:"+t1);
       
        OnPayInfo payInfo = new OnPayInfo();
        payInfo.isSuccess = false;
        payInfo.storeName = storeName;
        if (t != null)
        {
            Debug.Log("OnPurchaseFailed ! t.definition:" + t.definition);
            payInfo.goodsId = t.definition.id;
            payInfo.goodsType = ProductType2GoodsType(t.definition.type);
        }
        PayCallBack(payInfo);
    }

    private void OnPendingPurchase(string t)
    {
        //if (m_ConfirmCallBack != null)
        //{
        //    m_ConfirmCallBack(storeName, t);
        //}
    }

    private void OnInitializeFailed(InitializationFailureReason t)
    {
        Debug.LogError("IAP初始化失败！:"+t);
    }

    private void OnInitialized()
    {
        Debug.Log("IAP初始化成功！" );
    }

    public override void Pay(PayInfo info)
    {
        if(listener==null|| listener.m_StoreController == null)
        {
            OnPayInfo payInfo = new OnPayInfo();
            payInfo.isSuccess = false;
            payInfo.storeName = storeName;
            payInfo.goodsId = info.goodsID;
            payInfo.userID = info.userID;
            if (listener == null)
            {
                payInfo.error = "No Initialize";
            }
            else
                payInfo.error = "Initialize Failed";

            PayCallBack(payInfo);
            return;
        }

        if (Application.isEditor)
        {
            ConfirmPay(info.goodsID,info.tag,storeName.ToString());
        }
        else
            listener.PurchaseProduct(info.goodsID);
    }

    public override void ConfirmPay(string goodsID,  string tag, string StoreName)
    {
        Debug.Log("IAP ConfirmPay:" + goodsID);
        listener.ConfirmPendingPurchase(goodsID);
    }
    public override List<LocalizedGoodsInfo> GetAllGoodsInfo()
    {
        List<LocalizedGoodsInfo> infos = new List<LocalizedGoodsInfo>();
        if (listener == null)
        {
            Debug.LogError("IAP No Initialize!");
            return infos;
        }
        if (listener.m_StoreController == null)
        {
            Debug.LogError("IAP Initialize Failed!");
            return infos;
        }
        foreach (var item in listener.m_StoreController.products.all)
        {
            if (item.definition == null)
                continue;
            if (item.metadata == null)
                continue;
            LocalizedGoodsInfo f = new LocalizedGoodsInfo();
            f.goodsID = item.definition.id;
            f.isoCurrencyCode = item.metadata.isoCurrencyCode;
            f.localizedDescription = item.metadata.localizedDescription;
            f.localizedPrice = (float)item.metadata.localizedPrice;
            f.localizedPriceString = item.metadata.localizedPriceString;
            f.localizedTitle = item.metadata.localizedTitle;
            infos.Add(f);
        }
        return infos;
    }

    public override LocalizedGoodsInfo GetGoodsInfo(string goodsID)
    {
        if (listener == null)
        {
            Debug.LogError("IAP No Initialize!");
            return null;
        }
        if (listener.m_StoreController == null)
        {
            Debug.LogError("IAP Initialize Failed!");
            return null;
        }
        foreach (var item in listener.m_StoreController.products.all)
        {
            if (item.definition == null)
                continue;
            if (item.metadata == null)
                continue;
            if (item.definition.id != goodsID)
                continue;
            LocalizedGoodsInfo f = new LocalizedGoodsInfo();
            f.goodsID = goodsID;
            f.isoCurrencyCode = item.metadata.isoCurrencyCode;
            f.localizedDescription = item.metadata.localizedDescription;
            f.localizedPrice = (float)item.metadata.localizedPrice;
            f.localizedPriceString = item.metadata.localizedPriceString;
            f.localizedTitle = item.metadata.localizedTitle;
            return f;

        }
        return null;
    }
#endif
}

