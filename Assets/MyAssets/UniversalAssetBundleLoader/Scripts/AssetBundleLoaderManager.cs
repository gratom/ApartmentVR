/* author : Volkov Alexander (gratom)
 * email for any questions : gratomov@gmail.com
 * created in 2018 y.
 * 
 * Class created for easy loading any AssetBundles from GudHub.com
 *   
 * if you want to use it, just add a class "RemoteAssetBundles" object to your class. 
 * Make sure the Download Manager is exist on the scene. Set up the Download Manager. 
 * When you need it, add your RemoteAssetBundles object to the download queue using the function
 * 
 * code example:
 * -> AssetBundleLoaderManager.Instance.AddToLoadStack(YouObject, "Wanted name of object on disk", int PriorityLoading);
 * 
 * You can add as many RemoteAssetBundle to the queue as you like. You can also add a delegate to events in RemoteAssetBundle object,
 * and, rof example, when image was loaded it already can spawn on scene, or do something else.
 * 
 * For more features and specifications read the classes documentation. 
 */

using UnityEngine;
using AbstractRemoteLoadable;

namespace UniversalAssetBundleLoader
{

    /// <summary>
    /// This manager implements the correct boot-sequence of RemoteAssetBundle objects
    /// </summary>
    public class AssetBundleLoaderManager : AbstractManagerLoadable<AssetBundle> { }

    /// <summary>
    /// Class for Remote AssetBundles instances
    /// </summary>
    [System.Serializable]
    public class RemoteAssetBundle : RemoteLoadable<AssetBundle>
    {

        #region constructor

        /// <summary>
        /// Make initialization RemoteAssetBundle from parametrs
        /// </summary>
        /// <param name="AssetBundleURL">RemoteAssetBundle URL in GudHub</param>
        /// <param name="AssetBundleName">Name of RemoteAssetBundle. If you don`t choose own name, name will be created automaticly from URL</param>
        /// <param name="priority">Priority of RemoteAssetBundle. This is an indicator of importance. More important RemoteAssetBundles load faster.</param>
        public RemoteAssetBundle(string AssetBundleURL, string AssetBundleName = "", string parentItemURL = "", int priority = 0) : 
            base(AssetBundleURL, AssetBundleName, parentItemURL, priority) { }

        #endregion

        #region override public functions

        /// <summary>
        /// Initiate loading from path file
        /// </summary>
        /// <param name="path">Full path to file</param>
        public override void LoadItemFromFile(string path)
        {
            if (RemoteItemInstance == null)
            {
                AssetBundleCreateRequest aTemp = AssetBundle.LoadFromFileAsync(path);
                LoadedFrom = path;
                aTemp.completed += x =>
                {
                    RemoteItemInstance = aTemp.assetBundle;
                    if (RemoteItemInstance != null)
                    {
                        EventHappend(RemoteLoadableEvent.OnReady, "AssetBundle loaded and ready!");
                        IsReady = true;
                    }
                    else
                    {
                        Debug.LogError("On item " + Name + " bundle is can`t loaded.\nURL -> " + URL);
                        EventHappend(RemoteLoadableEvent.OnError, "Bundle not loaded! Something wrong whis it!");
                    }
                };
            }
        }

        #endregion

    }

}