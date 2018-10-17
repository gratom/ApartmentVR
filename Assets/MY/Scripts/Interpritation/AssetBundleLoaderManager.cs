using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Interface for load managment and load distribution
/// </summary>
public interface IAssetBundleLoadeble
{
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

    void BundleReady();

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
    public List<IAssetBundleLoadeble> AssetBundlesLoadableList;
}

/// <summary>
/// The class that can load and save in current folder the assetBundle, and get asset to it.
/// </summary>
public class AssetBundleLoaderManager : MonoBehaviour 
{

    /// <summary>
    /// Singleton
    /// </summary>
    public static AssetBundleLoaderManager Instance { get; private set; }

    /// <summary>
    /// Setting for this class
    /// </summary>
    public AssetBundleLoaderSetting Setting;

    private List<LoadingTread> LoadingTreadsList;
    public string AppPath { get; private set; }

    #region Unity functions

    #endregion

    #region public functions

    public void TryInit()
    {
        if (Instance == null)
        {
            Initialize();
        }
    }

    public void AddToLoadeble(IAssetBundleLoadeble item)
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
            LoadingTreadsList[i].AssetBundlesLoadableList = new List<IAssetBundleLoadeble>();
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
                    FileStream fs = new FileStream(path, FileMode.OpenOrCreate);
                    BinaryWriter bw = new BinaryWriter(fs);
                    bw.Write(www.bytes);
                    bw.Close();
                    fs.Close();
                }

                LoadingTreadsList[num].AssetBundlesLoadableList[0].BundleReady();
                LoadingTreadsList[num].AssetBundlesLoadableList.RemoveAt(0);
                yield return null;
                //if (Time.realtimeSinceStartup - LocalTimer > 0.03)
                //{
                //    LocalTimer = Time.realtimeSinceStartup;
                //    yield return null;
                //}
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
            }
        }   
    }

    #endregion

}
