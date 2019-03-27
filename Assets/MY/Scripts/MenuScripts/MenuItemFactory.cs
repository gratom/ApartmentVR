using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyVRMenu
{

    /// <summary>
    /// Class for creating special type of MenuItems
    /// </summary>
    public class MenuItemFactory : MonoBehaviour
    {

        /// <summary>
        /// Singleton
        /// </summary>
        public static MenuItemFactory Instance { get; private set; }

        public List<BaseMenuItemContainer> ListOfStandartItems;

        private Dictionary<string, BaseMenuItemContainer> DictionatyOfStandartItems;

        private bool IsReady;

        #region Unity functions

        private void Awake()
        {
            Initialize();
        }

        #endregion

        #region public functions

        public MenuItem GetMenuItem(string ContainerName, MonoBehaviour AttachedObject)
        {
            if (IsReady)
            {
                if (DictionatyOfStandartItems.ContainsKey(ContainerName))
                {
                    return DictionatyOfStandartItems[ContainerName].Init(AttachedObject);
                }
                Debug.LogError("MenuItem with this name is not exist. Name : " + ContainerName);
                return null;
            }
            Debug.LogError("You tried to use not ready MenuItemFactory class");
            return null;
        }

        #endregion

        #region private function

        private void Initialize()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            DictionatyOfStandartItems = new Dictionary<string, BaseMenuItemContainer>();
            for (int i = 0; i < ListOfStandartItems.Count; i++)
            {
                if (!DictionatyOfStandartItems.ContainsKey(ListOfStandartItems[i].ContainerName))
                {
                    DictionatyOfStandartItems.Add(ListOfStandartItems[i].ContainerName, ListOfStandartItems[i]);
                }
                else
                {
                    Debug.LogError("Founded two container with similar names. Change one of this names!");
                    return;
                }
            }
            IsReady = true;
        }

        #endregion

    }

}