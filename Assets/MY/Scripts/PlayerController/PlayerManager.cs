using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyPlayerControllers {

    /// <summary>
    /// Class for managing the playerControllers spawning and destroing
    /// </summary>
    public class PlayerManager : MonoBehaviour
    {
        /// <summary>
        /// Singleton
        /// </summary>
        public static PlayerManager Instance { get; private set; }

        /// <summary>
        /// The tag of gameobject in which cordinates will be spawn new controllers
        /// </summary>
        [SerializeField]
        private string TagOfPlayerControllerPlace;

        /// <summary>
        /// List of all controllers that can be spawned
        /// </summary>
        [SerializeField]
        private List<BasePlayerControllerContainer> PlayerControllersList;

        /// <summary>
        /// Current working controller, that spawned and placing on scene
        /// </summary>
        public GameObject CurrentWorkingController { get; private set; }

        /// <summary>
        /// Is there exist any controller
        /// </summary>
        public bool IsAnyControllerExist
        {
            get
            {
                return CurrentWorkingController != null;
            }
        }

        /// <summary>
        /// Type of last spawned controller
        /// </summary>
        public BasePlayerControllerContainer.PlayerControllerType CurrentControllerType
        {
            get
            {
                return _currentControllerType;
            }
            private set
            {
                _currentControllerType = value;
            }
        }
        private BasePlayerControllerContainer.PlayerControllerType _currentControllerType = BasePlayerControllerContainer.PlayerControllerType.None;

        /// <summary>
        /// Position, where is spawned last controller, and where be spanw next, if you called 'SpawnNewPlayerController' function
        /// </summary>
        public Vector3 RespawnControllerPosition
        {
            get; private set;
        }

        /// <summary>
        /// Debug information, nicely edited
        /// </summary>
        public string Information
        {
            get
            {
                return "PlayerManager : \n" +
                    "   Initiated : " + (Instance != null) + "\n" +
                    "   Controller : " + CurrentControllerType.ToString() + "\n" +
                    "   Respawn position : " + RespawnControllerPosition.ToString();
            }
        }

        /// <summary>
        /// Is initialization complete and succesful
        /// </summary>
        public bool isInitComplete { get; private set; }

        #region Unity functions

        private void Awake()
        {
            isInitComplete = Initialize();
        }

        #endregion

        #region public functions

        /// <summary>
        /// Spawn the new PlayerController, type of typeOfNewController in Special place for PlayerController
        /// </summary>
        /// <param name="typeOfNewController">type of new PlayerController, that will be spawned</param>
        /// <param name="isRespawn">Should the function create a new object instead of the old one that could have been created earlier. If its 'true', the old controller will be destroyed</param>
        /// <param name="isUpdateSpawnPosition">Should the function update the position of Special place, in which PlayerControlled will be spawned?(true if yes, false if not) The position will be updated, only if controller will be succesfully created</param>
        /// <returns>Return the new created GameObject of PlayerController, or, if </returns>
        public GameObject SpawnNewPlayerController(BasePlayerControllerContainer.PlayerControllerType typeOfNewController, bool isRespawn = true, bool isUpdateSpawnPosition = true)
        {
            if (isInitComplete)
            {
                for (int i = 0; i < PlayerControllersList.Count; i++)
                {
                    if (PlayerControllersList[i].controllerType == typeOfNewController)
                    {
                        if (CurrentWorkingController == null || isRespawn)
                        {
                            if (isUpdateSpawnPosition)
                            {
                                UpdateSpawnPosition();
                            }
                            DestroyCurrentController();
                            CurrentWorkingController = Instantiate(PlayerControllersList[i].PlayerControllerPrefab,
                                RespawnControllerPosition,
                                PlayerControllersList[i].PlayerControllerPrefab.transform.rotation);
                            PlayerControllersList[i].spawnAction();
                            CurrentControllerType = typeOfNewController;
                        }
                        else
                        {
                            Debug.Log("New player controller will not spawned, because the old one is exist, and 'isRespawn' = false");
                        }
                        break;
                    }
                }
                return CurrentWorkingController;
            }
            throw new System.Exception("You try to work with non-initialized module:" + name + ".");
        }

        /// <summary>
        /// Destroying current controller, is it exist
        /// </summary>
        public void DestroyCurrentController()
        {
            if(CurrentWorkingController != null)
            {
                Destroy(CurrentWorkingController);
                CurrentWorkingController = null;
                CurrentControllerType = BasePlayerControllerContainer.PlayerControllerType.None;
            }
        }

        /// <summary>
        /// Updating the spawnpoint for Any controllers, if it exist
        /// </summary>
        public void UpdateSpawnPosition()
        {
            GameObject specialPlaceForControllers = GameObject.FindGameObjectWithTag(TagOfPlayerControllerPlace);
            if (specialPlaceForControllers != null)
            {
                RespawnControllerPosition = specialPlaceForControllers.transform.position;
            }
            else
            {
                Debug.Log("The player spawn point is not found. Are you sure, that is right?\nThe steps to fix this problem:\n" +
                "   1). Load the scene in redactor.\n" +
                "   2). Create new GameObject. (Right mouse click on scene Hierarchy -> Create Empty)\n" +
                "   3). Choose your created GameObject and change his: Tag -> 'Respawn'.\n" +
                "   4). Save changes in scene.\n" +
                "   5). Make AssetBundle from this scene.\n" +
                "   6). Load created AssetBundle in GudHub, or change the old file of scene in local folder, that named 'LoadedAssetBundles'.\n" +
                "   7). Great. You amazing!");
                RespawnControllerPosition = new Vector3(0, 0, 0);
            }
        }

        #endregion

        #region private functions

        private bool Initialize()
        {            
            if (Instance == null)
            {
                Instance = this;
                if (PlayerControllersList != null)
                {
                    for(int i = 0; i < PlayerControllersList.Count; i++)
                    {
                        if (PlayerControllersList[i] != null)
                        {
                            PlayerControllersList[i].Init();
                        }
                        else
                        {
                            return false;
                        }
                    }
                    UpdateSpawnPosition();
                    return true;                    
                }
            }
            return false;
        }

        #endregion

    }

}