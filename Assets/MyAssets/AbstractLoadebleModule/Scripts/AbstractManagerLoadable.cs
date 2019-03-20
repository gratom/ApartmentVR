using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace AbstractRemoteLoadable
{


    public abstract class AbstractManagerLoadable<RemoteType> : MonoBehaviour
    {

        /// <summary>
        /// Singleton
        /// </summary>
        public static AbstractManagerLoadable<RemoteType> Instance { get; protected set; }

        /// <summary>
        /// Setting for this class
        /// </summary>
        public AbstractManagerLoadableSettings Setting;

        private List<LoadingTread<RemoteType>> LoadingTreads;

        #region Unity functions

        private void Awake()
        {
            Init();
        }

        #endregion

        #region public functions

        /// <summary>
        /// Adding current RemoteImage in 
        /// </summary>
        /// <param name="remoteImage"></param>
        public void AddToLoadStack(RemoteLoadable<RemoteType> remoteImage)
        {
            
            int num = 0;
            int MinCount = int.MaxValue;
            for (int i = 0; i < LoadingTreads.Count; i++)
            {
                if (LoadingTreads[i].LoadingStack.Count < MinCount)
                {
                    MinCount = LoadingTreads[i].LoadingStack.Count;
                    num = i;
                }
            }
            LoadingTreads[num].LoadingStack.Add(remoteImage);
        }

        #endregion

        #region private functions

        private void StartAllTreads()
        {
            for (int i = 0; i < LoadingTreads.Count; i++)
            {
                LoadingTreads[i].treadImitator = StartCoroutine(LoadingCorounite(LoadingTreads[i]));
            }
        }

        private void Init()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            LoadingTreads = new List<LoadingTread<RemoteType>>();

            for (int i = 0; i < Setting.MaxCountOfLoadingTread; i++)
            {
                LoadingTreads.Add(new LoadingTread<RemoteType>());
                LoadingTreads[i].LoadingStack = new List<RemoteLoadable<RemoteType>>();
            }

            if (Setting.Init())
            {
                StartAllTreads();
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogWarning("LoaderManager is non-initialized! -> " + this.GetType().ToString());
#endif
            }
        }

        #endregion

        #region coroutines

        private IEnumerator LoadingCorounite(LoadingTread<RemoteType> treadInstance)
        {
            while (true)
            {
                if (treadInstance.LoadingStack.Count > 0 && Time.unscaledDeltaTime < (1.0f / Setting.MinFrameRateForLoading))
                {
                    //find the maximum priority
                    int index = 0;
                    int MaxPriority = int.MinValue;
                    for (int i = 0; i < treadInstance.LoadingStack.Count; i++)
                    {
                        if (treadInstance.LoadingStack[i].Priority > MaxPriority) //TODO сделать интерфейс, в него вложить приоритет. Все наследники будут автоматически брать себе все необходимые свойста
                        {
                            MaxPriority = treadInstance.LoadingStack[i].Priority;
                            index = i;
                        }
                    }

                    //check on disk
                    string filePath = Setting.FileLocalPathFromName(treadInstance.LoadingStack[index].Name);

                    if (!File.Exists(filePath))
                    {
                        //make request
                        WWW www = new WWW(treadInstance.LoadingStack[index].URL);
                        yield return www;
                        if (!File.Exists(filePath))
                        {
                            FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate);
                            BinaryWriter bw = new BinaryWriter(fs);
                            bw.Write(www.bytes);
                            bw.Close();
                            fs.Close();
                        }
                    }
                    yield return new WaitForSecondsRealtime(Random.Range(0f, Setting.MaxCountOfLoadingTread / 5f)); //TODO по возможности исправить этот элегантный костыль
                    treadInstance.LoadingStack[index].LoadItemFromFile(filePath);
                    treadInstance.LoadingStack.RemoveAt(index);                    
                }
                else
                {
                    yield return null;
                }
            }
        }

        #endregion

    }

    /// <summary>
    /// Class for Remote Items instances
    /// </summary>
    [System.Serializable]
    public abstract class RemoteLoadable<RemoteItemType>
    {

        /// <summary>
        /// Types of events
        /// </summary>
        public enum RemoteLoadableEvent
        {
            OnReady,
            OnStartLoad,
            OnError
        }

        /// <summary>
        /// Type of delegate, that you can attach to current events in this class
        /// </summary>
        /// <param name="param">paramets of events</param>
        public delegate void RemoteLoadableDelegate(string param);

        /// <summary>
        /// Full path URL in GudHub
        /// </summary>
        public string URL
        {
            get
            {
                return _URL;
            }
            protected set
            {
                _URL = value;
            }
        }
        [SerializeField]
        private string _URL;

        /// <summary>
        /// Full path parent item URL in GudHub
        /// </summary>
        public string ParentItemURL
        {
            get
            {
                return _ParentItemURL;
            }
            protected set
            {
                _ParentItemURL = value;
            }
        }
        [SerializeField]
        private string _ParentItemURL;

        /// <summary>
        /// Name of this RemoteItem in local file system
        /// </summary>
        public string Name
        {
            get
            {
                return _Name;
            }
            protected set
            {
                _Name = value;
            }
        }
        [SerializeField]
        private string _Name;

        /// <summary>
        /// The full path to file of this RemoteItem
        /// </summary>
        public string LoadedFrom
        {
            get
            {
                return _LoadedFrom;
            }
            protected set
            {
                _LoadedFrom = value;
            }
        }
        [SerializeField]
        private string _LoadedFrom;

        /// <summary>
        /// Is RemoteItem loaded to RemoteItemInstance
        /// </summary>
        public bool IsReady
        {
            get
            {
                return _IsReady;
            }
            protected set
            {
                _IsReady = value;
            }
        }
        [SerializeField]
        private bool _IsReady;

        /// <summary>
        /// Priority of RemoteItem. This is an indicator of importance. More important RemoteItems load faster.
        /// </summary>
        public int Priority
        {
            get
            {
                return _Priority;
            }
            set
            {
                _Priority = value;
            }
        }
        [SerializeField]
        private int _Priority;

        protected bool isLoading = false;

        /// <summary>
        /// Loaded RemoteItem instance
        /// </summary>
        public RemoteItemType RemoteItemInstance
        {
            get
            {
                return _RemoteItemInstance;
            }
            protected set
            {
                _RemoteItemInstance = value;
            }
        }

        public AsyncOperation loadingOperation
        {
            get
            {
                return _loadingOperation;
            }
            set
            {
                _loadingOperation = value;
            }
        }
        protected AsyncOperation _loadingOperation;


        [SerializeField]
        private RemoteItemType _RemoteItemInstance;

        private Dictionary<RemoteLoadableEvent, List<RemoteLoadableDelegate>> EventDictionary;
        

        #region constructor

        /// <summary>
        /// Make initialization from parametrs
        /// </summary>
        /// <param name="ItemURL">Item URL in GudHub</param>
        /// <param name="ItemName">Name of item. If you don`t choose own name, name will be created automaticly from URL</param>
        /// <param name="priority">Priority of item. This is an indicator of importance. More important items load faster.</param>
        public RemoteLoadable(string ItemURL, string ItemName = "", string parentItemURL = "", int priority = 0)
        {
            if (ItemURL == "")
            {
                throw new System.ArgumentException("URL can not be null.", "ItemURL");
            }
            ParentItemURL = parentItemURL;
            URL = ItemURL;
            if (ItemName != "")
            {
                Name = ItemName;
            }
            else
            {
                Name = URL.Substring(URL.LastIndexOf('/') + 1);
#if UNITY_EDITOR
                Debug.LogWarning("Name was created automaticly from URL. Are you sure, that is right?\n Name : " + Name);
#endif
            }
            IsReady = false;
            Priority = priority;
            InitDictionary();
        }

        #endregion

        #region public functions

        /// <summary>
        /// Initiate loading from path file
        /// </summary>
        /// <param name="path">Full path to file</param>
        public abstract void LoadItemFromFile(string path);

        public virtual void CopyItemFrom(RemoteLoadable<RemoteItemType> originalRemoteItem, AbstractManagerLoadable<RemoteItemType> instanceOfLoadManager)
        {
            RemoteLoadableDelegate tempDelegate = new RemoteLoadableDelegate(x => { });
            tempDelegate = new RemoteLoadableDelegate(x =>
            {
                RemoteItemInstance = originalRemoteItem.RemoteItemInstance;
                originalRemoteItem.RemoveDelegateFromEvent(RemoteLoadableEvent.OnReady, tempDelegate);
                IsReady = true;
                EventHappend(RemoteLoadableEvent.OnReady, "Item copied from " + originalRemoteItem.Name);
            });
            originalRemoteItem.AddDelegateToEvent(RemoteLoadableEvent.OnReady, tempDelegate);
            isLoading = true;
            originalRemoteItem.StartLoad(instanceOfLoadManager);
        }

        /// <summary>
        /// Add delegate function to current event
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="functionInstance"></param>
        public void AddDelegateToEvent(RemoteLoadableEvent eventType, RemoteLoadableDelegate functionInstance)
        {
            EventDictionary[eventType].Add(functionInstance);
        }

        /// <summary>
        /// Removed delegate functions from current event
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="functionInstance"></param>
        public void RemoveDelegateFromEvent(RemoteLoadableEvent eventType, RemoteLoadableDelegate functionInstance)
        {
            if (EventDictionary[eventType].Contains(functionInstance))
            {
                EventDictionary[eventType].Remove(functionInstance);
            }
        }

        /// <summary>
        /// You can use this function, for object selfadded it to loadStack
        /// </summary>
        public virtual void StartLoad(AbstractManagerLoadable<RemoteItemType> instanceOfLoadManager)
        {
            if (instanceOfLoadManager != null)
            {
                if (!IsReady)
                {
                    if (!isLoading)
                    {
                        isLoading = true;
                        instanceOfLoadManager.AddToLoadStack(this);
                        EventHappend(RemoteLoadableEvent.OnStartLoad, "Start load item from URL:\n" + URL + "\nwith priority : " + Priority.ToString());
                    }
                }
                else
                {
                    //RemoteItemInstance already loaded
                    //call ready event
                    EventHappend(RemoteLoadableEvent.OnReady, "Item already loaded\nName : " + Name);
                }
            }
        }

        #endregion

        #region private functions

        private void InitDictionary()
        {
            EventDictionary = new Dictionary<RemoteLoadableEvent, List<RemoteLoadableDelegate>>();
            for (int i = 0; i < System.Enum.GetNames(typeof(RemoteLoadableEvent)).Length; i++)
            {
                EventDictionary.Add((RemoteLoadableEvent)i, new List<RemoteLoadableDelegate>());
            }
        }

        #endregion

        #region protected functions

        /// <summary>
        /// Event calling functions, that calls, when any of event are happend
        /// </summary>
        /// <param name="eventType">Type of happend event</param>
        /// <param name="param">String parametr of happend evemt</param>
        protected void EventHappend(RemoteLoadableEvent eventType, string param)
        {
            if (EventDictionary[eventType].Count > 0)
            {
                #region coping
                List<RemoteLoadableDelegate> tempExecutionList = new List<RemoteLoadableDelegate>();
                for (int i = 0; i < EventDictionary[eventType].Count; i++)
                {
                    tempExecutionList.Add(EventDictionary[eventType][i]);
                }
                #endregion
                #region execution
                for (int i = 0; i < tempExecutionList.Count; i++)
                {
                    tempExecutionList[i](param);
                }
                #endregion
            }
        }

        #endregion

    }

    /// <summary>
    /// Class for imitate treads
    /// </summary>
    public class LoadingTread<RemoteType>
    {

        /// <summary>
        /// Coroitine instance, in which tread working
        /// </summary>
        public Coroutine treadImitator;

        /// <summary>
        /// Stack of loading images
        /// </summary>
        public List<RemoteLoadable<RemoteType>> LoadingStack;

    }

    /// <summary>
    /// Setting class for ImageLoaderManager
    /// </summary>
    [System.Serializable]
    public class AbstractManagerLoadableSettings
    {

        [SerializeField]
        [Tooltip("The folder in which all downloaded items are saved")]
        /// <summary>
        /// The folder in which all downloaded items are saved
        /// </summary>
        private string _DataStoragePath;

        /// <summary>
        /// The folder in which all downloaded items are saved
        /// </summary>
        public string DataStoragePath
        {
            get
            {
                return _DataStoragePath;
            }
        }
                
        /// <summary>
        /// The minimum FPS for start loading next RemoteLoadable
        /// </summary>
        public float MinFrameRateForLoading
        {
            get
            {
                return _minFrameRateForLoading;
            }
        }
        [Range(25, 120)]
        [SerializeField]
        private float _minFrameRateForLoading;

        [Header("Count of loading treads")]
        [Tooltip("Maximum number of threads involved. Some of these threads may not be used if the number of downloaded Images is less than the number of threads.")]
        [Range(1, 15)]
        [SerializeField]
        /// <summary>
        /// Maximum number of threads involved.
        /// </summary>
        private int _MaxCountOfLoadingTread;        

        /// <summary>
        /// Maximum number of threads involved.
        /// </summary>
        public int MaxCountOfLoadingTread
        {
            get
            {
                return _MaxCountOfLoadingTread;
            }
        }

        /// <summary>
        /// Permanent app path, that initiated when module is initiated. The path of app data
        /// </summary>
        public string AppPath { get; private set; }

        #region public functions

        /// <summary>
        /// Get full local path from fileName
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public string FileLocalPathFromName(string FileName)
        {
            return AppPath + FileName;
        }

        /// <summary>
        /// Initiate setting module
        /// </summary>
        /// <returns>Has initialization passed successfully (return true, if succesfull, false - if not)</returns>
        public bool Init()
        {
            if (DataStoragePath != "")
            {
                try
                {
                    AppPath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/")) + DataStoragePath;
                    if (!Directory.Exists(AppPath))
                    {
                        Directory.CreateDirectory(AppPath);
                    }
                    return true;
                }
                catch (System.Exception e)
                {
                    Debug.Log(e.ToString());
                }
            }
            Debug.LogWarning("path is null");
            return false;
        }

        #endregion
    }

}