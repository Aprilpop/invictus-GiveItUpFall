using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.IO;
using System.Xml.Serialization;
using System.Linq;

public class LangItem
{
    [XmlAttribute]
    public string id;
    [XmlAttribute]
    public string value;
}

public delegate void OnDictionaryChanged();

[InitializeOnLoad]
public class LocalicaztionTool : EditorWindow
{
    public const string LOCALIZATION_DIR = "Assets/Localization";
    public const string DEFAULT_LANG = "Assets/Localization/english.txt";

    const string LOCALIZATION_KEY = "invictus_loc_asset_path";
    const string LOCALIZATION_FILTER_TEXT = "invictus_filter";
    const string LOCALIZATION_FILTER_KEY = "invictus_filter_key";
    const string LOCALIZATION_FILTER_VALUE = "invictus_filter_value";

    // Add menu named "My Window" to the Window menu
    [MenuItem("Invictus/Localization Tool")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        LocalicaztionTool window = (LocalicaztionTool)EditorWindow.GetWindow(typeof(LocalicaztionTool), true, "LocalicaztionTool");
        window.Initalize();
    }

    static LocalicaztionTool()
    {
        if (!Directory.Exists(LOCALIZATION_DIR))
            Directory.CreateDirectory(LOCALIZATION_DIR);
    }

    public static OnDictionaryChanged OnDictionaryChanged;
    public static Dictionary<string, string> Load(string path)
    {
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            if (json != null)
                return ((Dictionary<string, object>)MiniJSON.Json.Deserialize(json)).ToDictionary(k => k.Key, k => k.Value.ToString());
        }
        return new Dictionary<string, string>();
    }
    public static void Save(string lang, Dictionary<string, string> dictionary)
    {

        using (FileStream fs = new FileStream(lang, FileMode.Create))
        {
            using (StreamWriter writer = new StreamWriter(fs))
                writer.Write(MiniJSON.Json.Serialize(dictionary));
            fs.Close();
        }
        AssetDatabase.Refresh();
        if (lang == DEFAULT_LANG && OnDictionaryChanged != null)
            OnDictionaryChanged();
    }


    bool initialised = false;
    string localizationAsset ="";
    string addKey ="";
    string addValue ="";
    Vector2 scrollPos;
    string PROJECT_PATH;

    public Dictionary<string, string> dictionary = new Dictionary<string, string>();
 
    void ExportCSV(string csvFile)
    {
        StreamWriter file = new StreamWriter(csvFile,false,System.Text.Encoding.UTF8);

        foreach (var item in dictionary)
        {
            // eltávolítjuk a newline érékeket
            string value = item.Value.Replace("\r", "");
            //lecseréljük az idézőjelt és az entert
            value = value.Replace("\n", "{n}");
            value = value.Replace("\"", "{'}");
            file.WriteLine(String.Format("\"{0}\",\"{1}\"", item.Key, value));
        }
        file.Close();

    }
    void ImportCSV(string csvFile)
    {
        dictionary.Clear();
        StreamReader file = new StreamReader(csvFile, System.Text.Encoding.UTF8);

        while (file.Peek() >= 0){
            string line = file.ReadLine();


            string key = "";
            string value = "";
            if (line[0] == '"')
            {
                int index = line.IndexOf("\",");
                key = line.Substring(1, index - 1);
                value = line.Substring(index+2);
            }
            else
            {
                int index = line.IndexOf(',');
                key = line.Substring(0, index);
                value = line.Substring(index + 1);
            }
            if (value.Length > 0)
            {
                if (value[0] == '"')
                {
                    int index = value.LastIndexOf("\"");
                    value = value.Substring(1, index - 1);
                }
            }

            // Az idézőjel és az enterek visszaállítása
            key = key.Replace("{n}", "\n");
            key = key.Replace("{'}", "\"" );
            value = value.Replace("{n}", "\n");
            value = value.Replace("{'}", "\"");
            dictionary.Add(key, value);
        }
        file.Close();
        localizationAsset = null;
    }


    void Initalize()
    {
        if (!initialised)
        {
            localizationAsset = DEFAULT_LANG;
            dictionary = Load(DEFAULT_LANG);
            initialised = true;
        }
        ShowUtility();
    }

    private void OnEnable()
    {
        PROJECT_PATH = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/Assets"));
        OnDictionaryChanged += OnLangFileChanged;
    }

    private void OnDisable()
    {
        OnDictionaryChanged -= OnLangFileChanged;
    }

    void OnLangFileChanged()
    {
        if (localizationAsset == DEFAULT_LANG)
           dictionary = Load(DEFAULT_LANG);
    }


    void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.Label("File:",GUILayout.MaxWidth(30));
        GUILayout.TextField(localizationAsset);
        if (GUILayout.Button("Open",GUILayout.MaxWidth(50)))
        {
            string path = EditorUtility.OpenFilePanel("Open Language text", LOCALIZATION_DIR, "txt");
            if (path.Length > 0)
            {
                localizationAsset = path.Substring(path.LastIndexOf("Assets/"));
                dictionary = Load(localizationAsset);
            }
        }
        if (GUILayout.Button("Save", GUILayout.MaxWidth(50)))
        {
            if (localizationAsset != null)
                Save(localizationAsset,dictionary);
            else
            {
                string path = EditorUtility.SaveFilePanel("Save Language text", LOCALIZATION_DIR, "", "txt");
                if (path.Length > 0)
                {
                    localizationAsset = path.Substring(path.LastIndexOf("Assets/"));
                    Save(localizationAsset, dictionary);
                }
            }

        }

        if (GUILayout.Button("Save As", GUILayout.MaxWidth(60)))
        {
            string path =EditorUtility.SaveFilePanel("Save Language text", LOCALIZATION_DIR, "", "txt");
            if (path.Length > 0)
            {
                localizationAsset = path.Substring(path.LastIndexOf("Assets/"));
                Save(localizationAsset,dictionary);
            }
        }




        GUILayout.EndHorizontal();

        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Import from CSV"))
        {
            string csvFile = EditorUtility.OpenFilePanel("Import from CSV", PROJECT_PATH, "csv");
            if (csvFile.Length > 0)
                ImportCSV(csvFile);
        }

        if (GUILayout.Button("Export to CSV"))
        {
            string csvFile = EditorUtility.SaveFilePanel("Export to CSV", PROJECT_PATH,"", "csv");
            if (csvFile.Length > 0)
                ExportCSV(csvFile);
        }



        GUILayout.EndHorizontal();
        GUILayout.EndVertical();


        GUILayout.BeginVertical();
        GUILayout.Space(20);

        GUIAddItem();
        GUILayout.Space(10);

        GUILayout.BeginHorizontal();

        string filter_text = EditorPrefs.GetString(LOCALIZATION_FILTER_TEXT,"");
        bool filter_value = EditorPrefs.GetBool(LOCALIZATION_FILTER_VALUE);

        GUILayout.Label("Filter:", GUILayout.MaxWidth(60));
        filter_text = EditorGUILayout.TextField(filter_text);
        filter_value = GUILayout.Toggle(filter_value, "Value", GUILayout.MaxWidth(60));

        EditorPrefs.SetString(LOCALIZATION_FILTER_TEXT, filter_text);
        EditorPrefs.SetBool(LOCALIZATION_FILTER_VALUE,filter_value);

        GUILayout.EndHorizontal();
        GUILayout.Space(10);

        scrollPos = GUILayout.BeginScrollView(scrollPos,false,false);

        List<LangItem> changedItems = new List<LangItem>();
        List<string> deletedItems = new List<string>();

        bool usefilter = (filter_text.Length > 0) ? true : false;


        foreach (var item in dictionary)
        {
            if (usefilter)
            {

                if (item.Key.IndexOf(filter_text,StringComparison.OrdinalIgnoreCase) != -1 || (filter_value == true && item.Value.IndexOf(filter_text, StringComparison.OrdinalIgnoreCase) != -1))
                {
                    GUIItem(item, changedItems,deletedItems);
                }
            }
            else
            {
                GUIItem(item, changedItems,deletedItems);
            }

        }
        for (int i = 0; i < changedItems.Count; ++i)
            dictionary[changedItems[i].id] = changedItems[i].value;

        for (int i = 0; i < deletedItems.Count; ++i)
            dictionary.Remove(deletedItems[i]);

        GUILayout.EndVertical();
        GUILayout.EndScrollView();
    }

    void GUIItem(KeyValuePair<string,string> item, List<LangItem> changedItem, List<string> deletedItems)
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Remove", GUILayout.MaxWidth(60), GUILayout.ExpandHeight(true),GUILayout.MaxHeight(40)))
            deletedItems.Add(item.Key);
        GUILayout.BeginVertical();
        GUILayout.Label(item.Key + ":");
        string newValue = EditorGUILayout.TextArea(item.Value,GUILayout.MaxWidth(position.width-85));
        if (newValue != item.Value)
            changedItem.Add(new LangItem() { id = item.Key, value = newValue });
        GUILayout.Space(10);
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

    }

    void GUIAddItem()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add", GUILayout.MaxWidth(60), GUILayout.ExpandHeight(true),GUILayout.MaxHeight(40)))
        {
            if (addKey.Length > 0 && !dictionary.ContainsKey(addKey))
                dictionary[addKey] = addValue;
            else
                EditorUtility.DisplayDialog("Warning", "the key already exists or empty", "OK");
        }
        GUILayout.BeginVertical();
        addKey = EditorGUILayout.TextField(addKey);
        addValue = EditorGUILayout.TextArea(addValue);
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }

}