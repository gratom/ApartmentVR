using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Types of fiels in SceneObject
/// </summary>
public enum SceneObjectTypes
{
    SceneName,
    AssetBundleURL,
    PreImage
}

/// <summary>
/// Serializable class for start setting
/// </summary>
[System.Serializable]
public class SettingForFieldsInSceneObject : AbstractObjectConstructableComponentData<SceneObjectTypes> {}

/// <summary>
/// Class for Loading scenes
/// </summary>
[System.Serializable]
public class SceneObject : AbstractObjectConstructable<SceneObjectTypes>
{

    /// <summary>
    /// Name of this scene, that will be showed in choosing scene menu
    /// </summary>
    public string SceneName { get; private set; }

    /// <summary>
    /// A link where you can download this item from GudHub
    /// </summary>
    public string RealGudHubURL { get; private set; }

    /// <summary>
    /// Unique ID file in GudHub
    /// </summary>
    public string URLName { get; private set; }

    [Tooltip("This is a list of settings for the correct operation of the internal functions for initializing an item.\nThese settings are used to determine how to process data from JSON.")]
    [SerializeField]
    private List<SettingForFieldsInSceneObject> settingFieldList;

    #region public functions

    public override void InitDictionary()
    {
        FunctionsDictionary = new Dictionary<SceneObjectTypes, InitFunctions>
        {
            { SceneObjectTypes.SceneName, InitSceneName },
            { SceneObjectTypes.AssetBundleURL, InitURL },
            { SceneObjectTypes.PreImage, InitListOfItemsFor }
        };

        if (ComponentsDataList == null)
        {
            SettingListFieldToRealFields();
        }
    }

    public override void InitConstruct()
    {
        if (FunctionsDictionary != null)
        {
            for (int i = 0; i < ComponentsDataList.Count; i++)
            {
                if (FunctionsDictionary.ContainsKey(ComponentsDataList[i].valueType))
                {
                    FunctionsDictionary[ComponentsDataList[i].valueType](i);
                }
            }
        }
    }

    #endregion

    #region private functions

    private void SettingListFieldToRealFields()
    {
        ComponentsDataList = new List<AbstractObjectConstructableComponentData<SceneObjectTypes>>();
        for (int i = 0; i < settingFieldList.Count; i++)
        {
            ComponentsDataList.Add(settingFieldList[i]);
        }
    }

    private void InitSceneName(int num)
    {

    }

    private void InitURL(int num)
    {

    }

    private void InitListOfItemsFor(int num)
    {

    }

    #endregion

    #region interfaces
    #endregion

}
