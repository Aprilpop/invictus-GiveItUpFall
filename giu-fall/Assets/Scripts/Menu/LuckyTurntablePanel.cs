using DG.Tweening;
using MenuGUI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LuckyTurntablePanel : Menu
{
    public override int UniqueID { get { return (int)MenuTypes.LuckyTurntablePanel; } }

    private bool _isRotate = false;//是否旋转
    private float ContinuousTime = 2;//旋转时间
    private float Speed = 500;//旋转速度
    private float Angle = 0; // 这个是设置停止的角度
    private float _time;
    private int currentIndex;
    private int times = 0;
    private int currentRewardCoin;

    public GameObject[] propmtObject;
    public Button[] closePropmtObject;
    public Button[] videoRewardButton;
    public Button[] doubleRewardButton;

    public GameObject[] doubleRewardObject;
    public GameObject turntableObject;
    public CharacterSelection characterselect;
    public Button clickTurntableButton;

    private void Start()
    {
        clickTurntableButton.onClick.AddListener(StartTurntable);

        for (int i = 0; i < 6; i++)
        {
            closePropmtObject[i].onClick.AddListener(() => { OnNotReward(currentIndex -1); });
        }
        
        closePropmtObject[6].onClick.AddListener(() => { propmtObject[6].SetActive(false); });

        for (int i = 0; i < videoRewardButton.Length; i++)
        {
            videoRewardButton[i].onClick.AddListener(()=> { 
                EventDispatcher.Instance.AddEventListener(EventKey.AdShowSuccessCallBack, VideoRewardCoinDouble);
                PluginMercury.Instance.ActiveRewardVideo();
                propmtObject[currentIndex - 1].SetActive(false);
            });
        }
    }

    protected override void OnShow(ActivateParams activateParams)
    {

    }

    protected override void OnClose()
    {

    }

    void Update()
    {
        if (!_isRotate) return; //不旋转结束
        if (Time.time < _time) // 没结束
        {
            turntableObject.transform.Rotate(Vector3.forward * Speed * Time.deltaTime * -1);
        }
        else
        {
            //结束，使用DoTween旋转到结束角度，耗时1秒
            //这里有个360，使用来防止指针回转的，如果不加这个360，你会看到指针倒退
            turntableObject.transform.DORotate(new Vector3(0, 0, -360 + Angle), 5f, RotateMode.FastBeyond360);
            _isRotate = false; // 设置不旋转
            Invoke("OnReward",5);
        }
    }

    void OnNotReward(int index)
    {
        propmtObject[index].SetActive(false);

        switch (index)
        {
            case 0: ProfileManager.Instance.Coin += 100; break;
            case 1: ProfileManager.Instance.Coin += 200;break;
            case 2: ProfileManager.Instance.Coin += 400;break;
            case 3: ProfileManager.Instance.Coin += 600; break;
        }
    }

    void OnReward()
    {
        switch (currentIndex)
        {
            case 1: Debug.Log("金币100"); currentRewardCoin = 100; break;
            case 2: Debug.Log("金币200"); currentRewardCoin = 200; break;
            case 3: Debug.Log("金币400"); currentRewardCoin = 400; break;
            case 4: Debug.Log("金币600"); currentRewardCoin = 600; break;
            case 5: characterselect.UnlockTrail(); break;
            case 6: characterselect.UnlockRole(); break;
        }
        propmtObject[currentIndex - 1].SetActive(true);
        times = Mathf.Clamp(times + 1, 1, 3);
        PlayerPrefs.SetString("TurntableTimes", times.ToString());
        clickTurntableButton.interactable = true;
        EventDispatcher.Instance.RemoveEventListener(EventKey.AdShowSuccessCallBack, Init);
    }

    //外部调用，初始化时间和打开旋转
    private void StartTurntable()
    {
        bool isCanWatch = GetTimes();

        if (isCanWatch)
        {
            clickTurntableButton.interactable = false;
            EventDispatcher.Instance.AddEventListener(EventKey.AdShowSuccessCallBack, Init);
            PluginMercury.Instance.ActiveRewardVideo();
        }
        else
        {
            propmtObject[6].SetActive(true);
        }
    }

    void Init(string msg)
    {
        _time = Time.time + ContinuousTime;
        _isRotate = true;
        Angle = GetAngle();
    }

    bool GetTimes()
    {
        string turntableDate = PlayerPrefs.GetString("TurntableDate","");

        System.DateTime startTime = System.TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
        long curretnSeconds = (long)(DateTime.Now - startTime).TotalSeconds / (3600 * 24);

        if (turntableDate.Equals("") || !turntableDate.Contains(curretnSeconds.ToString()))
        {
            PlayerPrefs.SetString("TurntableDate", curretnSeconds.ToString());
            PlayerPrefs.SetString("TurntableTimes", "0");
            times = 0;
            return true;
        }

        times = int.Parse(PlayerPrefs.GetString("TurntableTimes"));

        if (times < 3){ return true; }

        return false;
    }

    private int GetAngle()
    {
        int role, trail, angle = 0;
        int number = RandNumber();

        characterselect.IsUnlockComplete(out role, out trail);

        if (number <= 10)
        {
            angle = UnityEngine.Random.Range(-20,20);
            currentIndex = 1;
        }
        else if (number <= 14)
        {
            angle = UnityEngine.Random.Range(40, 70);
            currentIndex = 2;
        }
        else if (number <= 16)
        {
            angle = UnityEngine.Random.Range(100, 130);
            currentIndex = 3;
        }
        else if (number <= 18)
        {
            angle = UnityEngine.Random.Range(160, 190);
            currentIndex = 4;
        }
        else if (number <= 19)
        {
            if (role == -1)
            {
                angle = UnityEngine.Random.Range(-20, 20);
                currentIndex = 1;
            }
            else
            {
                angle = UnityEngine.Random.Range(225, 250);
                currentIndex = 5;
            }
        }
        else if (number <= 20)
        {
            if (trail == -1)
            {
                angle = UnityEngine.Random.Range(-20, 20);
                currentIndex = 1;
            }
            else
            {
                angle = UnityEngine.Random.Range(285, 315);
                currentIndex = 6;
            }
        }

        return angle;
    }

    private int RandNumber()
    {
        return UnityEngine.Random.Range(1, 21);
    }

    private void VideoRewardCoinDouble(string msg)
    {
        int index = 0;

        switch (currentRewardCoin)
        {
            case 100: index = 0; ProfileManager.Instance.Coin += 200;  break;
            case 200: index = 1; ProfileManager.Instance.Coin += 400;  break;
            case 400: index = 2; ProfileManager.Instance.Coin += 800;  break;
            case 600: index = 3; ProfileManager.Instance.Coin += 1200; break;
        }

        doubleRewardObject[index].SetActive(true);
        doubleRewardButton[index].onClick.AddListener(()=> { doubleRewardObject[index].SetActive(false); });

        EventDispatcher.Instance.RemoveEventListener(EventKey.AdShowSuccessCallBack, VideoRewardCoinDouble);
    }
}
