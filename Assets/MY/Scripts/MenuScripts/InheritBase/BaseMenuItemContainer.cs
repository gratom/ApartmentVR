using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyVRMenu
{
    /// <summary>
    /// Class for pre-initialize custom menu item
    /// </summary>
    [System.Serializable]
    public class BaseMenuItemContainer : MonoBehaviour
    {
        /// <summary>
        /// Unique name of this container
        /// </summary>
        public string ContainerName;

        /// <summary>
        /// Instance of menuItem that will be intialized
        /// </summary>
        public MenuItem menuItem;

        /// <summary>
        /// Standart function to should be called to initialize 
        /// </summary>
        public virtual MenuItem Init(MonoBehaviour AttachedObject)
        {
            MenuItem copyOfMenuItem = Instantiate(menuItem);
            copyOfMenuItem.AttachedObject = AttachedObject;
            copyOfMenuItem.name = ContainerName;
            return copyOfMenuItem;
        }
    }
}