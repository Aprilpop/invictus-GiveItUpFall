using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MenuGUI;
using UnityEngine.UI;

public class ResultsParams : ActivateParams
{
    public string message;
}

[AddComponentMenu("Menu/Options")]
public class Options : Menu
{
    public override int UniqueID { get { return (int)MenuTypes.Options; } }

    [SerializeField] Button resetProfileButton;
    [SerializeField] Button soundButton;
    [SerializeField] Button musicButton;
    [SerializeField] Button noAds;
    [SerializeField] Button restorePurchase;

    [SerializeField] Button credits;

    [SerializeField] Image soundOff;
    [SerializeField] Image musicOff;

    [SerializeField] RotateTower tower;

    [SerializeField] Image drag;
    [SerializeField] Image touch;

    [SerializeField] Button unlockAll;

    [SerializeField] Image noAdsRing;
    [SerializeField] Image noAdsButton;
    [SerializeField] Text removeAdsText;

    [SerializeField] Text languageText;

    #region AnalyticsParameters
    bool currentControl;
    Language currentLanguage;
    bool currentSound;
    bool currentMusic;
    #endregion

    private void Awake()
    {
//#if _DEBUG
//        resetProfileButton.gameObject.SetActive(true);
//        resetProfileButton.onClick.AddListener(() => ResetProfile());
//        unlockAll.gameObject.SetActive(true);
//        unlockAll.onClick.AddListener(() => UnlockAll());
//#else
            resetProfileButton.gameObject.SetActive(false);
        unlockAll.gameObject.SetActive(false);
//#endif

        soundButton.onClick.AddListener(() => Sound());
        musicButton.onClick.AddListener(() => Music());
        noAds.onClick.AddListener(() => NoAds());
        restorePurchase.onClick.AddListener(() => ButtonRestorePurchases());
        credits.onClick.AddListener(() => Credits());
    }

    private void Credits()
    {

        MenuManager.Instance.Show((int)MenuTypes.Credits);
    }

    public void ButtonLanguagePressed()
    {
        ProfileManager.Instance.currentLanguageIndex++;

        if (ProfileManager.Instance.currentLanguageIndex >= LocalizationManager.Instance.Languages.Length)
            ProfileManager.Instance.currentLanguageIndex = 0;

        ChangeLanguage(ProfileManager.Instance.currentLanguageIndex);

        PlayerPrefs.SetInt("languageID", ProfileManager.Instance.currentLanguageIndex);
        PlayerPrefs.Save();
    }

    private void ChangeLanguage(int index)
    {
        LocalizationManager.Instance.SetLanguage(LocalizationManager.Instance.Languages[index].Id);
        languageText.text = LocalizationManager.Instance.CurrentLanguage.Name;
    }

    protected override void OnShow(ActivateParams activateParams)
    {
        if (ProfileManager.Instance.Sound)
        {
            soundOff.enabled = false;
        }
        else
        {
            soundOff.enabled = true;
        }

        if (ProfileManager.Instance.Music)
        {
            musicOff.enabled = false;
        }
        else
        {
            musicOff.enabled = true;
        }
        CheckControl();


        if (ProfileManager.Instance.noAdsPurchased)
        {
            noAdsRing.color = Color.green;
            noAdsButton.color = Color.green;
            noAds.interactable = false;
            removeAdsText.color = Color.black;
        }

        languageText.text = LocalizationManager.Instance.CurrentLanguage.Name;

        currentControl = ProfileManager.Instance.dragControl;
        currentLanguage = LocalizationManager.Instance.CurrentLanguage;
        currentSound = ProfileManager.Instance.Sound;
        currentMusic = ProfileManager.Instance.Music;
    }

    private void UnlockAll()
    {
        for (int i = 0; i < ProfileManager.Instance.Blobs.Length; i++)
        {
            ProfileManager.Instance.Blobs[i].unlocked = true;
        }
        for (int i = 0; i < ProfileManager.Instance.Trails.Length; i++)
        {
            ProfileManager.Instance.Trails[i].unlocked = true;
        }
        for (int i = 0; i < ProfileManager.Instance.musicsLocks.Length; i++)
        {
            ProfileManager.Instance.musicsLocks[i] = false;
        }
        ProfileManager.Instance.Save();
    }

    private void CheckControl()
    {
        if (tower.Drag)
        {
            drag.color = Color.green;
            touch.color = Color.white;
        }
        else
        {
            touch.color = Color.green;
            drag.color = Color.white;
        }
    }

    protected override void OnClose()
    {
        #region Analytics
        //if (currentControl != ProfileManager.Instance.dragControl)
        //    GameAnalyticsManager.LogDesignEvent("ChangedControl" + ":" + ProfileManager.Instance.dragControl);
        //if (currentLanguage != LocalizationManager.Instance.CurrentLanguage)
        //    GameAnalyticsManager.LogDesignEvent("ChangedLanguage" + ":" + LocalizationManager.Instance.CurrentLanguage.Name);
        //if (currentSound != ProfileManager.Instance.Sound)
        //    GameAnalyticsManager.LogDesignEvent("ChangedSound" + ":" + ProfileManager.Instance.Sound);
        //if (currentMusic != ProfileManager.Instance.Music)
        //    GameAnalyticsManager.LogDesignEvent("ChangedMusic" + ":" + ProfileManager.Instance.Music);
        #endregion
    }

    public void ResetProfile()
    {
        PlayerPrefs.DeleteAll();
        ProfileManager.Instance.Load();
        LevelManager.Instance.ResetLevel();
        ProfileManager.Instance.SetCharacter(ProfileManager.Instance.Blobs[0].name);
    }

    public void Sound()
    {
        if (ProfileManager.Instance.Sound)
        {
            soundOff.enabled = true;
            ProfileManager.Instance.Sound = false;
        }
        else
        {
            soundOff.enabled = false;
            ProfileManager.Instance.Sound = true;
        }
        ProfileManager.Instance.Save();
    }

    public void Music()
    {
        if (ProfileManager.Instance.Music)
        {
            musicOff.enabled = true;
            ProfileManager.Instance.Music = false;
        }
        else
        {
            musicOff.enabled = false;
            ProfileManager.Instance.Music = true;
        }
        ProfileManager.Instance.Save();
    }

    public void NoAds()
    {
        MenuManager.Instance.Show((int)MenuTypes.NoAdsPurchase);
    }

    

    public void ButtonRestorePurchases()
    {
      //  IAPManager.Instance.Restore();
    }

    //public void ButtonRestorePurchases()
    //{
    //    if (Application.internetReachability != NetworkReachability.NotReachable)
    //    {
    //        if (PluginManager.Instance.Microtransactions != null)
    //        {
    //            PluginManager.Instance.Microtransactions.RestorePurchases(result =>
    //            {
    //                if (result.success)
    //                    MenuManager.Instance.Show((int)MenuTypes.Success);
    //                else
    //                    MenuManager.Instance.Show((int)MenuTypes.Error, new ResultsParams { message = result.error });

    //            }, productHandler =>
    //            {

    //                foreach (string restoredProductName in productHandler.restoredProductNames)
    //                {
    //                    if (restoredProductName == eIAP.noAds.ToString())
    //                    {
    //                        ProfileManager.Instance.noAdsPurchased = true;
    //                        PluginManager.interstitialShouldOpen = false;
    //                        AdManagerIronsrc.Instance.DestroyBannerAd();

    //                    }
    //                }
    //                ProfileManager.Instance.Save();
    //            });
    //        }
    //        else
    //            MenuManager.Instance.Show((int)MenuTypes.Error, new ResultsParams { message = LocalizationManager.Instance.GetString("ID_RESTORE_FAIL") });
    //    }
    //    else
    //    {
    //        MenuManager.Instance.Show((int)MenuTypes.Error, new ResultsParams { message = LocalizationManager.Instance.GetString("ID_NO_INTERNET") });
    //    }
    //}

    public void SetControl(bool control)
    {
        Debug.Log(control);
       
        tower.Drag = control;
        ProfileManager.Instance.dragControl = control;
        ProfileManager.Instance.Save();
        CheckControl();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            MenuManager.Instance.Back();
    }

    public void HideNoAds()
    {
        if (ProfileManager.Instance.noAdsPurchased)
        {
            noAdsRing.color = Color.green;
            noAdsButton.color = Color.green;
            noAds.interactable = false;
            removeAdsText.color = Color.black;
        }
    }

}
