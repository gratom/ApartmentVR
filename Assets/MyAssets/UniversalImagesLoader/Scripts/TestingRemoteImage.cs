using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniversalImageLoader;

public class TestingRemoteImage : MonoBehaviour {

    public RemoteImage Img1;
    public RemoteImage Img2;
    public RemoteImage Img3;

    // Use this for initialization
    void Start()
    {
        Img1 = new RemoteImage("https://gudhub.com/userdata/2310/22543.png", "Img1.png", "", -1);
        Img1.AddDelegateToEvent(AbstractRemoteLoadable.RemoteLoadable<Texture2D>.RemoteLoadableEvent.OnReady, new RemoteImage.RemoteLoadableDelegate(x => { Debug.Log(Img1.Name + " Loaded!"); }));
        RemoteImageLoaderManager.Instance.AddToLoadStack(Img1);
        Img2 = new RemoteImage("https://gudhub.com/userdata/2310/22543.png", "Img2.png", "", -9);
        Img2.AddDelegateToEvent(AbstractRemoteLoadable.RemoteLoadable<Texture2D>.RemoteLoadableEvent.OnReady, new RemoteImage.RemoteLoadableDelegate(x => { Debug.Log(Img2.Name + " Loaded!"); }));
        RemoteImageLoaderManager.Instance.AddToLoadStack(Img2);
        Img3 = new RemoteImage("https://gudhub.com/userdata/2310/22543.png", "Img3.png", "", 10);
        Img3.AddDelegateToEvent(AbstractRemoteLoadable.RemoteLoadable<Texture2D>.RemoteLoadableEvent.OnReady, new RemoteImage.RemoteLoadableDelegate(x => { Debug.Log(Img3.Name + " Loaded!"); }));
        RemoteImageLoaderManager.Instance.AddToLoadStack(Img3);
    }

}
