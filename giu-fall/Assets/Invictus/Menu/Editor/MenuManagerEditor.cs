using System;
using System.Reflection;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEditor;
using MenuGUI;

public class CreateMenuWindow : EditorWindow
{
    const string TEMPFILE = "queuedmenu.txt";
    const string ASSEMBLY = ",Assembly-CSharp";
    const string SCRIPT_TEMPLATE = "Assets/Invictus/Menu/TemplateMenu.cs.txt";
    const string MENUTYPES_SCRIPT_PATH = "Assets/Menutypes.cs";


    static List<Type> menuTypes = new List<Type>();
    static string[] menuTypeNames;


    [MenuItem("Assets/Create/Menu Script", false, 80)]
    public static void CreateMenuScript(MenuCommand cmd)
    {
        if (Selection.activeObject != null)
        {
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (File.Exists(path))
                path = Path.GetDirectoryName(path);
            if (string.IsNullOrEmpty(path)) path = "Assets/";
            CreateMenuWindow window = (CreateMenuWindow)GetWindow(typeof(CreateMenuWindow), true, "Create Menu");
            window.scriptPath = path;
            window.scriptOnly = true;
            window.Show();
            //ProjectWindowUtil.CreateScriptAssetFromTemplateFile(SCRIPT_TEMPLATE, "NewMenu.cs");
        }

    }

    [MenuItem("GameObject/UI/Menu")]
    static void StartCreateMenu()
    {
        // Get existing open window or if none, make a new one:
        CreateMenuWindow window = (CreateMenuWindow)GetWindow(typeof(CreateMenuWindow), true, "Create Menu");
        window.scriptPath = "Assets/";
        window.scriptOnly = false;
        window.Show();
    }

    [UnityEditor.Callbacks.DidReloadScripts]
    static void DidReloadScripts()
    {
        CollectMenuTypes();
        if (File.Exists(TEMPFILE))
        {
            string name = File.ReadAllText(TEMPFILE);
            Type menuType = Type.GetType(name + ASSEMBLY);
            if (menuType != null)
                CreateMenu(name, menuType);
            File.Delete(TEMPFILE);
        }
        if (!File.Exists(MENUTYPES_SCRIPT_PATH))
            CreateMenuTypes();
    }

    static void CollectMenuTypes()
    {
        menuTypes.Clear();
        foreach (var assemblies in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (Type type in assemblies.GetTypes())
                if (type.IsSubclassOf(typeof(MenuGUI.Menu)))
                    menuTypes.Add(type);
        }
        menuTypeNames = new string[menuTypes.Count];
        for (int i = 0; i < menuTypeNames.Length; ++i)
            menuTypeNames[i] = menuTypes[i].Name;
    }

    static void CreateMenu(string name, Type menuType)
    {
        MenuManager menuManager = FindObjectOfType<MenuManager>();

        GameObject menu = ObjectFactory.CreateGameObject(name, new Type[] { typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster), menuType });
        if (menuManager)
            menu.transform.SetParent(menuManager.transform, true);
        menu.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        menu.layer = LayerMask.NameToLayer("UI");
        Selection.activeObject = menu;
    }

    static void CreateMenuTypes(string newType = null)
    {
        using (StreamWriter outfile = new StreamWriter(MENUTYPES_SCRIPT_PATH))
        {
            outfile.WriteLine("");
            outfile.WriteLine("public enum MenuTypes {");
            foreach (string name in menuTypeNames)
                outfile.WriteLine("\t" + name + ",");
            if (newType != null)
                outfile.WriteLine("\t" + newType);
            outfile.WriteLine("}");
        }
    }


    bool scriptOnly = false;
    string scriptPath;
    string mName;
    int selected;

    private void OnEnable()
    {
        string path = AssetDatabase.GetAssetPath(GetInstanceID());
        minSize = maxSize = new Vector2(400, 60);
        selected = 0;
        mName = "";
    }

    void OnGUI()
    {
        EditorGUI.BeginDisabledGroup(EditorApplication.isCompiling);
        if (!scriptOnly)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Select Menu:", GUILayout.Width(80));
            selected = EditorGUILayout.Popup(selected, menuTypeNames);
            if (GUILayout.Button("Create"))
                CreateMenu(menuTypeNames[selected], menuTypes[selected]);
            GUILayout.EndHorizontal();
        }
        else
            GUILayout.Space(20);

        GUILayout.BeginHorizontal();
        GUILayout.Label("New Menu:", GUILayout.Width(80));
        mName = EditorGUILayout.TextField("", mName);
        if (GUILayout.Button("Create") && mName.Length > 0)
        {
            string className = mName.Replace(" ", "");
            Type menuType = Type.GetType(className + ASSEMBLY);
            if (menuType == null)
            {
                CreateMenuTypes(className);
                string scriptText = File.ReadAllText(SCRIPT_TEMPLATE);
                scriptText = scriptText.Replace("#SCRIPTNAME#", className);
                File.WriteAllText(scriptPath + "/"+ className + ".cs", scriptText);
                //A létrhozandó menu nevét kiírjuk
                if (!scriptOnly)
                    File.WriteAllText(TEMPFILE, className); 
                AssetDatabase.Refresh();
                if (scriptOnly)
                    Close();
            }
            else
                EditorUtility.DisplayDialog("Error", "The " + mName + " menu is already exists!", "OK");
        }
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();
    }
}
/*
public class CreateMenuManagerWindow : EditorWindow
{
    [MenuItem("Assets/Create/MenuManger Script", false, 81)]
    public static void CreateMenuScript(MenuCommand cmd)
    {
        ProjectWindowUtil.CreateScriptAssetFromTemplateFile("Assets/Invictus/Menu/TemplateMenu.cs.txt", "NewMenuManager.cs");
    }

    [MenuItem("GameObject/UI/Menu")]
    static void StartCreateMenu()
    {
        // Get existing open window or if none, make a new one:
        CreateMenuWindow window = (CreateMenuWindow)GetWindow(typeof(CreateMenuWindow), true, "Create Menu");
        window.Show();
    }

    public string mName;
    public bool waiting;

    private void OnEnable()
    {
        minSize = maxSize = new Vector2(400, 60);
    }

    void OnGUI()
    {
        if (waiting)
        {
            GUILayout.Label("Menu name:");
            GUILayout.BeginHorizontal();
            GUILayout.Label(mName, GUI.skin.textField);
            GUILayout.Button("Create");
            GUILayout.EndHorizontal();

            if (!EditorApplication.isCompiling && mName != null)
            {
                Type menuType = Type.GetType(mName + ",Assembly-CSharp");
                if (menuType != null)
                {
                    CreateMenu(mName, menuType);
                    mName = null;
                    waiting = false;
                }
            }
        }
        else
        {
            GUILayout.Label("Menu name:");
            GUILayout.BeginHorizontal();
            mName = EditorGUILayout.TextField("", mName);
            if (GUILayout.Button("Create"))
            {
                Type menuType = Type.GetType(mName + ",Assembly-CSharp");
                if (menuType == null)
                {
                    string scriptText = File.ReadAllText("Assets/Invictus/Menu/TemplateMenu.cs.txt");
                    scriptText = scriptText.Replace("#SCRIPTNAME#", mName.Replace(" ", ""));
                    File.WriteAllText("Assets/" + mName + ".cs", scriptText);
                    AssetDatabase.Refresh();
                    waiting = true;
                }
                else
                {
                    CreateMenu(mName, menuType);
                    mName = null;
                }
            }
            GUILayout.EndHorizontal();
        }
    }

    GameObject CreateMenu(string name, Type menuType)
    {
        GameObject menu = ObjectFactory.CreateGameObject(name, new Type[] { typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster), menuType });
        menu.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        menu.layer = LayerMask.NameToLayer("UI");
        return menu;
    }
}
*/


    
[CustomEditor(typeof(MenuManager),true)]
public class MenuManagerEditor : Editor
{

    public class MenuContent : GUIContent
    {
        public int uniqueId;
        public MenuContent(string text, Texture image, int id = -1) : base(text, image)
        {
            uniqueId = id;
        }
    }


    [MenuItem("GameObject/UI/MenuManager")]
    static MenuManager CreateMenuManager()
    {
        if (FindObjectOfType<EventSystem>() == null)
            EditorApplication.ExecuteMenuItem("GameObject/UI/Event System");

        MenuManager menuManager = FindObjectOfType<MenuManager>();
        if (menuManager == null)
            menuManager = new GameObject("MenuManager", typeof(MenuManager)).GetComponent<MenuManager>();
        menuManager.gameObject.layer = LayerMask.NameToLayer("UI");
        return menuManager;
    }


    FieldInfo preloadedFiled;
    Texture scriptImage;


    private void OnEnable()
    {
        preloadedFiled = typeof(MenuManager).GetField("preloadedMenus", BindingFlags.Instance | BindingFlags.NonPublic);
        scriptImage = (Texture)EditorGUIUtility.Load("cs Script Icon");

    }


    private int GetMenuItems(int defValue, out MenuContent[] uIContents)
    {
        MenuManager menuManager = (MenuManager)target;
        List<MenuGUI.Menu> menus = new List<MenuGUI.Menu>();
        menuManager.GetComponentsInChildren(true,menus);
        if (preloadedFiled != null)
        {
            string[]  preloadedMenus =(string[])preloadedFiled.GetValue(menuManager);

            foreach(string path in preloadedMenus)
            {
                UnityEngine.Object obj = AssetDatabase.LoadMainAssetAtPath(ResourcesFiles.GetFullAssetPath(path, typeof(MenuGUI.Menu)));
                if (obj && obj.GetType() == typeof(GameObject))
                {
                    MenuGUI.Menu menu = (MenuGUI.Menu)(obj as GameObject)?.GetComponent(typeof(MenuGUI.Menu));
                    if (menu) menus.Add(menu);
                }
            }
        }

        int selected = 0;
        uIContents = new MenuContent[menus.Count + 1];
        uIContents[0] = new MenuContent("<No Start Menu>", scriptImage,-1);
        for (int i = 0; i < menus.Count; ++i)
        {
            int id = menus[i].UniqueID;
            uIContents[i+1] = new MenuContent(menus[i].GetType().Name, scriptImage,id);
            if (menus[i].UniqueID == defValue)
                selected = i+1;
        }
        return selected;
    }


    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUI.BeginChangeCheck();
        bool enterChildren = true;
        SerializedProperty iter = serializedObject.GetIterator();
        while (iter.NextVisible(enterChildren))
        {
            if (iter.name == "startMenu")
            {
                MenuContent[] menuIds;
                int selected = GetMenuItems(iter.intValue, out menuIds);
                selected = EditorGUILayout.Popup(new GUIContent(ObjectNames.NicifyVariableName(iter.name)), selected, menuIds);
                int uid = menuIds[selected].uniqueId;
                if (iter.intValue != uid)
                   iter.intValue = uid;
            }
            else
                EditorGUILayout.PropertyField(iter, true);
            enterChildren = false;
        }
        bool result = EditorGUI.EndChangeCheck();
        if (result)
            serializedObject.ApplyModifiedProperties();
    }
}
