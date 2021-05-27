using MenuGUI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SignInPanel : Menu
{
    public override int UniqueID { get { return (int)MenuTypes.SignInPanel; } }

    public Button[] receiveButton;
    public GameObject[] receivePromptObject;
    public GameObject[] iconObject;
    public GameObject[] countDownObject;
    public GameObject[] loginDaysObject;
    public GameObject[] completeObject;
    public Text[] countDownText;

    public CharacterSelection characterselect;

    private string date;
    private int[] times;
    private int day;
    private DateTime[] cuttentDateTime;

    void Start()
    {
        receiveButton[0].onClick.AddListener(() => { Reward(times[0], 0); });
        receiveButton[1].onClick.AddListener(() => { Reward(times[1], 1); });
        receiveButton[2].onClick.AddListener(() => { Reward(times[2], 2); });
    }

    protected override void OnShow(ActivateParams activateParams)
    {
        
    }

    protected override void OnClose()
    {
        
    }

    private void OnEnable()
    {
        InitData();
        InvokeRepeating("InitPanel", 0, 1);
    }

    private void InitData()
    {
        day = 1;
        times = new int[3] { 0, 0, 0 };
        cuttentDateTime = new DateTime[3] { DateTime.Now, DateTime.Now, DateTime.Now };

        System.DateTime startTime = System.TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
        long curretnSeconds = (long)(DateTime.Now - startTime).TotalSeconds / (3600 * 24);
        date = PlayerPrefs.GetString("SignInDate", "");

        if (date.Equals(""))
        {
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

        day = int.Parse(PlayerPrefs.GetString("Day"));
        times[0] = int.Parse(PlayerPrefs.GetString("OneDayTimes"));
        times[1] = int.Parse(PlayerPrefs.GetString("TwoDayTimes"));
        times[2] = int.Parse(PlayerPrefs.GetString("ThreeDayTimes"));
        cuttentDateTime[0] = DateTime.Parse(PlayerPrefs.GetString("DateTimeOne"));
        cuttentDateTime[1] = DateTime.Parse(PlayerPrefs.GetString("DateTimeTwo"));
        cuttentDateTime[2] = DateTime.Parse(PlayerPrefs.GetString("DateTimeThree"));

        if (!date.Contains(curretnSeconds.ToString()) && day < 3)
        {
            day = Mathf.Clamp(day + 1, 1, 3);
            if (day == 2)
            {
                cuttentDateTime[1] = DateTime.Now;
                PlayerPrefs.SetString("DateTimeTwo", cuttentDateTime[1].ToString());
            }
            else if (day == 3)
            {
                cuttentDateTime[2] = DateTime.Now;
                PlayerPrefs.SetString("DateTimeThree", cuttentDateTime[2].ToString());
            }

            PlayerPrefs.SetString("SignInDate", curretnSeconds.ToString());
            PlayerPrefs.SetString("Day", day.ToString());
        }
    }

    void InitPanel()
    {
        if (day >= 1)
        {
            switch (times[0])
            {
                case 0:
                    receiveButton[0].interactable = true;
                    receivePromptObject[0].SetActive(true);
                    countDownText[0].text = string.Format("{0:D2}:{1:D2}:{2:D2}", 0, 0, 0);
                    break;
                case 1: TimesOne(0); CountDawn(GetSubSeconds(cuttentDateTime[0], DateTime.Now), 0); break;
                case 2: TimesTwo(0); CountDawn(GetSubSeconds(cuttentDateTime[0], DateTime.Now), 0); break;
                default:
                    TimesThree(0);
                    countDownObject[0].SetActive(false);
                    completeObject[0].SetActive(true);
                    break;
            }
        }

        if (day >= 2)
        {
            loginDaysObject[0].SetActive(false);

            switch (times[1])
            {
                case 0:
                    receiveButton[1].interactable = true;
                    receivePromptObject[1].SetActive(true);
                    countDownText[1].text = string.Format("{0:D2}:{1:D2}:{2:D2}", 0, 0, 0);
                    countDownObject[1].SetActive(true);
                    break;
                case 1: countDownObject[1].SetActive(true); TimesOne(3); CountDawn(GetSubSeconds(cuttentDateTime[1], DateTime.Now), 1); break;
                case 2: countDownObject[1].SetActive(true); TimesTwo(3); CountDawn(GetSubSeconds(cuttentDateTime[1], DateTime.Now), 1); break;
                default:
                    TimesThree(3);
                    countDownObject[1].SetActive(false);
                    completeObject[1].SetActive(true);
                    break;
            }
        }

        if (day >= 3)
        {
            loginDaysObject[1].SetActive(false);

             switch (times[2])
             {
                 case 0:
                     receiveButton[2].interactable = true;
                     receivePromptObject[2].SetActive(true);
                     break;
                 default:
                     completeObject[2].SetActive(true);
                     break;
             }
        }
    }

    void TimesOne(int index)
    {
        iconObject[index].SetActive(false);
        iconObject[index + 1].SetActive(true);
        iconObject[index + 2].SetActive(false);
    }

    void TimesTwo(int index)
    {
        iconObject[index].SetActive(false);
        iconObject[index + 1].SetActive(false);
        iconObject[index + 2].SetActive(true);
    }

    void TimesThree(int index)
    {
        iconObject[index].SetActive(false);
        iconObject[index + 1].SetActive(false);
        iconObject[index + 2].SetActive(true);
    }

    public int GetSubSeconds(DateTime startTimer, DateTime endTimer)
    {
        TimeSpan startSpan = new TimeSpan(startTimer.Ticks);
        TimeSpan nowSpan = new TimeSpan(endTimer.Ticks);
        TimeSpan subTimer = nowSpan.Subtract(startSpan).Duration();
        return (int)subTimer.TotalSeconds;
    }

    void CountDawn(int seconds, int index)
    {
        int currentSecond = GetSubSeconds(cuttentDateTime[index], cuttentDateTime[index].AddHours(2));
        //int currentSecond = GetSubSeconds(cuttentDateTime[index], cuttentDateTime[index].AddSeconds(5));
        if (seconds < currentSecond)
        {
            seconds = currentSecond - seconds;
            int hour = (int)seconds / 3600;
            int minute = (int)(seconds - hour * 3600) / 60;
            int second = (int)(seconds - hour * 3600 - minute * 60);

            countDownText[index].text = string.Format("{0:D2}:{1:D2}:{2:D2}", hour, minute, second);
        }
        else
        {
            receiveButton[index].interactable = true;
            receivePromptObject[index].SetActive(true);
            countDownText[index].text = string.Format("{0:D2}:{1:D2}:{2:D2}", 0, 0, 0);
        }
    }

    public void Reward(int times, int index)
    {
        switch (index)
        {
            case 0:
                this.times[index] = times + 1;
                PlayerPrefs.SetString("DateTimeOne", DateTime.Now.ToString());
                PlayerPrefs.SetString("OneDayTimes", this.times[index].ToString());
                switch (times)
                {
                    case 0: CoinsReward(100); break;
                    case 1: RandRole(); break;
                    case 2: CoinsReward(150); break;
                }
                break;

            case 1:
                this.times[index] = times + 1;
                PlayerPrefs.SetString("DateTimeTwo", DateTime.Now.ToString());
                PlayerPrefs.SetString("TwoDayTimes", this.times[index].ToString());
                switch (times)
                {
                    case 0: RandRole();  break;
                    case 1: CoinsReward(120); break;
                    case 2: RandEffect();  break;
                }
                break;

            case 2:
                this.times[index] = times + 1;
                PlayerPrefs.SetString("DateTimeThree", DateTime.Now.ToString());
                PlayerPrefs.SetString("ThreeDayTimes", this.times[index].ToString());
                RandRole();
                break;
        }

        cuttentDateTime[index] = DateTime.Now;
        receiveButton[index].interactable = false;
        receivePromptObject[index].SetActive(false);
    }

    void CoinsReward(int coinsReward)
    {
        ProfileManager.Instance.Coin += coinsReward;
    }

    void RandRole()
    {
        characterselect.UnlockRole();
    }

    void RandEffect()
    {
        characterselect.UnlockTrail();
    }
}
