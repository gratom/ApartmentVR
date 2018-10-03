using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class 
/// </summary>
public enum SceneChangebleObjectTypes
{
    id,
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
public class SceneChangebleObject : AbstractObjectConstructable <SceneChangebleObjectTypes>, IAssetBundleLoadeble, IClickable
{

    public int ID { get; private set; }
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
        FunctionsDictionary.Add(SceneChangebleObjectTypes.id, InitID);
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
            ComponentsDataList.Add(settingFieldList[i]);
        }
    }

    private void InitURL(int num)
    {
        RealGudHubURL = ComponentsDataList[num].StringValue;
        URLName = RealGudHubURL.Substring(RealGudHubURL.LastIndexOf('/'));        
    }    

    private void InitName(int num)
    {
        ChangebleObjectName = ComponentsDataList[num].StringValue;
    }

    private void InitID(int num)
    {
        try
        {
            ID = int.Parse(ComponentsDataList[num].StringValue);
        }
        catch (System.Exception e)
        {
            Debug.Log(this.ToString() + e);
        }
    }

    private void InitTypeObject(int num)
    {
        ChangebleObjectType = ComponentsDataList[num].StringValue;
    }

    private void LoadAssetBundleFromURL() // сейчас можно не использовать, а потом, когда будет написан нормальный регулятор загрузок, чтобы не было просаживания FPS, нужно будет использовать эту функцию
    {
        StartLoadAssetBundle();
    }

    private void OnClickOnMenu(MonoBehaviour itemInstance)
    {

    }

    #endregion

    #region interfaces

    /// <summary>
    /// Load assetbandle right now
    /// </summary>
    public void StartLoadAssetBundle()
    {
        string path = Application.dataPath;
        path = path.Substring(0, path.LastIndexOf('/'));
        path += AssetBundleLoaderManager.Instance.Setting.DataStoragePath;
        AssetBundleInstance = AssetBundle.LoadFromFile(path + URLName);
        AssetGameObject = Instantiate((GameObject)AssetBundleInstance.LoadAsset(AssetBundleInstance.GetAllAssetNames()[0]), this.transform);
        AssetGameObject.transform.localPosition = new Vector3(0, 0, 0);
    }

    /// <summary>
    /// Get the object for show in menu, on click event
    /// </summary>
    /// <returns>the list of menu objects</returns>
    public List<MenuObject> GetListOfMenuObject()
    {
        List<MenuObject> returnedList = new List<MenuObject>();

        List<SceneChangebleObject> temp = SceneLoaderManager.Instance.GetItemsLikeThat(this);

        //add the other item, like this item...
        for (int i = 0; i < temp.Count; i++)
        {
            returnedList.Add(new MenuObject(MenuObject.TypeOfObject.firstLine, returnedList[i], OnClickOnMenu));
        }

        //add the textures
        return returnedList;
    }

    #endregion
}