/* author : Volkov Alexander (gratom)
 * email for any questions : gratomov@gmail.com
 * created in 2018 y.
 * 
 * Class created for easy loading any images from GudHub.com
 *   
 * if you want to use it, just add a class "RemoteImage" object to your class. 
 * Make sure the Download Manager is exist on the scene. Set up the Download Manager. 
 * When you need it, add your RemoteImage object to the download queue using the function
 * 
 * code example:
 * -> RemoteImageLoaderManager.Instance.AddToLoadStack(YouObject, "Wanted name of object on disk", int PriorityLoading);
 * 
 * You can add as many RemoteImages to the queue as you like. You can also add a delegate to events in RemoteImage object,
 * and, rof example, when image was loaded it already can spawn on scene, or do something else.
 * 
 * For more features and specifications read the classes documentation. 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace UniversalImageLoader
{

    /// <summary>
    /// This manager implements the correct boot-sequence of RemoteImage objects
    /// </summary>
    public class RemoteImageLoaderManager : MonoBehaviour
    {

        /// <summary>
        /// Singleton
        /// </summary>
        public static RemoteImageLoaderManager Instance { get; private set; }

        /// <summary>
        /// Setting for this class
        /// </summary>
        public ImageLoaderManagerSetting Setting;

        private List<LoadingTread> LoadingTreads;

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
        public void AddToLoadStack(RemoteImage remoteImage)
        {
            int num = 0;
            int MinCount = int.MaxValue;
            for(int i = 0; i < LoadingTreads.Count; i++)
            {
                if(LoadingTreads[i].LoadingStack.Count < MinCount)
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

            LoadingTreads = new List<LoadingTread>();

            for (int i = 0; i < Setting.MaxCountOfLoadingTread; i++)
            {
                LoadingTreads.Add(new LoadingTread());
                LoadingTreads[i].LoadingStack = new List<RemoteImage>();
            }

            if (Setting.Init())
            {
                StartAllTreads();
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogWarning("ImageLoaderManager is non-initialized!");
#endif
            }
        }

#endregion

#region coroutines

        private IEnumerator LoadingCorounite(LoadingTread treadInstance)
        {
            while (true)
            {
                if(treadInstance.LoadingStack.Count > 0)
                {
                    //find the maximum priority
                    int index = 0;
                    int MaxPriority = int.MinValue;
                    for(int i = 0; i < treadInstance.LoadingStack.Count; i++)
                    {
                        if(treadInstance.LoadingStack[i].Priority > MaxPriority)
                        {
                            MaxPriority = treadInstance.LoadingStack[i].Priority;
                            index = i;
                        }
                    }

                    treadInstance.LoadingStack[index].StartLoad();

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
                    treadInstance.LoadingStack[index].LoadImageFromFile(filePath);
                    treadInstance.LoadingStack.RemoveAt(index);

                }
                else
                {
                    yield return new WaitForSeconds(0.2f);
                }
            }
        }

#endregion

    }

    /// <summary>
    /// Class for Remote
    /// </summary>
    [System.Serializable]
    public class RemoteImage
    {

        /// <summary>
        /// Types of events
        /// </summary>
        public enum RemoteImageEvent
        {
            OnReady,
            OnStartLoad,
            OnError
        }

        /// <summary>
        /// Type of delegate, that you can attach to current events in this class
        /// </summary>
        /// <param name="param">paramets of events</param>
        public delegate void RemoteImageDelegate(string param);

        /// <summary>
        /// Full path URL in GudHub
        /// </summary>
        public string URL { get; private set; }

        /// <summary>
        /// Name of this image in local file system
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The full path to file of this texture
        /// </summary>
        public string LoadedFrom { get; private set; }

        /// <summary>
        /// Is image loaded to ImageInstance
        /// </summary>
        public bool IsReady { get; private set; }

        /// <summary>
        /// Priority of image. This is an indicator of importance. More important pictures load faster.
        /// </summary>
        public int Priority;

        /// <summary>
        /// Loaded image instance
        /// </summary>
        public Texture2D ImageInstance
        {
            get
            {
                return _ImageInstance;
            }
            private set
            {
                _ImageInstance = value;
            }
        }

        [SerializeField]
        private Texture2D _ImageInstance;

        private Dictionary<RemoteImageEvent, List<RemoteImageDelegate>> EventDictionary;

        #region constructor

        /// <summary>
        /// Make initialization from parametrs
        /// </summary>
        /// <param name="ImageURL">Image URL in GudHub</param>
        /// <param name="ImageName">Name of image. If you don`t choose own name, name will be created automaticly from URL</param>
        /// <param name="priority">Priority of image. This is an indicator of importance. More important pictures load faster.</param>
        public RemoteImage(string ImageURL, string ImageName = "", int priority = 0)
        {
            URL = ImageURL;
            if (ImageName != "")
            {
                Name = ImageName;
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
        /// Load PNG from path file
        /// </summary>
        /// <param name="filePath">Full path to png-file</param>
        /// <returns>Texture vs png image from file</returns>
        public static Texture2D LoadPNG(string filePath)
        {
            Texture2D tex = null;
            byte[] fileData;

            if (File.Exists(filePath))
            {
                fileData = File.ReadAllBytes(filePath);
                tex = new Texture2D(2, 2);
                tex.LoadImage(fileData);
            }
            return tex;
        }

        /// <summary>
        /// Add delegate function to current event
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="functionInstance"></param>
        public void AddDelegateToEvent(RemoteImageEvent eventType, RemoteImageDelegate functionInstance)
        {
            EventDictionary[eventType].Add(functionInstance);
        }

        /// <summary>
        /// Removed delegate functions from current event
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="functionInstance"></param>
        public void RemoveDelegateFromEvent(RemoteImageEvent eventType, RemoteImageDelegate functionInstance)
        {
            if (EventDictionary[eventType].Contains(functionInstance))
            {
                EventDictionary[eventType].Remove(functionInstance);
            }
        }

        /// <summary>
        /// Initiate loading from path file
        /// </summary>
        /// <param name="path">Full path to file</param>
        public void LoadImageFromFile(string path)
        {
            LoadedFrom = path;
            ImageInstance = LoadPNG(LoadedFrom);
            if(ImageInstance != null)
            {
                IsReady = true;
                EventHappend(RemoteImageEvent.OnReady, "Image loaded and ready.");
                return;
            }
            IsReady = false;
            EventHappend(RemoteImageEvent.OnError, "Image not loaded. Are you sure, that file exist?");
        }

        /// <summary>
        /// Function, that will be called on start loading
        /// </summary>
        public void StartLoad()
        {
            EventHappend(RemoteImageEvent.OnStartLoad, "Start load image from URL:\n" + URL + "\nwith priority : " + Priority.ToString());
        }

#endregion

#region private functions

        private void InitDictionary()
        {
            EventDictionary = new Dictionary<RemoteImageEvent, List<RemoteImageDelegate>>();
            for (int i = 0; i < System.Enum.GetNames(typeof(RemoteImageEvent)).Length; i++)
            {
                EventDictionary.Add((RemoteImageEvent)i, new List<RemoteImageDelegate>());
            }
        }

        private void EventHappend(RemoteImageEvent eventType, string param)
        {
            if (EventDictionary[eventType].Count > 0)
            {
#region coping
                List<RemoteImageDelegate> tempExecutionList = new List<RemoteImageDelegate>();
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
    public class LoadingTread
    {

        /// <summary>
        /// Coroitine instance, in which tread working
        /// </summary>
        public Coroutine treadImitator;

        /// <summary>
        /// Stack of loading images
        /// </summary>
        public List<RemoteImage> LoadingStack;
    }

    /// <summary>
    /// Setting class for ImageLoaderManager
    /// </summary>
    [System.Serializable]
    public class ImageLoaderManagerSetting
    {

        [SerializeField]
        [Tooltip("The folder in which all downloaded Images are saved")]
        /// <summary>
        /// The folder in which all downloaded Images are saved
        /// </summary>
        private string _DataStoragePath;

        /// <summary>
        /// The folder in which all downloaded Images are saved
        /// </summary>
        public string DataStoragePath
        {
            get
            {
                return _DataStoragePath;
            }
        }

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
