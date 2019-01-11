using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniversalAssetBundleLoader;

/// <summary>
/// Class for managment of scene loading and dynamic creating and replacing items on this scene
/// </summary>
public class SceneLoaderManager : MonoBehaviour 
{

    /// <summary>
    /// Singleton
    /// </summary>
    public static SceneLoaderManager Instance { get; private set; }
    
    [Tooltip("This prefab used for replacing and spawning new items typeof SceneChangableObject")]
    [Header("Prefab for SceneChangableObjects")]
    [SerializeField]
    private GameObject sceneChangebleObjectPrefab;

    [Tooltip("This prefab used for replacing and spawning new items typeof LoadedMaterial")]
    [Header("Prefab for LoadedMaterials")]
    [SerializeField]
    private GameObject loadedMaterialPrefab;

    private List<AbstractObjectConstructable<SceneChangebleObjectTypes>> UncashedAppDataOfSceneItems;
    private List<AbstractObjectConstructable<LoadedMaterialClassTypes>> UncashedAppDataOfMaterials;
    private bool isInit = false;

    #region Unity functions

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
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
            if (((SceneChangebleObject)UncashedAppDataOfSceneItems[i]).ChangebleObjectType == item.ChangebleObjectType && ((SceneChangebleObject)UncashedAppDataOfSceneItems[i]).ChangebleObjectType != "1")
            {
                returnedList.Add(GetCopyOf((SceneChangebleObject)UncashedAppDataOfSceneItems[i]));
            }
        }
        return returnedList;
    }

    /// <summary>
    /// Get the list of materials, typeof(LoadedMaterial), that can be applyed to this item
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

    /// <summary>
    /// Returns the first suitable material for this object.
    /// </summary>
    /// <param name="item">Instance of item for searching material</param>
    /// <returns>Loaded material copy</returns>
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

    #region non-used function

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

    #endregion

    /// <summary>
    /// Spawn new <code>SceneChangableObject</code> in <paramref name="parent"/> position
    /// </summary>
    /// <param name="parent">The Transform in which the subject with <paramref name="ID"/> will be spawned</param>
    /// <param name="ID">ID of item, that will be spawned</param>
    /// <returns>The copy of original object with same ID</returns>
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

    /// <summary>
    /// Spawn new SceneChangableObject in <paramref name="parent">parent</paramref> position
    /// </summary>
    /// <param name="parent">The <c>UnityEngine.Transform</c> in which the subject with <paramref name="ID"/> will be spawned</param>
    /// <param name="originalSceneChangable">The original of the object from which the copy will be made</param>
    /// <returns>The copy of original object</returns>
    public SceneChangebleObject SpawnSceneChangableHere(Transform parent, SceneChangebleObject originalSceneChangable)
    {
        SceneChangebleObject sceneChangableCopy = GetCopyOf(originalSceneChangable);
        sceneChangableCopy.gameObject.transform.parent = parent;
        sceneChangableCopy.gameObject.transform.localPosition = new Vector3(0, 0, 0);
        sceneChangableCopy.gameObject.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
        sceneChangableCopy.RemoteAssetBundleInstance.AddDelegateToEvent(AbstractRemoteLoadable.RemoteLoadable<AssetBundle>.RemoteLoadableEvent.OnReady,
            new AbstractRemoteLoadable.RemoteLoadable<AssetBundle>.RemoteLoadableDelegate(x => sceneChangableCopy.SpawnBundle()));
        sceneChangableCopy.SpawnBundle();
        return sceneChangableCopy;
    }

    /// <summary>
    /// Return the full copy of <paramref name="originalSceneChangable">original</paramref> object
    /// </summary>
    /// <param name="originalSceneChangable">The original of the object from which the copy will be made</param>
    /// <returns>Correctly made copy</returns>
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
        sceneChangableCopy.RemoteAssetBundleInstance.CopyItemFrom(originalSceneChangable.RemoteAssetBundleInstance, AssetBundleLoaderManager.Instance);
        return sceneChangableCopy;
    }

    /// <summary>
    /// Return the full copy of <paramref name="originalLoadedMaterial">original</paramref> object
    /// </summary>
    /// <param name="originalLoadedMaterial">The original of the object from which the copy will be made</param>
    /// <returns>Correctly made copy</returns>
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
        loadedMaterialCopy.RemoteAssetBundleInstance.CopyItemFrom(originalLoadedMaterial.RemoteAssetBundleInstance, AssetBundleLoaderManager.Instance);
        return loadedMaterialCopy;
    }

    /// <summary>
    /// Load to current scene all SceneChangableObjects and respawning it.
    /// </summary>
    public void LoadSceneObjects()
    {
        #region find sceneChangableObjects
        SceneChangebleObject[] tempSceneChangeble;
        GameObject gPlaces = GameObject.FindGameObjectWithTag("Places");
        if (gPlaces)
        {
            tempSceneChangeble = gPlaces.transform.GetComponentsInChildren<SceneChangebleObject>();
        }
        else
        {
            tempSceneChangeble = null;
        }
        if (tempSceneChangeble == null || tempSceneChangeble.Length == 0)
        {
            Debug.LogError("Scene have not any changable objects!");
            return;
        }
        #endregion

        #region Replace all sceneChangableObjects
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
    
    /// <summary>
    /// Initiating this manager
    /// </summary>
    public void Initialize()
    {
        if (!isInit)
        {
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

            isInit = true;
        }
    }

    #endregion
}