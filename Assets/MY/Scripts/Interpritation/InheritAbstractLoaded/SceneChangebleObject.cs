using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MyVRMenu;

/// <summary>
/// Class 
/// </summary>
public enum SceneChangebleObjectTypes
{
    nameObject,
    typeObject,
    AssetBundleURL,
    PreImage
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
public class SceneChangebleObject : AbstractObjectConstructable <SceneChangebleObjectTypes>, IAssetBundleLoadable, ISceneClickable, IMenuClickable
{
    /// <summary>
    /// Its the name of object. The name is not unique
    /// </summary>
    public string ChangebleObjectName { get; private set; }

    /// <summary>
    /// The type of this object. ChangebleObject type is needed to determine what can be replaced
    /// </summary>
    public string ChangebleObjectType { get; private set; }

    /// <summary>
    /// This is URL for downloading assetbundle, that will be used
    /// </summary>
    public string RealGudHubURL { get; private set; }

    /// <summary>
    /// It is the name of assetbundle on disk
    /// </summary>
    public string URLName { get; private set; }

    /// <summary>
    /// Sprite image, that loaded before real model or texture was loaded
    /// </summary>
    public Sprite PreImage { get; private set; }

    /// <summary>
    /// Setting for initializing fields value
    /// </summary>
    [SerializeField]
    private List<SettingForFieldsInSceneChangebleObject> settingFieldList;

    [SerializeField]
    private GameObject AssetGameObject;
    public AssetBundle AssetBundleInstance { get; private set; }

    public int itemID { get { return ID; } }

    private bool IsBundleAlreadyLoading = false;

    private Dictionary<AssetBundleLoaderManager.IAssetBundleLoadableEvent, List<AssetBundleLoaderManager.OnEventFunction>> EventDictionary;

    #region public functions

    /// <summary>
    /// Use this BEFORE initializating. This function make the inner cohesion in class instance 
    /// </summary>
    public override void InitDictionary()
    {
        FunctionsDictionary = new Dictionary<SceneChangebleObjectTypes, InitFunctions>();
        FunctionsDictionary.Add(SceneChangebleObjectTypes.nameObject, InitName);
        FunctionsDictionary.Add(SceneChangebleObjectTypes.AssetBundleURL, InitURL);
        FunctionsDictionary.Add(SceneChangebleObjectTypes.typeObject, InitTypeObject);

        if (ComponentsDataList == null)
        {
            SettingListFieldToRealFields();
        }
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
        if (AssetGameObject != null)
        {
            AssetGameObject.GetComponent<MeshRenderer>().material = material;
        }
    }

    /// <summary>
    /// Return the real center of AssetGameObject
    /// </summary>
    /// <returns>center of AssetGameObject</returns>
    public Vector3 GetBundleCenter()
    {
        if(AssetGameObject != null)
        {
            #region debug

            GameObject cord1 = new GameObject("Cub center");
            cord1.transform.position = AssetGameObject.GetComponent<BoxCollider>().center;
            GameObject cord2 = new GameObject("Bound center");
            cord2.transform.position = AssetGameObject.GetComponent<Renderer>().bounds.center - AssetGameObject.transform.position;


            #endregion

            return AssetGameObject.GetComponent<BoxCollider>().center;
        }
        return new Vector3();
    }

    /// <summary>
    /// Destroy the BoxCollider component in AssetGameObject
    /// </summary>
    public void DestroyAssetCollider()
    {
        if (AssetGameObject != null)
        {
            if (AssetGameObject.GetComponent<BoxCollider>() != null)
            {
                Destroy(AssetGameObject.GetComponent<BoxCollider>());
            }
        }
    }

    /// <summary>
    /// Spawn the GameObject from AssetBundle
    /// </summary>
    public void SpawnBundle()
    {
        if (AssetBundleInstance != null && AssetGameObject == null) 
        {
            AssetGameObject = Instantiate((GameObject)AssetBundleInstance.LoadAsset(AssetBundleInstance.GetAllAssetNames()[0]), this.transform); //поворот бандла определяется здесь, так как сам ChangableObject не повернут относительно точки спавна
            AssetGameObject.transform.localPosition = new Vector3(0, 0, 0);
            LoadedMaterial tempLoadedMaterial = SceneLoaderManager.Instance.GetMaterialForThat(this);
            if (tempLoadedMaterial != null)
            {
                if (tempLoadedMaterial.AssetBundleInstance != null)
                {
                    MaterialReady(tempLoadedMaterial);
                }
                else
                {
                    tempLoadedMaterial.AddListenerLoading(AssetBundleLoaderManager.IAssetBundleLoadableEvent.BundleReady, MaterialReady);
                    tempLoadedMaterial.StartLoadAssetBundle();
                }
            }
        }      
    }

    /// <summary>
    /// Returns a nicely edited string with information about an object.
    /// </summary>
    /// <returns>Nicely edited string</returns>
    public override string ToString()
    {
        string sReturned = "";
        sReturned += "name on scene: " + name + "\n";
        sReturned += "gudhub name: " + ChangebleObjectName + "\n";
        sReturned += "gudhub item ID: " + ID + "\n";
        sReturned += "type of object: " + ChangebleObjectType.ToString() + "\n";
        sReturned += "ID of AssetBundle: " + URLName;
        return sReturned;
    }

    #endregion

    #region private functions

    #region init functions

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

    private void ItitPreImage(int num)
    {

    }

    private void InitName(int num)
    {
        ChangebleObjectName = ComponentsDataList[num].StringValue;
    }

    private void InitTypeObject(int num)
    {
        ChangebleObjectType = ComponentsDataList[num].StringValue;
    }

    #endregion    

    private void InitEventDictionary()
    {
        EventDictionary = new Dictionary<AssetBundleLoaderManager.IAssetBundleLoadableEvent, List<AssetBundleLoaderManager.OnEventFunction>>();
        for (int i = 0; i < Enum.GetNames(typeof(AssetBundleLoaderManager.IAssetBundleLoadableEvent)).Length; i++)
        {
            EventDictionary[(AssetBundleLoaderManager.IAssetBundleLoadableEvent)i] = new List<AssetBundleLoaderManager.OnEventFunction>();
        }
    }

    private void EventHappend(AssetBundleLoaderManager.IAssetBundleLoadableEvent eventType)
    {
        if (EventDictionary == null)
        {
            InitEventDictionary();
            return;
        }
        //create new list, for stabil work
        List<AssetBundleLoaderManager.OnEventFunction> tempList = new List<AssetBundleLoaderManager.OnEventFunction>();
        for (int i = 0; i < EventDictionary[eventType].Count; i++)
        {
            tempList.Add(EventDictionary[eventType][i]);
        }        
        for (int i = 0; i < tempList.Count; i++)
        {
            tempList[i](this);
        }
    }

    private void MaterialReady(IAssetBundleLoadable item)
    {
        ChangeMaterialTo(((LoadedMaterial)item).loadedMaterial);
        Destroy(((LoadedMaterial)item).gameObject);
    }    

    #endregion

    #region interfaces

    /// <summary>
    /// Load assetbandle right now
    /// </summary>
    public void StartLoadAssetBundle()
    {
        if (!IsBundleAlreadyLoading)
        {
            AssetBundleLoaderManager.Instance.AddToLoadable(this);
            IsBundleAlreadyLoading = true;
            EventHappend(AssetBundleLoaderManager.IAssetBundleLoadableEvent.BundleStartLoading);
        }
        if(AssetBundleInstance != null)
        {
            EventHappend(AssetBundleLoaderManager.IAssetBundleLoadableEvent.BundleReady);
        }
    }

    /// <summary>
    /// Event function, that will be called when asset bundle will be loaded
    /// </summary>
    void IAssetBundleLoadable.BundleReady()
    {
        if (AssetBundleInstance == null)
        {
            string path = AssetBundleLoaderManager.Instance.AppPath;
            AssetBundleCreateRequest aTemp = AssetBundle.LoadFromFileAsync(path + URLName);
            aTemp.completed += x =>
            {
                AssetBundleInstance = aTemp.assetBundle;
                if (AssetBundleInstance != null)
                {
                    EventHappend(AssetBundleLoaderManager.IAssetBundleLoadableEvent.BundleReady);
                }
                else
                {
                    Debug.LogError("On item " + ID.ToString() + " bundle is can`t loaded. Try to change this ->\n" + JSONMainManager.Instance.GetRealItemURLByID(ID).ToString());
                }
            };
        }
    }

    /// <summary>
    /// Hide the bundle object
    /// </summary>
    public void BundleHide()
    {
        if (AssetGameObject != null)
        {
            AssetGameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Show the bundle object
    /// </summary>
    public void BundleShow()
    {
        if (AssetGameObject != null)
        {
            AssetGameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Return the name of asset bundle on disk
    /// </summary>
    /// <returns>name of asset bundle on disk</returns>
    public string GetURLName()
    {
        return URLName;
    }

    /// <summary>
    /// Return the URL path for downloading the asset buncdle from GudHub
    /// </summary>
    /// <returns></returns>
    public string GetRealURL()
    {
        return RealGudHubURL;
    }

    /// <summary>
    /// The function called when item clicked on menu, usually it placed in delegate
    /// </summary>
    /// <param name="itemInstance">object, on which cliked</param>
    public void OnMenuClick(MonoBehaviour itemInstance)
    {
        //TODO
        //Spawn here new object
        SceneChangebleObject tempSceneChangable = SceneLoaderManager.Instance.SpawnSceneChangableHere(((SceneChangebleObject)MenuManager.Instance.ObjectSelected).gameObject.transform.parent, ((SceneChangebleObject)itemInstance).ID);
        tempSceneChangable.StartLoadAssetBundle();
        //Click on this new object

        Destroy(((SceneChangebleObject)MenuManager.Instance.ObjectSelected).gameObject);
        //Rework this later! 
        ClickManager.Instance.ImitateClick(tempSceneChangable.gameObject);
    }

    /// <summary>
    /// Call when menu item is point
    /// </summary>
    /// <param name="menuItem"></param>
    public void OnPointFunction(MenuItem menuItem, bool isPointed)
    {
        if (menuItem != null)
        {
            if (isPointed)
            {
                menuItem.gameObject.transform.Rotate(new Vector3(0, 50 * Time.deltaTime, 0)); //TODO : remove this
                menuItem.Illumination.PlayEffect();
            }
            else
            {
                menuItem.Illumination.StopEffect();
            }
        }
    }

    /// <summary>
    /// Get the object for show in menu, on click event
    /// </summary>
    /// <returns>the list of menu objects</returns>
    public List<MenuItem> GetListOfMenuObject()
    {
        List<MenuItem> returnedList = new List<MenuItem>();       

        //add the other item, like this item...
        List<SceneChangebleObject> tempSceneObjectChangeble = SceneLoaderManager.Instance.GetItemsLikeThat(this);
        for (int i = 0; i < tempSceneObjectChangeble.Count; i++)
        {            
            GameObject gameObjectMenuObject = new GameObject("MemuElementObject" + i.ToString());
            MenuItem menuObject = gameObjectMenuObject.AddComponent<MenuItem>();
            menuObject.SetMenuObject(MenuLine.TypeOfLine.firstLine, tempSceneObjectChangeble[i]);
            returnedList.Add(menuObject);
        }

        //add the textures
        List<LoadedMaterial> tempLoadedMeterial = SceneLoaderManager.Instance.GetMaterialsForThat(this);
        for (int i = 0; i < tempLoadedMeterial.Count; i++)
        {
            GameObject gameObjectMenuObject = new GameObject("MemuElementMaterial" + i.ToString());
            MenuItem menuObject = gameObjectMenuObject.AddComponent<MenuItem>();
            menuObject.SetMenuObject(MenuLine.TypeOfLine.secondLine, tempLoadedMeterial[i]);
            returnedList.Add(menuObject);
        }

        return returnedList;
    }

    /// <summary>
    /// Adds a listener to one of the events
    /// </summary>
    /// <param name="bundleEvent">Type of event</param>
    /// <param name="onEventFunction">Delegate functions</param>
    public void AddListenerLoading(AssetBundleLoaderManager.IAssetBundleLoadableEvent bundleEvent, AssetBundleLoaderManager.OnEventFunction onEventFunction)
    {
        if(EventDictionary == null)
        {
            InitEventDictionary();
        }
        EventDictionary[bundleEvent].Add(onEventFunction);
    }

    /// <summary>
    /// Remove listener in one of the events
    /// </summary>
    /// <param name="bundleEvent">Type of event</param>
    /// <param name="onEventFunction">Delegate function</param>
    public void RemoveListenerLoading(AssetBundleLoaderManager.IAssetBundleLoadableEvent bundleEvent, AssetBundleLoaderManager.OnEventFunction onEventFunction)
    {
        if (EventDictionary == null)
        {
            InitEventDictionary();
        }
        EventDictionary[bundleEvent].Remove(onEventFunction);
    }

    /// <summary>
    /// Copies an AssetBundle from an object, placing a link to it in its Instance. 
    /// If the parent AssetBundle is empty, it calls the AssetBundle loading method and subscribes to the load event.
    /// </summary>
    /// <param name="itemOriginal">Other object, that contain AssetBundle</param>
    public void CopyBundleFrom(IAssetBundleLoadable itemOriginal)
    {
        if (itemOriginal.AssetBundleInstance != null)
        {
            AssetBundleInstance = itemOriginal.AssetBundleInstance;
            EventHappend(AssetBundleLoaderManager.IAssetBundleLoadableEvent.BundleReady);
        }
        else
        {
            //wait for it...
            itemOriginal.AddListenerLoading(AssetBundleLoaderManager.IAssetBundleLoadableEvent.BundleReady, CopyBundleFrom);
            itemOriginal.StartLoadAssetBundle();
        }
    }

    #endregion

}