using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Character
{
    public string name;
    public Sprite sprite;
    public bool unlocked;
}

[System.Serializable]
public class Trail
{
    public string name;
    public GameObject trail;
    public bool unlocked;
}

public class ProfileManager : MonoBehaviour
{
    private static ProfileManager instance;
    public static ProfileManager Instance { get { return instance; } }

    [Header("Prices"), Tooltip("The prices of shop items")]
    public int randomCharacterPrice = 300;
    public int randomTrailPrice = 300;
    public int randomMusicPrice = 300;

    [Header("Control")]
    public bool dragControl;
    
    [Header("Ingame")]
    public int coinForVideo = 300;
    [HideInInspector]
    public int levelColor = -1;

    public int coinPickupAmount = 3;
    [HideInInspector]
    public MusicArrayEnum currentMusic;
    [HideInInspector]
    public bool[] musicsLocks = new bool[9] {false, true, true, true, true, true, true, true, true };
    [HideInInspector]
    public int coinIncreased = 0;
    [HideInInspector]
    public bool tutorialDrag = false;
    [HideInInspector]
    public bool tutorialTouch = false;

    [HideInInspector]
    public int highScore, levelnumber;
    [HideInInspector]
    public string currentBlob,lastPlayedLevel;
    [HideInInspector]
    public string currentTrailName;
    [HideInInspector]
    public int boostOnLevel = 0;

    public int levelLength = 20;
    [HideInInspector]
    public int rewardProgress = 0;
    [HideInInspector]
    public float welcomeNotiTime = 0;
    [HideInInspector]
    public bool noAdsPurchased = false;
    [HideInInspector]
    public int currentLanguageIndex;
    [HideInInspector]
    public bool rewardDoubleWatched = false;

    ReminderNotification reminderNotification;
    
    GameObject currentTrail;
    public GameObject CurrentTrail { get { return currentTrail; } set { currentTrail = value; } }

    private int coin = 0;
    public int Coin
    {
        get { return coin; }
        set
        {
            if (coin == value)
                return;

            coinIncreased = value - coin;

            coin = value;
            EventManager.TriggerEvent("CoinChange");
        }
    }

    private int combo = -1;
    public int Combo
    {
        get { return combo; }
        set
        {
            if (combo == value)
                return;

            combo = value;

            if(combo != -1)
                EventManager.TriggerEvent("IncreaseCombo");
        }
    }

    private int score;
    public int Score
    {
        get { return score; }
        set
        {
            if (score == value)
                return;

            score = value;
            EventManager.TriggerEvent("IncreaseScore");

            if (score > highScore)
                highScore = score;
        }
    }

    bool soundEnabled = true;
    bool musicEnabled = true;

    public bool Sound
    {
        get { return soundEnabled; }
        set { soundEnabled = value; if (MusicManager.Instance) MusicManager.Instance.SetSoundVol(value); }
    }
    public bool Music
    {
        get { return musicEnabled; }
        set { musicEnabled = value; if (MusicManager.Instance) MusicManager.Instance.SetMusicVol(value); }
    }

    [SerializeField]
    Character[] blobs;
    public Character[] Blobs { get { return blobs; } }

    [SerializeField]
    Trail[] trails;
    public Trail[] Trails { get { return trails; } }

    //private void OnInitComplete()
    //{
    //    Debug.Log("Facebook inited");
    //}

    //private void OnHideUnity(bool isGameShown)
    //{
    //    Debug.Log("Facebook hide");
    //}


    private void Awake()
    {

        //FB.Init(this.OnInitComplete, this.OnHideUnity);
        //GameAnalyticsManager.Initialize();

        if (instance != null && instance != this)
            Destroy(this.gameObject);
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);

        int currentLanguageID = PlayerPrefs.GetInt("languageID", LocalizationManager.Instance.GetLanguageIndex(Application.systemLanguage));
        currentLanguageIndex = currentLanguageID;
        LocalizationManager.Instance.SetLanguage(LocalizationManager.Instance.Languages[currentLanguageID].Id);
    }

    void Start()
    {
        Load();
        reminderNotification = new ReminderNotification();
        
        LocalNotification.Instance.CancelNotifications();
    }

    public bool DescreaseCoin(int amount)
    {
        if (Coin >= amount)
        {
            Coin -= amount;
            return true;
        }
        else
            return false;
    }

    public void SetCharacter(string name)
    {
        currentBlob = name;
        GameLogic.Instance.Blob.gameObject.GetComponent<SpriteRenderer>().sprite = GetCharacterByName(currentBlob).sprite;  
    }

    public void SetTrail( int index)
    {
        currentTrailName = trails[index].name;
        //currentTrail = trails[index].trail;
        if (currentTrail != null)
            Destroy(currentTrail);

        currentTrail = Instantiate(trails[index].trail, trails[index].trail.transform.position, Quaternion.identity, Blob.Instance.transform);
    }

    public void SetTrail(string name)
    {
        currentTrailName = name;

        if (currentTrail != null)
            Destroy(currentTrail);

        currentTrail = Instantiate(GetTrailByName(name), Blob.Instance.transform.position, Quaternion.identity, Blob.Instance.transform);
    }

    public Character GetCharacterByName(string name)
    {
        for (int i = 0; i < blobs.Length; i++)
        {
            if (blobs[i].name.Equals(name))
            {
                return blobs[i];
            }
        }
        return null;
    }

    public int GetCharacterIndex(string name)
    {
        for (int i = 0; i < blobs.Length; i++)
        {
            if (blobs[i].name.Equals(name))
            {
                return i;
            }
        }
        return 0;
    }

    public int GetTrailIndex(string name)
    {
        for (int i = 0; i < trails.Length; i++)
        {
            if (trails[i].name.Equals(name))
            {
                return i;
            }
        }
        return 0;
    }

    public GameObject GetTrailByName(string name)
    {
        for (int i = 0; i < trails.Length; i++)
        {
            if (trails[i].name.Equals(name))
            {
                return trails[i].trail;
            }
        }
        return null;
    }

    public void Save()
    {
        PlayerPrefs.SetString("currentBlob", currentBlob);
        PlayerPrefs.SetString("currentTrail", currentTrailName);
        PlayerPrefs.SetInt("highscore", highScore);
        PlayerPrefs.SetInt("levelnumber", levelnumber);
        PlayerPrefs.SetInt("coin", coin);
        PlayerPrefs.SetInt("randomCharacterPrice", randomCharacterPrice);
        PlayerPrefs.SetInt("randomTrailPrice", randomTrailPrice);
        PlayerPrefs.SetInt("randomMusicPrice", randomMusicPrice);
        PlayerPrefs.SetFloat("welcomeNotiTime", welcomeNotiTime);
        PlayerPrefs.SetString("lastPlayedLevel", lastPlayedLevel);
        PlayerPrefs.SetInt("coinForVideo", coinForVideo);
        PlayerPrefs.SetInt("boostOnLevel", boostOnLevel);
        PlayerPrefs.SetInt("levelColor", levelColor);
        PlayerPrefs.SetInt("levelLength", levelLength);
        PlayerPrefs.SetInt("rewardProgress", rewardProgress);
        PlayerPrefs.SetInt("soundEnabled",soundEnabled ? 1 : 0);
        PlayerPrefs.SetInt("musicEnabled", musicEnabled ? 1 : 0);
        PlayerPrefs.SetInt("tutorialDrag", tutorialDrag ? 1 : 0);
        PlayerPrefs.SetInt("tutorialTouch", tutorialTouch ? 1 : 0);
        PlayerPrefs.SetString("currentMusic", currentMusic.ToString());
        PlayerPrefs.SetInt("noAdsPurchased", noAdsPurchased ? 1 : 0 );
        PlayerPrefs.SetInt("dragControl", dragControl ? 1 : 0);
        PlayerPrefs.SetInt("languageID", currentLanguageIndex);

        SaveCharacters(blobs);
        SaveTrails(trails);
        SaveMusics();
        PlayerPrefs.Save();
    }

    public void SaveCharacters(Character[] characters)
    {
        for (int i = 0; i < characters.Length; i++)
        {
            PlayerPrefs.SetInt(characters[i].name + "_unlocked",  characters[i].unlocked ? 1 : 0 );
        }
    }

    public void LoadCharacters(Character[] characters)
    {
        for (int i = 0; i < characters.Length; i++)
        {
            if(characters[i].name.Equals("Blob"))
                characters[i].unlocked = PlayerPrefs.GetInt(characters[i].name + "_unlocked", 1) == 1 ? true : false;
            else
                characters[i].unlocked = PlayerPrefs.GetInt(characters[i].name + "_unlocked", 0) == 1 ? true : false;
        }
    }

    public void SaveTrails(Trail[] trails)
    {
        for (int i = 0; i < trails.Length; i++)
        {
            PlayerPrefs.SetInt(trails[i].name + "_unlocked", trails[i].unlocked ? 1 : 0);
        }
    }

    public void LoadTrails(Trail[] trails)
    {
        for (int i = 0; i < trails.Length; i++)
        {
            if (trails[i].name.Equals("Basic"))
                trails[i].unlocked = PlayerPrefs.GetInt(trails[i].name + "_unlocked", 1) == 1 ? true : false;
            else
                trails[i].unlocked = PlayerPrefs.GetInt(trails[i].name + "_unlocked", 0) == 1 ? true : false;
        }
    }

    public void SaveMusics()
    {
        for (int i = 0; i < musicsLocks.Length; i++)
        {
            PlayerPrefs.SetInt("musics" + i, musicsLocks[i] ? 1 : 0);
        }
    }

    public void LoadMusics()
    {
        for (int i = 0; i < musicsLocks.Length; i++)
        {
            if(i == 0)
                musicsLocks[i] = PlayerPrefs.GetInt("musics" + i, 0) == 1 ? true : false;
            else
                musicsLocks[i] = PlayerPrefs.GetInt("musics" + i, 1) == 1 ? true : false;
        }
    }

    public void Load()
    {
        LoadCharacters(blobs);
        LoadTrails(trails);
        LoadMusics();
        currentBlob = PlayerPrefs.GetString("currentBlob", "Blob");
        currentTrailName = PlayerPrefs.GetString("currentTrail","Basic");
        highScore = PlayerPrefs.GetInt("highscore", 0);
        levelnumber = PlayerPrefs.GetInt("levelnumber", 1);
        coin = PlayerPrefs.GetInt("coin", 0);
        randomCharacterPrice = PlayerPrefs.GetInt("randomCharacterPrice", 300);
        randomTrailPrice = PlayerPrefs.GetInt("randomTrailPrice", 300);
        randomMusicPrice = PlayerPrefs.GetInt("randomMusicPrice", 300);
        welcomeNotiTime = PlayerPrefs.GetFloat("welcomeNotiTime", 0);
        lastPlayedLevel = PlayerPrefs.GetString("lastPlayedLevel","");
        coinForVideo = PlayerPrefs.GetInt("coinForVideo", 300);
        boostOnLevel =PlayerPrefs.GetInt("boostOnLevel", 0);
        levelColor = PlayerPrefs.GetInt("levelColor", -1);
        levelLength = PlayerPrefs.GetInt("levelLength", 20);
        soundEnabled = PlayerPrefs.GetInt("soundEnabled", 1) == 1 ? true : false;
        musicEnabled = PlayerPrefs.GetInt("musicEnabled", 1) == 1 ? true : false;
        rewardProgress = PlayerPrefs.GetInt("rewardProgress", 0);
        tutorialDrag = PlayerPrefs.GetInt("tutorialDrag", 1) == 1 ? true : false;
        tutorialTouch = PlayerPrefs.GetInt("tutorialTouch", 1) == 1 ? true : false;
        currentMusic = (MusicArrayEnum)System.Enum.Parse(typeof(MusicArrayEnum), PlayerPrefs.GetString("currentMusic", "First"));
        noAdsPurchased = PlayerPrefs.GetInt("noAdsPurchased", 0) == 1 ? true : false;
        dragControl = PlayerPrefs.GetInt("dragControl", 1) == 1 ? true : false;
        currentLanguageIndex = PlayerPrefs.GetInt("languageID", LocalizationManager.Instance.GetLanguageIndex(Application.systemLanguage));
    }

    private void OnApplicationQuit()
    {
        Save();
    }
}

public class ReminderNotification : NotificationRequester
{
    public bool OnNotificationRequest(out NotificationRequestData[] requestData)
    {
            requestData = new NotificationRequestData[1];

            requestData[0] = new NotificationRequestData()
                {
                    activationTime = DateTime.Now.Ticks + TimeSpan.TicksPerHour * 24,
                    title = LocalNotification.TITLE,
                    subtitle = LocalizationManager.Instance.GetString("ID_NOTIFICATIONS_REMINDER")
                };
                return true; 
    }

    public ReminderNotification()
    {
        LocalNotification.Instance.notificationRequesters.Add(this);
    }

}
