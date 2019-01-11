using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniversalAssetBundleLoader;

public class ChooseSceneManager : MonoBehaviour
{

    [Tooltip("This prefab used for spawning new items typeof SceneObject")]
    [Header("Prefab for SceneObjects")]
    [SerializeField]
    private GameObject sceneObjectPrefab;

    [SerializeField]
    private string[] ignoredScenes;

    [SerializeField]
    private bool IsBuildFromProjectScenes;

    [SerializeField]
    private bool IsBuildFromDiskScenes;

    private List<AbstractObjectConstructable<SceneObjectTypes>> UncashedAppDataOfSceneObjects;

    private void Awake()
    {
        Initialize();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            Application.Quit();
        }
    }

    private void Initialize()
    {
        
        #region Scenes from JSON
        //третье приложение это приложение с данными сцен
        UncashedAppDataOfSceneObjects = new List<AbstractObjectConstructable<SceneObjectTypes>>();
        for (int i = 0; i < JSONMainManager.Instance.AppDataLoaderInstance.ListOfAppsSetting[2].AppData.items_list.Count; i++)
        {
            //создаем тут итемы, по количеству в App
            GameObject gTemp = Instantiate(sceneObjectPrefab, this.transform);
            gTemp.name = "SceneObject" + i.ToString();
            gTemp.transform.position = new Vector3(i, 1.5f, 2);
            UncashedAppDataOfSceneObjects.Add(gTemp.GetComponent<SceneObject>());
            UncashedAppDataOfSceneObjects[i].InitDictionary();
        }

        JSONMainManager.Instance.FillDataToList(UncashedAppDataOfSceneObjects, JSONMainManager.Instance.AppDataLoaderInstance.ListOfAppsSetting[2].AppID);

        for (int i = 0; i < UncashedAppDataOfSceneObjects.Count; i++)
        {
            UncashedAppDataOfSceneObjects[i].InitConstruct();            
            int j = i;
            ((SceneObject)UncashedAppDataOfSceneObjects[i]).RemoteImageInstance.AddDelegateToEvent(AbstractRemoteLoadable.RemoteLoadable<Texture2D>.RemoteLoadableEvent.OnReady,
                x =>
                {
                    UncashedAppDataOfSceneObjects[j].gameObject.GetComponent<MeshRenderer>().material = new Material(UncashedAppDataOfSceneObjects[j].gameObject.GetComponent<MeshRenderer>().material);
                    UncashedAppDataOfSceneObjects[j].gameObject.GetComponent<MeshRenderer>().material.mainTexture = ((SceneObject)UncashedAppDataOfSceneObjects[j]).RemoteImageInstance.RemoteItemInstance;
                });
            ((SceneObject)UncashedAppDataOfSceneObjects[i]).RemoteImageInstance.StartLoad(UniversalImageLoader.RemoteImageLoaderManager.Instance);
            UncashedAppDataOfSceneObjects[i].gameObject.AddComponent<SimpleAction>().simpleActionDelegate = new SimpleAction.SimpleActionDelegate(() =>
            {
                ((SceneObject)UncashedAppDataOfSceneObjects[j]).RemoteAssetBundleInstance.AddDelegateToEvent(AbstractRemoteLoadable.RemoteLoadable<AssetBundle>.RemoteLoadableEvent.OnReady,
                    x =>
                    {                        
                        string[] scenePaths = ((SceneObject)UncashedAppDataOfSceneObjects[j]).RemoteAssetBundleInstance.RemoteItemInstance.GetAllScenePaths();
                        string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePaths[0]);
                        AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
                        asyncOperation.completed += y =>
                        {
                            Loader.Instance.AllManagersForScene.SetActive(true);
                            ((SceneObject)UncashedAppDataOfSceneObjects[j]).RemoteAssetBundleInstance.RemoteItemInstance.Unload(false);
                            SceneLoaderManager.Instance.LoadSceneObjects();                            
                        };                        
                    });
                StartCoroutine(LoadingVisualizerCoroutine(((SceneObject)UncashedAppDataOfSceneObjects[j])));
                ((SceneObject)UncashedAppDataOfSceneObjects[j]).RemoteAssetBundleInstance.StartLoad(AssetBundleLoaderManager.Instance);
            });
        }
        #endregion

        #region Scene from project

        if (IsBuildFromProjectScenes)
        {
            int iRealPos = 0;
            for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings; i++)
            {
                if (IsIgnore(NameFromIndex(i)))
                {
                    continue;
                }
                else
                {
                    //create object
                    GameObject gTemp = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    gTemp.name = "SceneObject_" + NameFromIndex(i);
                    gTemp.transform.localScale *= 0.3f;
                    gTemp.transform.position = new Vector3(iRealPos, 0.5f, 2);
                    GameObject gTemp2 = new GameObject("text");
                    gTemp2.transform.parent = gTemp.transform;
                    gTemp2.transform.localPosition = new Vector3(0, 0, -0.5f);
                    TextMesh tempText = gTemp2.AddComponent<TextMesh>();
                    tempText.text = NameFromIndex(i);
                    tempText.anchor = TextAnchor.MiddleCenter;
                    tempText.characterSize = 0.3f;
                    gTemp2.transform.localScale *= 0.3f;
                    iRealPos++;
                    //attach the delegate
                    int j = i;
                    gTemp.AddComponent<SimpleAction>().simpleActionDelegate = new SimpleAction.SimpleActionDelegate(() =>
                    {
                        AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(NameFromIndex(j));
                        asyncOperation.completed += y =>
                        {                            
                            Loader.Instance.AllManagersForScene.SetActive(true);
                            SceneLoaderManager.Instance.LoadSceneObjects();
                        };
                    });
                }
            }
        }
        #endregion

        #region Scenes from disk

        if (IsBuildFromDiskScenes)
        {

            //get all scenes from disk (bundled)

            //spawn all

            //attach the delegate

        }

        #endregion

        #region Creating the SceneClickTracker

        new GameObject("SimpleTracker").AddComponent<SceneClickTracker>();

        #endregion

    }

    private void AsyncOperation_completed(AsyncOperation obj)
    {
        throw new System.NotImplementedException();
    }

    private bool IsIgnore(string nameOfScene)
    {
        for (int i = 0; i < ignoredScenes.Length; i++)
        {
            if (nameOfScene == ignoredScenes[i])
            {
                return true;
            }
        }
        return false;
    }

    private IEnumerator LoadingVisualizerCoroutine(SceneObject sceneObject)
    {
        while (true)
        {
            yield return null;
            if (sceneObject.RemoteAssetBundleInstance.loadingOperation != null)
            {
                sceneObject.textMestName.text = Mathf.RoundToInt(sceneObject.RemoteAssetBundleInstance.loadingOperation.progress * 100).ToString() + "%";
                if (sceneObject.RemoteAssetBundleInstance.loadingOperation.isDone)
                {
                    sceneObject.textMestName.text = "Scene fully loaded. Wait a couple of seconds...";
                    break;
                }
            }
        }
    }

    private string NameFromIndex(int BuildIndex)
    {
        string path = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(BuildIndex);
        int slash = path.LastIndexOf('/');
        string name = path.Substring(slash + 1);
        int dot = name.LastIndexOf('.');
        return name.Substring(0, dot);
    }

}
