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

    [Tooltip("This prefab used for spawning Materials Group")]
    [Header("Prefab for MaterialGroup")]
    [SerializeField]
    private GameObject materialGroupPrefab;

    [SerializeField]
    private bool IsReinit;

    [SerializeField]
    private bool ImportSetting;

    private List<AbstractObjectConstructable<SceneChangeableObjectTypes>> UncashedAppDataOfSceneItems;
    private List<AbstractObjectConstructable<LoadedMaterialClassTypes>> UncashedAppDataOfMaterials;
    private List<AbstractObjectConstructable<MaterialGroupClassTypes>> UncashedAppDataOfMaterialGroups;
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
    public List<SceneChangeableObject> GetItemsLikeThat(SceneChangeableObject item)
    {
        List<SceneChangeableObject> returnedList = new List<SceneChangeableObject>();
        if (item.ChangeableObjectType != "1") //TODO rework this!!!
        {
            for (int i = 0; i < UncashedAppDataOfSceneItems.Count; i++)
            {
                if (((SceneChangeableObject)UncashedAppDataOfSceneItems[i]).ChangeableObjectType == item.ChangeableObjectType)
                {
                    returnedList.Add(GetCopyOf((SceneChangeableObject)UncashedAppDataOfSceneItems[i]));
                }
            }
        }
        return returnedList;
    }

    /// <summary>
    /// Get list of materials group, typeof(MaterialsGroup), that can be applied to this item
    /// </summary>
    /// <param name="item">Instance of item for searching material group</param>
    /// <returns>List of Material Group, that can be applied to this item</returns>
    public List<MaterialGroup> GetMaterialGroupForThat(SceneChangeableObject item)
    {
        List<MaterialGroup> returnedList = new List<MaterialGroup>();
        if (item.MaterialGroupsID != null)
        {
            for (int i = 0; i < UncashedAppDataOfMaterialGroups.Count; i++)
            {
                for (int j = 0; j < item.MaterialGroupsID.Length; j++)
                {
                    if (UncashedAppDataOfMaterialGroups[i].ID == item.MaterialGroupsID[j])
                    {
                        returnedList.Add(GetCopyOf((MaterialGroup)UncashedAppDataOfMaterialGroups[i]));
                        break;
                    }
                }
            }
        }
        return returnedList;
    }

    /// <summary>
    /// Get the list of materials, typeof(LoadedMaterial), that can be applied to this item
    /// </summary>
    /// <param name="item">Instance of item for searching material</param>
    /// <returns>List of LoadedMaterial that can be applied to this item</returns>
    public List<LoadedMaterial> GetMaterialsForThat(SceneChangeableObject item)
    {
        List<LoadedMaterial> returnedList = new List<LoadedMaterial>();
        if (item.MaterialGroupsID != null)
        {
            for (int i = 0; i < UncashedAppDataOfMaterials.Count; i++)
            {
                for (int j = 0; j < ((LoadedMaterial)UncashedAppDataOfMaterials[i]).MaterialGroupsID.Length; j++)
                {
                    for (int n = 0; n < item.MaterialGroupsID.Length; n++)
                    {
                        if (((LoadedMaterial)UncashedAppDataOfMaterials[i]).MaterialGroupsID[j] == item.MaterialGroupsID[n])
                        {
                            returnedList.Add(GetCopyOf((LoadedMaterial)UncashedAppDataOfMaterials[i]));
                            i++;
                            j = 0;
                            break;
                        }
                    }
                    if (i >= UncashedAppDataOfMaterials.Count)
                    {
                        break;
                    }
                }
            }
        }
        return returnedList;
    }

    /// <summary>
    /// Get the list of materials belong unique material group, type(LoadedMaterial)
    /// </summary>
    /// <param name="materialGroupID">Unique ID of Material Group</param>
    /// <returns>List of LoadedMaterial belong unique material group id</returns>
    public List<LoadedMaterial> GetMaterialsForThat(int materialGroupID)
    {
        List<LoadedMaterial> returnedList = new List<LoadedMaterial>();
        for (int i = 0; i < UncashedAppDataOfMaterials.Count; i++)
        {
            for (int j = 0; j < ((LoadedMaterial)UncashedAppDataOfMaterials[i]).MaterialGroupsID.Length; j++)
            {
                if(((LoadedMaterial)UncashedAppDataOfMaterials[i]).MaterialGroupsID[j] == materialGroupID)
                {
                    returnedList.Add(GetCopyOf((LoadedMaterial)UncashedAppDataOfMaterials[i]));
                    break;
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
    public LoadedMaterial GetMaterialForThat(SceneChangeableObject item)
    {
        if (item.MaterialGroupsID != null)
        {
            for (int i = 0; i < UncashedAppDataOfMaterials.Count; i++)
            {
                for (int j = 0; j < ((LoadedMaterial)UncashedAppDataOfMaterials[i]).MaterialGroupsID.Length; j++)
                {
                    for (int n = 0; n < item.MaterialGroupsID.Length; n++)
                    {
                        if (((LoadedMaterial)UncashedAppDataOfMaterials[i]).MaterialGroupsID[j] == item.MaterialGroupsID[n])
                        {
                            return GetCopyOf((LoadedMaterial)UncashedAppDataOfMaterials[i]);
                        }
                    }
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Spawn new <code>SceneChangableObject</code> in <paramref name="parent"/> position
    /// </summary>
    /// <param name="parent">The Transform in which the subject with <paramref name="ID"/> will be spawned</param>
    /// <param name="ID">ID of item, that will be spawned</param>
    /// <returns>The copy of original object with same ID</returns>
    public SceneChangeableObject SpawnSceneChangableHere(Transform parent, int ID)
    {
        for (int i = 0; i < UncashedAppDataOfSceneItems.Count; i++)
        {
            if (UncashedAppDataOfSceneItems[i].ID == ID)
            {
                return SpawnSceneChangableHere(parent, (SceneChangeableObject)UncashedAppDataOfSceneItems[i]);
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
    public SceneChangeableObject SpawnSceneChangableHere(Transform parent, SceneChangeableObject originalSceneChangable)
    {
        SceneChangeableObject sceneChangableCopy = GetCopyOf(originalSceneChangable);
        sceneChangableCopy.gameObject.transform.parent = parent;
        sceneChangableCopy.gameObject.transform.localPosition = new Vector3(0, 0, 0);
        sceneChangableCopy.gameObject.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
        sceneChangableCopy.RemoteAssetBundleInstance.AddDelegateToEvent(AbstractRemoteLoadable.RemoteLoadable<AssetBundle>.RemoteLoadableEvent.OnReady,
            new AbstractRemoteLoadable.RemoteLoadable<AssetBundle>.RemoteLoadableDelegate(x => sceneChangableCopy.SpawnBundle()));
        sceneChangableCopy.SpawnBundle();
        return sceneChangableCopy;
    }

    /// <summary>
    /// Replace sceneChangebleObject to new same object.
    /// </summary>
    /// <param name="sceneChangebleObject">SceneChangebleObject, that will be replaced to new same object</param>
    public void Reinit(SceneChangeableObject sceneChangebleObject)
    {
        SceneChangeableObject tempSceneChangableObject = GetCopyOf(sceneChangebleObject.ID);
        if (tempSceneChangableObject != null)
        {
            sceneChangebleObject.ComponentsDataList = tempSceneChangableObject.ComponentsDataList;
            sceneChangebleObject.InitDictionary();
            sceneChangebleObject.InitConstruct();
            sceneChangebleObject.AttachInteractive();
            Destroy(tempSceneChangableObject.gameObject);
        }
        else
        {
            Debug.LogError("ID of " + sceneChangebleObject.name + " is not correct! It is not exist in database.");
        }
    }

    /// <summary>
    /// Return the full copy of sceneChangable with <paramref name="ID">of</paramref> object
    /// </summary>
    /// <param name="ID">The ID of original object from which the copy will be made</param>
    /// <returns>Correctly made copy</returns>
    public SceneChangeableObject GetCopyOf(int ID)
    {
        for (int i = 0; i < UncashedAppDataOfSceneItems.Count; i++)
        {
            if (UncashedAppDataOfSceneItems[i].ID == ID)
            {
                return GetCopyOf((SceneChangeableObject)UncashedAppDataOfSceneItems[i]);
            }
        }
        return null;
    }

    /// <summary>
    /// Return the full copy of <paramref name="originalSceneChangable">original</paramref> object
    /// </summary>
    /// <param name="originalSceneChangable">The original of the object from which the copy will be made</param>
    /// <returns>Correctly made copy</returns>
    public SceneChangeableObject GetCopyOf(SceneChangeableObject originalSceneChangable)
    {
        //сначала создаем объект и позиционируем его
        GameObject gTemp = new GameObject("CopyOf_" + originalSceneChangable.name);

        //теперь можно сделать копию SceneChangableObject
        SceneChangeableObject sceneChangableCopy = gTemp.AddComponent<SceneChangeableObject>();
        sceneChangableCopy.ComponentsDataList = originalSceneChangable.ComponentsDataList;
        sceneChangableCopy.ID = originalSceneChangable.ID;
        sceneChangableCopy.InitDictionary();
        sceneChangableCopy.InitConstruct();
        sceneChangableCopy.RemoteAssetBundleInstance.CopyItemFrom(originalSceneChangable.RemoteAssetBundleInstance, AssetBundleLoaderManager.Instance);
        if (sceneChangableCopy.RemoteAssetBundleThumbnailInstance != null)
        {
            sceneChangableCopy.RemoteAssetBundleThumbnailInstance.CopyItemFrom(originalSceneChangable.RemoteAssetBundleThumbnailInstance, AssetBundleLoaderManager.Instance);
        }
        return sceneChangableCopy;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="materialGroup"></param>
    /// <returns></returns>
    public MaterialGroup GetCopyOf(MaterialGroup originalMaterialGroup)
    {
        GameObject gTemp = new GameObject("CopyOf_" + originalMaterialGroup.name);

        MaterialGroup materialGroupCopy = gTemp.AddComponent<MaterialGroup>();
        materialGroupCopy.ComponentsDataList = originalMaterialGroup.ComponentsDataList;
        materialGroupCopy.ID = originalMaterialGroup.ID;
        materialGroupCopy.InitDictionary();
        materialGroupCopy.InitConstruct();

        return materialGroupCopy;
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
        SceneChangeableObject[] tempSceneChangeble;
        GameObject gPlaces = GameObject.FindGameObjectWithTag("Places");
        if (gPlaces)
        {
            tempSceneChangeble = gPlaces.transform.GetComponentsInChildren<SceneChangeableObject>();
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
                if (tempSceneChangeble[i].ID == ((SceneChangeableObject)UncashedAppDataOfSceneItems[j]).ID)
                {
                    if (IsReinit)
                    {
                        Reinit(tempSceneChangeble[i]);
                    }
                    else
                    {
                        SpawnSceneChangableHere(tempSceneChangeble[i].transform.parent, (SceneChangeableObject)UncashedAppDataOfSceneItems[j]);
                        Destroy(tempSceneChangeble[i].gameObject);
                    }
                    break;
                }
            }
        }
        StartCoroutine(WaitFor(2));
        #endregion
    }

    /// <summary>
    /// Initiating this manager
    /// </summary>
    public void Initialize()
    {
        if (!isInit)
        {

            #region Import setting from JSON

            if (ImportSetting)
            {

                #region Material

                int MaterialSettingIndex = 0;
                for (int i = 0; i < JSONMainManager.Instance.AppDataLoaderInstance.settingClassInstance.Apps.Count; i++)
                {
                    if (JSONMainManager.Instance.AppDataLoaderInstance.settingClassInstance.Apps[i].AppType == AppSetting.AppType.materials.ToString())
                    {
                        MaterialSettingIndex = i;
                        break;
                    }
                }

                for (int i = 0; i < loadedMaterialPrefab.GetComponent<LoadedMaterial>().settingFieldList.Count; i++)
                {
                    for (int j = 0; j < JSONMainManager.Instance.AppDataLoaderInstance.settingClassInstance.Apps[MaterialSettingIndex].Fields.Count; j++)
                    {
                        if (loadedMaterialPrefab.GetComponent<LoadedMaterial>().settingFieldList[i].valueType.ToString() == JSONMainManager.Instance.AppDataLoaderInstance.settingClassInstance.Apps[MaterialSettingIndex].Fields[j].Name)
                        {
                            loadedMaterialPrefab.GetComponent<LoadedMaterial>().settingFieldList[i].IdField = JSONMainManager.Instance.AppDataLoaderInstance.settingClassInstance.Apps[MaterialSettingIndex].Fields[j].ID;
                            break;
                        }
                    }
                }

                #endregion

                #region SceneChangable

                int SceneChangableSettingIndex = 0;
                for (int i = 0; i < JSONMainManager.Instance.AppDataLoaderInstance.settingClassInstance.Apps.Count; i++)
                {
                    if (JSONMainManager.Instance.AppDataLoaderInstance.settingClassInstance.Apps[i].AppType == AppSetting.AppType.objects.ToString())
                    {
                        SceneChangableSettingIndex = i;
                        break;
                    }
                }

                for (int i = 0; i < sceneChangebleObjectPrefab.GetComponent<SceneChangeableObject>().settingFieldList.Count; i++)
                {
                    for (int j = 0; j < JSONMainManager.Instance.AppDataLoaderInstance.settingClassInstance.Apps[SceneChangableSettingIndex].Fields.Count; j++)
                    {
                        if (sceneChangebleObjectPrefab.GetComponent<SceneChangeableObject>().settingFieldList[i].valueType.ToString() == JSONMainManager.Instance.AppDataLoaderInstance.settingClassInstance.Apps[SceneChangableSettingIndex].Fields[j].Name)
                        {
                            sceneChangebleObjectPrefab.GetComponent<SceneChangeableObject>().settingFieldList[i].IdField = JSONMainManager.Instance.AppDataLoaderInstance.settingClassInstance.Apps[SceneChangableSettingIndex].Fields[j].ID;
                            break;
                        }
                    }
                }

                #endregion

                #region Material Group
                int MaterialGroupSettingIndex = 0;
                for (int i = 0; i < JSONMainManager.Instance.AppDataLoaderInstance.settingClassInstance.Apps.Count; i++)
                {
                    if (JSONMainManager.Instance.AppDataLoaderInstance.settingClassInstance.Apps[i].AppType == AppSetting.AppType.materialGroup.ToString())
                    {
                        MaterialGroupSettingIndex = i;
                        break;
                    }
                }

                for (int i = 0; i < materialGroupPrefab.GetComponent<MaterialGroup>().settingFieldList.Count; i++)
                {
                    for (int j = 0; j < JSONMainManager.Instance.AppDataLoaderInstance.settingClassInstance.Apps[MaterialGroupSettingIndex].Fields.Count; j++)
                    {
                        if (materialGroupPrefab.GetComponent<MaterialGroup>().settingFieldList[i].valueType.ToString() == JSONMainManager.Instance.AppDataLoaderInstance.settingClassInstance.Apps[MaterialGroupSettingIndex].Fields[j].Name)
                        {
                            materialGroupPrefab.GetComponent<MaterialGroup>().settingFieldList[i].IdField = JSONMainManager.Instance.AppDataLoaderInstance.settingClassInstance.Apps[MaterialGroupSettingIndex].Fields[j].ID;
                            break;
                        }
                    }
                }
                #endregion

            }

            #endregion

            #region initializing LoadedMaterials

            int MaterialAppIndex = 0;
            for (int i = 0; i < JSONMainManager.Instance.AppDataLoaderInstance.ListOfAppsSetting.Count; i++)
            {
                if (JSONMainManager.Instance.AppDataLoaderInstance.ListOfAppsSetting[i].appType == AppSetting.AppType.materials)
                {
                    MaterialAppIndex = i;
                    break;
                }
            }

            UncashedAppDataOfMaterials = new List<AbstractObjectConstructable<LoadedMaterialClassTypes>>();
            for (int i = 0; i < JSONMainManager.Instance.AppDataLoaderInstance.ListOfAppsSetting[MaterialAppIndex].AppData.items_list.Count; i++)
            {
                //создаем тут итемы, по количеству в App
                GameObject gTemp = Instantiate(loadedMaterialPrefab, this.transform);
                gTemp.name = "LoadedMaterial" + i.ToString();
                gTemp.transform.localPosition = new Vector3(0, 0, 0);
                UncashedAppDataOfMaterials.Add(gTemp.GetComponent<LoadedMaterial>());
                UncashedAppDataOfMaterials[i].InitDictionary();
            }

            JSONMainManager.Instance.FillDataToList(UncashedAppDataOfMaterials, JSONMainManager.Instance.AppDataLoaderInstance.ListOfAppsSetting[MaterialAppIndex].AppID);

            for (int i = 0; i < UncashedAppDataOfMaterials.Count; i++)
            {
                UncashedAppDataOfMaterials[i].InitConstruct();
            }

            #endregion

            #region initializing SceneChangableObjects

            int sceneChangableIndex = 0;
            for (int i = 0; i < JSONMainManager.Instance.AppDataLoaderInstance.ListOfAppsSetting.Count; i++)
            {
                if (JSONMainManager.Instance.AppDataLoaderInstance.ListOfAppsSetting[i].appType == AppSetting.AppType.objects)
                {
                    sceneChangableIndex = i;
                    break;
                }
            }

            UncashedAppDataOfSceneItems = new List<AbstractObjectConstructable<SceneChangeableObjectTypes>>();
            for (int i = 0; i < JSONMainManager.Instance.AppDataLoaderInstance.ListOfAppsSetting[sceneChangableIndex].AppData.items_list.Count; i++)
            {
                //создаем тут итемы, по количеству в App
                GameObject gTemp = Instantiate(sceneChangebleObjectPrefab, this.transform);
                gTemp.name = "SceneChangeble" + i.ToString();
                gTemp.transform.localPosition = new Vector3(0, 0, 0);
                UncashedAppDataOfSceneItems.Add(gTemp.GetComponent<SceneChangeableObject>());
                UncashedAppDataOfSceneItems[i].InitDictionary();
            }
            //заполняем данными
            JSONMainManager.Instance.FillDataToList(UncashedAppDataOfSceneItems, JSONMainManager.Instance.AppDataLoaderInstance.ListOfAppsSetting[sceneChangableIndex].AppID);
            //данные внутри, там, где и должны быть у всех элементов.
            //инициализируем все элементы (пока что не грузим AssetBundle)
            for (int i = 0; i < UncashedAppDataOfSceneItems.Count; i++)
            {
                UncashedAppDataOfSceneItems[i].InitConstruct();
            }

            #endregion

            #region Initializing Material Group
            int MaterialGroupAppIndex = 0;
            for (int i = 0; i < JSONMainManager.Instance.AppDataLoaderInstance.ListOfAppsSetting.Count; i++)
            {
                if (JSONMainManager.Instance.AppDataLoaderInstance.ListOfAppsSetting[i].appType == AppSetting.AppType.materialGroup)
                {
                    MaterialGroupAppIndex = i;
                    break;
                }
            }

            UncashedAppDataOfMaterialGroups = new List<AbstractObjectConstructable<MaterialGroupClassTypes>>();
            for (int i = 0; i < JSONMainManager.Instance.AppDataLoaderInstance.ListOfAppsSetting[MaterialGroupAppIndex].AppData.items_list.Count; i++)
            {
                //создаем тут итемы, по количеству в App
                GameObject gTemp = Instantiate(materialGroupPrefab, this.transform);
                gTemp.name = "MaterialGroup" + i.ToString();
                gTemp.transform.localPosition = new Vector3(0, 0, 0);
                UncashedAppDataOfMaterialGroups.Add(gTemp.GetComponent<MaterialGroup>());
                UncashedAppDataOfMaterialGroups[i].InitDictionary();
            }

            JSONMainManager.Instance.FillDataToList(UncashedAppDataOfMaterialGroups, JSONMainManager.Instance.AppDataLoaderInstance.ListOfAppsSetting[MaterialGroupAppIndex].AppID);

            for (int i = 0; i < UncashedAppDataOfMaterialGroups.Count; i++)
            {
                UncashedAppDataOfMaterialGroups[i].InitConstruct();
            }
            #endregion

            #region GenerateTypesSettingFile

#if UNITY_EDITOR

            Debug.Log("Try to generate TypesSettingFile...");
            int typeObjectFieldID = UncashedAppDataOfSceneItems[0].GetComponent<SceneChangeableObject>().GetDataByType(SceneChangeableObjectTypes.typeObject).IdField;
            string sOptions = "";
            for (int i = 0; i < JSONMainManager.Instance.AppDataLoaderInstance.ListOfAppsSetting[sceneChangableIndex].AppData.field_list.Count; i++)
            {
                if (JSONMainManager.Instance.AppDataLoaderInstance.ListOfAppsSetting[sceneChangableIndex].AppData.field_list[i].field_id == typeObjectFieldID)
                {
                    for (int j = 0; j < JSONMainManager.Instance.AppDataLoaderInstance.ListOfAppsSetting[sceneChangableIndex].AppData.field_list[i].data_model.options.Count; j++)
                    {
                        sOptions += float.Parse(JSONMainManager.Instance.AppDataLoaderInstance.ListOfAppsSetting[sceneChangableIndex].AppData.field_list[i].data_model.options[j].value).ToString() + "." +
                            JSONMainManager.Instance.AppDataLoaderInstance.ListOfAppsSetting[sceneChangableIndex].AppData.field_list[i].data_model.options[j].name + ",";
                    }
                    Debug.Log("Successfully generated TypesSettingFile.");
                    break;
                }
            }
            SaverLoaderModule.SaveMyDataToFile("typesSetting.config", sOptions);

            Debug.Log("Try to generate ObjectCasheFile...");
            ListOfSceneChangeableObjectCashed Box = new ListOfSceneChangeableObjectCashed();
            Box.sceneChangeableObjectCasheds = new List<SceneChangeableObjectCashed>();
            for (int i = 0; i < UncashedAppDataOfSceneItems.Count; i++)
            {
                Box.sceneChangeableObjectCasheds.Add(new SceneChangeableObjectCashed(
                    UncashedAppDataOfSceneItems[i].GetComponent<SceneChangeableObject>().ChangeableObjectType,
                    UncashedAppDataOfSceneItems[i].GetComponent<SceneChangeableObject>().ChangeableObjectName,
                    UncashedAppDataOfSceneItems[i].GetComponent<SceneChangeableObject>().ID));
            }
            SaverLoaderModule.SaveMyDataToFile("ObjectCashe.config", JsonUtility.ToJson(Box));
#endif

            #endregion

            isInit = true;
        }
    }

    #endregion

    #region Coroutines

    private IEnumerator WaitFor(float second)
    {
        yield return new WaitForSeconds(second);
        MyVRMenu.MenuManager.Instance.Initialize();
        if (UnityEngine.XR.XRDevice.isPresent)
        {
            MyPlayerControllers.PlayerManager.Instance.SpawnNewPlayerController(MyPlayerControllers.BasePlayerControllerContainer.PlayerControllerType.VRPlayerController);
            UnityEngine.XR.XRSettings.enabled = true;
        }
        else
        {
            MyPlayerControllers.PlayerManager.Instance.SpawnNewPlayerController(MyPlayerControllers.BasePlayerControllerContainer.PlayerControllerType.DefaultPlayerController);
        }
    }

    #endregion

}