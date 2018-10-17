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
    private List<AbstractObjectConstructable<LoadedMaterialClassTypes>> UncashedAppDataOfMateriasl;

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
            if (((SceneChangebleObject)UncashedAppDataOfSceneItems[i]).ChangebleObjectType == item.ChangebleObjectType)
            {
                returnedList.Add((SceneChangebleObject)UncashedAppDataOfSceneItems[i]);
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

        for (int i = 0; i < UncashedAppDataOfMateriasl.Count; i++)
        {
            for (int j = 0; j < ((LoadedMaterial)UncashedAppDataOfMateriasl[i]).ListOfItemsFor.Length; j++)
            {
                if (((LoadedMaterial)UncashedAppDataOfMateriasl[i]).ListOfItemsFor[j] == item.ID)
                {
                    returnedList.Add((LoadedMaterial)UncashedAppDataOfMateriasl[i]);
                }
            }
        }

        return returnedList;
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

    #endregion

    #region private functions

    private void Initialize()
    {
        //первыи делом получаем все предустановленные изменяемые объекты на сцене
        SceneChangebleObject[] tempSceneChangeble = GameObject.FindObjectsOfType<SceneChangebleObject>();
        for (int i = 0; i < tempSceneChangeble.Length; i++)
        {
            tempSceneChangeble[i].InitDictionary();
            tempSceneChangeble[i].InitConstruct();
        }
        //Uncashing the app data...

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
            //Debug.Log(UncashedAppDataOfSceneItems[j].name + ";" + ((SceneChangebleObject)UncashedAppDataOfSceneItems[j]).URLName + "__" + ((SceneChangebleObject)UncashedAppDataOfSceneItems[j]).ChangebleObjectName);
        }

        //теперь нужно найти элементы на сцене, которые являются ChangebleObject и поменять их на те, что только что инициализированны
        for (int i = 0; i < tempSceneChangeble.Length; i++)
        {//перебираем все предустановленные итемы
            for (int j = 0; j < UncashedAppDataOfSceneItems.Count; j++)
            {//перебираем все скачанные и инициализированные итемы
                if (tempSceneChangeble[i].ID == ((SceneChangebleObject)UncashedAppDataOfSceneItems[j]).ID)
                {//если находим соответствие, то заменяем старый предустановленный объект на новый (удаляя старый)
                    UncashedAppDataOfSceneItems[j].gameObject.transform.position = tempSceneChangeble[i].gameObject.transform.position;                    
                    Destroy(tempSceneChangeble[i].gameObject);
                    //грузим AssetBundle в новый объект
                    ((IAssetBundleLoadeble)UncashedAppDataOfSceneItems[j]).StartLoadAssetBundle();
                    break;
                }
            }
        }

        //второе приложение должно быть Materials
        UncashedAppDataOfMateriasl = new List<AbstractObjectConstructable<LoadedMaterialClassTypes>>();
        for (int i = 0; i < JSONMainManager.Instance.AppDataLoaderInstance.ListOfAppsSetting[1].AppData.items_list.Count; i++)
        {
            //создаем тут итемы, по количеству в App
            GameObject gTemp = Instantiate(loadedMaterialPrefab, this.transform);
            gTemp.name = "LoadedMaterial" + i.ToString();
            gTemp.transform.localPosition = new Vector3(0, 0, 0);
            UncashedAppDataOfMateriasl.Add(gTemp.GetComponent<LoadedMaterial>());
            UncashedAppDataOfMateriasl[i].InitDictionary();
        }   

        JSONMainManager.Instance.FillDataToList(UncashedAppDataOfMateriasl, JSONMainManager.Instance.AppDataLoaderInstance.ListOfAppsSetting[1].AppID);

        for (int i = 0; i < UncashedAppDataOfMateriasl.Count; i++)
        {            
            UncashedAppDataOfMateriasl[i].InitConstruct();
        }
        //wait for scene element is loaded and unpacked
        //TODO...
    }


    #endregion

    #region coroutines
    #endregion
}
