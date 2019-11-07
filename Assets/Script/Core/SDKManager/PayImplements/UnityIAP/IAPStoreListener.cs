
#if UnityIAP
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Purchasing;


public class IAPStoreListener: IStoreListener
{
    /// <summary>
    /// 初始化成功
    /// </summary>
    public  CallBack onInitialized;
    /// <summary>
    /// 初始化失败
    /// </summary>
    public  CallBack<InitializationFailureReason> onInitializeFailed;
    /// <summary>
    /// 支付失败
    /// </summary>
    public  CallBack<Product, PurchaseFailureReason> onPurchaseFailed;
    /// <summary>
    /// 支付成功
    /// </summary>
    public  CallBack<Product,string> onPurchaseSuccess;




    public AppStore appStore
    {
        get
        {
            return standardPurchasingModule.appStore;
        }
    }

    public IStoreController m_StoreController;          // The Unity Purchasing system.
    private IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.
    private StandardPurchasingModule standardPurchasingModule;



    public  void Initialize(AppStore appStore, List<ProductDefinition> products)
    {
        // If we have already connected to Purchasing ...
        if (IsInitialized())
        {
            // ... we are done here.
            return;
        }
        standardPurchasingModule = StandardPurchasingModule.Instance(appStore);
        // Create a builder, first passing in a suite of Unity provided stores.
        var builder = ConfigurationBuilder.Instance(standardPurchasingModule);

        //// Add a product to sell / restore by way of its identifier, associating the general identifier
        //// with its store-specific identifiers.
        //builder.AddProduct(kProductIDConsumable, ProductType.Consumable);
        //// Continue adding the non-consumable product.
        //builder.AddProduct(kProductIDNonConsumable, ProductType.NonConsumable);
        //// And finish adding the subscription product. Notice this uses store-specific IDs, illustrating
        //// if the Product ID was configured differently between Apple and Google stores. Also note that
        //// one uses the general kProductIDSubscription handle inside the game - the store-specific IDs 
        //// must only be referenced here. 
        ////builder.AddProduct(kProductIDSubscription, ProductType.Subscription, new IDs(){
        ////        { kProductNameAppleSubscription, AppleAppStore.Name },
        ////        { kProductNameGooglePlaySubscription, GooglePlay.Name },
        ////    });
        builder.AddProducts(products);
        // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
        // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
        UnityPurchasing.Initialize(this, builder);
    }


    public  bool IsInitialized()
    {
        // Only say we are initialized if both the Purchasing references are set.
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }

    public void PurchaseProduct(string productId)
    {
        // ... look up the Product reference with the general product identifier and the Purchasing 
        // system's products collection.
        Product product = m_StoreController.products.WithID(productId);

        // If the look up found a product for this device's store and that product is ready to be sold ... 
        if (product != null && product.availableToPurchase && IsInitialized())
        {
            Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
            // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
            // asynchronously.
            m_StoreController.InitiatePurchase(product);
        }
        // Otherwise ...
        else
        {
            PurchaseFailureReason reason = PurchaseFailureReason.PurchasingUnavailable;
            if (!IsInitialized())
            {
                // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
                Debug.LogError("RestorePurchases FAIL. Not initialized.");
            }
            Debug.Log("支付失败：productId :" + productId + " product ：" + product);
            if (product != null)
            {
                Debug.Log("支付失败：product.availableToPurchase ：" + product.availableToPurchase);
            }
            else
            {
                reason = PurchaseFailureReason.ProductUnavailable;
            }
            if (onPurchaseFailed != null)
            {
                onPurchaseFailed(product, reason);
            }
            // ... report the product look-up failure situation  
            Debug.LogError("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
        }

    }

    public void ConfirmPendingPurchase(string productId)
    {
       
        Product product = m_StoreController.products.WithID(productId);
        if ( product != null && IsInitialized())
        {
            m_StoreController.ConfirmPendingPurchase(product);
        }
        else
        {
            if (!IsInitialized())
            {
                // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
                Debug.LogError("RestorePurchases FAIL. Not initialized.");
            }
            if (onPurchaseFailed!=null)
            {
                onPurchaseFailed(product, PurchaseFailureReason.Unknown);
            }
            Debug.LogError("ConfirmPendingPurchase : no productId :" + productId);
        }
    }

    // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
    // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
    public void RestorePurchases()
    {
        // If Purchasing has not yet been set up ...
        if (!IsInitialized())
        {
            // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
            Debug.Log("RestorePurchases FAIL. Not initialized.");
            return;
        }

        // If we are running on an Apple device ... 
        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            // ... begin restoring purchases
            Debug.Log("RestorePurchases started ...");

            // Fetch the Apple store-specific subsystem.
            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
            // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
            // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
            apple.RestoreTransactions((result) => {
                // The first phase of restoration. If no more responses are received on ProcessPurchase then 
                // no purchases are available to be restored.
                Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
            });
        }
        // Otherwise ...
        else
        {
            // We are not running on an Apple device. No work is necessary to restore purchases.
            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }


    //  
    // --- IStoreListener
    //
    private IAppleExtensions m_AppleExtensions;
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        // Purchasing has succeeded initializing. Collect our Purchasing references.
        Debug.Log("OnInitialized: PASS");

        // Overall Purchasing system, configured with products for this application.
        m_StoreController = controller;
        // Store specific subsystem, for accessing device-specific store features.
        m_StoreExtensionProvider = extensions;
        m_AppleExtensions = extensions.GetExtension<IAppleExtensions>();
        // On Apple platforms we need to handle deferred purchases caused by Apple's Ask to Buy feature.
        // On non-Apple platforms this will have no effect; OnDeferred will never be called.
        m_AppleExtensions.RegisterPurchaseDeferredListener(OnDeferred);

        if (onInitialized != null)
        {
            onInitialized();
        }

        foreach (var item in controller.products.all)
        {
            Debug.Log("localizedPriceString :" + item.metadata.localizedPriceString + "\n" +
                "localizedTitle :" + item.metadata.localizedTitle + "\n" +
                "localizedDescription :" + item.metadata.localizedDescription + "\n" +
                "isoCurrencyCode :" + item.metadata.isoCurrencyCode + "\n" +
                "localizedPrice :" + item.metadata.localizedPrice + "\n" +
                "\n");
        }
    }


    public void OnInitializeFailed(InitializationFailureReason error)
    {
        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        Debug.LogError("OnInitializeFailed InitializationFailureReason:" + error);
        if (onInitializeFailed != null)
        {
            onInitializeFailed(error);
        }
    }


    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
        PurchaseProcessingResult purchaseState;
        string transactionReceipt = args.purchasedProduct.receipt;
        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.tvOS)
        {
            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
             transactionReceipt = apple.GetTransactionReceiptForProduct(args.purchasedProduct);
            Debug.Log("Apple transaction Receipt :"+ transactionReceipt);
            //这里的transactionReceipt是 Base64加密后的交易收据，将这个收据发送给远程服务器就会得到订单信息的 Json 数据
            purchaseState = PurchaseProcessingResult.Pending;

        }
        else
        {
            purchaseState = PurchaseProcessingResult.Complete;
        }
        if (onPurchaseSuccess != null)
        {
            onPurchaseSuccess(args.purchasedProduct,transactionReceipt);
        }
       
        // Return a flag indicating whether this product has completely been received, or if the application needs 
        // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
        // saving purchased products to the cloud, and when that save is delayed. 

        return purchaseState;
    }


    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
        // this reason with the user to guide their troubleshooting actions.
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
        if (onPurchaseFailed != null)
        {
            onPurchaseFailed(product, failureReason);
        }
    }

    /// <summary>
    /// iOS Specific.
    /// This is called as part of Apple's 'Ask to buy' functionality,
    /// when a purchase is requested by a minor and referred to a parent
    /// for approval.
    ///
    /// When the purchase is approved or rejected, the normal purchase events
    /// will fire.
    /// </summary>
    /// <param name="item">Item.</param>
    private void OnDeferred(Product item)
    {
        Debug.Log("Purchase deferred: " + item.definition.id);
    }
}
#endif

