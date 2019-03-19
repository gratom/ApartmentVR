using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(AppSetting))]
public class AppSettingEditor : PropertyDrawer
{

    private int StandartElementHeight = 18;
    private int Offset = 2;
    private int currentHeight = 100;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return currentHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        string AppName = property.FindPropertyRelative("FileName").stringValue;
        if(AppName.IndexOf(".") > 0)
        {
            AppName = AppName.Substring(0, AppName.IndexOf("."));
        }
        EditorGUI.LabelField(position,"App " + AppName);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        #region set file name of local storage

        var fileNameRect = new Rect(position.x + 130, position.y + StandartElementHeight + Offset, position.width - 130, StandartElementHeight);
        var fileNameLabelRect = new Rect(position.x + 60, position.y + StandartElementHeight + Offset, position.width - 60, StandartElementHeight);
        EditorGUI.LabelField(fileNameLabelRect, "file name:");
        property.FindPropertyRelative("FileName").stringValue = EditorGUI.TextField(fileNameRect, property.FindPropertyRelative("FileName").stringValue);

        #endregion

        #region set Application ID

        var AppIDRect = new Rect(position.x + 130, position.y + (StandartElementHeight + Offset) * 2, position.width - 130, StandartElementHeight);
        var AppIDLabelRect = new Rect(position.x + 60, position.y + (StandartElementHeight + Offset) * 2, position.width - 60, StandartElementHeight);
        EditorGUI.LabelField(AppIDLabelRect, "AppID:");
        property.FindPropertyRelative("AppID").intValue = EditorGUI.IntField(AppIDRect, property.FindPropertyRelative("AppID").intValue);

        #endregion

        #region set Application type

        var AppTypeRect = new Rect(position.x + 130, position.y + (StandartElementHeight + Offset) * 3, position.width - 130, StandartElementHeight);
        var AppTypeLabelRect = new Rect(position.x + 60, position.y + (StandartElementHeight + Offset) * 3, position.width - 60, StandartElementHeight);
        EditorGUI.LabelField(AppTypeLabelRect, "App type:");
        string[] arrayOfOptions = System.Enum.GetNames(typeof(AppSetting.AppType));
        property.FindPropertyRelative("_appType").enumValueIndex = EditorGUI.Popup(AppTypeRect, property.FindPropertyRelative("_appType").enumValueIndex, arrayOfOptions);

        #endregion

        #region Display the view ID

        var viewIDRect = new Rect(position.x + 60, position.y + (StandartElementHeight + Offset) * 4, position.width - 60, StandartElementHeight);
        EditorGUI.LabelField(viewIDRect, "view ID : " + property.FindPropertyRelative("ViewID").intValue.ToString());

        #endregion

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
        
    }

}