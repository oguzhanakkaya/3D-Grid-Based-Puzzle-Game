using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.Purchasing;
using Unity.VisualScripting;

public class StoreItems : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI priceText;
    

    public void SetPriceText()
    {
        var product = CodelessIAPStoreListener.Instance.GetProduct(GetComponent<CodelessIAPButton>().productId);
        priceText.text = product.metadata.localizedPrice.ToString() + " " + product.metadata.isoCurrencyCode.ToString();
    }
    public void PurchaseSuccess()
    {
        var product = CodelessIAPStoreListener.Instance.GetProduct(GetComponent<CodelessIAPButton>().productId);

      /*  EventDataManager.SendRealPaymentEvent((float)product.metadata.localizedPrice,
            GetComponent<CodelessIAPButton>().productId,
            product.metadata.isoCurrencyCode.ToString());*/
    }

}
