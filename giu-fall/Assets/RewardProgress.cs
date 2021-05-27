using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RewardProgress : MonoBehaviour
{
    [SerializeField] Color color;

    [SerializeField]
    Image[] emptyStars;

    [SerializeField] GameObject openChest;

    [SerializeField] Image chest;


    private void OnEnable()
    {
        chest.enabled = true;
        if (ProfileManager.Instance.rewardProgress == 4)
        {
            chest.enabled = false;
            openChest.SetActive(true);
        }
        else
        {
            for (int i = 0; i < emptyStars.Length; i++)
            {
                emptyStars[i].enabled = true;
            }
           
            if (ProfileManager.Instance.rewardProgress != 0)
            {

                for (int i = 0; i < ProfileManager.Instance.rewardProgress; i++)
                {
                    emptyStars[i].enabled = false;
                }                
            }

            
            StartCoroutine(MakeProgress());
        }

    }

    IEnumerator MakeProgress()
    {
        yield return new WaitForSeconds(1f);
        emptyStars[ProfileManager.Instance.rewardProgress].enabled = false;
        ProfileManager.Instance.rewardProgress += 1;
        ProfileManager.Instance.Save();
        SoundManager.Play(SfxArrayEnum.ProgressValidate);
        AfterProgress();
    }

    private void AfterProgress()
    {
        if (ProfileManager.Instance.rewardProgress == 4)
        {
            chest.enabled = false;
            openChest.SetActive(true);
        }
        else
        {
            StartCoroutine(Continue());
        }
    }
    
    IEnumerator Continue()
    {
       
        yield return new WaitForSeconds(1f);
        GameLogic.Instance.OnWin();
        this.gameObject.SetActive(false);
    }

}
