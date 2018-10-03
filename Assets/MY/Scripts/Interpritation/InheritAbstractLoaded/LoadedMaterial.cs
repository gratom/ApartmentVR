using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LoadedMaterialClassTypes
{
    id,
    nameMaterial,
    AssetBundleURL,
    ForItemsWithID
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
public class LoadedMaterial : AbstractObjectConstructable <LoadedMaterialClassTypes>, IAssetBundleLoadeble
{

    public int ID { get; private set; }
    public string LoadedMaterialName { get; private set; }
    public int[] ListOfItemsFor  { get; private set; }
    public string RealGudHubURL { get; private set; }
    public string URLName { get; private set; }

    [SerializeField]
    private List<SettingForFieldsInLoadedMaterial> settingFieldList;

    private AssetBundle AssetBundleInstance;

    void Awake()
    {
        //Test1();
    }

    #region public override functions

    /// <summary>
    /// Use this BEFORE initializating. This function make the inner cohesion in class instance 
    /// </summary>
    public override void InitDictionary()
    {
        FunctionsDictionary = new Dictionary<LoadedMaterialClassTypes, InitFunctions>();
        FunctionsDictionary.Add(LoadedMaterialClassTypes.id, InitID);
        FunctionsDictionary.Add(LoadedMaterialClassTypes.nameMaterial, InitName);
        FunctionsDictionary.Add(LoadedMaterialClassTypes.AssetBundleURL, LoadAssetBundleFromURL);
        FunctionsDictionary.Add(LoadedMaterialClassTypes.ForItemsWithID, InitListOfItemsFor); 
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
#if UNITY_EDITOR
            //Debug.Log("Heh, another one item is initiated!");
#endif
        }
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

    private void LoadAssetBundleFromURL(int num)
    {
        RealGudHubURL = ComponentsDataList[num].StringValue;
        URLName = RealGudHubURL.Substring(RealGudHubURL.LastIndexOf('/'));
    }

    private void InitName(int num)
    {
        LoadedMaterialName = ComponentsDataList[num].StringValue;
    }

    private void InitID(int num)
    {
        try
        {
            ID = int.Parse(ComponentsDataList[num].StringValue);
        }
        catch (System.Exception e)
        {
            Debug.Log(this.ToString() + e);
        }
    }

    private void InitListOfItemsFor(int num)
    {
        string[] temp = ComponentsDataList[num].StringValue.Split(',');
        for (int i = 0; i < ListOfItemsFor.Length; i++)
        {
            ListOfItemsFor[i] = int.Parse(temp[i].Substring(temp[i].IndexOf('.') + 1));
        }
    }

    #endregion

    #region interfaces

    public void StartLoadAssetBundle()
    {
        string path = Application.dataPath;
        path = path.Substring(0, path.LastIndexOf('/'));
        path += AssetBundleLoaderManager.Instance.Setting.DataStoragePath;
        AssetBundleInstance = AssetBundle.LoadFromFile(path + URLName);
        if (this.gameObject.GetComponent<MeshRenderer>() == null)
        {
            this.gameObject.AddComponent<MeshRenderer>();
        }
        this.gameObject.GetComponent<MeshRenderer>().material = (Material)AssetBundleInstance.LoadAsset(AssetBundleInstance.GetAllAssetNames()[0]);
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
