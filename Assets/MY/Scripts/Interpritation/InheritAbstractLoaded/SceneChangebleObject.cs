using System.Collections.Generic;
using UnityEngine;
using MyVRMenu;
using UniversalAssetBundleLoader;

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
public class SceneChangebleObject : AbstractObjectConstructable<SceneChangebleObjectTypes>, ISceneClickable, IMenuClickable
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
    public RemoteAssetBundle RemoteAssetBundleInstance
    {
        get
        {
            return _remoteAssetBundleInstance;
        }
        private set
        {
            _remoteAssetBundleInstance = value;
        }
    }
    [SerializeField]
    private RemoteAssetBundle _remoteAssetBundleInstance;

    public AbstractObjectConstructableComponentData<int>listOfRemote;

    public int itemID { get { return ID; } }    

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
        if (AssetGameObject != null)
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
            BoxCollider[] boxColliders = AssetGameObject.GetComponents<BoxCollider>();
            {
                for(int i = 0; i < boxColliders.Length; i++)
                Destroy(boxColliders[i]);
            }
        }
    }

    /// <summary>
    /// Spawn the GameObject from AssetBundle
    /// </summary>
    public void SpawnBundle()
    {
        if (RemoteAssetBundleInstance.IsReady && AssetGameObject == null)
        {
            AssetGameObject = Instantiate((GameObject)RemoteAssetBundleInstance.RemoteItemInstance.LoadAsset(RemoteAssetBundleInstance.RemoteItemInstance.GetAllAssetNames()[0]), this.transform);
            //поворот бандла определяется здесь, так как сам ChangableObject не повернут относительно точки спавна
            AssetGameObject.transform.localPosition = new Vector3(0, 0, 0);
            LoadedMaterial tempLoadedMaterial = SceneLoaderManager.Instance.GetMaterialForThat(this);
            if (tempLoadedMaterial != null)
            {
                tempLoadedMaterial.RemoteAssetBundleInstance.AddDelegateToEvent(AbstractRemoteLoadable.RemoteLoadable<AssetBundle>.RemoteLoadableEvent.OnReady,
                    new AbstractRemoteLoadable.RemoteLoadable<AssetBundle>.RemoteLoadableDelegate(x =>
                    {
                        tempLoadedMaterial.LoadMaterial();
                        MaterialReady(tempLoadedMaterial);
                    }));
                tempLoadedMaterial.RemoteAssetBundleInstance.StartLoad(AssetBundleLoaderManager.Instance);
            }
        }
    }

    /// <summary>
    /// Called on menu Draw
    /// </summary>
    /// <param name="menuItem">menu item, that called this function</param>
    public void OnMenuDraw(MenuItem menuItem)
    {
        if (menuItem)
        {
            //get the real size
            SpawnBundle();
            if (AssetGameObject != null)
            {
                DestroyAssetCollider();
                float r = 0.075f; //расстояние стороны куба, в который модель вписывается. Эту переменную стоит перенести в настройки меню

                MeshRenderer coolMesh = AssetGameObject.GetComponent<MeshRenderer>();
                Vector3 vmin = coolMesh.bounds.min;
                Vector3 vmax = coolMesh.bounds.max;

                float f = 0;
                if (f < vmax.x - vmin.x) { f = vmax.x - vmin.x; }
                if (f < vmax.y - vmin.y) { f = vmax.y - vmin.y; }
                if (f < vmax.z - vmin.z) { f = vmax.z - vmin.z; }
                float scale = f / r;
                AssetGameObject.transform.localScale /= scale;
                Vector3 vTemp = AssetGameObject.transform.position - coolMesh.bounds.center;
                AssetGameObject.transform.position += new Vector3(vTemp.x, vTemp.y + ((coolMesh.bounds.max.y - coolMesh.bounds.min.y) / 2), vTemp.z);
            }
            else
            {
                RemoteAssetBundleInstance.AddDelegateToEvent(AbstractRemoteLoadable.RemoteLoadable<AssetBundle>.RemoteLoadableEvent.OnReady,
                    new AbstractRemoteLoadable.RemoteLoadable<AssetBundle>.RemoteLoadableDelegate(x => { OnMenuDraw(menuItem); }));
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
            RemoteAssetBundleInstance = new RemoteAssetBundle(RealGudHubURL, URLName, JSONMainManager.Instance.GetRealItemURLByID(ID));
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

    private void MaterialReady(LoadedMaterial item)
    {
        //Debug.Log(URLName + "destr!  " + (item == null) + "\n" + item.loadedMaterial.name);
        ChangeMaterialTo(item.loadedMaterial);
        //Debug.Log("try to destroy : " + item.name);
        Destroy(item.gameObject);
        //Debug.Log("item destroyed : " + item.name);
    }

    #endregion

    #region interfaces

    /// <summary>
    /// Load assetbandle right now
    /// </summary>
    public void StartLoadAssetBundle()
    {
        RemoteAssetBundleInstance.StartLoad(AssetBundleLoaderManager.Instance);
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
        if (menuItem)
        {
            if (menuItem.Illumination)
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
            menuObject.onDrawFunction = tempSceneObjectChangeble[i].OnMenuDraw;
            returnedList.Add(menuObject);
        }

        //add the textures
        List<LoadedMaterial> tempLoadedMeterial = SceneLoaderManager.Instance.GetMaterialsForThat(this);
        for (int i = 0; i < tempLoadedMeterial.Count; i++)
        {
            GameObject gameObjectMenuObject = new GameObject("MemuElementMaterial" + i.ToString());
            MenuItem menuObject = gameObjectMenuObject.AddComponent<MenuItem>();
            menuObject.SetMenuObject(MenuLine.TypeOfLine.secondLine, tempLoadedMeterial[i]);
            menuObject.onDrawFunction = tempLoadedMeterial[i].OnMenuDraw;
            returnedList.Add(menuObject);
        }

        return returnedList;
    }

    public string getTypeClickable()
    {
        return ChangebleObjectType;
    }

    #endregion

}