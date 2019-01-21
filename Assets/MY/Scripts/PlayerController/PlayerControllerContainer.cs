using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyPlayerControllers
{

    /// <summary>
    /// Class for 
    /// </summary>
    [System.Serializable]
    public class PlayerControllerContainer : MonoBehaviour
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

    }

}
