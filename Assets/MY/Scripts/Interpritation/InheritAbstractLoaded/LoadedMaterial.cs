using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LoadedMaterialClassTypes
{
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
public class LoadedMaterial : AbstractObjectConstructable <LoadedMaterialClassTypes>, IAssetBundleLoadeble, IMenuClickable
{
    public string LoadedMaterialName { get; private set; }
    public int[] ListOfItemsFor  { get; private set; }
    public string RealGudHubURL { get; private set; }
    public string URLName { get; private set; }

    [SerializeField]
    private List<SettingForFieldsInLoadedMaterial> settingFieldList;

    private AssetBundle AssetBundleInstance;

    #region Unity functions

    void Awake()
    {
        //Test1();
    }

    #endregion

    #region public override functions

    /// <summary>
    /// Use this BEFORE initializating. This function make the inner cohesion in class instance 
    /// </summary>
    public override void InitDictionary()
    {
        FunctionsDictionary = new Dictionary<LoadedMaterialClassTypes, InitFunctions>();
        FunctionsDictionary.Add(LoadedMaterialClassTypes.nameMaterial, InitName);
        FunctionsDictionary.Add(LoadedMaterialClassTypes.AssetBundleURL, InitURL);
        FunctionsDictionary.Add(LoadedMaterialClassTypes.ForItemsWithID, InitListOfItemsFor);

        SettingListFieldToRealFields();
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

    #region interfaces

    public void StartLoadAssetBundle()
    {
        if (AssetBundleInstance == null)
        {
            AssetBundleLoaderManager.Instance.AddToLoadeble(this);
        }        
        BundleShow();
    }

    void IAssetBundleLoadeble.BundleReady()
    {
        if (AssetBundleInstance == null)
        {
            string path = AssetBundleLoaderManager.Instance.AppPath;
            AssetBundleInstance = AssetBundle.LoadFromFile(path + URLName);
            if (this.gameObject.GetComponent<MeshRenderer>() == null)
            {
                this.gameObject.AddComponent<MeshRenderer>();
            }
            this.gameObject.GetComponent<MeshRenderer>().material = (Material)AssetBundleInstance.LoadAsset(AssetBundleInstance.GetAllAssetNames()[0]);
        }
        BundleShow();
    }

    public void BundleHide()
    {
        if (this.gameObject.GetComponent<MeshRenderer>() != null)
        {
            this.gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    public void BundleShow()
    {
        if (this.gameObject.GetComponent<MeshRenderer>() != null)
        {
            this.gameObject.GetComponent<MeshRenderer>().enabled = true;
        }
    }

    public string GetURLName()
    {
        return URLName;
    }

    public string GetRealURL()
    {
        return RealGudHubURL;
    }

    public void onMenuClick(MonoBehaviour itemInstance)
    {
        try
        {
            ((SceneChangebleObject)MenuManager.Instance.objectSelected).ChangeMaterialTo(itemInstance.GetComponent<MeshRenderer>().material);
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
