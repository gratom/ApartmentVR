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

        public List<MenuItem> ListOfStandartItems;

        private Dictionary<MenuLine.TypeOfLine, MenuItem> DictionatyOfStandartItems;

        #region Unity functions

        private void Awake()
        {
            Initialize();
        }

        #endregion

        #region public functions

        public MenuItem GetMenuItem(MenuLine.TypeOfLine typeOfLine, MonoBehaviour AttachedObject)
        {
            MenuItem menuObject = Instantiate(DictionatyOfStandartItems[typeOfLine]);
            if (typeOfLine == MenuLine.TypeOfLine.secondLine)
            {
                menuObject.effectGameObject.transform.GetChild(0).GetComponent<TextMesh>().text = ((LoadedMaterial)AttachedObject).LoadedMaterialName; // ужасный костыль, убрать
            }
            menuObject.typeOfObject = typeOfLine;
            menuObject.AttachedObject = AttachedObject;
            return menuObject;
        }

        #endregion

        #region private function

        private void Initialize()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            DictionatyOfStandartItems = new Dictionary<MenuLine.TypeOfLine, MenuItem>();
            for (int i = 0; i < ListOfStandartItems.Count; i++)
            {
                DictionatyOfStandartItems.Add(ListOfStandartItems[i].typeOfObject, ListOfStandartItems[i]);
            }
        }

        #endregion

    }

}