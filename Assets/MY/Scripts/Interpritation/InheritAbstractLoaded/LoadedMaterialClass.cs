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

public class LoadedMaterialClass : AbstractObjectConstructable <LoadedMaterialClassTypes>, IAssetBundleLoadeble
{

    public int ID { get; private set; }
    public string LoadedMaterialName { get; private set; }
    public List<string> ListOfItemsFor  { get; private set; }
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
        //TO DO: сделать парсинг для определения принадлежности
    }

    #endregion

    public void StartLoadAssetBundle()
    {

    }
}
