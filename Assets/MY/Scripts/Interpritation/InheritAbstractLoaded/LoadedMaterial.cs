using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MyVRMenu;

public enum LoadedMaterialClassTypes
{
    nameMaterial,
    AssetBundleURL,
    ForItemsWithID,
    PreImage
}

/// <summary>
/// Serializeble class for start setting
/// </summary>
[System.Serializable]
public class SettingForFieldsInLoadedMaterial : AbstractObjectConstructableComponentData<LoadedMaterialClassTypes> { }

/// <summary>
/// Class for dynamic loaded materials
/// </summary>
[System.Serializable]
public class LoadedMaterial : AbstractObjectConstructable <LoadedMaterialClassTypes>, IAssetBundleLoadable, IMenuClickable
{
    /// <summary>
    /// Name of material, name of this item in GudHub
    /// </summary>
    public string LoadedMaterialName { get; private set; }

    /// <summary>
    /// List of items on which you can apply this material
    /// This list stores item IDs in "int" type
    /// </summary>
    public int[] ListOfItemsFor  { get; private set; }

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
    private List<SettingForFieldsInLoadedMaterial> settingFieldList;
    
    /// <summary>
    /// Material that loaded from AssetBundle
    /// </summary>
    public Material loadedMaterial { get; private set; }

    /// <summary>
    /// AssetBundle instance, that contains the material
    /// </summary>
    public AssetBundle AssetBundleInstance { get; private set; }

    private Dictionary<AssetBundleLoaderManager.IAssetBundleLoadableEvent, List<AssetBundleLoaderManager.OnEventFunction>> EventDictionary;

    private bool IsBundleAlreadyLoading = false;

    #region public functions

    /// <summary>
    /// Use this BEFORE initializating. This function make the inner cohesion in class instance 
    /// </summary>
    public override void InitDictionary()
    {
        FunctionsDictionary = new Dictionary<LoadedMaterialClassTypes, InitFunctions>();
        FunctionsDictionary.Add(LoadedMaterialClassTypes.nameMaterial, InitName);
        FunctionsDictionary.Add(LoadedMaterialClassTypes.AssetBundleURL, InitURL);
        FunctionsDictionary.Add(LoadedMaterialClassTypes.ForItemsWithID, InitListOfItemsFor);

        if (ComponentsDataList == null)
        {
            SettingListFieldToRealFields();
        }
    }

    /// <summary>
    /// The Initializing functions. Make the initialization by data from ComponentsDataList.
    /// </summary>
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

    /// <summary>
    /// Try to load material on loadedMaterial property from AssetBundleInstance
    /// </summary>
    public void LoadMaterial()
    {
        if (AssetBundleInstance != null)
        {
            loadedMaterial = (Material)AssetBundleInstance.LoadAsset(AssetBundleInstance.GetAllAssetNames()[0]);
        }
    }

    /// <summary>
    /// Returns a nicely edited string with information about an object.
    /// </summary>
    /// <returns>Nicely edited string</returns>
    public override string ToString()
    {
        string sReturned = "";
        sReturned += "name on scene: " + name + "\n";
        sReturned += "gudhub name: " + LoadedMaterialName + "\n";
        sReturned += "gudhub item ID: " + ID + "\n";
        sReturned += "ID of AssetBundle: " + URLName + "\n";
        sReturned += "item reference: \n{\n";
        for(int i = 0; i < ListOfItemsFor.Length; i++)
        {
            sReturned += "    " + ListOfItemsFor[i] + ";\n";
        }
        sReturned += "}";
        return sReturned;
    }

    #endregion

    #region private functions

    #region init functions

    private void SettingListFieldToRealFields()
    {
        ComponentsDataList = new List<AbstractObjectConstructableComponentData<LoadedMaterialClassTypes>>();
        for (int i = 0; i < settingFieldList.Count; i++)
        {
            ComponentsDataList.Add(settingFieldList[i]);
        }
    }

    private void InitURL(int num)
    {
        try
        {
            URLName = ComponentsDataList[num].StringValue;
            RealGudHubURL = JSONMainManager.Instance.GetRealFileURLById(URLName);
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    private void InitName(int num)
    {
        LoadedMaterialName = ComponentsDataList[num].StringValue;
    }

    private void InitListOfItemsFor(int num)
    {
        string[] temp = ComponentsDataList[num].StringValue.Split(',');
        ListOfItemsFor = new int[temp.Length];
        for (int i = 0; i < ListOfItemsFor.Length; i++)
        {
            ListOfItemsFor[i] = int.Parse(temp[i].Substring(temp[i].IndexOf('.') + 1));
        }
    }

    #endregion

    private void InitEventDictionary()
    {
        EventDictionary = new Dictionary<AssetBundleLoaderManager.IAssetBundleLoadableEvent, List<AssetBundleLoaderManager.OnEventFunction>>();
        for (int i = 0; i < Enum.GetNames(typeof(AssetBundleLoaderManager.IAssetBundleLoadableEvent)).Length; i++)
        {
            EventDictionary[(AssetBundleLoaderManager.IAssetBundleLoadableEvent)i] = new List<AssetBundleLoaderManager.OnEventFunction>();
        }
    }

    private void EventHappend(AssetBundleLoaderManager.IAssetBundleLoadableEvent eventType)
    {
        if (EventDictionary == null)
        {
            InitEventDictionary();
            return;
        }
        //create new list, for stabil work
        List<AssetBundleLoaderManager.OnEventFunction> tempList = new List<AssetBundleLoaderManager.OnEventFunction>();
        for (int i = 0; i < EventDictionary[eventType].Count; i++)
        {
            tempList.Add(EventDictionary[eventType][i]);
        }
        for (int i = 0; i < tempList.Count; i++)
        {
            tempList[i](this);
        }
    }

    #endregion

    #region interfaces

    /// <summary>
    /// Initiate loading assetBundle
    /// </summary>
    public void StartLoadAssetBundle()
    {        
        if (!IsBundleAlreadyLoading)
        {
            AssetBundleLoaderManager.Instance.AddToLoadable(this);            
            IsBundleAlreadyLoading = true;
            EventHappend(AssetBundleLoaderManager.IAssetBundleLoadableEvent.BundleStartLoading);
        }
        if (AssetBundleInstance != null)
        {
            EventHappend(AssetBundleLoaderManager.IAssetBundleLoadableEvent.BundleReady);
        }
    }

    /// <summary>
    /// Calling when bundle is ready to load to this item
    /// </summary>
    void IAssetBundleLoadable.BundleReady()
    {
        if (AssetBundleInstance == null)
        {
            string path = AssetBundleLoaderManager.Instance.AppPath;
            AssetBundleCreateRequest aTemp = AssetBundle.LoadFromFileAsync(path + URLName);
            aTemp.completed += x => 
            {
                AssetBundleInstance = aTemp.assetBundle;
                LoadMaterial();
                EventHappend(AssetBundleLoaderManager.IAssetBundleLoadableEvent.BundleReady);
            };            
        }
        
    }

    /// <summary>
    /// Hide assetBundle. In this item do nothing
    /// </summary>
    public void BundleHide()
    {
        //do nothing
    }

    /// <summary>
    /// Show assetBundle. In this item do nothing
    /// </summary>
    public void BundleShow()
    {
        //do nothing
    }

    /// <summary>
    /// Adds a listener to one of the events
    /// </summary>
    /// <param name="bundleEvent">Type of event</param>
    /// <param name="onEventFunction">Delegate functions</param>
    public void AddListenerLoading(AssetBundleLoaderManager.IAssetBundleLoadableEvent bundleEvent, AssetBundleLoaderManager.OnEventFunction onEventFunction)
    {
        if (EventDictionary == null)
        {
            InitEventDictionary();
        }
        EventDictionary[bundleEvent].Add(onEventFunction);
    }

    /// <summary>
    /// Remove listener in one of the events
    /// </summary>
    /// <param name="bundleEvent">Type of event</param>
    /// <param name="onEventFunction">Delegate function</param>
    public void RemoveListenerLoading(AssetBundleLoaderManager.IAssetBundleLoadableEvent bundleEvent, AssetBundleLoaderManager.OnEventFunction onEventFunction)
    {
        if (EventDictionary == null)
        {
            InitEventDictionary();
        }
        EventDictionary[bundleEvent].Remove(onEventFunction);
    }

    /// <summary>
    /// Return the URL name of this item (index of file in gudhub)
    /// </summary>
    /// <returns>String URL name</returns>
    public string GetURLName()
    {
        return URLName;
    }

    /// <summary>
    /// Return the real URL of gudhub for loading this AssetBundle
    /// </summary>
    /// <returns>String with real URL for loading</returns>
    public string GetRealURL()
    {
        return RealGudHubURL;
    }

    /// <summary>
    /// The function called when item clicked on menu, usually it placed in delegate
    /// </summary>
    /// <param name="itemInstance">object, on which cliked</param>
    public void OnMenuClick(MonoBehaviour itemInstance)
    {
        try
        {
            ((SceneChangebleObject)MenuManager.Instance.ObjectSelected).ChangeMaterialTo(loadedMaterial);
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    /// <summary>
    /// Call when menu item is point
    /// </summary>
    /// <param name="menuItem">Item, that pointed</param>
    /// <param name="isPointed">Parametr, that controling animation</param>
    public void OnPointFunction(MenuItem menuItem, bool isPointed)
    {
        if (menuItem != null)
        {
            if (isPointed)
            {
                menuItem.Illumination.PlayEffect();
            }
            else
            {
                menuItem.Illumination.StopEffect();
            }
        }
    }

    /// <summary>
    /// Copies an AssetBundle from an object, placing a link to it in its Instance. 
    /// If the parent AssetBundle is empty, it calls the AssetBundle loading method and subscribes to the load event.
    /// </summary>
    /// <param name="itemOriginal">Other object, that contain AssetBundle</param>
    public void CopyBundleFrom(IAssetBundleLoadable itemOriginal)
    {
        //делаем пометку что мы копия, и что мы ждем, или уже получили свой бандл
        IsBundleAlreadyLoading = true;

        if (itemOriginal.AssetBundleInstance != null)
        {
            AssetBundleInstance = itemOriginal.AssetBundleInstance;
            LoadMaterial();
            EventHappend(AssetBundleLoaderManager.IAssetBundleLoadableEvent.BundleReady);
        }
        else
        {
            itemOriginal.AddListenerLoading(AssetBundleLoaderManager.IAssetBundleLoadableEvent.BundleReady, CopyBundleFrom);
            itemOriginal.StartLoadAssetBundle();
        }
    }

    #endregion

    #region tests

#if UNITY_EDITOR

    private void Test1()
    {
        string temp = "1234.1234";
        Debug.Log(temp.Substring(temp.IndexOf('.') + 1));
    }

#endif

    #endregion

}
