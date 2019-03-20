using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

namespace MyPlayerControllers
{

    /// <summary>
    /// Class for 
    /// </summary>
    [System.Serializable]
    public class BasePlayerControllerContainer : MonoBehaviour
    {
        /// <summary>
        /// Type of PlayerController
        /// </summary>
        public enum PlayerControllerType
        {
            None,
            VRPlayerController,
            DefaultPlayerController
        }

        public delegate void OnSpawnAction();

        public GameObject PlayerControllerPrefab;
        public PlayerControllerType controllerType;

        protected Camera PlayerCamera;

        public OnSpawnAction spawnAction
        {
            get
            {
                if (_spawnAction != null)
                {
                    return _spawnAction;
                }
                else
                {
                    return () => { Debug.Log("Null delegate from PlayerController was called. Just to let you know."); };
                }
            }
            set
            {
                _spawnAction = value;
            }
        }
        private OnSpawnAction _spawnAction;

        public virtual void Init()
        {
            if(PlayerControllerPrefab == null)
            {
                Debug.LogError("PlayerControllerPrefab is can not be null!");
            }
        }

        protected void ChangePostProcessing(bool canChange)
        {
            if(canChange)
            {
                #region Changing postprocessing

                PlayerCamera = PlayerManager.Instance.CurrentWorkingController.GetComponentInChildren<Camera>();
                if (GameObject.FindGameObjectWithTag("Respawn") != null)
                {
                    if (GameObject.FindGameObjectWithTag("Respawn").GetComponentInChildren<Camera>() != null)
                    {
                        GameObject camera = GameObject.FindGameObjectWithTag("Respawn").GetComponentInChildren<Camera>().gameObject;
                        if (camera.GetComponent<PostProcessingBehaviour>() != null)
                        {
                            if (camera.GetComponent<PostProcessingBehaviour>().profile != null)
                            {
                                PlayerCamera.GetComponent<PostProcessingBehaviour>().profile = camera.GetComponent<PostProcessingBehaviour>().profile;
                                Destroy(camera);
                            }
                            else
                            {
                                Debug.Log("Can't find profile on PostProcessingBehaviour script");
                            }
                        }
                        else
                        {
                            Debug.Log("Can't find PostProcessingScript on scene camera");
                        }
                    }
                    else
                    {
                        Debug.Log("Can't find camera on scene, use player's camera");
                    }
                }
                else
                {
                    Debug.Log("Can't find respawn object on scene");
                }

                #endregion
            }
        }

    }

}
