using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class 
/// </summary>
public enum SceneChangebleObjectTypes
{
    nameObject,
    typeObject,
    AssetBundleURL
}

/// <summary>
/// Serializeble class for start setting
/// </summary>
[System.Serializable]
public class SettingForFieldsInSceneChangebleObject : AbstractObjectConstructableComponentData<SceneChangebleObjectTypes> { }

/// <summary>
/// Class for changeble object on scene.
/// </summary>
[System.Serializable]
public class SceneChangebleObject : AbstractObjectConstructable <SceneChangebleObjectTypes>, IAssetBundleLoadeble, IClickable, IMenuClickable
{
    public string ChangebleObjectName { get; private set; }
    public string ChangebleObjectType { get; private set; }
    public string RealGudHubURL { get; private set; }
    public string URLName { get; private set; }

    [SerializeField]
    private List<SettingForFieldsInSceneChangebleObject> settingFieldList;

    private GameObject AssetGameObject;
    private AssetBundle AssetBundleInstance;

    #region public override functions

    /// <summary>
    /// Use this BEFORE initializating. This function make the inner cohesion in class instance 
    /// </summary>
    public override void InitDictionary()
    {
        FunctionsDictionary = new Dictionary<SceneChangebleObjectTypes, InitFunctions>();
        FunctionsDictionary.Add(SceneChangebleObjectTypes.nameObject, InitName);
        FunctionsDictionary.Add(SceneChangebleObjectTypes.AssetBundleURL, InitURL);
        FunctionsDictionary.Add(SceneChangebleObjectTypes.typeObject, InitTypeObject);

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

    /// <summary>
    /// Change GameObject material to current material
    /// </summary>
    /// <param name="material">the material to be changed</param>
    public void ChangeMaterialTo(Material material)
    {
        AssetGameObject.GetComponent<MeshRenderer>().material = material;
    }

    #endregion

    #region private functions

    private void SettingListFieldToRealFields()
    {
        ComponentsDataList = new List<AbstractObjectConstructableComponentData<SceneChangebleObjectTypes>>();
        for (int i = 0; i < settingFieldList.Count; i++)
        {
            ComponentsDataList.Add(new AbstractObjectConstructableComponentData<SceneChangebleObjectTypes>());
            ComponentsDataList[i].IdField = settingFieldList[i].IdField;
            ComponentsDataList[i].valueType = settingFieldList[i].valueType;
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
        ChangebleObjectName = ComponentsDataList[num].StringValue;
    }

    private void InitTypeObject(int num)
    {
        ChangebleObjectType = ComponentsDataList[num].StringValue;
    }

    private void LoadAssetBundleFromURL() // сейчас можно не использовать, а потом, когда будет написан нормальный регулятор загрузок, чтобы не было просаживания FPS, нужно будет использовать эту функцию
    {
        StartLoadAssetBundle();
    }

    #endregion

    #region interfaces

    /// <summary>
    /// Load assetbandle right now
    /// </summary>
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
            //Debug.Log(name + ";" + ChangebleObjectName + ";urlname=" + URLName + ";" + ComponentsDataList[2].StringValue);
            AssetBundleInstance = AssetBundle.LoadFromFile(path + URLName);
            AssetGameObject = Instantiate((GameObject)AssetBundleInstance.LoadAsset(AssetBundleInstance.GetAllAssetNames()[0]), this.transform);
            AssetGameObject.transform.localPosition = new Vector3(0, 0, 0);
        }
        BundleShow();
    }

    public void BundleHide()
    {
        if (AssetGameObject != null)
        {
            AssetGameObject.SetActive(false);
        }
    }

    public void BundleShow()
    {
        if (AssetGameObject != null)
        {
            AssetGameObject.SetActive(true);
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
        Vector3 tempPos = ((SceneChangebleObject)MenuManager.Instance.objectSelected).transform.position;
        MenuManager.Instance.ClickedOnClickable((SceneChangebleObject)itemInstance);
        ((SceneChangebleObject)itemInstance).transform.position = tempPos;
        ((SceneChangebleObject)itemInstance).BundleShow();
    }

    /// <summary>
    /// Get the object for show in menu, on click event
    /// </summary>
    /// <returns>the list of menu objects</returns>
    public List<MenuObject> GetListOfMenuObject()
    {
        List<MenuObject> returnedList = new List<MenuObject>();       

        //add the other item, like this item...
        List<SceneChangebleObject> tempSceneObjectChangeble = SceneLoaderManager.Instance.GetItemsLikeThat(this);
        for (int i = 0; i < tempSceneObjectChangeble.Count; i++)
        {
            if (tempSceneObjectChangeble[i] != this)
            {
                GameObject gameObjectMenuObject = new GameObject("MemuElementObject" + i.ToString());
                MenuObject menuObject = gameObjectMenuObject.AddComponent<MenuObject>();
                menuObject.SetMenuObject(MenuObject.TypeOfObject.firstLine, tempSceneObjectChangeble[i]);
                returnedList.Add(menuObject);
            }
        }

        //add the textures
        List<LoadedMaterial> tempLoadedMeterial = SceneLoaderManager.Instance.GetMaterialsForThat(this);
        for (int i = 0; i < tempLoadedMeterial.Count; i++)
        {
            GameObject gameObjectMenuObject = new GameObject("MemuElementMaterial" + i.ToString());
            MenuObject menuObject = gameObjectMenuObject.AddComponent<MenuObject>();
            menuObject.SetMenuObject(MenuObject.TypeOfObject.secondLine, tempLoadedMeterial[i]);
            returnedList.Add(menuObject);
        }

        return returnedList;
    }

    #endregion

}