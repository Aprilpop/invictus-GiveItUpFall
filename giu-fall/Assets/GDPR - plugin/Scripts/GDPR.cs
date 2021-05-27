using UnityEngine.Networking;
using System.Collections;
using UnityEngine;

public enum GDPR_Aswer
{
    None,
    Enable,
    Disable
}

public enum Privacy_Modified
{
    Unknown,
    Modified,
    NotModified
}

public class GDPR : MonoBehaviour
{
    static GDPR instance;
    public static GDPR Instance { get { return instance; } }

    public string url = "https://adservice.google.com/getconfig/pubvendors";
    public string privacy = "http://privacy.invictus.com/";
    public string youlu = "https://mobile.51wnl.com/commercial/gameprivacy.html";

    [SerializeField]
    GDPR_Popup popup;

    bool GDPR_Checked = false;
    Privacy_Modified privacyModified = Privacy_Modified.Unknown;

    string modifiedTime = "NOT_MODIFIED";

    public Privacy_Modified PrivacyModified { get { return privacyModified; } }

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    System.Action<bool> CheckGDPR_Result;
    public void CheckGDPR(System.Action<bool> result)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            result(false);
            return;
        }

        CheckGDPR_Result = result;
        StartCoroutine(InEUCheck());
    }

    System.Action<Privacy_Modified> PrivacyModeifedResutlst;
    public void CheckModified(System.Action<Privacy_Modified> result)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            result(Privacy_Modified.Unknown);
            return;
        }

        PrivacyModeifedResutlst = result;
        StartCoroutine(PrivacyModifiedCheck());
    }

    IEnumerator InEUCheck()
    {
        using (WWW www = new WWW(url))
        {
            yield return www;

            if (www.text.Equals("{\"is_request_in_eea_or_unknown\":true}"))
            {
                PlayerPrefs.SetInt("GDPR", 1);
                CheckGDPR_Result(true);
            }
            else
                CheckGDPR_Result(false);
        }
    }

    IEnumerator PrivacyModifiedCheck()
    {
        UnityWebRequest www = UnityWebRequest.Get(privacy);

        string LastModifiedDate = PlayerPrefs.GetString("lastmodif", "NO_SAVED_DATA");

        if (!LastModifiedDate.Equals("NO_SAVED_DATA"))
            www.SetRequestHeader("If-Modified-Since", LastModifiedDate);

        yield return www.SendWebRequest();

        if (!LastModifiedDate.Equals("NO_SAVED_DATA"))
        {

            if (www.responseCode == 200)
            {
                PrivacyModeifedResutlst(Privacy_Modified.Modified);
                modifiedTime = www.GetResponseHeader("Last-Modified");
            }
            else if (www.responseCode == 304)
                PrivacyModeifedResutlst(Privacy_Modified.NotModified);
            else
                PrivacyModeifedResutlst(Privacy_Modified.Unknown);

        }
        else if (www.GetResponseHeader("Last-Modified") != null)
        {
            modifiedTime = www.GetResponseHeader("Last-Modified");
        }
    }

    public void ShowPopup(string popupType, string title, string description, string positiveButtonText = "同意并进入下一步")
    {
        popup.SetTexts(title, description, positiveButtonText);
        popup.ShowPopup(succes =>
        {
            PlayerPrefs.SetInt("GDPR_" + popupType + "_enable", (succes == false) ? 0 : 1);

            if (!modifiedTime.Equals("NOT_MODIFIED"))
                PlayerPrefs.SetString("lastmodif", modifiedTime);

            PlayerPrefs.Save();

        });
    }

    public GDPR_Aswer GetRequestTypeAnswer(string popupType)
    {
        int savedAswer = PlayerPrefs.GetInt("GDPR_" + popupType + "_enable", -1);

        switch (savedAswer)
        {
            //Még nem válaszolt
            case -1:
                return GDPR_Aswer.None;
            //Elutasította
            case 0:
                return GDPR_Aswer.Disable;
            //Elfogadta
            case 1:
                return GDPR_Aswer.Enable;
        }

        return GDPR_Aswer.None;
    }

    public void OpenPrivacyPolicyLink()
    {
        Application.OpenURL(youlu);
    }
}
