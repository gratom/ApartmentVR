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
    public static SceneLoaderManager Instance;

    private List<AbstractObjectConstructable<SceneChangebleObjectTypes>> UncashedAppDataOfSceneItems;
    private List<AbstractObjectConstructable<LoadedMaterialClassTypes>> UncashedAppDataOfMateriasl;

    #region Unity functions

    void Awake()
    {
        Initialize();
    }

    #endregion

    #region public functions

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

    public List<LoadedMaterial> GetMaterialsForThat()
    {
        List<LoadedMaterial> returnedList = new List<LoadedMaterial>();

        

        return returnedList;
    }

    #endregion

    #region private functions

    private void Initialize()
    {
        //Uncashing the app data...
        UncashedAppDataOfSceneItems = JSONMainManager.Instance.FillDataToList(UncashedAppDataOfSceneItems, JSONMainManager.Instance.AppDataLoaderInstance.ListOfAppsSetting[0].AppID); //TODO: change the index of app 
        UncashedAppDataOfMateriasl = JSONMainManager.Instance.FillDataToList(UncashedAppDataOfMateriasl, JSONMainManager.Instance.AppDataLoaderInstance.ListOfAppsSetting[1].AppID);

        for (int i = 0; i < UncashedAppDataOfSceneItems.Count; i++)
        {
            UncashedAppDataOfSceneItems[i].InitDictionary();
            UncashedAppDataOfSceneItems[i].InitConstruct();
        }


        //wait for scene element is loaded and unpacked
    }


    #endregion

    #region coroutines
    #endregion
}
