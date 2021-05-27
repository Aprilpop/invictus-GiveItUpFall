using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Language
{
    [SerializeField]
    SystemLanguage id;
    [SerializeField]
    string name;
    [SerializeField]
    TextAsset descriptiveAsset;

    public SystemLanguage Id { get { return id; } }
    public string Name { get { return name; } }
    public TextAsset DescriptiveAsset { get { return descriptiveAsset; } }

}

public class LocalizationManager : MonoBehaviour {

    static LocalizationManager instance;

    public static LocalizationManager Instance
    {
        get
        {
            if (instance == null)
                return ((GameObject)Instantiate(Resources.Load("LocalizationManager"))).GetComponent<LocalizationManager>();
            return instance;

        }

    }

    [SerializeField]
    SystemLanguage defaultLanguage;

    [SerializeField]
    Language[] languages;

    Language currentLanguage;

    Dictionary<string, string> dictionary = new Dictionary<string, string>();


    public Language CurrentLanguage{ get { return currentLanguage; } }
    public Language[] Languages { get { return languages; } }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SetLanguage(defaultLanguage);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public int GetLanguageIndex(SystemLanguage id)
    {
        if (IsLanguageExist(id))
        {
            int i = 0;
            foreach (Language lang in languages)
            {
                if (lang.Id == id)
                    return i;
                i++;
            }
            return 0;
        }
        else
            return 0;
    }

    public bool IsLanguageExist(SystemLanguage id)
    {
        foreach (Language lang in languages)
            if (lang.Id == id)
                return true;
        return false;
    }

    public bool SetLanguage(SystemLanguage id)
    {
        if (currentLanguage == null || currentLanguage.Id != id)
        {
            Language current = null;
            foreach (Language lang in languages)
                if (lang.Id == id)
                {
                    current = lang;
                    break;
                }
            if (current != null)
            {
                currentLanguage = current;
                if (current.DescriptiveAsset)
                    dictionary = ((Dictionary<string, object>)MiniJSON.Json.Deserialize(current.DescriptiveAsset.text)).ToDictionary(k => k.Key, k => k.Value.ToString());
                else
                    dictionary.Clear();
                return true;
            }
            return false;
        }
        return true;
    }

    public string GetString(string key)
    {
        string value;
        if (dictionary.TryGetValue(key, out value))
            return value;
        return string.Format("Key:{0} not found in {1}!",key,currentLanguage.Id);
    }
}
