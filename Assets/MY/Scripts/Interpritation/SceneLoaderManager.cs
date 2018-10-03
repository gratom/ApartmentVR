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
    /// <param name="oldObject"></param>
    /// <param name="newObject"></param>
    public void ChangeObjectOnScene(SceneChangebleObject oldObject, SceneChangebleObject newObject)
    {

    }

    #endregion

    #region private functions

    private void Initialize()
    {
        //Uncashing the app data...

        //первое приложение должно быть SceneItems
        UncashedAppDataOfSceneItems = JSONMainManager.Instance.FillDataToList(UncashedAppDataOfSceneItems, JSONMainManager.Instance.AppDataLoaderInstance.ListOfAppsSetting[0].AppID);
        //второе приложение должно быть Materials
        UncashedAppDataOfMateriasl = JSONMainManager.Instance.FillDataToList(UncashedAppDataOfMateriasl, JSONMainManager.Instance.AppDataLoaderInstance.ListOfAppsSetting[1].AppID);

        for (int i = 0; i < UncashedAppDataOfSceneItems.Count; i++)
        {
            UncashedAppDataOfSceneItems[i].InitDictionary();
            UncashedAppDataOfSceneItems[i].InitConstruct();
        }

        for (int i = 0; i < UncashedAppDataOfMateriasl.Count; i++)
        {
            UncashedAppDataOfMateriasl[i].InitDictionary();
            UncashedAppDataOfMateriasl[i].InitConstruct();
        }
        //wait for scene element is loaded and unpacked
        //TODO...
    }


    #endregion

    #region coroutines
    #endregion
}
