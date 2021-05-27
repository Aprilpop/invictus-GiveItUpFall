using MenuGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

[System.Serializable]
public class MusicButton
{
    public int ID;
    public Button button;
    public MusicArrayEnum musicEnum;
    public Image lockImage;
    public Image playImage;
    public Image playingImage;
    public bool locked;
}

[AddComponentMenu("Menu/CharacterSelection")]
public class CharacterSelection : Menu
{
    public override int UniqueID { get { return (int)MenuTypes.CharacterSelection; } }

    [SerializeField] CanvasGroup root;

    [SerializeField] Text coin;
    [SerializeField] Text randomCharacterPrice;
    [SerializeField] Text randomTrailPrice;
    [SerializeField] Text randomMusicPrice;

    [SerializeField] int increasePrice = 250;

    [SerializeField] Button randomCharacterButton;
    [SerializeField] Button randomTrailButton;
    [SerializeField] Button randomMusicButton;

    [SerializeField] Button[] blobButtons;
    [SerializeField] Button[] trailButtons;

    [SerializeField] Text coinForVideo;
    [SerializeField] Button coinVideoButton;

    [SerializeField] Button blobNavigateButton;
    [SerializeField] Button trailNavigateButton;

    [SerializeField] GameObject BlobPanel;
    [SerializeField] GameObject TrailPanel;
    [SerializeField] GameObject MusicPanel;

    [SerializeField] RectTransform blobRing;
    [SerializeField] RectTransform trailRing;

    [SerializeField] Button DebugGold;

    [SerializeField]
    RectTransform selectedBlob;

    [SerializeField]
    RectTransform selectedTrail;

    [SerializeField]
    Animator animator;

    [SerializeField] float findingTime = 0.2f;

    int unlockID;

    [SerializeField]
    GameObject RightBlobNotification;

    [SerializeField]
    GameObject RightTrailNotification;

    [SerializeField]
    GameObject LeftTrailNotification;

    [SerializeField]
    GameObject LeftMusicNotification;

    [SerializeField] MusicButton[] musics;

    private void Awake()
    {
        for (int i = 0; i < ProfileManager.Instance.Blobs.Length; i++)
        {
            int index = i;
            blobButtons[index].onClick.AddListener(() => ProfileManager.Instance.SetCharacter(ProfileManager.Instance.Blobs[index].name));
            blobButtons[index].onClick.AddListener(() => SetSelectedBlob());
        }

        for (int i = 0; i < ProfileManager.Instance.Trails.Length; i++)
        {
            int index = i;
            trailButtons[index].onClick.AddListener(() => ProfileManager.Instance.SetTrail(ProfileManager.Instance.Trails[index].name));
            trailButtons[index].onClick.AddListener(() => SetSelectedTrail());
        }
        
        randomCharacterButton.onClick.AddListener(() => BuyRandomBlob());
        randomTrailButton.onClick.AddListener(() => BuyRandomTrail());
        randomMusicButton.onClick.AddListener(() => BuyMusic());

        coinVideoButton.onClick.AddListener(() => GetCoinForVideo());
        Debug.Log("初始化了");
        
    //#if _DEBUG
    //    DebugGold.gameObject.SetActive(true);
    //    DebugGold.onClick.AddListener(() => GetCoin());
    //#else
        DebugGold.gameObject.SetActive(false);
    //#endif
        
    }

    

    private void LoadMusics()
    {
        for (int i = 0; i < musics.Length; i++)
        {
            musics[i].locked = ProfileManager.Instance.musicsLocks[i];
        }
    }

    private void Navigate()
    {
        if (BlobPanel.activeInHierarchy)
        {
            animator.Play("OpenTrails");
            CheckForUnlockableTrails();
            //SetNotification();
        }
        else if (TrailPanel.activeInHierarchy)
        {
            animator.Play("OpenBlobs");
            CheckForUnlockableCharacters();
        //    SetNotification();
        }
    }

    public void OpenBlobPanel()
    {
        animator.Play("OpenBlobs");
        CheckForUnlockableCharacters();
        SetNotification();
        //if (ProfileManager.Instance.Coin >= ProfileManager.Instance.randomMusicPrice || ProfileManager.Instance.Coin >= ProfileManager.Instance.randomTrailPrice)
        //{
        //    RightBlobNotification.SetActive(true);

        //}
        //else
        //{
        //    RightBlobNotification.SetActive(false);
        //}
    }

    public void SetNotification()
    {
        int coin = ProfileManager.Instance.Coin;

            if (coin >= ProfileManager.Instance.randomMusicPrice || coin >= ProfileManager.Instance.randomTrailPrice)
            {
                RightBlobNotification.SetActive(true);
            }
            else
            {
                RightBlobNotification.SetActive(false);
            }
        
   
            if (coin >= ProfileManager.Instance.randomMusicPrice)
            {
                RightTrailNotification.SetActive(true);
            }
            else
            {
                RightTrailNotification.SetActive(false);
            }

            if (coin >= ProfileManager.Instance.randomCharacterPrice)
            {
                LeftTrailNotification.SetActive(true);

            }
            else
            {
                LeftTrailNotification.SetActive(false);
            }
  
            if (coin >= ProfileManager.Instance.randomCharacterPrice || coin >= ProfileManager.Instance.randomTrailPrice)
            {
                LeftMusicNotification.SetActive(true);
            }
            else
            {
                LeftMusicNotification.SetActive(false);
            }
        
    }

    public void OpenTrailsPanel()
    {
        if (BlobPanel.activeInHierarchy)
        {
            animator.Play("OpenTrails");
            CheckForUnlockableTrails();
        }
        else
        {
            animator.Play("OpenTrailsFromMusic");
            CheckForUnlockableTrails();
        }
        SetNotification();
    }

    public void OpenMusicPanel()
    {
        animator.Play("OpenMusics");
        CheckMusicButtons();
        SetNotification();
    }

    public void SetSelectedBlob()
    {
        int currentBlobIndex = ProfileManager.Instance.GetCharacterIndex(ProfileManager.Instance.currentBlob);
        selectedBlob.anchoredPosition = blobButtons[currentBlobIndex].GetComponent<RectTransform>().anchoredPosition;
    }

    public void SetSelectedTrail()
    {
        int currentTrailIndex = ProfileManager.Instance.GetTrailIndex(ProfileManager.Instance.currentTrailName);
        selectedTrail.anchoredPosition = trailButtons[currentTrailIndex].GetComponent<RectTransform>().anchoredPosition;
    }


    protected override void OnShow(ActivateParams activateParams)
    {
        LoadMusics();
        SetSelectedBlob();
        SetSelectedTrail();
        EventManager.StartListening("VideoAdAvailable", VideoAvailable);
        EventManager.StartListening("VideoAdNotAvailable", VideoNotAvailable);
        EventManager.StartListening("CoinChange", UpdateCoin);
        randomCharacterPrice.text = "300"/*ProfileManager.Instance.randomCharacterPrice.ToString()*/;
        randomTrailPrice.text = "180"/*ProfileManager.Instance.randomTrailPrice.ToString()*/;
        randomMusicPrice.text = ProfileManager.Instance.randomMusicPrice.ToString();

        coin.text = ProfileManager.Instance.Coin.ToString();
        coinForVideo.text = "+" + ProfileManager.Instance.coinForVideo.ToString();

        for (int i = 0; i < ProfileManager.Instance.Blobs.Length; i++)
        {
            if (ProfileManager.Instance.Blobs[i].unlocked)
            {
                blobButtons[i].interactable = true;
                for (int j = 0; j < blobButtons[i].transform.childCount; j++)
                {
                    blobButtons[i].transform.GetChild(j).GetComponent<Image>().gameObject.SetActive(false);// .enabled = false;
                }
            }
            else
            {
                blobButtons[i].interactable = false;
            }
        }

        for (int i = 0; i < ProfileManager.Instance.Trails.Length; i++)
        {
            if (ProfileManager.Instance.Trails[i].unlocked)
            {
                trailButtons[i].interactable = true;
                for (int j = 0; j < trailButtons[i].transform.childCount; j++)
                {
                    trailButtons[i].transform.GetChild(j).GetComponent<Image>().gameObject.SetActive(false);// enabled = false;
                }
            }
            else
                trailButtons[i].interactable = false;
        }

        CheckForUnlockableCharacters();
        CheckForUnlockableTrails();
        CheckMusicButtons();
        CheckNetwork();

        SetNotification();

       // EventDispatcher.Instance.AddEventListener(EventKey.AdShowSuccessCallBack, OnVideoPlayOverCallBacks);
    }

    protected override void OnClose()
    {
        EventManager.StopListening("CoinChange", UpdateCoin);
        EventManager.StopListening("VideoAdAvailable", VideoAvailable);
        EventManager.StopListening("VideoAdNotAvailable", VideoNotAvailable);

        EventDispatcher.Instance.RemoveEventListener(EventKey.AdShowSuccessCallBack, OnVideoPlayOverCallBacks);
    }

    public void GetCoinForVideo()
    {
        // AdManagerIronsrc.Instance.ShowVideoAd(results =>
        // {
        //    if (results)
        //    {
        //        ProfileManager.Instance.Coin += ProfileManager.Instance.coinForVideo;
        //        ProfileManager.Instance.Save();
        //    }
        // });
        Debug.Log("点击了广告");
        EventDispatcher.Instance.AddEventListener(EventKey.AdShowSuccessCallBack, OnVideoPlayOverCallBacks);
        PluginMercury.Instance.ActiveRewardVideo();
        
    }

    public void GetCoin()
    {
        ProfileManager.Instance.Coin += 1000;
        ProfileManager.Instance.Save();
    }

    private void VideoAvailable()
    {
        coinVideoButton.interactable = true;
    }

    private void VideoNotAvailable()
    {
        coinVideoButton.interactable = false;
    }

    private void UpdateCoin()
    {
        coin.text = ProfileManager.Instance.Coin.ToString();
       // randomCharacterPrice.text = ProfileManager.Instance.randomCharacterPrice.ToString();
        //randomTrailPrice.text = ProfileManager.Instance.randomTrailPrice.ToString();
        //randomMusicPrice.text = ProfileManager.Instance.randomMusicPrice.ToString();
        SetNotification();
    }

    private void ForbidButtons(bool forbid)
    {
        if(forbid)
            root.blocksRaycasts = false;
        else
            root.blocksRaycasts = true;
    }

    public void BuyRandomBlob()
    {
        if (ProfileManager.Instance.DescreaseCoin(300/*ProfileManager.Instance.randomCharacterPrice*/))
        {
            ForbidButtons(true);
            unlockID = FindRandomBlob();
            //Debug.Log("unlockID:" + unlockID);
            FindLockedPositions();
        }
        else
        {
            SoundManager.Play(SfxArrayEnum.NoCoin, transform.position);
            coin.GetComponent<Animator>().Play("NotEnoughCoin", -1, 0f);
            Debug.Log("Don't have enough coin!");
            if (Application.internetReachability != NetworkReachability.NotReachable)
                MenuManager.Instance.Show((int)MenuTypes.WatchAd);
        }
    }

    public void BuyRandomTrail()
    {
        if (ProfileManager.Instance.DescreaseCoin(180/*ProfileManager.Instance.randomTrailPrice*/))
        {
            ForbidButtons(true);
            unlockID = FindRandomTrail();
            FindLockedPositions();
        }
        else
        {
            SoundManager.Play(SfxArrayEnum.NoCoin, transform.position);
            coin.GetComponent<Animator>().Play("NotEnoughCoin", -1, 0f);
            Debug.Log("Don't have enough coin!");
            if (Application.internetReachability != NetworkReachability.NotReachable)
                MenuManager.Instance.Show((int)MenuTypes.WatchAd);
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

    private void FindLockedPositions()
    {
        
        List<Vector2> buttonPositions = new List<Vector2>();
        
        if (BlobPanel.activeInHierarchy)
        {
            randomCharacterButton.gameObject.SetActive(false);

            for (int i = 0; i < blobButtons.Length; i++)
            {
                if (!blobButtons[i].interactable)
                {
                    buttonPositions.Add(blobButtons[i].GetComponent<RectTransform>().anchoredPosition);
                }
            }

            StartCoroutine(UnlockAnimation(buttonPositions,blobRing));

        }
        else if (TrailPanel.activeInHierarchy)
        {

            randomTrailButton.gameObject.SetActive(false);
            for (int i = 0; i < trailButtons.Length; i++)
            {
                if (!trailButtons[i].interactable)
                {
                    buttonPositions.Add(trailButtons[i].GetComponent<RectTransform>().anchoredPosition);
                }
            }

            StartCoroutine(UnlockAnimation(buttonPositions, trailRing));
        }
    }

    IEnumerator UnlockAnimation(List<Vector2> positions, RectTransform ring)
    {
        int count = positions.Count;
        int index = Random.Range(0, positions.Count);
        ring.gameObject.SetActive(true);
        while (count > 0)
        {
            index = Random.Range(0, positions.Count);
            ring.anchoredPosition = positions[index];
            SoundManager.Play(SfxArrayEnum.PlatformBreak, transform.position);
            count--;
            yield return new WaitForSeconds(findingTime);
            yield return null;
        }     
    
        if (BlobPanel.activeInHierarchy)
        {
            
            int characterId = unlockID;
            //Debug.Log("buy" + ":" + ProfileManager.Instance.Blobs[characterId].name + ":" + ProfileManager.Instance.randomCharacterPrice.ToString());
            //GameAnalyticsManager.LogDesignEvent("buy" + ":" + ProfileManager.Instance.Blobs[characterId].name + ":" + ProfileManager.Instance.randomCharacterPrice.ToString());
            ring.anchoredPosition = blobButtons[characterId].GetComponent<RectTransform>().anchoredPosition;
            ring.GetComponentInChildren<Transform>(true).gameObject.SetActive(true);
            SoundManager.Play(SfxArrayEnum.Win, transform.position);
            blobButtons[characterId].interactable = true;
            for (int j = 0; j < blobButtons[characterId].transform.childCount; j++)
            {
                blobButtons[characterId].transform.GetChild(j).GetComponent<Image>().gameObject.SetActive(false);// .enabled = false;
            }
            yield return new WaitForSeconds(1.5f);
            ProfileManager.Instance.Blobs[characterId].unlocked = true;
            
            ProfileManager.Instance.randomCharacterPrice += increasePrice;
            CheckForUnlockableCharacters();
            ProfileManager.Instance.SetCharacter(ProfileManager.Instance.Blobs[characterId].name);
            SetSelectedBlob();
            ProfileManager.Instance.Save();
            
        }
        else if (TrailPanel.activeInHierarchy)
        {
            int trailId = unlockID;
            //Debug.Log("buy" + ":" + ProfileManager.Instance.Trails[trailId].name + ":" + ProfileManager.Instance.randomTrailPrice.ToString());
            //GameAnalyticsManager.LogDesignEvent("buy" + ":" + ProfileManager.Instance.Trails[trailId].name + ":" + ProfileManager.Instance.randomTrailPrice.ToString());
            ring.anchoredPosition = trailButtons[trailId].GetComponent<RectTransform>().anchoredPosition;
            ring.GetComponentInChildren<Transform>(true).gameObject.SetActive(true);
            SoundManager.Play(SfxArrayEnum.Win, transform.position);
            trailButtons[trailId].interactable = true;

            for (int j = 0; j < trailButtons[trailId].transform.childCount; j++)
            {
                trailButtons[trailId].transform.GetChild(j).GetComponent<Image>().gameObject.SetActive(false);// .enabled = false;
            }
            yield return new WaitForSeconds(1.5f);
            ProfileManager.Instance.Trails[trailId].unlocked = true;
            
            ProfileManager.Instance.randomTrailPrice += increasePrice;
            CheckForUnlockableTrails();
            ProfileManager.Instance.SetTrail(ProfileManager.Instance.Trails[trailId].name);
            SetSelectedTrail();
            ProfileManager.Instance.Save();
        }
        UpdateCoin();
        ring.GetComponentInChildren<Transform>(true).gameObject.SetActive(false);
        ring.gameObject.SetActive(false);
        ForbidButtons(false);
    }

    private int FindRandomBlob()
    {
        List<int> unlockables = new List<int>();
        for (int i = 0; i < ProfileManager.Instance.Blobs.Length; i++)
        {
            if (ProfileManager.Instance.Blobs[i].unlocked == false)
            {
                unlockables.Add(i);
            }
        }

        int characterId = Random.Range(0, unlockables.Count);

        if (unlockables.Count == 0) return -1;

        return unlockables[characterId];
    }

    private int FindRandomTrail()
    {
        List<int> unlockables = new List<int>();
        for (int i = 0; i < ProfileManager.Instance.Trails.Length; i++)
        {
            if (ProfileManager.Instance.Trails[i].unlocked == false)
            {
                unlockables.Add(i);
            }
        }

        int trailId = Random.Range(0, unlockables.Count);

        if (unlockables.Count == 0) return -1;

        return unlockables[trailId];
    }

    private void CheckForUnlockableTrails()
    {
        for (int i = 0; i < ProfileManager.Instance.Trails.Length; i++)
        {
            if (ProfileManager.Instance.Trails[i].unlocked == false)
            {
                randomTrailButton.gameObject.SetActive(true);
                break;
            }
            else
                randomTrailButton.gameObject.SetActive(false);
        }
    }

    private void CheckForUnlockableCharacters()
    {
        for (int i = 0; i < ProfileManager.Instance.Blobs.Length; i++)
        {
            if (ProfileManager.Instance.Blobs[i].unlocked == false)
            {
                randomCharacterButton.gameObject.SetActive(true);
                break;
            }
            else
                randomCharacterButton.gameObject.SetActive(false);
        }
    }

    private void CheckUnlockableMusic()
    {
        randomMusicButton.gameObject.SetActive(false);
        for (int i = 0; i < musics.Length; i++)
        {
            if (musics[i].locked)
            {
                randomMusicButton.gameObject.SetActive(true);
                break;
            }
        }
    }

    private void CheckMusicButtons()
    {
        for (int i = 0; i < musics.Length; i++)
        {
            if (musics[i].locked)
            {               
                musics[i].lockImage.enabled = true;
                musics[i].playImage.enabled = false;
                musics[i].playingImage.enabled = false;
                musics[i].button.interactable = false;
            }
            else
            {
                musics[i].button.interactable = true;
                musics[i].lockImage.enabled = false;
                if (MusicManager.Instance.last == musics[i].musicEnum)
                {
                    musics[i].playingImage.enabled = true;
                    musics[i].playImage.enabled = false;
                }
                else
                {
                    musics[i].playImage.enabled = true;
                    musics[i].playingImage.enabled = false;
                }
            }
        }
        CheckUnlockableMusic();
    }

    public void BuyMusic()
    {
        EventDispatcher.Instance.AddEventListener(EventKey.AdShowSuccessCallBack, UnlockMusic);
        PluginMercury.Instance.ActiveRewardVideo();
        /*if (ProfileManager.Instance.DescreaseCoin(ProfileManager.Instance.randomMusicPrice))
        {
            List<MusicButton> buttons = new List<MusicButton>();
            randomMusicButton.gameObject.SetActive(false);
            for (int i = 0; i < musics.Length; i++)
            {
                if (musics[i].locked)
                {
                    buttons.Add(musics[i]);
                }
            }
            int index = Random.Range(0,buttons.Count);
            for (int i = 0; i < musics.Length; i++)
            {
                if(musics[i].ID == buttons[index].ID)
                {
                    musics[i].locked = false;
                    ProfileManager.Instance.musicsLocks[i] = musics[i].locked;
                    SoundManager.Play(SfxArrayEnum.Win, transform.position);
                    //Debug.Log("buy" + ":" + musics[i].musicEnum + ":" + ProfileManager.Instance.randomMusicPrice.ToString());
                    //GameAnalyticsManager.LogDesignEvent("buy" + ":" + musics[i].musicEnum + ":" + ProfileManager.Instance.randomMusicPrice.ToString());
                    ProfileManager.Instance.randomMusicPrice += increasePrice;
                    ProfileManager.Instance.Save();
                    UpdateCoin();
                    PlayMusic(i);
                    break;
                }
            }

            CheckMusicButtons();
        }
        else
        {
            SoundManager.Play(SfxArrayEnum.NoCoin, transform.position);
            coin.GetComponent<Animator>().Play("NotEnoughCoin", -1, 0f);
            Debug.Log("Don't have enough coin!");
            if (Application.internetReachability != NetworkReachability.NotReachable)
                MenuManager.Instance.Show((int)MenuTypes.WatchAd);
        }*/


    }

    void UnlockMusic(string msg)
    {
        List<MusicButton> buttons = new List<MusicButton>();
        randomMusicButton.gameObject.SetActive(false);
        for (int i = 0; i < musics.Length; i++)
        {
            if (musics[i].locked)
            {
                buttons.Add(musics[i]);
            }
        }
        int index = Random.Range(0, buttons.Count);
        for (int i = 0; i < musics.Length; i++)
        {
            if (musics[i].ID == buttons[index].ID)
            {
                musics[i].locked = false;
                ProfileManager.Instance.musicsLocks[i] = musics[i].locked;
                SoundManager.Play(SfxArrayEnum.Win, transform.position);
                ProfileManager.Instance.randomMusicPrice += increasePrice;
                ProfileManager.Instance.Save();
                UpdateCoin();
                PlayMusic(i);
                break;
            }
        }

        CheckMusicButtons();

        EventDispatcher.Instance.RemoveEventListener(EventKey.AdShowSuccessCallBack, UnlockMusic);
    }

    public void PlayMusic(int index)
    {
        if (MusicManager.Instance.last == musics[index].musicEnum)
            return;
        

        MusicManager.Instance.PlayMusic(musics[index].musicEnum);
        ProfileManager.Instance.currentMusic = musics[index].musicEnum;
        ProfileManager.Instance.Save();
        CheckMusicButtons();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && root.blocksRaycasts)
            MenuManager.Instance.Back();
    }
    void OnVideoPlayOverCallBacks(string msg)
    {
        ProfileManager.Instance.Coin += ProfileManager.Instance.coinForVideo;
        coin.text = ProfileManager.Instance.Coin.ToString();
        EventDispatcher.Instance.RemoveEventListener(EventKey.AdShowSuccessCallBack, OnVideoPlayOverCallBacks);
    }

    public void UnlockRole()
    {
        unlockID = FindRandomBlob();

        if (unlockID == -1) return;

        ProfileManager.Instance.Blobs[unlockID].unlocked = true;
        ProfileManager.Instance.randomCharacterPrice += increasePrice;
        CheckForUnlockableCharacters();
        ProfileManager.Instance.SetCharacter(ProfileManager.Instance.Blobs[unlockID].name);
        SetSelectedBlob();
        ProfileManager.Instance.Save();
    }

    public void UnlockTrail()
    {
        unlockID = FindRandomTrail();

        if (unlockID == -1) return;

        ProfileManager.Instance.Trails[unlockID].unlocked = true;
        ProfileManager.Instance.randomTrailPrice += increasePrice;
        CheckForUnlockableTrails();
        ProfileManager.Instance.SetTrail(ProfileManager.Instance.Trails[unlockID].name);
        SetSelectedTrail();
        ProfileManager.Instance.Save();
    }

    public void IsUnlockComplete(out int role,out int trail)
    {
        role = FindRandomBlob();
        trail = FindRandomTrail();
    }
}
