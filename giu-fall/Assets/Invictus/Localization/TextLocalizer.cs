using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TextLocalizer : MonoBehaviour
{
    [SerializeField]
    int caseType = 0;
    public int CaseType
    {
        get { return caseType; }
    }

    [SerializeField,ReadOnly]
    string key;
    public string Key
    {
        get { return key; }
    }

    void OnEnable()
    {
        if (key != null)
        {
            switch(caseType)
            {
                case 2:
                    GetComponent<Text>().text = LocalizationManager.Instance.GetString(key).ToUpper();
                    break;
                case 1:
                    GetComponent<Text>().text = LocalizationManager.Instance.GetString(key).ToLower();
                    break;
                default:
                    GetComponent<Text>().text = LocalizationManager.Instance.GetString(key);
                    break;
            }
        }
    }

}