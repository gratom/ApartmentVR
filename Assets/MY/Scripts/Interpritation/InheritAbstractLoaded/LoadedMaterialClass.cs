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

[SerializeField]
public class LoadedMaterialClass : AbstractObjectConstructable <LoadedMaterialClassTypes>, IAssetBundleLoadeble
{

    public int ID { get; private set; }
    public string LoadedMaterialName { get; private set; }
    public string[] ListOfItemsFor  { get; private set; }
    public string RealGudHubURL { get; private set; }
    public string URLName { get; private set; }

    private AssetBundle AssetBundleInstance; 

    #region public override functions

    public override void InitDictionary()
    {
        FunctionsDictionary = new Dictionary<LoadedMaterialClassTypes, InitFunctions>();
        FunctionsDictionary.Add(LoadedMaterialClassTypes.id, InitID);
        FunctionsDictionary.Add(LoadedMaterialClassTypes.nameMaterial, InitName);
        FunctionsDictionary.Add(LoadedMaterialClassTypes.AssetBundleURL, LoadAssetBundleFromURL);
        FunctionsDictionary.Add(LoadedMaterialClassTypes.ForItemsWithID, InitListOfItemsFor); 
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
#if UNITY_EDITOR
            //Debug.Log("Heh, another one item is initiated!");
#endif
        }
    }

    #endregion

    #region private functions

    private void LoadAssetBundleFromURL(int num)
    {
        RealGudHubURL = ComponentsDataList[num].StringValue;
        URLName = RealGudHubURL.Substring(RealGudHubURL.LastIndexOf('/'));
        StartLoadAssetBundle();
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
        ListOfItemsFor = ComponentsDataList[num].StringValue.Split(',');
    }

    #endregion

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
}
