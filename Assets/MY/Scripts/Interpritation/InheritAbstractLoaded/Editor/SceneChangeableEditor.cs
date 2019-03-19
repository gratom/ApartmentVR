using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SceneChangeableObject))]
public class SceneChangeableEditor : Editor
{

    private static Dictionary<string, int> DropDownTypesDictionary;

    private static ListOfSceneChangeableObjectCashed ListBox;

    private int currentTypeChoise
    {
        get
        {
            return _currentTypeChoise;
        }
        set
        {
            if (_currentTypeChoise != value)
            {
                currentNameChoise = 0;
            }
            _currentTypeChoise = value;
        }
    }
    private int _currentTypeChoise;

    private int currentNameChoise;

    private static string[] ArrayOfTypeOptions;

    public void Awake()
    {
        TryInitDisctionary();
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("Object ID : " + ((SceneChangeableObject)target).ID);

        #region CreateDropDown

        if (ArrayOfTypeOptions != null)
        {
            if (((SceneChangeableObject)target).ID != 0)
            {
                for (int i = 0; i < ListBox.sceneChangeableObjectCasheds.Count; i++)
                {
                    if (((SceneChangeableObject)target).ID == ListBox.sceneChangeableObjectCasheds[i].objectID)
                    {
                        for(int j = 0; j < ArrayOfTypeOptions.Length; j++)
                        {
                            if (DropDownTypesDictionary[ArrayOfTypeOptions[j]].ToString() == ListBox.sceneChangeableObjectCasheds[i].objectType)
                            {
                                currentTypeChoise = j;

                                break;
                            }
                        }
                        break;
                    }
                }
            }
            currentTypeChoise = EditorGUILayout.Popup("Type of object : ", currentTypeChoise, ArrayOfTypeOptions);
            ((SceneChangeableObject)target).ChangeableObjectType = DropDownTypesDictionary[ArrayOfTypeOptions[currentTypeChoise]].ToString();
        }
        else
        {
            EditorGUILayout.LabelField("Object name : " + ((SceneChangeableObject)target).ChangeableObjectType);
        }
        #endregion

        #region Change name
        if (ListBox != null)
        {
            List<string> ListOfOptions = new List<string>();
            for (int i = 0; i < ListBox.sceneChangeableObjectCasheds.Count; i++)
            {
                if (ListBox.sceneChangeableObjectCasheds[i].objectType == ((SceneChangeableObject)target).ChangeableObjectType)
                {
                    ListOfOptions.Add(ListBox.sceneChangeableObjectCasheds[i].objectName);
                }
            }
            string[] ArrayOfOptions = new string[ListOfOptions.Count];
            ListOfOptions.CopyTo(ArrayOfOptions, 0);

            if (((SceneChangeableObject)target).ID != 0)
            {
                for (int i = 0; i < ArrayOfOptions.Length; i++)
                {
                    if (ArrayOfOptions[i] == ((SceneChangeableObject)target).ChangeableObjectName)
                    {
                        currentNameChoise = i;
                        break;
                    }
                }
            }
            currentNameChoise = EditorGUILayout.Popup("Object name : ", currentNameChoise, ArrayOfOptions);
            for (int i = 0; i < ListBox.sceneChangeableObjectCasheds.Count; i++)
            {
                if (ArrayOfOptions[currentNameChoise] == ListBox.sceneChangeableObjectCasheds[i].objectName)
                {
                    ((SceneChangeableObject)target).ChangeableObjectName = ListBox.sceneChangeableObjectCasheds[i].objectName;
                    ((SceneChangeableObject)target).ID = ListBox.sceneChangeableObjectCasheds[i].objectID;
                    break;
                }
            }
        }
        else
        {
            EditorGUILayout.LabelField("Object name : " + ((SceneChangeableObject)target).ChangeableObjectName);
        }
        #endregion

        EditorGUILayout.PropertyField(this.serializedObject.FindProperty("settingFieldList"), true);
        EditorGUILayout.PropertyField(this.serializedObject.FindProperty("AssetGameObject"));
        EditorGUILayout.PropertyField(this.serializedObject.FindProperty("_remoteAssetBundleInstance"), true);
        EditorGUILayout.PropertyField(this.serializedObject.FindProperty("_RemoteAssetBundleThumbnailInstance"), true);

    }

    private void TryInitDisctionary()
    {

        #region init types
        string typeSetting = SaverLoaderModule.LoadMyDataFromFile("typesSetting.config");
        if (typeSetting != "")
        {
            DropDownTypesDictionary = new Dictionary<string, int>();
            string[] sTempArray = typeSetting.Split(',');
            for (int i = 0; i < sTempArray.Length; i++)
            {
                if (sTempArray[i] != "")
                {
                    DropDownTypesDictionary.Add(sTempArray[i].Substring(sTempArray[i].IndexOf('.') + 1), int.Parse(sTempArray[i].Substring(0, sTempArray[i].IndexOf('.'))));
                }
            }

            ArrayOfTypeOptions = new string[DropDownTypesDictionary.Count];
            DropDownTypesDictionary.Keys.CopyTo(ArrayOfTypeOptions, 0);

        }
        #endregion

        #region init names
        string ObjectCashedstring = SaverLoaderModule.LoadMyDataFromFile("ObjectCashe.config");
        if (ObjectCashedstring != "")
        {
            ListBox = JsonUtility.FromJson<ListOfSceneChangeableObjectCashed>(ObjectCashedstring);
        }
        #endregion
    }

}


