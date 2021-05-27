using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MenuGUI;

[AddComponentMenu("Menu/NoAdsPurchase")]
public class NoAdsPurchase : Menu
{

    public override int UniqueID { get { return (int)MenuTypes.NoAdsPurchase; } }

    protected override void OnShow(ActivateParams activateParams)
    {

    }

    protected override void OnClose()
    {

    }

    //public void BuyNoAds()
    //{
    //    IAPManager.Instance.PurchaseNonConsumableProduct(eIAP.noAds);
    //}

    public void BuyNoAds()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
           // IAPManager.Instance.PurchaseNonConsumableProduct(eIAP.noAds);
        }
        else
        {
            MenuManager.Instance.Show((int)MenuTypes.Error, new ResultsParams { message = LocalizationManager.Instance.GetString("ID_NO_INTERNET") });
        }
            //if (Application.internetReachability != NetworkReachability.NotReachable)
            //{
            //    PluginManager.Instance.Microtransactions.BuyProduct(eIAP.noAds.ToString(), false,
            //       result =>
            //       {
            //           if (result.success)
            //           {

            //               ProfileManager.Instance.noAdsPurchased = true;
            //               PluginManager.interstitialShouldOpen = false;
            //               if (MenuManager.Instance.GetMenu((int)MenuTypes.MainMenu).gameObject.activeInHierarchy)
            //                   MenuManager.Instance.GetMenu((int)MenuTypes.MainMenu).gameObject.GetComponent<MainMenu>().HideNoAds();
            //               else
            //               if (MenuManager.Instance.GetMenu((int)MenuTypes.Options).gameObject.activeInHierarchy)
            //                   MenuManager.Instance.GetMenu((int)MenuTypes.Options).gameObject.GetComponent<Options>().HideNoAds();

            //               ProfileManager.Instance.Save();
            //               AdManagerIronsrc.Instance.DestroyBannerAd();
            //               MenuManager.Instance.Back();
            //               MenuManager.Instance.Show((int)MenuTypes.Success);
            //           }
            //           else
            //               MenuManager.Instance.Show((int)MenuTypes.Error, new ResultsParams { message = result.error });
            //       });
            //}
            //else
            //{
            //    MenuManager.Instance.Show((int)MenuTypes.Error, new ResultsParams { message = LocalizationManager.Instance.GetString("ID_NO_INTERNET") });
            //}
        }

        private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            MenuManager.Instance.Back();
    }

}
