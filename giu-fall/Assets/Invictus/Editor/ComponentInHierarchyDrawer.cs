using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ComponentInHierarchyAttribute))]
public class ComponentInHierarchyDrawer : PropertyDrawer
{
    static string[] UNITYNAMESPACES = new string[]
    {
        "UnityEngine",
        "UnityEngine.Accessibility",
        "UnityEngine.Advertisements",
        "UnityEngine.AI",
        "UnityEngine.Analytics",
        "UnityEngine.Animations",
        "UnityEngine.Apple",
        "UnityEngine.Assertions",
        "UnityEngine.Audio",
        "UnityEngine.CrashReportHandler",
        "UnityEngine.Events",
        "UnityEngine.EventSystems",
        "UnityEngine.Experimental",
        "UnityEngine.iOS",
        "UnityEngine.Jobs",
        "UnityEngine.Networking",
        "UnityEngine.Playables",
        "UnityEngine.Profiling",
        "UnityEngine.Purchasing",
        "UnityEngine.Rendering",
        "UnityEngine.SceneManagement",
        "UnityEngine.Scripting",
        "UnityEngine.Serialization",
        "UnityEngine.SocialPlatforms",
        "UnityEngine.SpatialTracking",
        "UnityEngine.Sprites",
        "UnityEngine.TestTools",
        "UnityEngine.Tilemaps",
        "UnityEngine.Timeline",
        "UnityEngine.tvOS",
        "UnityEngine.U2D",
        "UnityEngine.UI",
        "UnityEngine.UI.Extensions",
        "UnityEngine.Video",
        "UnityEngine.Windows",
        "UnityEngine.WSA",
        "UnityEngine.XR"
    };

    private Type GetTypeFromPropertyType(string internalType)
    {
        if (internalType == "PPtr<$GameObject>")
            return typeof(GameObject);

        string typename = internalType.Replace("PPtr<$", "");
        typename = typename.Replace(">", "");
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type type = assembly.GetType(typename);
            if (type != null)
                return type;
            else
            {
                foreach (string nameSpace in UNITYNAMESPACES)
                {
                    type = assembly.GetType(nameSpace + "." + typename);
                    if (type != null)
                        return type;
                }
            }
        }
        return null;
    }


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        Type type = GetTypeFromPropertyType(property.type);
        if (type != null && (type == typeof(GameObject) || type.IsSubclassOf(typeof(Component)) ) )
        {
            MonoBehaviour obj = property.serializedObject.targetObject as MonoBehaviour;
            if (obj != null)
            {
                Rect p1 = new Rect(position.x, position.y, position.width - 30, position.height);
                Rect p2 = new Rect(p1.xMax+5, p1.y, 25, position.height);
                bool isGameObject = type == typeof(GameObject);
                string typeName = !isGameObject ? type.Name : "GameObject";

                Texture2D texture = AssetPreview.GetMiniTypeThumbnail(type);
                if (texture == null && type.IsSubclassOf(typeof(MonoBehaviour)))
                    texture = (Texture2D)typeof(EditorGUIUtility).GetMethod("LoadIcon", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).Invoke(null, new object[] { "cs Script Icon" });

                List<UnityEngine.Object> objects = GetComponentsInParents(obj.transform, (!isGameObject) ? type : typeof(Transform));
                objects.AddRange(obj.transform.GetComponentsInChildren((!isGameObject) ? type : typeof(Transform), true));
                UnityEngine.Object curr_object = property.objectReferenceValue;
                if (curr_object != null && isGameObject)
                    curr_object = ((GameObject)curr_object).transform;


                GUIContent[] names = new GUIContent[objects.Count + 1];
                names[objects.Count] = new GUIContent("<null>", texture);
                int selected = objects.Count;
                for (int i = 0; i < objects.Count; ++i)
                {
                    names[i] = new GUIContent(objects[i].name + " (" + i + ")", texture);
                    if (curr_object == objects[i])
                        selected = i;
                }

                label.tooltip += "(Type: " + typeName + ")";
                selected = EditorGUI.Popup(p1, label, selected, names);
                UnityEngine.Object sel_object = (selected < objects.Count) ? objects[selected] : null;

                if (curr_object != sel_object)
                    property.objectReferenceValue = (!isGameObject || sel_object == null) ? sel_object : ((Transform)sel_object).gameObject;

                if (sel_object != null && GUI.Button(p2, EditorGUIUtility.IconContent("ViewToolZoom On") ) )
                    EditorGUIUtility.PingObject(sel_object);
            }
            else
                EditorGUI.PropertyField(position, property);
        }
        else
            EditorGUI.PropertyField(position, property);
        EditorGUI.EndProperty();
    }

    Transform GetRoot(Transform child)
    {
        while (child.parent != null)
            child = child.parent;
        return child;
    }



    List<UnityEngine.Object> GetComponentsInParents(Transform child, Type type)
    {
        List<UnityEngine.Object> components = new List<UnityEngine.Object>();

        Transform parent = child.parent;
        while (parent != null)
        {
            components.InsertRange(0,parent.GetComponents(type));
            parent = parent.parent;
        }
        return components;
    }


}