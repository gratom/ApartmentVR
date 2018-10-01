using System.Collections;
using System.Collections.Generic;
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

/// <summary>
/// The class that can load and save in current folder the assetBundle, and get asset to it.
/// </summary>
public class AssetBundleLoaderManager : MonoBehaviour 
{

    /// <summary>
    /// Singleton
    /// </summary>
    public static AssetBundleLoaderManager Instance;

    /// <summary>
    /// Setting for this class
    /// </summary>
    public AssetBundleLoaderSetting Setting;

    #region Unity functions

    void Awake()
    {
        Initialize();
    }

    #endregion

    #region public functions

    public void AddToLoadeble(IAssetBundleLoadeble item)
    {

    }

    #endregion

    #region private functions

    private void Initialize()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    #endregion

}
