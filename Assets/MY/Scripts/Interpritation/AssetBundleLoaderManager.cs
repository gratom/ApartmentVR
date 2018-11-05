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
    /// this function call when the bundle is ready to load from disk and use
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

    AssetBundle AssetBundleInstance { get; }

}

/// <summary>
/// Setting class for AssetBundleLoaderManager
/// </summary>
[System.Serializable]
public class AssetBundleLoaderSetting
{
    public string DataStoragePath;
    public int MaxCountOfLoadingTread;
}

public class LoadingTread
{
    public Coroutine CoroutineInnstance;
    public List<IAssetBundleLoadable> AssetBundlesLoadableList;
}

/// <summary>
/// The class that can load and save in current folder the assetBundle, and get asset to it.
/// </summary>
public class AssetBundleLoaderManager : MonoBehaviour 
{
    public delegate void OnEventFunction(IAssetBundleLoadable item = null);

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
        //Debug.Log("loading item " + item.GetURLName() + " adding to tread " + tempNum.ToString());
    }

    #endregion

    #region private functions

    private void Initialize()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        AppPath = Application.persistentDataPath + Setting.DataStoragePath;
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
                    LoadingTreadsList[i].CoroutineInnstance = StartCoroutine(LoadAssetBundleCoroutine(i));
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
                    StopCoroutine(LoadingTreadsList[i].CoroutineInnstance);
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

                //Debug.Log("Try to load ->" + URLs[0]);

                if (!File.Exists(path))
                {
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
                //if (Time.realtimeSinceStartup - LocalTimer > 0.03)
                //{
                //    LocalTimer = Time.realtimeSinceStartup;
                //    yield return null;
                //}
                #endregion
            }
            else
            {
                yield return new WaitForSeconds(0.1f); //waiting time for next loading try
            }
        }   
    }

    #endregion

}
