using System;
using System.Collections;
using System.Collections.Generic;
using Mai.MainMenu;
//using Newtonsoft.Json.Linq;
using TMPro;
//using Unity.Services.RemoteConfig;
using UnityEngine;

[Serializable]
public class OtherBundleClass
{
    public int numberOfCoin;
    public int numberOfFreezeTimeBooster;
    public int numberOfPassangerBooster;
    //public int unlimitedTime;

    public OtherBundleClass(int numberOfCoin, int numberOfFreezeTimeBooster, int numberOfPassangerBooster/*, int unlimitedTime*/)
    {
        this.numberOfCoin = numberOfCoin;
        this.numberOfFreezeTimeBooster = numberOfFreezeTimeBooster;
        this.numberOfPassangerBooster = numberOfPassangerBooster;
       // this.unlimitedTime = unlimitedTime;
    }
}
[Serializable]
public class StarterBundleClass
{
    public int numberOfCoin;
    public int numberOfFreezeTimeBooster;
    public int numberOfPassangerBooster;

    public StarterBundleClass(int numberOfCoin, int numberOfFreezeTimeBooster, int numberOfPassangerBooster)
    {
        this.numberOfCoin = numberOfCoin;
        this.numberOfFreezeTimeBooster = numberOfFreezeTimeBooster;
        this.numberOfPassangerBooster = numberOfPassangerBooster;
    }
}
[Serializable]
public class ShopJSON
{
    public List<OtherBundleClass> otherBundleClass;
    public StarterBundleClass starterBundleClass;
    public int[] coinAmounts;

    public ShopJSON(List<OtherBundleClass> miniBundleClass, StarterBundleClass starterBundleClass, int[] coinAmounts)
    {
        this.otherBundleClass = miniBundleClass;
        this.starterBundleClass = starterBundleClass;
        this.coinAmounts = coinAmounts;
    }
}

[Serializable]
public class BundleTextClass
{
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI numberOfUnscrewBooster;
    public TextMeshProUGUI numberOfExtraMoveBooster;
  //  public TextMeshProUGUI unlimitedHeartTimeText;

}

public class Store : MonoBehaviour
{
    public List<BundleTextClass> bundleTextClass;

    [SerializeField] private TextMeshProUGUI starterBundleCointText;
    [SerializeField] private TextMeshProUGUI starterBundleUnscrewBoosterText;
    [SerializeField] private TextMeshProUGUI starterBundleExtraMoveBoosterText;


    [SerializeField] private List<TextMeshProUGUI> goldInfoTexts;

    public ShopJSON shopJSON;

    public void Initialize()
    {
    }
    public void SetText()
    {
        var otherBundleClass = shopJSON.otherBundleClass;
        for (int i = 0; i < otherBundleClass.Count; i++)
        {
            bundleTextClass[i].coinText.text = otherBundleClass[i].numberOfCoin.ToString();
            bundleTextClass[i].numberOfUnscrewBooster.text = "x" + otherBundleClass[i].numberOfPassangerBooster.ToString();
            bundleTextClass[i].numberOfExtraMoveBooster.text = "x" + otherBundleClass[i].numberOfFreezeTimeBooster.ToString();
        }

        starterBundleCointText.text = shopJSON.starterBundleClass.numberOfCoin.ToString();
        starterBundleUnscrewBoosterText.text = "x" + shopJSON.starterBundleClass.numberOfPassangerBooster.ToString();
        starterBundleExtraMoveBoosterText.text = "x" + shopJSON.starterBundleClass.numberOfFreezeTimeBooster.ToString();

        for (int i = 0; i < goldInfoTexts.Count; i++)
        {
            goldInfoTexts[i].text = shopJSON.coinAmounts[i].ToString();
        }
    }
    public void OtherBundlePackBought(int i)
    {
        var otherBundleClass = shopJSON.otherBundleClass[i];

        GameManager.Instance.OnCoinGained(otherBundleClass.numberOfCoin);

        UIManager.Instance.gameUI.AddFreezeTimeBooster(otherBundleClass.numberOfFreezeTimeBooster);
        UIManager.Instance.gameUI.AddPassangerBooster(otherBundleClass.numberOfPassangerBooster);
  
    }
    public void StarterBundlePackBought()
    {
        GameManager.Instance.OnCoinGained(shopJSON.starterBundleClass.numberOfCoin);
     
        UIManager.Instance.gameUI.AddFreezeTimeBooster(shopJSON.starterBundleClass.numberOfFreezeTimeBooster);
        UIManager.Instance.gameUI.AddPassangerBooster(shopJSON.starterBundleClass.numberOfPassangerBooster);
     
    }
    public void NoAdsBought()
    {
    }
   
    public void CoinPackBought(int i)
    {
        GameManager.Instance.OnCoinGained(shopJSON.coinAmounts[i]);
    }

}
