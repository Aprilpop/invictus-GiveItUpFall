using System;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(FlagAttribute))]
public class EnumFlagDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        FlagAttribute flagSettings = (FlagAttribute)attribute;
        string propName = flagSettings.enumName;
        if (string.IsNullOrEmpty(propName))
            propName = ObjectNames.NicifyVariableName(property.name);

        EditorGUI.BeginProperty(position, label, property);
        EditorGUI.BeginChangeCheck();
        Enum enumNew = EditorGUI.EnumFlagsField(position, propName, (Enum)Enum.ToObject(fieldInfo.FieldType, property.intValue));
        if (EditorGUI.EndChangeCheck())
            property.intValue = (int)Convert.ChangeType(enumNew, fieldInfo.FieldType);
        EditorGUI.EndProperty();
    }
}