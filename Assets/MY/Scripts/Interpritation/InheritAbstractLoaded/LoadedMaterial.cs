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
    public string LoadedMaterialName { get; private set; }
    public int[] ListOfItemsFor  { get; private set; }
    public string RealGudHubURL { get; private set; }
    public string URLName { get; private set; }

    [SerializeField]
    private List<SettingForFieldsInLoadedMaterial> settingFieldList;
    
    public Material loadedMaterial { get; private set; }
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

    public void LoadMaterial()
    {
        if (AssetBundleInstance != null)
        {
            loadedMaterial = (Material)AssetBundleInstance.LoadAsset(AssetBundleInstance.GetAllAssetNames()[0]);
        }
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

    void IAssetBundleLoadable.BundleReady()
    {
        if (AssetBundleInstance == null)
        {
            string path = AssetBundleLoaderManager.Instance.AppPath;
            AssetBundleInstance = AssetBundle.LoadFromFile(path + URLName);            
        }
        LoadMaterial();
        EventHappend(AssetBundleLoaderManager.IAssetBundleLoadableEvent.BundleReady);
    }

    public void BundleHide()
    {

    }

    public void BundleShow()
    {

    }

    public void AddListenerLoading(AssetBundleLoaderManager.IAssetBundleLoadableEvent bundleEvent, AssetBundleLoaderManager.OnEventFunction onEventFunction)
    {
        if (EventDictionary == null)
        {
            InitEventDictionary();
        }
        EventDictionary[bundleEvent].Add(onEventFunction);
    }

    public void RemoveListenerLoading(AssetBundleLoaderManager.IAssetBundleLoadableEvent bundleEvent, AssetBundleLoaderManager.OnEventFunction onEventFunction)
    {
        if (EventDictionary == null)
        {
            InitEventDictionary();
        }
        EventDictionary[bundleEvent].Remove(onEventFunction);
    }

    public string GetURLName()
    {
        return URLName;
    }

    public string GetRealURL()
    {
        return RealGudHubURL;
    }

    public void OnMenuClick(MonoBehaviour itemInstance)
    {
        try
        {
            ((SceneChangebleObject)MenuManager.Instance.ObjectSelected).ChangeMaterialTo(loadedMaterial);
            Debug.Log("Try to change material!" + ((SceneChangebleObject)MenuManager.Instance.ObjectSelected).name);
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

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
