using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// Setting for get the access and load the data of current App
/// </summary>
[System.Serializable]
public class AppSetting
{

    /// <summary>
    /// Types of loaded app
    /// </summary>
    public enum AppType
    {
        objects,
        materials,
        scenes,
        materialGroup
    }

    /// <summary>
    /// Type of this appSetting
    /// </summary>
    public AppType appType
    {
        get
        {
            return _appType;
        }
    }
    [SerializeField]
    [Tooltip("Type of this appSetting")]
    private AppType _appType;

    [Tooltip("Name of file, where saved the JSON-data of this App")]
    /// <summary>
    /// Name of file, where be saved the JSON of this Application
    /// </summary>
    public string FileName;

    [Tooltip("Application ID, it used for find necessary Application in the app list")]
    /// <summary>
    /// Application ID, it used for find necessary Application in the app list
    /// </summary>
    public int AppID;

    /// <summary>
    /// View ID used for show the errors in app items
    /// </summary>
    public int ViewID;

    /// <summary>
    /// Application data
    /// </summary>
    [HideInInspector]
    public App AppData
    {
        get
        {
            return _appData;
        }
        set
        {
            _appData = value;
            UpdateViewID();
        }
    }
    private App _appData;

    /// <summary>
    /// String for JSONdata
    /// </summary>
    [HideInInspector]
    public string LocalStorageString;    

    /// <summary>
    /// Get the state of app setting
    /// </summary>
    [HideInInspector]
    public bool IsReady
    {
        get
        {
            return AppData != null && StorageIs;
        }
    }

    /// <summary>
    /// Is local storage exist
    /// </summary>
    [HideInInspector]
    public bool StorageIs
    {
        get
        {
            return LocalStorageString != "";
        }
    }

    /// <summary>
    /// Module for request to gudhub
    /// </summary>
    [HideInInspector]
    public RequestsModule requestsModule { get; private set; }

    /// <summary>
    /// Init the request module 
    /// </summary>
    public void InitRequestModuleHere(GameObject gameObject)
    {
        requestsModule = gameObject.AddComponent<RequestsModule>();
        requestsModule.InitEventsDictionary();
    }

    /// <summary>
    /// Try the load app data in JSON from local file
    /// </summary>
    public void TryLoadLocalStorage()
    {
        LocalStorageString = SaverLoaderModule.LoadMyDataFrom(Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/")) + JSONMainManager.Instance.AppDataLoaderInstance.JSONFolder + FileName);
    }

    /// <summary>
    /// Try to save the string data in LocalStorageString in file on disk
    /// </summary>
    public void CreateLocalStorage()
    {
        SaverLoaderModule.SaveMyDataTo(Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/")) + JSONMainManager.Instance.AppDataLoaderInstance.JSONFolder + FileName, LocalStorageString);
    }

    #region private functions

    private void UpdateViewID()
    {
        if (AppData != null)
        {
            for (int i = 0; i < AppData.views_list.Count; i++)
            {
                if (AppData.views_list[i].name == "Form")
                {                    
                    ViewID = AppData.views_list[i].view_id;
                }
            }
        }
    }    

    #endregion
}

/// <summary>
/// Setting class for JSONMainManager
/// </summary>
[System.Serializable]
public class AppDataLoader
{

    [Tooltip("The list of app setting, that contain id, token and name of local file, in which app data was saved. You must fill them in to access the data you need.")]
    /// <summary>
    /// The list of app setting, that contain id, token and name of local file, in which app data was saved
    /// </summary>
    public List<AppSetting> ListOfAppsSetting;

    [Header("Folder for JSON saves")]
    /// <summary>
    /// Folder for json saves
    /// </summary>
    public string JSONFolder;

    [Header("Key to get access to GubHub Account")]
    /// <summary>
    /// Key to get access to GubHub Account (its like login and password)
    /// </summary>
    public string Key;

    [SerializeField]
    [Header("Should script load the setting from disk?")]
    /// <summary>
    /// Load of not the setting from standart config file
    /// </summary>
    private bool IsLoadSettingFromDisk = false;

    /// <summary>
    /// Token for get the one-off access to the Application data 
    /// </summary>
    [HideInInspector]
    public string AccessToken;

    /// <summary>
    /// Setting data loaded from disk
    /// </summary>
    [HideInInspector]
    public SettingFromJson.AppDataSettingJsonClass settingClassInstance;

    /// <summary>
    /// Constructor
    /// </summary>
    public AppDataLoader()
    {
        Key = "key dont exist!";
        ListOfAppsSetting = new List<AppSetting>();
    }

    #region public functions

    /// <summary>
    /// Init All request modules in all apps
    /// </summary>
    public void InitRequestModulesHere(GameObject gameObjectPlace)
    {
        CreateFolderForJson();

        #region loading settign from config file

        if (IsLoadSettingFromDisk)
        {
            string sTemp = SaverLoaderModule.LoadMyDataFromFile("setting.config");
            settingClassInstance = JsonUtility.FromJson<SettingFromJson.AppDataSettingJsonClass>(sTemp);
            
            Key = settingClassInstance.AuthKey;

            for (int i = 0; i < ListOfAppsSetting.Count; i++)
            {
                for(int j = 0; j < settingClassInstance.Apps.Count; j++)
                {
                    if (ListOfAppsSetting[i].appType.ToString() == settingClassInstance.Apps[j].AppType)
                    {
                        ListOfAppsSetting[i].AppID = settingClassInstance.Apps[j].AppID;
                        break;
                    }
                }       
            }
        }

        #endregion

        #region saveJson to setting file

        //SettingFromJson.AppDataSettingJsonClass tempSettingClass = new SettingFromJson.AppDataSettingJsonClass();
        //tempSettingClass.AuthKey = Key;
        //tempSettingClass.Apps = new List<SettingFromJson.App>();
        //for (int i = 0; i < ListOfAppsSetting.Count; i++)
        //{
        //    tempSettingClass.Apps.Add(new SettingFromJson.App(ListOfAppsSetting[i].appType.ToString(), ListOfAppsSetting[i].AppID));
        //    tempSettingClass.Apps[i].Fields = new List<SettingFromJson.Field>();
        //    for (int j = 0; j < 4; j++)
        //    {
        //        tempSettingClass.Apps[i].Fields.Add(new SettingFromJson.Field(j * i, "Type of field"));
        //    }
        //}

        //SaverLoaderModule.SaveMyDataToFile("setting.config", JsonUtility.ToJson(tempSettingClass));

        #endregion

        Key = System.Uri.EscapeDataString(Key);

        for (int i = 0; i < ListOfAppsSetting.Count; i++)
        {
            GameObject RequestModuleGameObject = new GameObject("RequestModuleGameObject" + i.ToString());
            RequestModuleGameObject.transform.parent = gameObjectPlace.transform;
            ListOfAppsSetting[i].InitRequestModuleHere(RequestModuleGameObject);
        }        

    }

    /// <summary>
    /// Get the app data by app index
    /// </summary>
    /// <param name="index">app index</param>
    /// <returns>app data</returns>
    public App GetAppDataByID(int index)
    {
        for (int i = 0; i < ListOfAppsSetting.Count; i++)
        {
            if (ListOfAppsSetting[i].AppID == index)
            {
                return ListOfAppsSetting[i].AppData;
            }
        }
        return null;
    }

    #endregion

    #region private functions

    private void CreateFolderForJson()
    {
        string AppPath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/")) + JSONFolder;
        if (!Directory.Exists(AppPath))
        {
            Directory.CreateDirectory(AppPath);
        }
    }

    #endregion

}


/// <summary>
/// Main Manager for simple getting the value. Need the initialization before using. (placed in awake)
/// </summary>
public class JSONMainManager : MonoBehaviour 
{

    /// <summary>
    /// Singleton
    /// </summary>
    public static JSONMainManager Instance { get; private set; }

    /// <summary>
    /// Request module for public access to events
    /// </summary>
    public RequestsModule requestModuleInstance { get; private set; }

    /// <summary>
    /// Setting for this class
    /// </summary>
    [SerializeField]
    [Header("Setting for this class. You need fill it!")]
    public AppDataLoader AppDataLoaderInstance;    

    /// <summary>
    /// Ready to use if is true
    /// </summary>
    public bool IsReady { get; private set; }

    private bool InternetIs = false;       

    #region Unity Functions

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        StartCoroutine(InitManager());
    }

    #endregion

    #region Public fuctions

    /// <summary>
    /// Fill the data in the list by App ID and return it
    /// </summary>
    /// <param name="EmptyList">the empty list, that will be filled</param>
    /// <param name="AppID">App ID, from which information is taken</param>
    /// <returns></returns>
    public void FillDataToList<TypeElement>(List<AbstractObjectConstructable<TypeElement>> EmptyList, int AppID) where TypeElement : struct, System.IConvertible
    {
        App app = AppDataLoaderInstance.GetAppDataByID(AppID);
        if (app != null)
        {
            for (int i = 0; i < EmptyList.Count; i++)
            {               
                EmptyList[i] = JSONInterpritator.GetStringValueByID(app, EmptyList[i], i);
            }            
        }
    }

    /// <summary>
    /// Getting the real URL of file by file ID in AppData
    /// </summary>
    /// <param name="URLID">The file ID</param>
    /// <returns>The URL of file</returns>
    public string GetRealFileURLById(string URLID)
    {
        for (int i = 0; i < AppDataLoaderInstance.ListOfAppsSetting.Count; i++)
        {
            for (int j = 0; j < AppDataLoaderInstance.ListOfAppsSetting[i].AppData.file_list.Count; j++)
            {
                if (AppDataLoaderInstance.ListOfAppsSetting[i].AppData.file_list[j].file_id.ToString() == URLID)
                {
                    return AppDataLoaderInstance.ListOfAppsSetting[i].AppData.file_list[j].url;
                }
            }
        }
        //Debug.Log("Item is not exist!" + "\nFile ID " + URLID);
        return "";
    }

    public string GetRealItemURLByID(int ID)
    {
        string ReturnedString = "https://gudhub.com/act/open_item/";
        for(int i = 0; i < AppDataLoaderInstance.ListOfAppsSetting.Count; i++)
        {
            for(int j = 0; j < AppDataLoaderInstance.ListOfAppsSetting[i].AppData.items_list.Count; j++)
            {
                if(AppDataLoaderInstance.ListOfAppsSetting[i].AppData.items_list[j].item_id == ID)
                {
                    int appID = AppDataLoaderInstance.ListOfAppsSetting[i].AppData.app_id;
                    int viewID = AppDataLoaderInstance.ListOfAppsSetting[i].ViewID;
                    ReturnedString += appID.ToString() + "/" + viewID.ToString() + "/" + ID.ToString();
                    return ReturnedString;
                }
            }
        }
        return ReturnedString;
    }

    #endregion

    #region private functions        

    private void BadInternet(string error)
    {
        Debug.Log(error);
        InternetIs = false;
        EndInternet();
    }

    private void GoodInternet(string responce)
    {
        InternetIs = true;
        EndInternet();
    }

    private void EndInternet()
    {
        requestModuleInstance.RemoveDelegate(RequestsModule.RequestEvents.NoInternet, BadInternet);
        requestModuleInstance.RemoveDelegate(RequestsModule.RequestEvents.FindInternet, GoodInternet);
        Caser();
    }

    private void Caser()
    {
        if (InternetIs)
        {
            //Auth and load App list
            AuthFunction();
        }
        else
        {
            //Load from local storage
            for (int i = 0; i < AppDataLoaderInstance.ListOfAppsSetting.Count; i++)
            {
                if (AppDataLoaderInstance.ListOfAppsSetting[i].StorageIs)
                {
                    AppDataLoadEnd(AppDataLoaderInstance.ListOfAppsSetting[i].LocalStorageString);
                    //AppDataLoaderInstance.ListOfAppsSetting[i].AppData = JSONModule.StringToAppData(AppDataLoaderInstance.ListOfAppsSetting[i].LocalStorageString);
                }
            }
        }        
    }  

    private void AuthFunction()
    {
        //auth
        requestModuleInstance.AddDelegate(RequestsModule.RequestEvents.AuthentificationOK, AuthEnd);
        requestModuleInstance.AddDelegate(RequestsModule.RequestEvents.AuthentificationFailed, ErrorFunc);
        requestModuleInstance.AuthentificationStart(AppDataLoaderInstance.Key);
    }

    private void ErrorFunc(string errorResponce)
    {
        Debug.Log(errorResponce);
    }

    private void AuthEnd(string Responce)
    {
        requestModuleInstance.RemoveDelegate(RequestsModule.RequestEvents.AuthentificationOK, AuthEnd);
        requestModuleInstance.AddDelegate(RequestsModule.RequestEvents.GettingAppListOK, AppListLoadEnd);
        requestModuleInstance.AddDelegate(RequestsModule.RequestEvents.GettingAppListFailed, ErrorFunc);
        AppDataLoaderInstance.AccessToken = JSONModule.StringToUserInformation(Responce).accesstoken;
        requestModuleInstance.GetAppListStart(AppDataLoaderInstance.AccessToken);
    }

    private void AppListLoadEnd(string Responce)
    {
        requestModuleInstance.RemoveDelegate(RequestsModule.RequestEvents.GettingAppListOK, AppListLoadEnd);

        tempStorage tS = JSONModule.StringToAppList(Responce);
        for (int i = 0; i < tS.apps_list.Count; i++)
        {
            for (int j = 0; j < AppDataLoaderInstance.ListOfAppsSetting.Count; j++)
            {
                if (tS.apps_list[i].app_id == AppDataLoaderInstance.ListOfAppsSetting[j].AppID)
                {
                    #region cool function for cool server
                    //if (tS.apps_list[j].last_update == JSONModule.StringToAppData(LocalStorageString).last_update)
                    //{
                    //    IsOutdated = false;
                    //}                    
                    //reload all, because the server is wrong
                    #endregion
                    AppDataLoaderInstance.ListOfAppsSetting[j].requestsModule.AddDelegate(RequestsModule.RequestEvents.AppDataOK, AppDataLoadEnd);
                    AppDataLoaderInstance.ListOfAppsSetting[j].requestsModule.AddDelegate(RequestsModule.RequestEvents.AppDataFailed, ErrorFunc);
                    AppDataLoaderInstance.ListOfAppsSetting[j].LocalStorageString = ""; // clean string for ready function work correct
                    AppDataLoaderInstance.ListOfAppsSetting[j].requestsModule.AppDataStart(AppDataLoaderInstance.ListOfAppsSetting[j].AppID, AppDataLoaderInstance.AccessToken);
                }
            }
        }       
    }

    private void AppDataLoadEnd(string Responce)
    {
        App AppData = JSONModule.StringToAppData(Responce);
        for (int i = 0; i < AppDataLoaderInstance.ListOfAppsSetting.Count; i++)
        {
            if (AppDataLoaderInstance.ListOfAppsSetting[i].AppID == AppData.app_id)
            {
                AppDataLoaderInstance.ListOfAppsSetting[i].requestsModule.RemoveDelegate(RequestsModule.RequestEvents.AppDataOK, AppDataLoadEnd);
                AppDataLoaderInstance.ListOfAppsSetting[i].AppData = AppData;
                AppDataLoaderInstance.ListOfAppsSetting[i].LocalStorageString = Responce;
                AppDataLoaderInstance.ListOfAppsSetting[i].CreateLocalStorage();
                IsReady = IsGlobalReady();
                return;
            }
        }
    }

    private bool IsGlobalReady()
    {
        for (int i = 0; i < AppDataLoaderInstance.ListOfAppsSetting.Count; i++)
        {
            if (!AppDataLoaderInstance.ListOfAppsSetting[i].IsReady)
            {
                return false;
            }
        }
        return true;
    }

    #endregion

    #region Coroutines

    private IEnumerator InitManager()
    {
        yield return null; //ждем, на случай того, что менеджер создан динамически.        

        //app init data must be not null
        if (AppDataLoaderInstance == null)
        {
            AppDataLoaderInstance = new AppDataLoader();
        }

        if (AppDataLoaderInstance.ListOfAppsSetting.Count == 0)
        {
#if UNITY_EDITOR
            Debug.LogError("AppDataLoaderInstance non-initialized or app count == 0", this);
            yield break;
#endif
        }

        AppDataLoaderInstance.InitRequestModulesHere(this.gameObject);

        for (int i = 0; i < AppDataLoaderInstance.ListOfAppsSetting.Count; i++)
        {
            AppDataLoaderInstance.ListOfAppsSetting[i].TryLoadLocalStorage();
        }
        
        // проверка интернета, проводится для всех, используем нулевой элемент
        // делаем нулевой элемент главным
        requestModuleInstance = AppDataLoaderInstance.ListOfAppsSetting[0].requestsModule;

        requestModuleInstance.AddDelegate(RequestsModule.RequestEvents.FindInternet, GoodInternet);
        requestModuleInstance.AddDelegate(RequestsModule.RequestEvents.NoInternet, BadInternet);
        requestModuleInstance.CheckInternetStart();
    }

    #endregion

}
