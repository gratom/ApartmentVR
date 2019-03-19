using System.Collections.Generic;
using UnityEngine;
using MyVRMenu;
using UniversalAssetBundleLoader;

/// <summary>
/// Types of field in LoadedMaterial
/// </summary>
public enum LoadedMaterialClassTypes
{
    nameMaterial,
    AssetBundleURL,
    refToObjects,
    thumbnail,
    materialGroupReference
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
public class LoadedMaterial : AbstractObjectConstructable<LoadedMaterialClassTypes>
{
    /// <summary>
    /// Name of material, name of this item in GudHub
    /// </summary>
    public string LoadedMaterialName { get; private set; }

    /// <summary>
    /// List of items on which you can apply this material
    /// This list stores item IDs in "int" type
    /// </summary>
    public int[] ListOfItemsFor { get; private set; }

    /// <summary>
    /// Unique ID file in GudHub
    /// </summary>
    public string URLName { get; private set; }

    /// <summary>
    /// List of ID of material group in which this material added
    /// </summary>
    public int[] MaterialGroupsID { get; private set; }

    [Tooltip("This is a list of settings for the correct operation of the internal functions for initializing an item.\nThese settings are used to determine how to process data from JSON.")]
    public List<SettingForFieldsInLoadedMaterial> settingFieldList;

    /// <summary>
    /// Material that loaded from AssetBundle
    /// </summary>
    public Material loadedMaterial
    {
        get
        {
            return _loadedMaterial;
        }
        private set
        {
            _loadedMaterial = value;
        }
    }
    [SerializeField]
    private Material _loadedMaterial;

    /// <summary>
    /// AssetBundle instance, that contains the material
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

    #region public functions

    /// <summary>
    /// Use this BEFORE initializating. This function make the inner cohesion in class instance 
    /// </summary>
    public override void InitDictionary()
    {
        FunctionsDictionary = new Dictionary<LoadedMaterialClassTypes, InitFunctions>
        {
            { LoadedMaterialClassTypes.nameMaterial, InitName },
            { LoadedMaterialClassTypes.AssetBundleURL, InitURL },
            { LoadedMaterialClassTypes.refToObjects, InitListOfItemsFor },
            { LoadedMaterialClassTypes.materialGroupReference, InitMaterialGroup }
        };

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
        if (RemoteAssetBundleInstance.IsReady)
        {
            if (!loadedMaterial)
            {
                loadedMaterial = (Material)RemoteAssetBundleInstance.RemoteItemInstance.LoadAsset(RemoteAssetBundleInstance.RemoteItemInstance.GetAllAssetNames()[0]);
            }
        }
    }

    /// <summary>
    /// Function, that used for draw this item in menu
    /// </summary>
    /// <param name="menuItem"></param>
    public void OnMenuDraw(MenuItem menuItem)
    {
        LoadMaterial();
        if (loadedMaterial != null)
        {
            if (menuItem)
            {
                if (menuItem.GetComponent<MeshRenderer>() != null)
                {
                    menuItem.GetComponent<MeshRenderer>().material = loadedMaterial;
                }
            }
        }
        else
        {
            RemoteAssetBundleInstance.AddDelegateToEvent(AbstractRemoteLoadable.RemoteLoadable<AssetBundle>.RemoteLoadableEvent.OnReady,
                new AbstractRemoteLoadable.RemoteLoadable<AssetBundle>.RemoteLoadableDelegate(x => { OnMenuDraw(menuItem); }));
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
        sReturned += "ID of AssetBundle: " + RemoteAssetBundleInstance.Name + "\n";
        sReturned += "item reference: \n{\n";
        for (int i = 0; i < ListOfItemsFor.Length; i++)
        {
            sReturned += "    " + ListOfItemsFor[i] + ";\n";
        }
        sReturned += "}";
        return sReturned;
    }

    #endregion

    #region private functions

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
            RemoteAssetBundleInstance = new RemoteAssetBundle(
                JSONMainManager.Instance.GetRealFileURLById(ComponentsDataList[num].StringValue),
                ComponentsDataList[num].StringValue,
                JSONMainManager.Instance.GetRealItemURLByID(ID));
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
        if (ComponentsDataList[num] != null && ComponentsDataList[num].StringValue != null && ComponentsDataList[num].StringValue != "")
        {
            string[] temp = ComponentsDataList[num].StringValue.Split(',');
            ListOfItemsFor = new int[temp.Length];
            for (int i = 0; i < ListOfItemsFor.Length; i++)
            {
                ListOfItemsFor[i] = int.Parse(temp[i].Substring(temp[i].IndexOf('.') + 1));
            }
        }
        else
        {
            ListOfItemsFor = new int[0];
        }
    }

    private void InitMaterialGroup(int num)
    {
        if (ComponentsDataList[num] != null && ComponentsDataList[num].StringValue != null && ComponentsDataList[num].StringValue != "")
        {
            string[] temp = ComponentsDataList[num].StringValue.Split(',');
            MaterialGroupsID = new int[temp.Length];
            for (int i = 0; i < MaterialGroupsID.Length; i++)
            {
                MaterialGroupsID[i] = int.Parse(temp[i].Substring(temp[i].IndexOf('.') + 1));
            }
        }
        else
        {
            MaterialGroupsID = new int[0];
        }
    }

    #endregion

    #region interfaces

    /// <summary>
    /// Initiate loading assetBundle
    /// </summary>
    public void StartLoadAssetBundle()
    {
        RemoteAssetBundleInstance.AddDelegateToEvent(AbstractRemoteLoadable.RemoteLoadable<AssetBundle>.RemoteLoadableEvent.OnReady,
            new AbstractRemoteLoadable.RemoteLoadable<AssetBundle>.RemoteLoadableDelegate(x =>
            {
                LoadMaterial();
            }));
        RemoteAssetBundleInstance.StartLoad(AssetBundleLoaderManager.Instance);
    }

    /// <summary>
    /// The function called when item clicked on menu, usually it placed in delegate
    /// </summary>
    /// <param name="itemInstance">object, on which cliked</param>
    public void OnMenuClick(MonoBehaviour itemInstance)
    {
        try
        {
            LoadMaterial();
            if (loadedMaterial)
            {
                ((SceneChangeableObject)MenuManager.Instance.ObjectSelected).ChangeMaterialTo(loadedMaterial);
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
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
