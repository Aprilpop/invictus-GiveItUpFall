using UnityEngine;
using MenuGUI;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

[AddComponentMenu("Menu/MainMenu")]
public class MainMenu : Menu
{
    public override int UniqueID { get { return (int)MenuTypes.MainMenu; } }

    [SerializeField] Button playButton;
    [SerializeField] Button labButton;
    [SerializeField] Button optionsButton;

    [SerializeField] Text highScore;

    [SerializeField] Animator laboratoryAnimator;

    [SerializeField, Header("Game Name Title")] Image gameNameTitle;
    [SerializeField] Sprite englishSprite;
    [SerializeField] Sprite chineseSprite;

    [SerializeField] GameObject shopNotification;

    [SerializeField] Button coinVideoButton;
    [SerializeField] Button mainCoinVideoButton;

    [SerializeField] Text coin;
    [SerializeField] Text coinForVideo;

    [SerializeField] Button noAds;

    [SerializeField] GameObject testOverTipImage;
    [SerializeField] GameObject testTipText;
    [SerializeField] GameObject loading;
    [SerializeField] GameObject promptPointObject;

    [SerializeField] Button dailySignInButton;
    [SerializeField] Button happyTurntableButton;
    [SerializeField] Button saveDesktopButton;
    [SerializeField] Button tiktokButton;

    private void Awake()
    {
        playButton.onClick.AddListener(OnClickPlay);
        labButton.onClick.AddListener(() => { MenuManager.Instance.Show((int)MenuTypes.CharacterSelection); OpenShopPanel(); });
        optionsButton.onClick.AddListener(() => Options());
        coinVideoButton.onClick.AddListener(() => GetCoinForVideo());
        mainCoinVideoButton.onClick.AddListener(() => MainGetCoinForVideo());
        noAds.onClick.AddListener(() => NoAds());
        saveDesktopButton.onClick.AddListener(SaveDesktop);
        tiktokButton.onClick.AddListener(FollowDouyinUser);

        if ((int)StarkSDKSpace.StarkSDK.s_ContainerEnv.m_HostEnum == 2 || (int)StarkSDKSpace.StarkSDK.s_ContainerEnv.m_HostEnum == 4)
        {
            tiktokButton.gameObject.SetActive(true);
        }
        else tiktokButton.gameObject.SetActive(false);

        //InvictusMoreGames.MoreGamesBoxController.Instance.Hide();
        //测试
        //InvictusMoreGames.MoreGamesBoxController.Instance.onJsonReadSuccess += (bool success) => ShowMoreGames();
        //InvictusMoreGames.MoreGamesBoxController.Instance.onClicked += (string success) =>
        //    GameAnalyticsManager.LogDesignEvent("MoreGamesClicked" + ":" + InvictusMoreGames.MoreGamesBoxController.Instance.gameBox.gameName.GetLanguageElement(SystemLanguage.English).value);

        // testOverTipImage.SetActive(PlugingSingmaan.Instance.isDebugMode && PlugingSingmaan.Instance.IsTestOver());
        testTipText.SetActive(PlugingSingmaan.Instance.isDebugMode);
    }

    void OpenShopPanel()
    {
        Debug.Log("打开商店Banaer广告");
    }

    void OnClickPlay()
    {
        loading.SetActive(true);
        MenuManager.Instance.Show((int)MenuTypes.InGame);
        Invoke("OnCloseLoadingPanel",2);
        StartRecord();
    }

    void StartRecord()
    {
        Debug.Log("开始录屏");
        ByteDanceSDKManager.Instance.StartRecord();
    }

    void OnCloseLoadingPanel()
    {
        loading.SetActive(false);
    }

    private void ShowMoreGames()
    {

        //测试
        return;
        if (Application.internetReachability != NetworkReachability.NotReachable && InvictusMoreGames.MoreGamesBoxController.Instance.JsonReadSuccess && !InvictusMoreGames.MoreGamesBoxController.Instance.IsActive)
        {
            InvictusMoreGames.MoreGamesBoxController.Instance.Show();
            InvictusMoreGames.MoreGamesBoxController.Instance.ShowNewGame();
        }
        else
            InvictusMoreGames.MoreGamesBoxController.Instance.Hide();
    }

    private void Start()
    {
        MusicManager.Instance.PlayMusic(ProfileManager.Instance.currentMusic);
        string noti = LocalizationManager.Instance.GetString("ID_NOTIFICATIONS_REMINDER");
    }

    public void HideMoreGames()
    {
        //测试
        return;
        InvictusMoreGames.MoreGamesBoxController.Instance.Hide();
    }

    protected override void OnShow(ActivateParams activateParams)
    {
        EventManager.StartListening("VideoAdAvailable", VideoAvailable);
        EventManager.StartListening("VideoAdNotAvailable", VideoNotAvailable);
        coin.text = ProfileManager.Instance.Coin.ToString();
        coinForVideo.text = "+" + ProfileManager.Instance.coinForVideo.ToString();
        highScore.text = ProfileManager.Instance.highScore.ToString();
        GameLogic.Instance.CanRotate = false;
        PurchaseAvailable();
        EventManager.StartListening("CoinChange", UpdateCoin);

        CheckNetwork();

        if (ProfileManager.Instance.noAdsPurchased)
        {
            noAds.gameObject.SetActive(false);
        }

        if (LocalizationManager.Instance.CurrentLanguage.Name == "English")
        {
            gameNameTitle.sprite = englishSprite;
        }
        else //if (LocalizationManager.Instance.CurrentLanguage.Name == "中文(简体)"/*"Chinese"*/)
        {
            gameNameTitle.sprite = chineseSprite;
        }
        ShowMoreGames();
    }

    public void HideNoAds()
    {
        if (ProfileManager.Instance.noAdsPurchased)
        {
            noAds.gameObject.SetActive(false);
        }

    }

    private void OnApplicationPause(bool pause)
    {
        if (!pause)
        {
            CheckNetwork();
        }

    }

    private void CheckNetwork()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
            VideoAvailable();
        else
            VideoNotAvailable();
    }

    public void NoAds()
    {
        MenuManager.Instance.Show((int)MenuTypes.NoAdsPurchase);
    }

    private void VideoAvailable()
    {
        Debug.Log("VideoAvailable");
        coinVideoButton.interactable = true;
    }

    private void VideoNotAvailable()
    {
        Debug.Log("VideoNotAvailable");
        coinVideoButton.interactable = false;
    }

    private void UpdateCoin()
    {
        coin.text = ProfileManager.Instance.Coin.ToString();
        PurchaseAvailable();
    }

    public void GetCoinForVideo()
    {
        //AdManagerIronsrc.Instance.ShowVideoAd(results =>
        //{
        //    if (results)
        //    {
        //        ProfileManager.Instance.Coin += ProfileManager.Instance.coinForVideo;
        //        ProfileManager.Instance.Save();
        //    }
        //});
    }

    public void MainGetCoinForVideo()
    {
        EventDispatcher.Instance.AddEventListener(EventKey.AdShowSuccessCallBack, OnVideoPlayOverCallBacks);
        PluginMercury.Instance.ActiveRewardVideo();
    }

    void OnVideoPlayOverCallBacks(string msg)
    {
        ProfileManager.Instance.Coin += 100;
        coin.text = ProfileManager.Instance.Coin.ToString();
        EventDispatcher.Instance.RemoveEventListener(EventKey.AdShowSuccessCallBack, OnVideoPlayOverCallBacks);
    }

    public void PurchaseAvailable()
    {
        int coin = ProfileManager.Instance.Coin;

        if (coin >= ProfileManager.Instance.randomCharacterPrice || coin >= ProfileManager.Instance.randomMusicPrice || coin >= ProfileManager.Instance.randomTrailPrice)
        {
            laboratoryAnimator.Play("Available");
            shopNotification.SetActive(true);
        }
        else
        {
            laboratoryAnimator.Play("Idle");
            shopNotification.SetActive(false);
        }
    }

    protected override void OnClose()
    {
        HideMoreGames();
        EventManager.StopListening("CoinChange", UpdateCoin);
        EventManager.StopListening("VideoAdAvailable", VideoAvailable);
        EventManager.StopListening("VideoAdNotAvailable", VideoNotAvailable);
    }

    public void Options()
    {
        MenuManager.Instance.Show((int)MenuTypes.Options);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && MenuManager.Instance.CurrentMenu.UniqueID == (int)MenuTypes.MainMenu)
            Application.Quit();
    }

    //保存桌面
    void SaveDesktop()
    {
        Debug.Log("SaveDesktop");
        ByteDanceSDKManager.Instance.SaveToDesktop();
    }

    //关注抖音
    void FollowDouyinUser()
    {
        Debug.Log("FollowDouyinUser");
        ByteDanceSDKManager.Instance.FollowDouyin();
    }

    public void OpenSingInPanel()
    {
        MenuManager.Instance.Show((int)MenuTypes.SignInPanel);
    }

    public void OpenLuckyTurntablePanel()
    {
        MenuManager.Instance.Show((int)MenuTypes.LuckyTurntablePanel);
    }

    private void OnEnable()
    {
       InvokeRepeating("SingInPrompt",0,1);
    }

    private void OnDisable()
    {
        CancelInvoke("SingInPrompt");
    }

    void SingInPrompt()
    {
        string date = PlayerPrefs.GetString("SignInDate", "");
        DateTime[] cuttentDateTime = new DateTime[3] { DateTime.Now, DateTime.Now, DateTime.Now };

        if (date.Equals(""))
        {
            System.DateTime startTime = System.TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            long curretnSeconds = (long)(DateTime.Now - startTime).TotalSeconds / (3600 * 24);

            PlayerPrefs.SetString("SignInDate", curretnSeconds.ToString());
            PlayerPrefs.SetString("DateTimeOne", cuttentDateTime[0].ToString());
            PlayerPrefs.SetString("DateTimeTwo", cuttentDateTime[1].ToString());
            PlayerPrefs.SetString("DateTimeThree", cuttentDateTime[2].ToString());
            PlayerPrefs.SetString("Day", "1");
            PlayerPrefs.SetString("OneDayTimes", "0");
            PlayerPrefs.SetString("TwoDayTimes", "0");
            PlayerPrefs.SetString("ThreeDayTimes", "0");
            return;
        }

        int[] times = new int[3];
        int day = int.Parse(PlayerPrefs.GetString("Day"));
        times[0] = int.Parse(PlayerPrefs.GetString("OneDayTimes"));
        times[1] = int.Parse(PlayerPrefs.GetString("TwoDayTimes"));
        times[2] = int.Parse(PlayerPrefs.GetString("ThreeDayTimes"));
        cuttentDateTime[0] = DateTime.Parse(PlayerPrefs.GetString("DateTimeOne"));
        cuttentDateTime[1] = DateTime.Parse(PlayerPrefs.GetString("DateTimeTwo"));
        cuttentDateTime[2] = DateTime.Parse(PlayerPrefs.GetString("DateTimeThree"));

        if (day >= 1)
        {
            switch (times[0])
            {
                case 0: promptPointObject.SetActive(true); break;
                case 1: CountDawn(GetSubSeconds(cuttentDateTime[0], DateTime.Now), 0, cuttentDateTime); break;
                case 2: CountDawn(GetSubSeconds(cuttentDateTime[0], DateTime.Now), 0, cuttentDateTime); break;
                default:
                    promptPointObject.SetActive(false);
                    break;
            }
        }

        if (day >= 2)
        {
            switch (times[1])
            {
                case 0: promptPointObject.SetActive(true); break;
                case 1: CountDawn(GetSubSeconds(cuttentDateTime[1], DateTime.Now), 1, cuttentDateTime); break;
                case 2: CountDawn(GetSubSeconds(cuttentDateTime[1], DateTime.Now), 1, cuttentDateTime); break;
                default: promptPointObject.SetActive(false); break;
            }
        }

        if (day >= 3)
        {
            switch (times[2])
            {
                case 0: promptPointObject.SetActive(true); break;
                default: promptPointObject.SetActive(false); break;
            }
        }
    }

    public int GetSubSeconds(DateTime startTimer, DateTime endTimer)
    {
        TimeSpan startSpan = new TimeSpan(startTimer.Ticks);
        TimeSpan nowSpan = new TimeSpan(endTimer.Ticks);
        TimeSpan subTimer = nowSpan.Subtract(startSpan).Duration();
        return (int)subTimer.TotalSeconds;
    }

    void CountDawn(int seconds, int index,DateTime[] cuttentDateTime)
    {
        int currentSecond = GetSubSeconds(cuttentDateTime[index], cuttentDateTime[index].AddHours(2));
        if (seconds < currentSecond){
            promptPointObject.SetActive(false);
        }else{
            promptPointObject.SetActive(true);
        }
    }
}
