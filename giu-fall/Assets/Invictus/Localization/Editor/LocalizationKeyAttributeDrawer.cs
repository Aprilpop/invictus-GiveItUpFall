using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;
using System.IO;


[CustomPropertyDrawer(typeof(LocalizationKeyAttribute))]
public class LocalizationKeyDrawer : PropertyDrawer
{
    static string[] keys = new string[0];

    [InitializeOnLoadMethod]
    static void OnProjectLoadedInEditor()
    {
        keys = LocalicaztionTool.Load(LocalicaztionTool.DEFAULT_LANG).Keys.ToArray();
        LocalicaztionTool.OnDictionaryChanged += OnLangFileChanged;
    }
  
    static void OnLangFileChanged()
    {
        keys = LocalicaztionTool.Load(LocalicaztionTool.DEFAULT_LANG).Keys.ToArray();
    }

    int selectedKey = 0;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        int selected = Array.IndexOf(keys, property.stringValue);
        int newSelected = EditorGUI.Popup(position, label.text, selected, keys);
        if (newSelected != selected)
            property.stringValue = keys[newSelected];
    }
}

