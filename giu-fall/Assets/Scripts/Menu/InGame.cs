using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MenuGUI;


[AddComponentMenu("Menu/InGame")]
public class InGame : Menu
{
    public override int UniqueID { get { return (int)MenuTypes.InGame; } }

    Transform cameraHolder;

    [SerializeField] Text currentscoreText;
    [SerializeField] Text currentLevelText;
    [SerializeField] Text nextLevelText;
    [SerializeField] Text coinText;
    [SerializeField] Text comboText;

    [SerializeField] ProgressBar levelProgress;

    [SerializeField] Button secondChanceButton;
    [SerializeField] Button restartButton;

    [SerializeField] Button nextLevelButton;
    [SerializeField] Button newRestartButton;
    [SerializeField] Button reviveButton;
    [SerializeField] Button shareButton;

    [SerializeField] GameObject completed;
    [SerializeField] Text completedText;

    [SerializeField] Text coinAddText;

    [SerializeField] Button backButton;

    [SerializeField] Text restartText;

    [SerializeField] Text levelBonusText;
    [SerializeField] Text currentBonusText;

    [SerializeField]
    GameObject winParticles;

    [SerializeField] Animator animator;

    [SerializeField] GameObject tutorialDrag;
    [SerializeField] GameObject tutorialTouch;

    [SerializeField] GameObject tutorialDragPrompt;
    [SerializeField] GameObject tutorialTouchPrompt;
    IEnumerator restart;
    [SerializeField] GameObject backGroundObject;
    [SerializeField] GameObject rewardProgressBar;
    [SerializeField] GameObject openReward;

    [SerializeField] GameObject bonusLevelHolder;
    [SerializeField] Text bonusLevelText;

    [SerializeField]
    float delay = 3f;

    [SerializeField]
    float waitAfterWin = 2f;

    [SerializeField]
    float bonusLevelDelay = 2f;

    float time = 0;

    [SerializeField]
    GameObject shopNotification;

    [SerializeField] Button debugBoost;

    bool secondChanceAvailable = true;

    [SerializeField] Button testWinButton;
    [SerializeField] Button tripleAccessButton;
    [SerializeField] Button nextLevel;
    private void Awake()
    {
        shareButton.onClick.AddListener(Share);
        nextLevel.onClick.AddListener(LoadNextLevel);
        cameraHolder = Camera.main.GetComponentInParent<CameraFollow>().gameObject.transform;

        reviveButton.onClick.AddListener(()=>{
            EventDispatcher.Instance.AddEventListener(EventKey.AdShowSuccessCallBack, OnVideoPlayOver);
            PluginMercury.Instance.ActiveRewardVideo();
        });
        nextLevelButton.onClick.AddListener(NextLevelVideo);
        tripleAccessButton.onClick.AddListener(TripleAccess);
        /*secondChanceButton.onClick.AddListener(() => {
            EventDispatcher.Instance.AddEventListener(EventKey.AdShowSuccessCallBack, OnVideoPlayOver);
            PluginMercury.Instance.ActiveRewardVideo();
            //SecondChance();
        });*/

        newRestartButton.onClick.AddListener(Restart);
        restartButton.onClick.AddListener(() => Restart());
        testWinButton.gameObject.SetActive(PlugingSingmaan.Instance.isDebugMode);
        if (PlugingSingmaan.Instance.isDebugMode)
        {
            testWinButton.onClick.AddListener(() =>
            {
                EventManager.TriggerEvent("Win");
                testWinButton.gameObject.SetActive(false);
            });
        }
        
//#if _DEBUG
//        debugBoost.gameObject.SetActive(true);
//        debugBoost.onClick.AddListener(() => Blob.Instance.ActivateBoost(5f));
//#else
            debugBoost.gameObject.SetActive(false);
//#endif
    }

    private void LoadNextLevel()
    {
        backGroundObject.SetActive(false);
        winParticles.SetActive(false);
        completed.SetActive(false);

        if (ProfileManager.Instance.levelnumber % 5 == 0)
            GameLogic.Instance.OnWin();
        else
            RewardProgress();
        Debug.LogError("下一关");
    }

    private void TripleAccess()
    {
        EventDispatcher.Instance.AddEventListener(EventKey.AdShowSuccessCallBack, OnTripleAccess);
        PluginMercury.Instance.ActiveRewardVideo();
    }

    private void OnTripleAccess(string msg)
    {
        int completeBonus = (ProfileManager.Instance.levelnumber * 10) / 3;
        ProfileManager.Instance.Coin += (completeBonus * 2);
        EventDispatcher.Instance.RemoveEventListener(EventKey.AdShowSuccessCallBack, OnTripleAccess);
    }

    protected override void OnShow(ActivateParams activateParams)
    {
        EventManager.StartListening("CoinChange", UpdateCoin);
        EventManager.StartListening("IncreaseCombo", IncreaseCombo);
        EventManager.StartListening("IncreaseScore", UpdateScore);
        EventManager.StartListening("Die", Die);
        EventManager.StartListening("Win", Win);
        EventManager.StartListening("NextLevel", NextLevel);

        backButton.gameObject.SetActive(true);
        Blob.Instance.SetBlobState(BlobState.Play);
        ProfileManager.Instance.Score = 0;
        ResetInGameUI();

        #region GameAnalytics
        //GameAnalyticsManager.LogProgressionEvent(GAProgressionStatus.Start, ProfileManager.Instance.levelnumber.ToString());
        Debug.Log("Analytics: " + "Start " + Application.version + " " + ProfileManager.Instance.levelnumber.ToString("00000"));
        //GameAnalyticsManager.LogProgressionEvent(GAProgressionStatus.Start, Application.version,ProfileManager.Instance.levelnumber.ToString("00000"));
        #endregion

        if (ProfileManager.Instance.tutorialDrag && ProfileManager.Instance.dragControl)
        {
            tutorialDrag.SetActive(true);
            EventManager.StartListening("HideDragTutorial", HideDragTutorial);
        }
        else if (ProfileManager.Instance.tutorialTouch && !ProfileManager.Instance.dragControl)
        {
            tutorialTouch.SetActive(true);
            EventManager.StartListening("HideTouchTutorial", HideTouchTutorial);
        }
    }

    private void OnEnable()
    {
        if (ProfileManager.Instance.dragControl)
        {
            tutorialDragPrompt.SetActive(true);
            Invoke("OnCloseDragPrompt", 3);
        }
        else if (!ProfileManager.Instance.dragControl)
        {
            tutorialTouchPrompt.SetActive(true);
            Invoke("OnCloseTouchPrompt", 3);
        }
    }

    private void OnDisable()
    {

    }

    void OnCloseDragPrompt()
    {
        tutorialDragPrompt.SetActive(false);
    }

    void OnCloseTouchPrompt()
    {
        tutorialTouchPrompt.SetActive(false);
    }

    private void HideDragTutorial()
    {
        tutorialDrag.SetActive(false);
        EventManager.StopListening("HideDragTutorial", HideDragTutorial);
        ProfileManager.Instance.tutorialDrag = false;
        ProfileManager.Instance.Save();
    }

    private void HideTouchTutorial()
    {
        tutorialTouch.SetActive(false);
        EventManager.StopListening("HideTouchTutorial", HideTouchTutorial);
        ProfileManager.Instance.tutorialTouch = false;
        ProfileManager.Instance.Save();
    }

    protected override void OnClose()
    {
        GameLogic.Instance.BackToMain();
        ResetInGameUI();
        EventManager.StopListening("CoinChange", UpdateCoin);
        EventManager.StopListening("IncreaseCombo", IncreaseCombo);
        EventManager.StopListening("IncreaseScore", UpdateScore);
        EventManager.StopListening("Die", Die);
        EventManager.StopListening("Win", Win);
        EventManager.StopListening("NextLevel", NextLevel);
    }

    public void StopListetingOnBack()
    {
        EventManager.StopListening("CoinChange", UpdateCoin);
        EventManager.StopListening("IncreaseCombo", IncreaseCombo);
        EventManager.StopListening("IncreaseScore", UpdateScore);
        EventManager.StopListening("Die", Die);
        EventManager.StopListening("Win", Win);
        EventManager.StopListening("NextLevel", NextLevel);
    }

    private void NextLevel()
    {
        #region GameAnalytics
        //GameAnalyticsManager.LogProgressionEvent(GAProgressionStatus.Start, ProfileManager.Instance.levelnumber.ToString());
        Debug.Log("Analytics: " + "Start " + Application.version + " " + ProfileManager.Instance.levelnumber.ToString("00000"));
        //GameAnalyticsManager.LogProgressionEvent(GAProgressionStatus.Start, Application.version, ProfileManager.Instance.levelnumber.ToString("00000"));
        #endregion
        ResetInGameUI();
        ByteDanceSDKManager.Instance.StartRecord();
    }

    private void ResetInGameUI()
    {
        if (PlugingSingmaan.Instance.isDebugMode)
        {
            testWinButton.gameObject.SetActive(true);
        }
        
        secondChanceAvailable = true;
        backButton.gameObject.SetActive(true);
        currentscoreText.text = ProfileManager.Instance.Score.ToString();
        comboText.gameObject.SetActive(false);
        time = 0;
        comboText.text = "+" + ProfileManager.Instance.Combo;
        animator.Play("Open");
        GameLogic.Instance.CanRotate = true;
        completed.SetActive(false);
        coinText.text = ProfileManager.Instance.Coin.ToString();
        secondChanceButton.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        backGroundObject.SetActive(false);
        OnOpenCloseButton(false);

        rewardProgressBar.SetActive(false);
        openReward.SetActive(false);
        PurchaseAvailable();
        ProgressBarInit();
    }

    private void ProgressBarInit()
    {
        if (ProfileManager.Instance.levelnumber % 5 == 0)
            bonusLevelText.text = LocalizationManager.Instance.GetString("ID_BONUS_LEVEL");
        else
            bonusLevelText.text = LocalizationManager.Instance.GetString("ID_LEVEL_NUMBER") + ProfileManager.Instance.levelnumber;

        StartCoroutine(ShowBonusLevelTitle());
        currentLevelText.text = ProfileManager.Instance.levelnumber.ToString();
        nextLevelText.text = (ProfileManager.Instance.levelnumber + 1).ToString();
        levelProgress.CurrentValue = 0f;
        levelProgress.MaxValue = Mathf.Abs(LevelManager.Instance.finalpoint.transform.position.y);

        //if(0 == ProfileManager.Instance.levelnumber%5)
        //{
        //    PluginMercury.Instance.ActiveRewardVideo();屏蔽每五关看广告
        //}
    }

    public void PurchaseAvailable()
    {
        int coin = ProfileManager.Instance.Coin;

        if (coin >= ProfileManager.Instance.randomCharacterPrice || coin >= ProfileManager.Instance.randomMusicPrice || coin >= ProfileManager.Instance.randomTrailPrice)
        {
            shopNotification.SetActive(true);
        }
        else
        {
            shopNotification.SetActive(false);
        }
    }

    IEnumerator ShowBonusLevelTitle()
    {
        bonusLevelHolder.SetActive(true);
        yield return new WaitForSeconds(bonusLevelDelay);
        bonusLevelHolder.SetActive(false);
    }

    private void RewardProgress()
    {
        rewardProgressBar.gameObject.SetActive(true);
        backGroundObject.SetActive(true);
    }


    private void NextLevelVideo()
    {
        EventDispatcher.Instance.AddEventListener(EventKey.AdShowSuccessCallBack, OnNextLevelVideoSuccess);
        PluginMercury.Instance.ActiveRewardVideo();
    }

    public void OnNextLevelVideoSuccess(string msg)
    {
        backButton.gameObject.SetActive(false);
        int completeBonus = (ProfileManager.Instance.levelnumber * 10) / 3;
        ProfileManager.Instance.Coin += completeBonus;
        ProfileManager.Instance.Save();
        StartCoroutine(WaitWin());
    }

    IEnumerator WaitWin()
    {
        GameLogic.Instance.OnWin();
        OnOpenCloseButton(false);
        completed.SetActive(false);
        winParticles.SetActive(false);
        backGroundObject.SetActive(false);
        yield return new WaitForSeconds(5);
        EventDispatcher.Instance.RemoveEventListener(EventKey.AdShowSuccessCallBack, OnNextLevelVideoSuccess);
        //if (ProfileManager.Instance.levelnumber % 5 == 0)
        //else
        //   RewardProgress();

        // EventDispatcher.Instance.RemoveEventListener(EventKey.AdShowSuccessCallBack, OnNextLevelVideoSuccess);
    }


    private void Win()
    {
        ByteDanceSDKManager.Instance.StopRecord();

        backButton.gameObject.SetActive(false);
        winParticles.SetActive(true);
        int completeBonus = (ProfileManager.Instance.levelnumber * 10) / 3;
        completedText.text = "Level " + ProfileManager.Instance.levelnumber + " completed!";
        levelBonusText.text = "+" + completeBonus * 2;
        currentBonusText.text = "+" + completeBonus;
        ProfileManager.Instance.Coin += completeBonus;
        ProfileManager.Instance.Save();
        completed.SetActive(true);
        backGroundObject.SetActive(true);
        #region GameAnalytics
        //GameAnalyticsManager.LogProgressionEvent(GAProgressionStatus.Complete, ProfileManager.Instance.levelnumber.ToString(), ProfileManager.Instance.Score);
        Debug.Log("Analytics: " + "Complete " + Application.version + " " + ProfileManager.Instance.levelnumber.ToString("00000") + " " + ProfileManager.Instance.Score);
        //GameAnalyticsManager.LogProgressionEvent(GAProgressionStatus.Complete, Application.version, ProfileManager.Instance.levelnumber.ToString("00000"), ProfileManager.Instance.Score);
        #endregion

       // StartCoroutine(WaitAfterWin());
    }

    IEnumerator WaitAfterWin()
    {
        yield return new WaitForSeconds(waitAfterWin);
        backGroundObject.SetActive(false);
        winParticles.SetActive(false);
        completed.SetActive(false);

        if (ProfileManager.Instance.levelnumber % 5 == 0)
            GameLogic.Instance.OnWin();
        else
            RewardProgress();
    }

    private void IncreaseCombo()
    {
        time = 1;
        comboText.text = "+" + (ProfileManager.Instance.levelnumber + ProfileManager.Instance.Combo);
    }

    private void UpdateScore()
    {
        currentscoreText.text = ProfileManager.Instance.Score.ToString();
    }

    private void UpdateCoin()
    {
        coinText.text = ProfileManager.Instance.Coin.ToString();
        StartCoroutine(AddCoinText(1f));
    }

    IEnumerator AddCoinText(float time)
    {
        coinAddText.text = "+" + ProfileManager.Instance.coinIncreased;
        coinAddText.gameObject.SetActive(true);
        yield return new WaitForSeconds(time);
        coinAddText.gameObject.SetActive(false);
    }

    private void Die()
    {
        #region GameAnalytics
        //GameAnalyticsManager.LogProgressionEvent(GAProgressionStatus.Fail, ProfileManager.Instance.levelnumber.ToString(), ProfileManager.Instance.Score);
        Debug.Log("Analytics: " + "Fail " + Application.version + " " + ProfileManager.Instance.levelnumber.ToString("00000") + " " + ProfileManager.Instance.Score);
        //GameAnalyticsManager.LogProgressionEvent(GAProgressionStatus.Fail, Application.version, ProfileManager.Instance.levelnumber.ToString("00000"), ProfileManager.Instance.Score);
        #endregion

        backButton.gameObject.SetActive(false);
        if (Application.internetReachability != NetworkReachability.NotReachable && levelProgress.CurrentValue >= levelProgress.MaxValue / 2 && secondChanceAvailable)
        {
            OnOpenCloseButton(true);
            restartText.text = LocalizationManager.Instance.GetString("ID_NO_THANKS");
            //secondChanceButton.gameObject.SetActive(true);
            //restart = ActivateRestartButton();
            //StartCoroutine(restart);
            Debug.Log("11111");
        }
        else
        {
            restartText.text = LocalizationManager.Instance.GetString("ID_RESTART");
            Debug.Log("2222");
            restartButton.gameObject.SetActive(true);
            backGroundObject.SetActive(true);
        }

        EndRecord();
    }

    void EndRecord()
    {
        Debug.Log("结束录屏");
        ByteDanceSDKManager.Instance.StopRecord();
    }

    void StartRecord()
    {
        Debug.Log("开始录屏");
        ByteDanceSDKManager.Instance.StartRecord();
    }

    IEnumerator ActivateRestartButton()
    {
        yield return new WaitForSecondsRealtime(delay);
        restartButton.gameObject.SetActive(true);
    }

    private void SecondChance()
    {
        // if (PlugingSingmaan.Instance.isDebugMode)
        // {}
            OnOpenCloseButton(false);
            //StopCoroutine(restart);
            secondChanceButton.gameObject.SetActive(false);
            restartButton.gameObject.SetActive(false);
            backButton.gameObject.SetActive(true);
            backGroundObject.SetActive(false);
        GameLogic.Instance.StartFromCheckpoint();
        //AdManagerIronsrc.Instance.ShowVideoAd(results =>
        //{
        //    if (results)
        //    {
        //        StopCoroutine(restart);
        //        secondChanceButton.gameObject.SetActive(false);
        //        restartButton.gameObject.SetActive(false);
        //        backButton.gameObject.SetActive(true);
        //        GameLogic.Instance.StartFromCheckpoint();
        //        secondChanceAvailable = false;
        //    }
        //});      
    }

    private void Restart()
    {
        if (PlugingSingmaan.Instance.isDebugMode)
        {
            OnOpenCloseButton(false);
            secondChanceButton.gameObject.SetActive(false);
            restartButton.gameObject.SetActive(false);
            backButton.gameObject.SetActive(true);
            GameLogic.Instance.StartFromCheckpoint();
        }
        else
        {
            OnOpenCloseButton(false);
            secondChanceAvailable = true;
            secondChanceButton.gameObject.SetActive(false);
            restartButton.gameObject.SetActive(false);
            GameLogic.Instance.Restart();
            ResetInGameUI();
            Debug.Log("Analytics: " + "Restart " + Application.version + " " + ProfileManager.Instance.levelnumber.ToString("00000"));
            //GameAnalyticsManager.LogProgressionEvent(GAProgressionStatus.Start, Application.version, ProfileManager.Instance.levelnumber.ToString("00000"));
        }

        StartRecord();
    }

    private void Update()
    {
        if (time > 0)
        {
            comboText.gameObject.SetActive(true);
            time -= Time.deltaTime;
        }
        else
        {
            comboText.gameObject.SetActive(false);
            time = 0;
        }

        levelProgress.CurrentValue = Mathf.Abs(cameraHolder.position.y);

        if (Input.GetKeyDown(KeyCode.Escape) && backButton.isActiveAndEnabled)
            MenuManager.Instance.Back();
    }

    void OnVideoPlayOver(string msg)
    {
        EventDispatcher.Instance.RemoveEventListener(EventKey.AdShowSuccessCallBack, OnVideoPlayOver);
        SecondChance();
        StartRecord();
    }

    void OnOpenCloseButton(bool isActiva)
    {
        nextLevelButton.gameObject.SetActive(isActiva);
        newRestartButton.gameObject.SetActive(isActiva);
        reviveButton.gameObject.SetActive(isActiva);
        shareButton.gameObject.SetActive(isActiva);
        backGroundObject.SetActive(isActiva);
    }

    public void Share()
    {
        ByteDanceSDKManager.Instance.onShareResult = ShareReward;
        ByteDanceSDKManager.Instance.Share();
    }

    void ShareReward()
    { 
        ProfileManager.Instance.Coin += 300;
    }
}
