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

using UnityEngine;
using System.IO;
using AbstractRemoteLoadable;

namespace UniversalImageLoader
{

    /// <summary>
    /// This manager implements the correct boot-sequence of RemoteImage objects
    /// </summary>
    public class RemoteImageLoaderManager : AbstractManagerLoadable<Texture2D> { }

    /// <summary>
    /// Class for Remote Image instances
    /// </summary>
    [System.Serializable]
    public class RemoteImage : RemoteLoadable<Texture2D>
    {

        #region constructor

        /// <summary>
        /// Make initialization RemoteImage from parametrs
        /// </summary>
        /// <param name="RemoteImageURL">RemoteImage URL in GudHub</param>
        /// <param name="RemoteImageName">Name of RemoteImage. If you don`t choose own name, name will be created automaticly from URL</param>
        /// <param name="priority">Priority of RemoteImage. This is an indicator of importance. More important RemoteImage load faster.</param>
        public RemoteImage(string RemoteImageURL, string RemoteImageName = "", string parentItemURL = "", int priority = 0) : base(RemoteImageURL, RemoteImageName, parentItemURL, priority) { }

        #endregion

        #region override public functions

        /// <summary>
        /// Initiate loading from path file
        /// </summary>
        /// <param name="path">Full path to file</param>
        public override void LoadItemFromFile(string path)
        {
            LoadedFrom = path;
            RemoteItemInstance = LoadPNG(LoadedFrom);
            if (RemoteItemInstance != null)
            {
                IsReady = true;
                EventHappend(RemoteLoadableEvent.OnReady, "Image loaded and ready.");
                return;
            }
            IsReady = false;
            EventHappend(RemoteLoadableEvent.OnError, "Image not loaded. Are you sure, that file exist?");
        }

        #endregion

        #region public static functions

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

        #endregion

    }

}
