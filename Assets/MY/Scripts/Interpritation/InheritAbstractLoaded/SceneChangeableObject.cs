using System.Collections.Generic;
using UnityEngine;
using MyVRMenu;
using UniversalAssetBundleLoader;

/// <summary>
/// Class
/// </summary>
public enum SceneChangeableObjectTypes
{
    nameObject,
    typeObject,
    AssetBundleURL,
    thumbnail,
    materialGroupReference
}

/// <summary>
/// Serializeble class for start setting
/// </summary>
[System.Serializable]
public class SettingForFieldsInSceneChangebleObject : AbstractObjectConstructableComponentData<SceneChangeableObjectTypes> { }

/// <summary>
/// Class for changeble object on scene.
/// </summary>
[System.Serializable]
public class SceneChangeableObject : AbstractObjectConstructable<SceneChangeableObjectTypes>, ISceneClickable
{
    /// <summary>
    /// Its the name of object. The name is not unique
    /// </summary>
    [SerializeField]
    public string ChangeableObjectName;

    /// <summary>
    /// The type of this object. ChangebleObject type is needed to determine what can be replaced
    /// </summary>
    [SerializeField]
    public string ChangeableObjectType;

    /// <summary>
    /// List of ID of material group, that can apply to this sceneChangeable
    /// </summary>
    public int[] MaterialGroupsID { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    public int CurrentGroupMaterialID { get; private set; }

    /// <summary>
    /// Setting for initializing fields value
    /// </summary>
    [SerializeField]
    public List<SettingForFieldsInSceneChangebleObject> settingFieldList;

    [SerializeField]
    private GameObject AssetGameObject;

    /// <summary>
    /// Remote assetbundle of big object model
    /// </summary>
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

    /// <summary>
    /// Remote assetbundle of mitiature of big object model
    /// </summary>
    public RemoteAssetBundle RemoteAssetBundleThumbnailInstance
    {
        get
        {
            return _RemoteAssetBundleThumbnailInstance;
        }
        private set
        {
            _RemoteAssetBundleThumbnailInstance = value;
        }
    }
    [SerializeField]
    private RemoteAssetBundle _RemoteAssetBundleThumbnailInstance;

    /// <summary>
    /// 
    /// </summary>
    public AbstractObjectConstructableComponentData<int> listOfRemote;


    #region public functions

    /// <summary>
    /// Use this BEFORE initializating. This function make the inner cohesion in class instance 
    /// </summary>
    public override void InitDictionary()
    {
        FunctionsDictionary = new Dictionary<SceneChangeableObjectTypes, InitFunctions>()
        {
            { SceneChangeableObjectTypes.nameObject, InitName },
            { SceneChangeableObjectTypes.AssetBundleURL, InitURL },
            { SceneChangeableObjectTypes.typeObject, InitTypeObject },
            { SceneChangeableObjectTypes.thumbnail, InitThumbnail },
            { SceneChangeableObjectTypes.materialGroupReference, InitMaterialGroup }
        };

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
                for (int i = 0; i < boxColliders.Length; i++)
                    Destroy(boxColliders[i]);
            }
        }
    }

    /// <summary>
    /// Adds the InteractiveObject to own AssetGameObject, if it exist
    /// </summary>
    public void AttachInteractive()
    {
        if (AssetGameObject != null)
        {
            if (AssetGameObject.GetComponent<InteractiveObject>() != null)
            {
                AssetGameObject.GetComponent<InteractiveObject>().OnActiveAction = () =>
                {
                    MenuManager.Instance.ClickedOnClickable(this);
                };
            }
            else
            {
                AssetGameObject.AddComponent<InteractiveObject>().OnActiveAction = () =>
                {
                    MenuManager.Instance.ClickedOnClickable(this);
                };
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
            AttachInteractive();
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
    public void onDrawFunction(MenuItem menuItem)
    {
        if (menuItem)
        {
            if (RemoteAssetBundleThumbnailInstance != null)
            {
                SpawnThumbnail();
            }
            else
            {
                SpawnBundle();
            }

            if (AssetGameObject != null)
            {
                DestroyAssetCollider();

                Quaternion tempQuaternion = AssetGameObject.transform.rotation;
                AssetGameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));

                MeshRenderer coolMesh = AssetGameObject.GetComponent<MeshRenderer>();
                Vector3 vmin = coolMesh.bounds.min;
                Vector3 vmax = coolMesh.bounds.max;

                float f = 0;
                if (f < vmax.x - vmin.x) { f = vmax.x - vmin.x; }
                if (f < vmax.y - vmin.y) { f = vmax.y - vmin.y; }
                //if (f < vmax.z - vmin.z) { f = vmax.z - vmin.z; }
                //так как все модели после 3D-макса криво повернуты, то высота в моделях меряется не по Y-координате, а по Z-координате.
                float scale = f / menuItem.MenuItemSize;
                AssetGameObject.transform.localScale /= scale;
                AssetGameObject.transform.rotation = tempQuaternion;
                Vector3 vTemp = AssetGameObject.transform.position - coolMesh.bounds.center;
                AssetGameObject.transform.position += new Vector3(vTemp.x, vTemp.y + ((coolMesh.bounds.max.y - coolMesh.bounds.min.y) / 2), vTemp.z);
            }
            else
            {
                if (RemoteAssetBundleThumbnailInstance != null)
                {
                    RemoteAssetBundleThumbnailInstance.AddDelegateToEvent(AbstractRemoteLoadable.RemoteLoadable<AssetBundle>.RemoteLoadableEvent.OnReady,
                    new AbstractRemoteLoadable.RemoteLoadable<AssetBundle>.RemoteLoadableDelegate(x => { onDrawFunction(menuItem); }));
                }
                else
                {
                    RemoteAssetBundleInstance.AddDelegateToEvent(AbstractRemoteLoadable.RemoteLoadable<AssetBundle>.RemoteLoadableEvent.OnReady,
                    new AbstractRemoteLoadable.RemoteLoadable<AssetBundle>.RemoteLoadableDelegate(x => { onDrawFunction(menuItem); }));
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
        sReturned += "gudhub name: " + ChangeableObjectName + "\n";
        sReturned += "gudhub item ID: " + ID + "\n";
        sReturned += "type of object: " + ChangeableObjectType.ToString() + "\n";
        return sReturned;
    }

    public void SetMaterialGroupID(MaterialGroup materialGroupInstance)
    {
        CurrentGroupMaterialID = materialGroupInstance.ID;
    }
    #endregion

    #region private functions

    #region init functions

    private void SettingListFieldToRealFields()
    {
        ComponentsDataList = new List<AbstractObjectConstructableComponentData<SceneChangeableObjectTypes>>();
        for (int i = 0; i < settingFieldList.Count; i++)
        {
            ComponentsDataList.Add(new AbstractObjectConstructableComponentData<SceneChangeableObjectTypes>());
            ComponentsDataList[i].IdField = settingFieldList[i].IdField;
            ComponentsDataList[i].valueType = settingFieldList[i].valueType;
        }
    }

    private void InitURL(int num)
    {
        try
        {
            RemoteAssetBundleInstance = new RemoteAssetBundle(
                JSONMainManager.Instance.GetRealFileURLById(ComponentsDataList[num].StringValue),
                ComponentsDataList[num].StringValue,
                JSONMainManager.Instance.GetRealItemURLByID(ID));
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    private void InitThumbnail(int num)
    {
        try
        {
            RemoteAssetBundleThumbnailInstance = new RemoteAssetBundle(
                JSONMainManager.Instance.GetRealFileURLById(ComponentsDataList[num].StringValue),
                ComponentsDataList[num].StringValue,
                JSONMainManager.Instance.GetRealItemURLByID(ID));
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    private void InitName(int num)
    {
        ChangeableObjectName = ComponentsDataList[num].StringValue;
    }

    private void InitTypeObject(int num)
    {
        ChangeableObjectType = ComponentsDataList[num].StringValue;
    }

    private void InitMaterialGroup(int num)
    {
        if (ComponentsDataList[num] != null && ComponentsDataList[num].StringValue != null && ComponentsDataList[num].StringValue != "")
        {
            string[] temp = ComponentsDataList[num].StringValue.Split(',');
            MaterialGroupsID = new int[temp.Length];
            for (int i = 0; i < MaterialGroupsID.Length; i++)
            {
                MaterialGroupsID[i] = int.Parse(temp[i].Substring(temp[i].IndexOf('.') + 1));
            }
            CurrentGroupMaterialID = MaterialGroupsID[0];
        }
        else
        {
            MaterialGroupsID = new int[0];
        }
    }

    #endregion

    private void SpawnThumbnail()
    {
        if (RemoteAssetBundleThumbnailInstance.IsReady && AssetGameObject == null)
        {
            AssetGameObject = Instantiate((GameObject)RemoteAssetBundleThumbnailInstance.RemoteItemInstance.LoadAsset(RemoteAssetBundleThumbnailInstance.RemoteItemInstance.GetAllAssetNames()[0]), this.transform);
            //поворот бандла определяется здесь, так как сам ChangableObject не повернут относительно точки спавна
            AssetGameObject.transform.localPosition = new Vector3(0, 0, 0);
        }
    }

    private void MaterialReady(LoadedMaterial item)
    {
        ChangeMaterialTo(item.loadedMaterial);
        Destroy(item.gameObject);
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
        SceneChangeableObject sTemp = itemInstance as SceneChangeableObject;
        if (sTemp != null)
        {
            if (sTemp.AssetGameObject != null)
            {
                SceneChangeableObject tempSceneChangable = SceneLoaderManager.Instance.SpawnSceneChangableHere(((SceneChangeableObject)MenuManager.Instance.ObjectSelected).gameObject.transform.parent, ((SceneChangeableObject)itemInstance).ID);
                tempSceneChangable.StartLoadAssetBundle();
                Destroy(((SceneChangeableObject)MenuManager.Instance.ObjectSelected).gameObject);
                ClickManager.Instance.ImitateClick(tempSceneChangable.AssetGameObject.GetComponent<InteractiveObject>());
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
        List<SceneChangeableObject> tempSceneObjectChangeble = SceneLoaderManager.Instance.GetItemsLikeThat(this);
        for (int i = 0; i < tempSceneObjectChangeble.Count; i++)
        {
            int j = i;
            MenuItem menuObject = MenuItemFactory.Instance.GetMenuItem("menuItemForSceneChangeable", tempSceneObjectChangeble[i]);
            menuObject.onDrawFunction = () =>
            {
                tempSceneObjectChangeble[j].onDrawFunction(menuObject);
            };
            menuObject.OnActiveAction = () =>
            {
                tempSceneObjectChangeble[j].OnMenuClick(menuObject.AttachedObject);
            };
            menuObject.OnPointerHover = () =>
            {
                menuObject.effectGameObject.SetActive(true);
                menuObject.animator.SetBool("isBig", true);
            };
            menuObject.OnPointerLeft = () =>
            {
                menuObject.effectGameObject.SetActive(false);
                menuObject.animator.SetBool("isBig", false);
            };
            returnedList.Add(menuObject);
        }

        //group of materials
        List<MaterialGroup> tempMaterialGroup = SceneLoaderManager.Instance.GetMaterialGroupForThat(this);
        for (int i = 0; i < tempMaterialGroup.Count; i++)
        {
            MenuItem menuObject = MenuItemFactory.Instance.GetMenuItem("menuItemForMaterialGroup", tempMaterialGroup[i]);
            int j = i;
            menuObject.onDrawFunction = () =>
            {

            };
            menuObject.OnActiveAction = () =>
            {
                SetMaterialGroupID(tempMaterialGroup[j]);
                if (MenuManager.Instance.ObjectSelected as SceneChangeableObject)
                {
                    ClickManager.Instance.ImitateClick(((SceneChangeableObject)MenuManager.Instance.ObjectSelected).AssetGameObject.GetComponent<InteractiveObject>());
                }
            };
            menuObject.OnPointerHover = () =>
            {

            };
            menuObject.OnPointerLeft = () =>
            {

            };
            returnedList.Add(menuObject);
        }

        //add the textures
        List<LoadedMaterial> tempLoadedMeterial = SceneLoaderManager.Instance.GetMaterialsForThat(CurrentGroupMaterialID);
        for (int i = 0; i < tempLoadedMeterial.Count; i++)
        {
            MenuItem menuObject = MenuItemFactory.Instance.GetMenuItem("menuItemForLoadedMaterial", tempLoadedMeterial[i]);
            int j = i;
            menuObject.onDrawFunction = () =>
            {
                tempLoadedMeterial[j].OnMenuDraw(menuObject);
            };
            menuObject.OnActiveAction = () =>
            {
                tempLoadedMeterial[j].OnMenuClick(menuObject.AttachedObject);
            };
            menuObject.OnPointerHover = () =>
            {
                menuObject.effectGameObject.SetActive(true);
            };
            menuObject.OnPointerLeft = () =>
            {
                menuObject.effectGameObject.SetActive(false);
            };
            returnedList.Add(menuObject);
        }

        return returnedList;
    }

    #endregion

}

[System.Serializable]
public class SceneChangeableObjectCashed
{
    public string objectType;
    public string objectName;
    public int objectID;

    public SceneChangeableObjectCashed(string _objectType, string _objectName, int _objectID)
    {
        objectType = _objectType;
        objectName = _objectName;
        objectID = _objectID;
    }

}

public class ListOfSceneChangeableObjectCashed
{
    public List<SceneChangeableObjectCashed> sceneChangeableObjectCasheds;
}
