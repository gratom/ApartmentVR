using UnityEngine;
using UniversalAssetBundleLoader;
using UnityEditor;

[CustomPropertyDrawer(typeof(RemoteAssetBundle))]
public class RemoteAssetBundleEditor : PropertyDrawer
{

    private int StandartElementHeight = 18;
    private int Offset = 3;
    private bool isOpen;
    private int currentHeight = 21;

    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        GUIStyle style = new GUIStyle();
        style.richText = true;

        // Draw label
        //EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rects
        var isReadyRect = new Rect(position.x + Offset + StandartElementHeight, position.y + Offset, position.width, position.height);
        var openCloseButtonRect = new Rect(position.x, position.y, StandartElementHeight, StandartElementHeight);

        if (isOpen)
        {

            var urlRect = new Rect(position.x + 60, position.y + StandartElementHeight + Offset, position.width - 60, StandartElementHeight);
            var loadedFromRect = new Rect(position.x + 60, position.y + (StandartElementHeight + Offset) * 2, position.width - 60, StandartElementHeight);
            var priorityRect = new Rect(position.x, position.y + (StandartElementHeight + Offset) * 3, position.width, StandartElementHeight);

            var buttonURLRect = new Rect(position.x, position.y + StandartElementHeight + Offset, 60, StandartElementHeight);
            var buttonloadedFromRect = new Rect(position.x, position.y + (StandartElementHeight + Offset) * 2, 60, StandartElementHeight);

            // Draw fields - passs GUIContent.none to each so they are drawn without labels
            EditorGUI.LabelField(urlRect, property.FindPropertyRelative("_ParentItemURL").stringValue);
            EditorGUI.LabelField(loadedFromRect, property.FindPropertyRelative("_LoadedFrom").stringValue);
            EditorGUI.LabelField(priorityRect, "Loading priority : " + property.FindPropertyRelative("_Priority").intValue.ToString());
            if (GUI.Button(buttonURLRect, "open ->"))
            {
                if (property.FindPropertyRelative("_ParentItemURL").stringValue != "")
                {
                    Application.OpenURL(property.FindPropertyRelative("_ParentItemURL").stringValue);
                }
            }
            if (GUI.Button(buttonloadedFromRect, "open ->"))
            {
                string temp = property.FindPropertyRelative("_LoadedFrom").stringValue;
                if (temp != "")
                {
                    EditorUtility.OpenFilePanel("That is where file is it", temp.Substring(0, temp.LastIndexOf('/')), "");
                }
            }
        }

        if (property.FindPropertyRelative("_IsReady").boolValue)
        {
            EditorGUI.LabelField(isReadyRect, "AssetBundle item " + property.FindPropertyRelative("_Name").stringValue + "<color=#005000>(READY)</color>", style);
        }
        else
        {
            EditorGUI.LabelField(isReadyRect, "AssetBundle item " + property.FindPropertyRelative("_Name").stringValue + "<color=#c05050>(NOT READY)</color>", style);
        }

        if (GUI.Button(openCloseButtonRect, (isOpen) ? ("-") : ("+")))
        {
            isOpen = !isOpen;
            currentHeight = isOpen ? (StandartElementHeight + Offset) * 4 : StandartElementHeight + Offset;
        }

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {

        return currentHeight;

    }

}