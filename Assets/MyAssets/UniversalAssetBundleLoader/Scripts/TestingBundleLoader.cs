using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniversalAssetBundleLoader;

public class TestingBundleLoader : MonoBehaviour {
    
    public RemoteAssetBundle bnd1;
    public RemoteAssetBundle bnd2;
    public RemoteAssetBundle bnd3;

    // Use this for initialization
    void Start()
    {
        bnd1 = new RemoteAssetBundle("https://gudhub.com/userdata/11487/44551.drsamsung01dw80j3020usblack", "bnd1", "", -1);
        bnd1.AddDelegateToEvent(AbstractRemoteLoadable.RemoteLoadable<AssetBundle>.RemoteLoadableEvent.OnReady, new AbstractRemoteLoadable.RemoteLoadable<AssetBundle>.RemoteImageDelegate(x => 
        {
            Debug.Log(bnd1.Name + " Loaded!");
        }));
        UniversalAssetBundleLoader.AssetBundleLoaderManager.Instance.AddToLoadStack(bnd1);

        bnd2 = new RemoteAssetBundle("https://gudhub.com/userdata/11487/44551.drsamsung01dw80j3020usblack", "Bundle 2", "", 5);
        bnd2.AddDelegateToEvent(AbstractRemoteLoadable.RemoteLoadable<AssetBundle>.RemoteLoadableEvent.OnReady, new AbstractRemoteLoadable.RemoteLoadable<AssetBundle>.RemoteImageDelegate(x =>
        {
            Debug.Log(bnd2.Name + " Loaded!");
        }));
        UniversalAssetBundleLoader.AssetBundleLoaderManager.Instance.AddToLoadStack(bnd2);

        bnd3 = new RemoteAssetBundle("https://gudhub.com/userdata/11487/44551.drsamsung01dw80j3020usblack", "Asset 3", "", -10);
        bnd3.AddDelegateToEvent(AbstractRemoteLoadable.RemoteLoadable<AssetBundle>.RemoteLoadableEvent.OnReady, new AbstractRemoteLoadable.RemoteLoadable<AssetBundle>.RemoteImageDelegate(x =>
        {
            Debug.Log(bnd3.Name + " Loaded!");
        }));
        UniversalAssetBundleLoader.AssetBundleLoaderManager.Instance.AddToLoadStack(bnd3);

    }
}
