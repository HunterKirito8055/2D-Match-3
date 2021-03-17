using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Purchasing;

public class UnityIAP : MonoBehaviour, IStoreListener
{
    public Text lowBuyText, highBuyText;
    public static UnityIAP instance;

    private static IStoreController storeController;          // The Unity Purchasing system.
    private static IExtensionProvider storeExtensionProvider; // The store-specific Purchasing subsystems.

    public static string removeAds = "RemoveAds";
    public static string buylowGold = "buy500Coins";
    public static string buyhighGold = "buy1000Coins";

    // Google Play Store-specific product identifier subscription product.
    private static string googlePlaySubscription = "com.unity3d.subscription.original";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        //if (storeController == null)
        //{
        //    // Begin to configure our connection to Purchasing
        //    InitilizePurchase();
        //}
    }
    // Update is called once per frame

    public void InitilizePurchase()
    {
        if (IsInitilize())
        {
            // If we have already connected to Purchasing ...
            return;
        }

        //Create a builder, first passing in a suite of Unity provided stores.
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        // Add a product to sell / restore by way of its identifier, associating the general identifier
        // with its store-specific identifiers.
        builder.AddProduct(removeAds, ProductType.NonConsumable);
        builder.AddProduct(buylowGold, ProductType.Consumable);
        builder.AddProduct(buyhighGold, ProductType.Consumable);

        // To be Started the remainder of the set-up with an asynchrounous call, passing the configuration(Builder)
        // and this class' instance. Expect a Debug Console response either in OnInitialized or OnInitializeFailed.
        UnityPurchasing.Initialize(this, builder);
    }
    void BuyProductID(string productID)
    {
        if (IsInitilize())
        {
            // ... look up the Product reference with the general product identifier and the Purchasing 
            // system's products collection.
            Product product = storeController.products.WithID(productID);

            // If the look up found a product for this device's store and that product is ready to be sold ... 

            if (product != null && product.availableToPurchase)
            {
                // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed asynchronously.
                storeController.InitiatePurchase(product);
            }
            else
            {
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        else
        {
            Debug.Log(productID + "Purchase Failed");
        }
    }
    public bool IsInitilize()
    {
        return storeController != null && storeExtensionProvider != null;
    }

    public void BuyConsumable1000Coins()
    {
        BuyProductID(buyhighGold);
        AddGold(buyhighGold);
    }
    public void BuyConsumable500Coins()
    {
        BuyProductID(buylowGold);
        AddGold(buylowGold);
    }
    public void BuyNonConsumable()
    {
        BuyProductID(removeAds);
    }


    //Store Listener
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("OnInitialized: PASS");
        storeController = controller;
        storeExtensionProvider = extensions;
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("OnInitialized: Failed");
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        if (String.Equals(purchaseEvent.purchasedProduct.definition.id, removeAds, StringComparison.Ordinal))
        {
            Debug.Log("purchased " + removeAds + "successfully");
        }
        else if (String.Equals(purchaseEvent.purchasedProduct.definition.id, buylowGold, StringComparison.Ordinal))
        {
            Debug.Log("purchased " + buylowGold + "successfully");
        }
        else if (String.Equals(purchaseEvent.purchasedProduct.definition.id, buyhighGold, StringComparison.Ordinal))
        {
            Debug.Log("purchased " + buyhighGold + "successfully");
        }
        return PurchaseProcessingResult.Complete;
    }


    public Text GoldCountText;
    public int GoldCount
    {
        get
        {
            return goldCount;
        }
        set
        {
            goldCount = value;
            GoldCountText.text = goldCount.ToString();
        }
    }
    int goldCount;
    void AddGold(string val)
    {
        GoldCount += int.Parse(val);
    }
    public void InitialiseAfterFetching(string low, string high)
    {
        buylowGold = low;
        buyhighGold = high;
        lowBuyText.text = "Get " + low + "Gold";
        highBuyText.text = "Get " + high + "Gold";

        // Begin to configure our connection to Purchasing
        InitilizePurchase();
    }
}
