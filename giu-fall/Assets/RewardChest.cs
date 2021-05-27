using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardChest : MonoBehaviour
{

    [SerializeField] Button doubleRewardButton;
    [SerializeField] Button claimRewardButton;

    [SerializeField] Text rewardAmountText;

    int rewardAmount;

    private void Awake()
    {
        doubleRewardButton.onClick.AddListener(() => {
            EventDispatcher.Instance.AddEventListener(EventKey.AdShowSuccessCallBack, OnVideoPlayOver);
            PluginMercury.Instance.ActiveRewardVideo();
            //DoubleReward();
        });
        claimRewardButton.onClick.AddListener(() => ClaimReward());
    }

    private void ClaimReward()
    {
        doubleRewardButton.interactable = false;
        claimRewardButton.interactable = false;
        ProfileManager.Instance.Coin += rewardAmount;
        ProfileManager.Instance.Save();
        StartCoroutine(Continue());
    }

    private void DoubleReward()
    {
        //AdManagerIronsrc.Instance.ShowVideoAd(results =>
        //{
        //    if (results)
        //    {
        doubleRewardButton.interactable = false;
        claimRewardButton.interactable = false;
        ProfileManager.Instance.Coin += (rewardAmount * 2);
        rewardAmountText.text = "+" + (rewardAmount * 2);
        ProfileManager.Instance.Save();
        StartCoroutine(Continue());
        ProfileManager.Instance.rewardDoubleWatched = true;
        //    }
        //});
        PluginMercury.Instance.ActiveRewardVideo();
    }

    IEnumerator Continue()
    {
        yield return new WaitForSeconds(1f);
        GameLogic.Instance.OnWin();
        ProfileManager.Instance.rewardProgress = 0;
        ProfileManager.Instance.Save();
    }

    private void OnEnable()
    {
        doubleRewardButton.interactable = true;
        claimRewardButton.interactable = true;
        rewardAmount = (ProfileManager.Instance.levelnumber * 10) / 2;
        rewardAmountText.text = "+" + rewardAmount;

        if (Application.internetReachability == NetworkReachability.NotReachable)
            doubleRewardButton.gameObject.SetActive(false);
        else
            doubleRewardButton.gameObject.SetActive(true);
    }

    private void OnDisable()
    {

    }

    void Start()
    {

    }

    void OnVideoPlayOver(string msg)
    {
        EventDispatcher.Instance.RemoveEventListener(EventKey.AdShowSuccessCallBack, OnVideoPlayOver);
        DoubleReward();
    }
}
