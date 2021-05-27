using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(ResourcePathAttribute))]
public class ResourcePathDrawer : PropertyDrawer
{
    static int GetMainAssetInstanceID(string path)
    {
        System.Reflection.MethodInfo[] methods = typeof(AssetDatabase).GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
        System.Reflection.MethodInfo method = typeof(AssetDatabase).GetMethod("GetMainAssetInstanceID", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
        int result = (int)method.Invoke(null, new object[] { path });
        return result;
    }

    bool mousedown = false;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        float searchButtonWidth = 18f;
        Rect rect = new Rect(position.x, position.y, position.width - searchButtonWidth, position.height);
        Rect selectButtonRect = new Rect(position.x + position.width - searchButtonWidth, position.y, searchButtonWidth, position.height);

        Type resourceType = ((ResourcePathAttribute)attribute).ResourceType;
        if (resourceType == typeof(Sprite))
        {
            EditorGUI.LabelField(position, "ResourcePath Attribute not work with embedded types!");
            return;
        }
        EditorGUI.BeginProperty(position, label, property);
        string fullpath = ResourcesFiles.GetFullAssetPath(property.stringValue, resourceType);
        string path = (property.stringValue.Length > 0) ? fullpath != null ? property.stringValue : "<Missing>" : "None";
        int index = path.LastIndexOfAny(new char[]{ '/','\\'});
        if (index > 0)
            path = path.Substring(index+1);

        GUIStyle style = new GUIStyle(EditorStyles.textField);
        GUIContent resource = new GUIContent(path + " (" + resourceType.Name + ")" );

        if (property.stringValue.Length > 0)
        {
            Texture texture =  AssetPreview.GetMiniTypeThumbnail(resourceType);
            if (texture == null && resourceType.IsSubclassOf(typeof(MonoBehaviour)))
                texture = (Texture)EditorGUIUtility.Load("cs Script Icon");
            resource.image = texture;
            style.imagePosition = ImagePosition.ImageLeft;
        }
        EditorGUI.LabelField(rect, new GUIContent(ObjectNames.NicifyVariableName(property.name)), resource ,style);
        //EditorGUIUtility.AddCursorRect(rect, MouseCursor.Link);
        if (rect.Contains(Event.current.mousePosition))
        {
            if (Event.current.type == EventType.DragUpdated)
            {
                if (DragAndDrop.paths.Length == 1 && ResourcesFiles.IsResourceAsset(DragAndDrop.paths[0],resourceType))
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                else
                    DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                Event.current.Use();
            }
            else if (Event.current.type == EventType.DragPerform)
            {
                string value = DragAndDrop.paths[0].Substring(DragAndDrop.paths[0].IndexOf("/Resources/") + 11);
                value = value.Substring(0, value.IndexOf("."));
                property.stringValue = value;
                property.serializedObject.ApplyModifiedProperties();
                Event.current.Use();
            }
            else if (Event.current.button == 0 && Event.current.type == EventType.MouseDown)
            {
                mousedown = true;
                Event.current.Use();
            }
            else if (Event.current.button == 0 && Event.current.type == EventType.MouseUp) {
                if (mousedown)
                {
                    mousedown = false;
                    EditorGUIUtility.PingObject(GetMainAssetInstanceID(fullpath));
                    Event.current.Use();
                }
            }
        }

        if (GUI.Button(selectButtonRect,"", GUI.skin.GetStyle("IN ObjectField")))
            ResourceFilesWindow.Create(property, resourceType);

        EditorGUI.EndProperty();
    }
}

[InitializeOnLoad]
public class ResourcesFiles : AssetPostprocessor
{
    static List<string> resourcesFiles = new List<string>();
    static ResourcesFiles()
    {
        string[] paths = AssetDatabase.GetAllAssetPaths();
        foreach(string path in paths)
        {

            if (path.Contains("/Resources/") && File.GetAttributes(path) != FileAttributes.Directory)
                resourcesFiles.Add(path);
        }
        resourcesFiles.Sort();
    }

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (string str in importedAssets)
        {
            if (str.Contains("/Resources/") && !resourcesFiles.Contains(str))
                resourcesFiles.Add(str);
        }

        foreach (string str in deletedAssets)
        {
            if (str.Contains("/Resources/"))
                resourcesFiles.Remove(str);
        }
        for (int i = 0; i < movedAssets.Length; i++)
        {
            int index = resourcesFiles.IndexOf(movedFromAssetPaths[i]);
            if (index > 0)
                resourcesFiles.RemoveAt(index);
            if (movedAssets[i].Contains("/Resources/"))
                resourcesFiles.Add(movedAssets[i]);
        }
       resourcesFiles.Sort();
    }

    static bool IsSameType(string path, Type type)
    {
        if (type == null)
            throw new NullReferenceException("Type cannot be null");
        UnityEngine.Object obj = AssetDatabase.LoadMainAssetAtPath(path);
        if (obj)
        {
            Type objType = obj.GetType();
            bool result = objType == type;
            if (!result && type.IsSubclassOf(typeof(Component)) && objType == typeof(GameObject))
                result = (obj as GameObject).GetComponent(type) != null;
            return result;
        }
        return false;
    }


    public static string GetFullAssetPath(string path,Type type) {

        if (path != null && path.Length > 0)
        {
            foreach (string file in resourcesFiles)
            {
                if (file.Contains(path) && IsSameType(file, type))
                    return file;
            }
        }
        return null;
    }

    public static bool IsResourceAsset(string path,Type type) {
        if (resourcesFiles.Contains(path))
            return IsSameType(path, type);
        return false;
    }

    public static List<string> GetResourcesAssets(Type type)
    {
        List<string> resources = new List<string>();
        foreach(string path in resourcesFiles)
        {
            if (IsSameType(path,type))
                resources.Add(path);
        }
        return resources;
    }
}

public class ResourceFilesWindow : EditorWindow
{
    Type                resourceType;
    Texture             resourceIcon;
    List<string>        resourceFiles;
    SerializedProperty  target;


    GUIContent          filterContent;
    string              filter_text = "";
    Vector2             scrollPos;


    public static void Create(SerializedProperty prop,Type type)
    {
        ((ResourceFilesWindow)GetWindow(typeof(ResourceFilesWindow), true, "Select Resource File")).Show(prop, type);
    }


    public void Show(SerializedProperty prop, Type type)
    {
        target = prop;
        resourceType = type;
        resourceFiles = ResourcesFiles.GetResourcesAssets(type);
        resourceIcon = AssetPreview.GetMiniTypeThumbnail(type);
        if (resourceIcon == null && type.IsSubclassOf(typeof(MonoBehaviour)))
            resourceIcon = (Texture)EditorGUIUtility.Load("cs Script Icon");
        filterContent = new GUIContent("",(Texture)EditorGUIUtility.Load("Search Icon"));
        Show();
    }



    private void OnLostFocus()
    {
        Close();
    }



    void OnGUI()
    {
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();

        GUILayout.Label(filterContent, GUILayout.MaxWidth(20),GUILayout.MaxHeight(20));
        filter_text = EditorGUILayout.TextField(filter_text);
        GUILayout.EndHorizontal();
        GUILayout.Space(10);

        scrollPos = GUILayout.BeginScrollView(scrollPos, false, false);

        bool usefilter = (filter_text.Length > 0) ? true : false;

        GUIItem("None", "", false);
        foreach (var item in resourceFiles)
        {
            string value = item.Substring(item.IndexOf("/Resources/") + 11);
            value = value.Substring(0, value.IndexOf("."));
            if (usefilter)
            {
                if (item.IndexOf(filter_text, StringComparison.OrdinalIgnoreCase) != -1)
                    GUIItem(value,value);
            }
            else
                GUIItem(value,value);
        }
        GUILayout.EndVertical();
        GUILayout.EndScrollView();
    }

    void GUIItem(string item, string value, bool showIcon = true)
    {
        if (GUILayout.Button(new GUIContent(item, showIcon ?resourceIcon: null), GUILayout.MaxHeight(20)) && target != null)
        {
            target.stringValue = value;
            target.serializedObject.ApplyModifiedProperties();
        }
    }
    
}


/*
namespace UIElementsExamples
{
    public class ListViewExampleWindow : EditorWindow
    {
        [MenuItem("Window/ListViewExampleWindow")]
        public static void OpenDemoManual()
        {
            GetWindow<ListViewExampleWindow>().Show();
        }

        public void OnEnable()
        {
            // Create some list of data, here simply numbers in interval [1, 1000]
            const int itemCount = 1000;
            var items = new List<string>(itemCount);
            for (int i = 1; i <= itemCount; i++)
                items.Add(i.ToString());

            // The "makeItem" function will be called as needed
            // when the ListView needs more items to render
            Func<VisualElement> makeItem = () => new Label();

            // As the user scrolls through the list, the ListView object
            // will recycle elements created by the "makeItem"
            // and invoke the "bindItem" callback to associate
            // the element with the matching data item (specified as an index in the list)
            Action<VisualElement, int> bindItem = (e, i) => (e as Label).text = items[i];

            // Provide the list view with an explict height for every row
            // so it can calculate how many items to actually display
            const int itemHeight = 16;

            var listView = new ListView(items, itemHeight, makeItem, bindItem);

            listView.selectionType = SelectionType.Multiple;

            listView.onItemChosen += obj => Debug.Log(obj);
            listView.onSelectionChanged += objects => Debug.Log(objects);

            listView.style.flexGrow = 1.0f;
            var tf = new TextField("Test");

            tf.RegisterValueChangedCallback(value =>
            {
                int i = 0;


            });
            rootVisualElement.Add(tf);
            rootVisualElement.Add(listView);
        }
    }
}
*/