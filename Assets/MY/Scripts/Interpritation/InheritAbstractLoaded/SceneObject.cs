using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniversalAssetBundleLoader;
using UniversalImageLoader;

/// <summary>
/// Types of fiels in SceneObject
/// </summary>
public enum SceneObjectTypes
{
    SceneName,
    AssetBundleURL,
    PreImageURL
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

    public TextMesh textMestName;

    /// <summary>
    /// A link where you can download this item from GudHub
    /// </summary>
    public string RealGudHubURL { get; private set; }

    /// <summary>
    /// Unique ID file in GudHub
    /// </summary>
    public string URLNameRemoteAssetBundle { get; private set; }

    /// <summary>
    /// Unique ID file in GudHub
    /// </summary>
    public string URLNameRemoteImage { get; private set; }

    /// <summary>
    /// Instance of Remote AssetBundle in this class
    /// </summary>
    public RemoteAssetBundle RemoteAssetBundleInstance
    {
        get
        {
            return _remoteAssetBundleInstance;
        }
        private set
        {
            _remoteAssetBundleInstance = value;
        }
    }
    [SerializeField]
    private RemoteAssetBundle _remoteAssetBundleInstance;

    /// <summary>
    /// Preloaded image for showing in menu
    /// </summary>
    public RemoteImage RemoteImageInstance
    {
        get
        {
            return _remoteImageInstance;
        }
        set
        {
            _remoteImageInstance = value;
        }
    }
    [SerializeField]
    private RemoteImage _remoteImageInstance;
    
    [Tooltip("This is a list of settings for the correct operation of the internal functions for initializing an item.\nThese settings are used to determine how to process data from JSON.")]
    [SerializeField]
    private List<SettingForFieldsInSceneObject> settingFieldList;

    #region public functions

    public override void InitDictionary()
    {
        FunctionsDictionary = new Dictionary<SceneObjectTypes, InitFunctions>
        {
            { SceneObjectTypes.SceneName, InitSceneName },
            { SceneObjectTypes.AssetBundleURL, InitAssetBundleURL },
            { SceneObjectTypes.PreImageURL, InitImageURL }
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
            ComponentsDataList.Add(new AbstractObjectConstructableComponentData<SceneObjectTypes>());
            ComponentsDataList[i].IdField = settingFieldList[i].IdField;
            ComponentsDataList[i].valueType = settingFieldList[i].valueType;
        }
    }

    private void InitSceneName(int num)
    {
        SceneName = ComponentsDataList[num].StringValue;
        textMestName.text = SceneName;
    }

    private void InitAssetBundleURL(int num)
    {
        try
        {
            URLNameRemoteAssetBundle = ComponentsDataList[num].StringValue;
            RemoteAssetBundleInstance = new RemoteAssetBundle(JSONMainManager.Instance.GetRealFileURLById(URLNameRemoteAssetBundle)
                , URLNameRemoteAssetBundle
                , JSONMainManager.Instance.GetRealItemURLByID(ID));
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    private void InitImageURL(int num)
    {
        try
        {
            URLNameRemoteImage = ComponentsDataList[num].StringValue;
            RemoteImageInstance = new RemoteImage(JSONMainManager.Instance.GetRealFileURLById(URLNameRemoteImage)
                , URLNameRemoteImage
                , JSONMainManager.Instance.GetRealItemURLByID(ID));
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    #endregion

    #region interfaces
    #endregion

}
