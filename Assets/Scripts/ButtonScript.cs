using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour
{
    public enum PurchaseItems { RemoveAds, PurchaseLowCoins, PurchaseHighCoins }

    public PurchaseItems purchaseItem;
    public void PurchaseBtn()
    {
        switch (purchaseItem)
        {
            case PurchaseItems.RemoveAds:
                UnityIAP.instance.BuyNonConsumable();
                break;
            case PurchaseItems.PurchaseLowCoins:
                UnityIAP.instance.BuyConsumable500Coins();
                break;
            case PurchaseItems.PurchaseHighCoins:
                UnityIAP.instance.BuyConsumable1000Coins();
                break;
            default:
                break;
        }
    }
}
