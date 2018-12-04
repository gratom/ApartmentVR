using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Interface for load managment and load distribution
/// </summary>
public interface IAssetBundleLoadable
{

    /// <summary>
    /// Set the bundle like in original object
    /// </summary>
    /// <param name="itemOriginal">object original</param>
    void CopyBundleFrom(IAssetBundleLoadable itemOriginal);

    /// <summary>
    /// Starting load asset bundle
    /// </summary>
    void StartLoadAssetBundle();

    /// <summary>
    /// Hide loaded bundle
    /// </summary>
    void BundleHide();

    /// <summary>
    /// Show loaded bundle, if it exist
    /// </summary>
    void BundleShow();

    /// <summary>
    /// Return the URL name, file name
    /// </summary>
    /// <returns>real file name on disk</returns>
    string GetURLName();

    /// <summary>
    /// Return the gudhub url
    /// </summary>
    /// <returns>gudhub url file name</returns>
    string GetRealURL();

    /// <summary>
    /// This function call when the bundle is ready to load from disk and use
    /// </summary>
    void BundleReady();

    /// <summary>
    /// Add the listener to current event
    /// </summary>
    /// <param name="bundleEvent">type of event</param>
    /// <param name="onEventFunction">Delegate will be called on event</param>
    void AddListenerLoading(AssetBundleLoaderManager.IAssetBundleLoadableEvent bundleEvent, AssetBundleLoaderManager.OnEventFunction onEventFunction);

    /// <summary>
    /// Remove the current listener on current event
    /// </summary>
    /// <param name="bundleEvent">type of event</param>
    /// <param name="onEventFunction">Delegate, that will be removed from current event</param>
    void RemoveListenerLoading(AssetBundleLoaderManager.IAssetBundleLoadableEvent bundleEvent, AssetBundleLoaderManager.OnEventFunction onEventFunction);

    /// <summary>
    /// Instance of asset bundle
    /// </summary>
    AssetBundle AssetBundleInstance { get; }

    /// <summary>
    /// ID parent item
    /// </summary>
    int itemID { get; }

}

/// <summary>
/// Setting class for AssetBundleLoaderManager
/// </summary>
[System.Serializable]
public class AssetBundleLoaderSetting
{

    [Tooltip("The folder in which all downloaded AssetBundles are saved")]
    /// <summary>
    /// The folder in which all downloaded AssetBundles are saved
    /// </summary>
    public string DataStoragePath;

    [Header("Count of loading treads")]
    [Tooltip("Maximum number of threads involved. Some of these threads may not be used if the number of downloaded AssetBundles is less than the number of threads.")]
    [Range(1, 15)]
    /// <summary>
    /// Maximum number of threads involved.
    /// </summary>
    public int MaxCountOfLoadingTread;

}

/// <summary>
/// Class for creating a load stream and for assigning loadable items. For load any AssetBundle for your class, you need to inherit the IAssetBundleLoadable interface.
/// </summary>
public class LoadingTread
{

    /// <summary>
    /// Instance of "Tread"
    /// </summary>
    public Coroutine CoroutineInstance;

    /// <summary>
    /// List of asset bundles, that will be loaded. 
    /// </summary>
    public List<IAssetBundleLoadable> AssetBundlesLoadableList;

}

/// <summary>
/// The class that can load and save in current folder the assetBundle, and get asset to it.
/// </summary>
public class AssetBundleLoaderManager : MonoBehaviour 
{

    /// <summary>
    /// Delegate for any of Event of IAssetBundleLoadable.
    /// </summary>
    /// <param name="item"></param>
    public delegate void OnEventFunction(IAssetBundleLoadable item = null);

    /// <summary>
    /// Type of events in IAssetBundleLoadable
    /// </summary>
    public enum IAssetBundleLoadableEvent
    {
        BundleReady,
        BundleStartLoading,
        BundleError
    }

    /// <summary>
    /// Singleton
    /// </summary>
    public static AssetBundleLoaderManager Instance { get; private set; }

    /// <summary>
    /// Setting for this class
    /// </summary>
    public AssetBundleLoaderSetting Setting;

    /// <summary>
    /// Permanent app path, that initiated when module is initiated. The path of app data
    /// </summary>
    public string AppPath { get; private set; }

    private List<LoadingTread> LoadingTreadsList;

    private float LocalTimer;

    #region Unity functions

    #endregion

    #region public functions

    /// <summary>
    /// Trying to initiate this module
    /// </summary>
    public void TryInit()
    {
        if (Instance == null)
        {
            Initialize();
        }
    }

    /// <summary>
    /// Add the item to queue loading, and when the assetbundle was loaded will be called function bundleReady()
    /// </summary>
    /// <param name="item">IAssetBundleLoadeble item, that will be loaded bundle</param>
    public void AddToLoadable(IAssetBundleLoadable item)
    {
        int tempNum = 0;
        int tempCount = int.MaxValue;
        for (int i = 0; i < LoadingTreadsList.Count; i++)
        {
            if (tempCount > LoadingTreadsList[i].AssetBundlesLoadableList.Count)
            {
                tempCount = LoadingTreadsList[i].AssetBundlesLoadableList.Count;
                tempNum = i;
            }
        }
        LoadingTreadsList[tempNum].AssetBundlesLoadableList.Add(item);
    }

    #endregion

    #region private functions

    private void Initialize()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        AppPath = Application.dataPath.Substring(0,Application.dataPath.LastIndexOf("/")) + Setting.DataStoragePath;
        if (!Directory.Exists(AppPath))
        {
            Directory.CreateDirectory(AppPath);
        }

        InitLoadingTreads();
        StartAllCoroutines();
    }

    private void InitLoadingTreads()
    {
        LoadingTreadsList = new List<LoadingTread>();
        for (int i = 0; i < Setting.MaxCountOfLoadingTread; i++)
        {
            LoadingTreadsList.Add(new LoadingTread());
            LoadingTreadsList[i].AssetBundlesLoadableList = new List<IAssetBundleLoadable>();
        }
    }

    private void StartAllCoroutines()
    {
        if (LoadingTreadsList != null)
        {
            for (int i = 0; i < LoadingTreadsList.Count; i++)
            {
                if (LoadingTreadsList[i] != null)
                {
                    LoadingTreadsList[i].CoroutineInstance = StartCoroutine(LoadAssetBundleCoroutine(i));
                }
            }
        }
    }

    private void StopAllLoadingCoroutines()
    {
        if (LoadingTreadsList != null)
        {
            for (int i = 0; i < LoadingTreadsList.Count; i++)
            {
                if (LoadingTreadsList[i] != null)
                {
                    StopCoroutine(LoadingTreadsList[i].CoroutineInstance);
                }
            }
        }
    }

    #endregion

    #region Coroutines

    private IEnumerator LoadAssetBundleCoroutine(int num)
    {
        while (true)
        {
            if (LoadingTreadsList[num].AssetBundlesLoadableList.Count > 0)
            {                     
                string path = Path.Combine(AppPath, LoadingTreadsList[num].AssetBundlesLoadableList[0].GetURLName());
                string RealGudhubURL = LoadingTreadsList[num].AssetBundlesLoadableList[0].GetRealURL();                
                if (!File.Exists(path))
                {
#if UNITY_EDITOR
                    {
                        Debug.Log("Try to load ->" + RealGudhubURL + "\nItemID = " + LoadingTreadsList[num].AssetBundlesLoadableList[0].itemID);
                    }
#endif
                    if(RealGudhubURL == "")
                    {                        
                        Debug.Log("Cant load item bundle." + LoadingTreadsList[num].AssetBundlesLoadableList[0].itemID.ToString());
                        Debug.LogError("Loading manager get error.\nMaybe you should to fix this item -> " + JSONMainManager.Instance.GetRealItemURLByID(LoadingTreadsList[num].AssetBundlesLoadableList[0].itemID));                        
                        Application.OpenURL(JSONMainManager.Instance.GetRealItemURLByID(LoadingTreadsList[num].AssetBundlesLoadableList[0].itemID));
                        LoadingTreadsList[num].AssetBundlesLoadableList.RemoveAt(0);
                        continue;
                    }
                    WWW www = new WWW(RealGudhubURL);                    
                    yield return www;
                    if (!File.Exists(path))
                    {
                        FileStream fs = new FileStream(path, FileMode.OpenOrCreate);
                        BinaryWriter bw = new BinaryWriter(fs);
                        bw.Write(www.bytes);
                        bw.Close();
                        fs.Close();
                    }
                }
                LoadingTreadsList[num].AssetBundlesLoadableList[0].BundleReady();
                LoadingTreadsList[num].AssetBundlesLoadableList.RemoveAt(0);
                yield return null;
                #region realtimer
                //if (Time.realtimeSinceStartup - LocalTimer > 0.01f)
                //{
                //    LocalTimer = Time.realtimeSinceStartup;
                //    yield return null;
                //}
                #endregion
            }
            else
            {
                yield return new WaitForSeconds(0.2f); //waiting time for next loading try
            }
        }   
    }

    #endregion

}
