using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for managment of scene loading and dynamic creating and replacing items on this scene
/// </summary>
public class SceneLoaderManager : MonoBehaviour 
{

    /// <summary>
    /// Singleton
    /// </summary>
    public static SceneLoaderManager Instance { get; private set; }

    public GameObject sceneChangebleObjectPrefab;
    public GameObject loadedMaterialPrefab;

    private List<AbstractObjectConstructable<SceneChangebleObjectTypes>> UncashedAppDataOfSceneItems;
    private List<AbstractObjectConstructable<LoadedMaterialClassTypes>> UncashedAppDataOfMaterials;

    #region Unity functions

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        Initialize();
    }

    #endregion

    #region public functions

    /// <summary>
    /// Get the list of items typeof(SceneChangebleObject) that have same type
    /// </summary>
    /// <param name="item">Instance of item for comparison</param>
    /// <returns>List of items with same type like input item</returns>
    public List<SceneChangebleObject> GetItemsLikeThat(SceneChangebleObject item)
    {
        List<SceneChangebleObject> returnedList = new List<SceneChangebleObject>();

        for (int i = 0; i < UncashedAppDataOfSceneItems.Count; i++)
        {
            if (((SceneChangebleObject)UncashedAppDataOfSceneItems[i]).ChangebleObjectType == item.ChangebleObjectType && ((SceneChangebleObject)UncashedAppDataOfSceneItems[i]).ChangebleObjectType != "2")
            {
                returnedList.Add(GetCopyOf((SceneChangebleObject)UncashedAppDataOfSceneItems[i]));
            }
        }
        return returnedList;
    }

    /// <summary>
    /// Get the list of material, typeof(LoadedMaterial), that can be applyed to this item
    /// </summary>
    /// <param name="item">Instance of item for searching material</param>
    /// <returns>List of LoadedMaterial that can be applyed to this item</returns>
    public List<LoadedMaterial> GetMaterialsForThat(SceneChangebleObject item)
    {
        List<LoadedMaterial> returnedList = new List<LoadedMaterial>();

        for (int i = 0; i < UncashedAppDataOfMaterials.Count; i++)
        {
            for (int j = 0; j < ((LoadedMaterial)UncashedAppDataOfMaterials[i]).ListOfItemsFor.Length; j++)
            {
                if (((LoadedMaterial)UncashedAppDataOfMaterials[i]).ListOfItemsFor[j] == item.ID)
                {
                    returnedList.Add(GetCopyOf((LoadedMaterial)UncashedAppDataOfMaterials[i]));
                }
            }
        }
        return returnedList;
    }

    public LoadedMaterial GetMaterialForThat(SceneChangebleObject item)
    {
        for (int i = 0; i < UncashedAppDataOfMaterials.Count; i++)
        {
            for (int j = 0; j < ((LoadedMaterial)UncashedAppDataOfMaterials[i]).ListOfItemsFor.Length; j++)
            {
                if (((LoadedMaterial)UncashedAppDataOfMaterials[i]).ListOfItemsFor[j] == item.ID)
                {
                    return GetCopyOf((LoadedMaterial)UncashedAppDataOfMaterials[i]);
                }
            }
        }
        return null;        
    }

    /// <summary>
    /// Change the old object on scene to new object. Replace thoose two items. old object not will be destroed
    /// </summary>
    /// <param name="oldObject">old object, that will be deleted</param>
    /// <param name="newObject">new object, that will be placed</param>
    public void ChangeObjectOnScene(SceneChangebleObject oldObject, SceneChangebleObject newObject)
    {
        Vector3 tempPos = oldObject.gameObject.transform.position;
        oldObject.gameObject.transform.position = newObject.gameObject.transform.position;
        newObject.gameObject.transform.position = tempPos;
    }

    public SceneChangebleObject SpawnSceneChangableHere(Transform parent, int ID)
    {
        for(int i = 0; i < UncashedAppDataOfSceneItems.Count; i++)
        {
            if(UncashedAppDataOfSceneItems[i].ID == ID)
            {
                return SpawnSceneChangableHere(parent, (SceneChangebleObject)UncashedAppDataOfSceneItems[i]);
            }
        }
        return null;
    }

    public SceneChangebleObject SpawnSceneChangableHere(Transform parent, SceneChangebleObject originalSceneChangable)
    {
        SceneChangebleObject sceneChangableCopy = GetCopyOf(originalSceneChangable);
        sceneChangableCopy.gameObject.transform.parent = parent;
        sceneChangableCopy.gameObject.transform.localPosition = new Vector3(0, 0, 0);
        sceneChangableCopy.gameObject.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
        ((IAssetBundleLoadable)sceneChangableCopy).AddListenerLoading(AssetBundleLoaderManager.IAssetBundleLoadableEvent.BundleReady, 
            new AssetBundleLoaderManager.OnEventFunction(x => sceneChangableCopy.SpawnBundle()));
        return sceneChangableCopy;
    }

    public SceneChangebleObject GetCopyOf(SceneChangebleObject originalSceneChangable)
    {
        //сначала создаем объект и позиционируем его
        GameObject gTemp = new GameObject("CopyOf_" + originalSceneChangable.name);

        //теперь можно сделать копию SceneChangableObject
        SceneChangebleObject sceneChangableCopy = gTemp.AddComponent<SceneChangebleObject>();
        sceneChangableCopy.ComponentsDataList = originalSceneChangable.ComponentsDataList;
        sceneChangableCopy.ID = originalSceneChangable.ID;
        sceneChangableCopy.InitDictionary();
        sceneChangableCopy.InitConstruct();
        ((IAssetBundleLoadable)sceneChangableCopy).CopyBundleFrom(originalSceneChangable);
        return sceneChangableCopy;
    }

    public LoadedMaterial GetCopyOf(LoadedMaterial originalLoadedMaterial)
    {
        //сначала создаем объект и позиционируем его
        GameObject gTemp = new GameObject("CopyOf_" + originalLoadedMaterial.name);       

        //теперь можно сделать копию LoadedMaterialObject
        LoadedMaterial loadedMaterialCopy = gTemp.AddComponent<LoadedMaterial>();
        loadedMaterialCopy.ComponentsDataList = originalLoadedMaterial.ComponentsDataList;
        loadedMaterialCopy.ID = originalLoadedMaterial.ID;
        loadedMaterialCopy.InitDictionary();
        loadedMaterialCopy.InitConstruct();
        ((IAssetBundleLoadable)loadedMaterialCopy).CopyBundleFrom(originalLoadedMaterial);
        return loadedMaterialCopy;
    }

    #endregion

    #region private functions

    private void SpawnOnLoadEventFunction(IAssetBundleLoadable item)
    {

    }

    private void Initialize()
    {

        //первым делом получаем все предустановленные изменяемые объекты на сцене
        SceneChangebleObject[] tempSceneChangeble = FindObjectsOfType<SceneChangebleObject>();

        #region initializing LoadedMaterials

        //второе приложение должно быть Materials
        UncashedAppDataOfMaterials = new List<AbstractObjectConstructable<LoadedMaterialClassTypes>>();
        for (int i = 0; i < JSONMainManager.Instance.AppDataLoaderInstance.ListOfAppsSetting[1].AppData.items_list.Count; i++)
        {
            //создаем тут итемы, по количеству в App
            GameObject gTemp = Instantiate(loadedMaterialPrefab, this.transform);
            gTemp.name = "LoadedMaterial" + i.ToString();
            gTemp.transform.localPosition = new Vector3(0, 0, 0);
            UncashedAppDataOfMaterials.Add(gTemp.GetComponent<LoadedMaterial>());
            UncashedAppDataOfMaterials[i].InitDictionary();
        }

        JSONMainManager.Instance.FillDataToList(UncashedAppDataOfMaterials, JSONMainManager.Instance.AppDataLoaderInstance.ListOfAppsSetting[1].AppID);

        for (int i = 0; i < UncashedAppDataOfMaterials.Count; i++)
        {
            UncashedAppDataOfMaterials[i].InitConstruct();
        }

        #endregion

        #region initializing SceneChangableObjects

        //первое приложение должно быть SceneItems
        UncashedAppDataOfSceneItems = new List<AbstractObjectConstructable<SceneChangebleObjectTypes>>();
        for (int i = 0; i < JSONMainManager.Instance.AppDataLoaderInstance.ListOfAppsSetting[0].AppData.items_list.Count; i++)
        {
            //создаем тут итемы, по количеству в App
            GameObject gTemp = Instantiate(sceneChangebleObjectPrefab, this.transform);
            gTemp.name = "SceneChangeble" + i.ToString();
            gTemp.transform.localPosition = new Vector3(0, 0, 0);
            UncashedAppDataOfSceneItems.Add(gTemp.GetComponent<SceneChangebleObject>());
            UncashedAppDataOfSceneItems[i].InitDictionary();
        }
        //заполняем данными
        JSONMainManager.Instance.FillDataToList(UncashedAppDataOfSceneItems, JSONMainManager.Instance.AppDataLoaderInstance.ListOfAppsSetting[0].AppID);
        //данные внутри, там, где и должны быть у всех элементов.
        //инициализируем все элементы (пока что не грузим AssetBundle)
        for (int i = 0; i < UncashedAppDataOfSceneItems.Count; i++)
        {
            UncashedAppDataOfSceneItems[i].InitConstruct();
        }

        #endregion

        #region replace SceneChangable
        
        for (int i = 0; i < tempSceneChangeble.Length; i++)
        {//перебираем все предустановленные итемы
            for (int j = 0; j < UncashedAppDataOfSceneItems.Count; j++)
            {//перебираем все скачанные и инициализированные итемы
                if (tempSceneChangeble[i].ID == ((SceneChangebleObject)UncashedAppDataOfSceneItems[j]).ID)
                {
                    SpawnSceneChangableHere(tempSceneChangeble[i].transform.parent, (SceneChangebleObject)UncashedAppDataOfSceneItems[j]);
                    Destroy(tempSceneChangeble[i].gameObject);
                    break;
                }
            }
        }

        #endregion

    }

    #endregion
}