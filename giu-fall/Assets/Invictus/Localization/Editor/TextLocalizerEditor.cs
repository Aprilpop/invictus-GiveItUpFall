using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;
using System.IO;

[CustomEditor(typeof(TextLocalizer)),CanEditMultipleObjects]
public class TextLocalizerEditor : Editor
{

    static string[] caseTypeNames = { "Original", "Lover", "Upper" };


    [MenuItem("GameObject/UI/Localized Text", false,2001 )]
    static void CreateTextWithLocalizer()
    {
        GameObject go = new GameObject("Text", typeof(Text), typeof(TextLocalizer));
        if (Selection.activeTransform != null)
            go.transform.parent = Selection.activeTransform;
    }


    string newkey = null;
    int selectedKey = 0;
    Dictionary<string,string> dictionary = new Dictionary<string, string>();

    private void OnEnable()
    {
        dictionary = LocalicaztionTool.Load(LocalicaztionTool.DEFAULT_LANG);
        LocalicaztionTool.OnDictionaryChanged += OnLangFileChanged;


    }

    private void OnDisable()
    {
        LocalicaztionTool.OnDictionaryChanged -= OnLangFileChanged;
    }

    void OnLangFileChanged()
    {
        dictionary = LocalicaztionTool.Load(LocalicaztionTool.DEFAULT_LANG);
    }



    private bool CreateEntry(string key, string value)
    {
        if (dictionary.ContainsKey(key))
            return false;
        else
            dictionary.Add(key, value);
        return true;
    }

    Rect buttonRect;
    public override void OnInspectorGUI()
    {
        SerializedProperty caseTypeproperty = serializedObject.FindProperty("caseType");
        int selectedEnum = caseTypeproperty.intValue;

        string key = serializedObject.FindProperty("key").stringValue;

        for (int i = 0; i < targets.Length; ++i)
        {
            TextLocalizer textLocalizer = (TextLocalizer)targets[i];
            SerializedObject serializedObject = new SerializedObject(textLocalizer);
            SerializedProperty keyProperty = serializedObject.FindProperty("key");
            if (keyProperty.stringValue != null && !dictionary.ContainsKey(keyProperty.stringValue))
            {
                keyProperty.stringValue = null;
                serializedObject.ApplyModifiedProperties();
            }
            if (key != keyProperty.stringValue)
                key = "<different id>";

            if (selectedEnum != -1)
            {
                SerializedProperty caseProperty = serializedObject.FindProperty("caseType");
                if (caseProperty.intValue != selectedEnum)
                    selectedEnum = -1;
            }
        }


        EditorGUI.BeginChangeCheck();
        selectedEnum = EditorGUILayout.Popup(caseTypeproperty.displayName, selectedEnum, caseTypeNames);
        if (EditorGUI.EndChangeCheck())
            SelectCaseType(selectedEnum);

        EditorGUILayout.BeginHorizontal();
        GUI.enabled = false;
        EditorGUILayout.TextField("Key", key);
        GUI.enabled = true;
        if (GUILayout.Button("Select", GUILayout.Width(90)))
            KeySelectionWindow.Create(buttonRect, this);
        if (Event.current.type == EventType.Repaint)
            buttonRect = GUILayoutUtility.GetLastRect();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        newkey = EditorGUILayout.TextField("Create Key", newkey);
        if (GUILayout.Button("Create", GUILayout.Width(90)))
        {
            string defText = ((TextLocalizer)target).GetComponent<Text>().text;
            if (CreateEntry(newkey, defText))
            {
                LocalicaztionTool.Save(LocalicaztionTool.DEFAULT_LANG, dictionary);
                SelectKey(newkey);
            }
            else
            {
                EditorUtility.DisplayDialog("Localizer", string.Format("The {0} key already exists!", newkey), "OK");
            }
        }
        EditorGUILayout.EndHorizontal();

    }


    void SelectCaseType(int index)
    {
        foreach (TextLocalizer textLocalizer in targets)
        {
            SerializedObject serializedObject = new SerializedObject(textLocalizer);
            string key = serializedObject.FindProperty("key").stringValue;
            string value;
            if (dictionary.TryGetValue(key,out value))
            {
                switch (index)
                {
                    case 2:
                        textLocalizer.GetComponent<Text>().text = value.ToUpper();
                        break;
                    case 1:
                        textLocalizer.GetComponent<Text>().text = value.ToLower();
                        break;
                    default:
                        textLocalizer.GetComponent<Text>().text = value;
                        break;
                }
            }
            serializedObject.FindProperty("caseType").intValue = index;
            serializedObject.ApplyModifiedProperties();
        }
    }

    public void SelectKey(string key)
    {
        foreach (TextLocalizer textLocalizer in targets)
        {
            SerializedObject serializedObject = new SerializedObject(textLocalizer);
            SerializedProperty keyProperty = serializedObject.FindProperty("key");
            int index = serializedObject.FindProperty("caseType").intValue;

            switch (index)
            {
                case 2:
                    textLocalizer.GetComponent<Text>().text = dictionary[key].ToUpper();
                    break;
                case 1:
                    textLocalizer.GetComponent<Text>().text = dictionary[key].ToLower();
                    break;
                default:
                    textLocalizer.GetComponent<Text>().text = dictionary[key];
                    break;
            }
            keyProperty.stringValue = key;
            serializedObject.ApplyModifiedProperties();
        }
    }
}


public class KeySelectionWindow : EditorWindow
{
    Dictionary<string, string> dictionary = new Dictionary<string, string>();

    private void OnEnable()
    {
        dictionary = LocalicaztionTool.Load(LocalicaztionTool.DEFAULT_LANG);
        LocalicaztionTool.OnDictionaryChanged += OnLangFileChanged;
    }

    private void OnDisable()
    {
        LocalicaztionTool.OnDictionaryChanged -= OnLangFileChanged;
    }

    void OnLangFileChanged()
    {
        dictionary = LocalicaztionTool.Load(LocalicaztionTool.DEFAULT_LANG);
    }


    Vector2 scrollPos;
    string filter_text ="";
    TextLocalizerEditor target = null;

    public static void Create(Rect rc, TextLocalizerEditor textLocalizer)
    {
        //KeySelectionWindow window = EditorWindow.GetWindow<KeySelectionWindow>(false,"");
        KeySelectionWindow window = ScriptableObject.CreateInstance<KeySelectionWindow>();
        window.target = textLocalizer;
        rc = new Rect(GUIUtility.GUIToScreenPoint(new Vector2(rc.xMin, rc.yMin)), rc.size);
        window.ShowAsDropDown(rc,new Vector2(240,400));
    }

    void OnGUI()
    {
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();

        GUILayout.Label("Filter:", GUILayout.MaxWidth(60));
        filter_text = EditorGUILayout.TextField(filter_text);

        GUILayout.EndHorizontal();
        GUILayout.Space(10);

        scrollPos = GUILayout.BeginScrollView(scrollPos, false, false);

        List<LangItem> changedItems = new List<LangItem>();
        List<string> deletedItems = new List<string>();

        bool usefilter = (filter_text.Length > 0) ? true : false;

        TextAnchor old = GUI.skin.button.alignment;
        GUI.skin.button.alignment = TextAnchor.MiddleLeft;
        foreach (var item in dictionary)
        {
            if (usefilter)
            {
                if (item.Key.IndexOf(filter_text, StringComparison.OrdinalIgnoreCase) != -1)
                    GUIItem(item);
            }
            else
                GUIItem(item);

        }
        GUI.skin.button.alignment = old;

        GUILayout.EndVertical();
        GUILayout.EndScrollView();
    }

    void GUIItem(KeyValuePair<string, string> item)
    {
        if (GUILayout.Button(item.Key) && target != null)
        {
            target.SelectKey(item.Key);
            Close();
        }
    }
}
