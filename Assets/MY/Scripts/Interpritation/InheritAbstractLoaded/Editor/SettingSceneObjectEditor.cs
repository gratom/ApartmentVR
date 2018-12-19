using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//[CustomPropertyDrawer(typeof(SettingForFieldsInSceneObject))]
//public class SettingSceneObjectEditor : PropertyDrawer {

//    private SceneObjectTypes sot;

//    // Draw the property inside the given rect
//    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//    {
//        // Using BeginProperty / EndProperty on the parent property means that
//        // prefab override logic works on the entire property.
//        EditorGUI.BeginProperty(position, label, property);

//        GUIStyle style = new GUIStyle();
//        style.richText = true;

//        // Draw label
//        //EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

//        // Don't make child fields be indented
//        var indent = EditorGUI.indentLevel;
//        EditorGUI.indentLevel = 0;

//        // Calculate rects
//        var isReadyRect = new Rect(position.x + Offset + StandartElementHeight, position.y + Offset, position.width, position.height);

//        EditorGUILayout.PropertyField(property.FindPropertyRelative("valueType"));

//        // Set indent back to what it was
//        EditorGUI.indentLevel = indent;

//        EditorGUI.EndProperty();
//    }

//    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//    {

//        return currentHeight;

//    }

//}