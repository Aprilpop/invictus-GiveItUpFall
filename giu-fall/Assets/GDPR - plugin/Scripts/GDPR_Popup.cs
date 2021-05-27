using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GDPR_Popup : MonoBehaviour {

    public Text titleText;
    public Text descriptionText;
    public Text positiveButtonText;
    public Text negativeButtonText;

    System.Action<bool> PopupResults = null;

    public void ShowPopup(System.Action<bool> result = null)
    {
        PopupResults = result;
        gameObject.SetActive(true);
    }

    public void SetTexts(string title, string description, string positiveText = "OK", string negativeText = "Decline")
    {
        titleText.text = title;
        descriptionText.text = description;
        positiveButtonText.text = positiveText;
        negativeButtonText.text = negativeText;
    }

    public void PositiveEvent()
    {
        PopupResults(true);
        gameObject.SetActive(false);

    }

    public void NegativeEvent()
    {
        PopupResults(false);
        gameObject.SetActive(false);
    }

    public void OpenPrivacyPolicy()
    {
        GDPR.Instance.OpenPrivacyPolicyLink();
    }

}
