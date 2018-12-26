using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniversalAssetBundleLoader;
using UniversalImageLoader;

public class Loader : MonoBehaviour
{

    /// <summary>
    /// Singleton
    /// </summary>
    public static Loader Instance { get; private set; }

    public bool IsLoadSceneFromBundle;

    /// <summary>
    /// Setting class for JSONMainManager
    /// </summary>
    public AppDataLoader AppDataLoaderInstance;

    /// <summary>
    /// Instance of AssetBundle loader
    /// </summary>
    public AssetBundleLoaderManager assetBundleLoaderManager;

    /// <summary>
    /// Instance of RemoteImage loader
    /// </summary>
    public RemoteImageLoaderManager remoteImageLoaderManager;

    /// <summary>
    /// AllManagers need to scene working
    /// </summary>
    public GameObject AllManagersForScene;

    /// <summary>
    /// Name of scene, that will be loaded after loading
    /// </summary>
    public string SceneLoadAfterLoading;

    void Start()
    {
        StartCoroutine(LoadingCoroutine());
    }

    private void Initialize()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        
    }

    private IEnumerator LoadingCoroutine()
    {
        yield return null;
        LoadingVisualizer.Instance.StatusBarText = "Initiate the main objects and scripts...\n";
        LoadingVisualizer.Instance.counter++;
        GameObject managersMainGameObject = new GameObject("Managers");
        DontDestroyOnLoad(managersMainGameObject);

        yield return null;
        GameObject JSONManagerGameObject = new GameObject("JSONManagerGameObject");
        JSONManagerGameObject.transform.parent = managersMainGameObject.transform;
        JSONManagerGameObject.AddComponent<JSONMainManager>();
        JSONManagerGameObject.GetComponent<JSONMainManager>().AppDataLoaderInstance = AppDataLoaderInstance;
        LoadingVisualizer.Instance.StatusBarText += "Initiate JSONManager...\nTry to check the internet connection...\n";
        yield return null;
        LoadingVisualizer.Instance.counter++;
        
        //tracking the events of loading data
        JSONMainManager.Instance.requestModuleInstance.AddDelegate(RequestsModule.RequestEvents.FindInternet, InternetEvent);
        JSONMainManager.Instance.requestModuleInstance.AddDelegate(RequestsModule.RequestEvents.NoInternet, ErrorEvent);
        JSONMainManager.Instance.requestModuleInstance.AddDelegate(RequestsModule.RequestEvents.AuthentificationOK, AuthEvent);
        JSONMainManager.Instance.requestModuleInstance.AddDelegate(RequestsModule.RequestEvents.AuthentificationFailed, ErrorEvent);
        JSONMainManager.Instance.requestModuleInstance.AddDelegate(RequestsModule.RequestEvents.GettingAppListOK, GetAppList);
        JSONMainManager.Instance.requestModuleInstance.AddDelegate(RequestsModule.RequestEvents.GettingAppListFailed, ErrorEvent);
        
        LoadingVisualizer.Instance.counter += 10;

        for (int i = 0; i < JSONMainManager.Instance.AppDataLoaderInstance.ListOfAppsSetting.Count; i++)
        {
            JSONMainManager.Instance.AppDataLoaderInstance.ListOfAppsSetting[i].requestsModule.AddDelegate(RequestsModule.RequestEvents.AppDataOK, AppDataLoaded);
            JSONMainManager.Instance.AppDataLoaderInstance.ListOfAppsSetting[i].requestsModule.AddDelegate(RequestsModule.RequestEvents.AppDataFailed, ErrorEvent);
        }
        
        //create loader assetBundle
        assetBundleLoaderManager.transform.parent = managersMainGameObject.transform;
        remoteImageLoaderManager.transform.parent = managersMainGameObject.transform;
        AllManagersForScene.transform.parent = managersMainGameObject.transform;

        while (!JSONMainManager.Instance.IsReady)
        {
            yield return new WaitForSeconds(0.2f);
        }

        LoadingVisualizer.Instance.counter += 100;

        yield return new WaitForSeconds(1);

        if (IsLoadSceneFromBundle)
        {
            AssetBundle bundle = AssetBundle.LoadFromFile(Application.dataPath + "/" + SceneLoadAfterLoading);
            string[] scenePaths = bundle.GetAllScenePaths();
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePaths[0]);
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneLoadAfterLoading);
        }
    }

    private void ErrorEvent(string Responce)
    {
        LoadingVisualizer.Instance.StatusBarText += "Error:" + Responce + "\n";
    }

    private void InternetEvent(string Responce)
    {
        JSONMainManager.Instance.requestModuleInstance.RemoveDelegate(RequestsModule.RequestEvents.FindInternet, InternetEvent);
        LoadingVisualizer.Instance.StatusBarText += "Connection OK.\nTry to authenticate...\n";
        LoadingVisualizer.Instance.counter += 7;        
    }

    private void AuthEvent(string Responce)
    {
        JSONMainManager.Instance.requestModuleInstance.RemoveDelegate(RequestsModule.RequestEvents.AuthentificationOK, AuthEvent);
        LoadingVisualizer.Instance.StatusBarText += "Authenticate OK.\nTry to load main App list...\n";
        LoadingVisualizer.Instance.counter += 13;
    }

    private void GetAppList(string Responce)
    {
        JSONMainManager.Instance.requestModuleInstance.RemoveDelegate(RequestsModule.RequestEvents.GettingAppListOK, GetAppList);
        LoadingVisualizer.Instance.StatusBarText += "Main app list loaded.\nTry to load allication data...\n";
        LoadingVisualizer.Instance.counter += 17;
    }

    private void AppDataLoaded(string Responce)
    {
        string s = Responce.Substring(Responce.IndexOf("app_name") + 11, Responce.IndexOf("\"", Responce.IndexOf("app_name") + 11) - (Responce.IndexOf("app_name") + 11));
        LoadingVisualizer.Instance.StatusBarText += "App loaded:" + s + "\n";
        LoadingVisualizer.Instance.counter += 22;
    }

}
