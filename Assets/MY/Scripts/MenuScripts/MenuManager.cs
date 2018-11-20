﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyVRMenu
{

    /// <summary>
    /// Inteface for clickable object
    /// </summary>
    public interface ISceneClickable
    {
        List<MenuItem> GetListOfMenuObject();
    }

    /// <summary>
    /// inteface for every object that will be shown on menu
    /// </summary>
    public interface IMenuClickable
    {

        void OnMenuClick(MonoBehaviour itemInstance);

        void OnPointFunction(MenuItem menuItem, bool isPointed);

    }

    /// <summary>
    /// Class for menu object. All menu items are such classes
    /// </summary>
    public class MenuItem : MonoBehaviour
    {

        public delegate void OnClickDelegate(MonoBehaviour item);

        public delegate void OnPoint(MenuItem menuItem, bool isPointed);

        public MenuLine.TypeOfLine typeOfObject;
        public OnClickDelegate onClick;
        public OnPoint onPoint;
        public float CurrentRotation;
        public MonoBehaviour ItemInstance { get; private set; }
        public LineEffects Illumination;

        public void SetMenuObject(MenuLine.TypeOfLine menuItemType, MonoBehaviour referenceItem)
        {
            typeOfObject = menuItemType;
            ItemInstance = referenceItem;
            onClick = ((IMenuClickable)referenceItem).OnMenuClick;
            onPoint = ((IMenuClickable)referenceItem).OnPointFunction;
        }

        public void DeleteThis()
        {
            Destroy(this.gameObject);
        }

        public void HideElement()
        {
            gameObject.SetActive(false);
        }

        public void ShowElement()
        {
            gameObject.SetActive(true);
        }

    }

    /// <summary>
    /// Class for lines of menu
    /// </summary>
    [System.Serializable]
    public class MenuLine
    {

        public enum TypeOfLine
        {
            firstLine,
            secondLine
        }

        /// <summary>
        /// Prefab for line on menu
        /// </summary>
        public GameObject PrefabLine;

        /// <summary>
        /// Distance to line
        /// </summary>
        public float LineDistance;

        /// <summary>
        /// Angle, that add to each item, offset in degree
        /// </summary>
        public float StartAngle;

        /// <summary>
        /// Angle, that show in which place the menu end draw
        /// </summary>
        public float FinishAngle;

        /// <summary>
        /// Angle for each segment of menu item in line
        /// </summary>
        public float AngleForEachSegment;

        /// <summary>
        /// Angle for all line
        /// </summary>
        public float LineAngle;

        /// <summary>
        /// Type of line
        /// </summary>
        public TypeOfLine typeLine;

        public float scaleForItems;

        [SerializeField]
        private float _CurrentLineRotation;

        public float CurrentLineRotation
        {
            get
            {
                return _CurrentLineRotation;
            }
            private set
            {
                _CurrentLineRotation = value;
            }
        }

        private List<MenuItem> MenuItems;

        #region public function

        /// <summary>
        /// Create on this line menu items from current list
        /// </summary>
        public void SetLine(List<MenuItem> menuItems, Transform atPosition)
        {
            CurrentLineRotation = 0;
            int pos = 0;
            if (MenuItems == null)
            {
                MenuItems = new List<MenuItem>();
                for (int i = 0; i < menuItems.Count; i++)
                {
                    if (menuItems[i].typeOfObject == typeLine)
                    {
                        MenuItems.Add(menuItems[i]);
                        CreateMenuObjectAtPosition(menuItems[i], pos, MenuManager.Instance.menuPosition);
                        UpdateItemVisible(MenuItems[MenuItems.Count - 1]);
                        pos++;
                    }
                }
            }
        }

        /// <summary>
        /// Deleting all items from this menu
        /// </summary>
        public void DestroyAllMenuItems()
        {
            if (MenuItems != null)
            {
                for (int i = 0; i < MenuItems.Count; i++)
                {
                    Object.Destroy(MenuItems[i].gameObject);
                }
                MenuItems = null;
            }
        }

        public void ScrollLine(float angle)
        {
            CurrentLineRotation += angle;            
            if (CurrentLineRotation < (-MenuItems.Count + 1) * AngleForEachSegment + FinishAngle - StartAngle)
            {
                CurrentLineRotation = (-MenuItems.Count + 1) * AngleForEachSegment + FinishAngle - StartAngle;
            }
            if (CurrentLineRotation > 0)
            {
                CurrentLineRotation = 0;
            }
            UpdateLine();
        }

        public void UpdateLineVisible()
        {
            for(int i = 0; i < MenuItems.Count; i++)
            {
                UpdateItemVisible(MenuItems[i]);
            }
        }

        public void SelectItem(int num)
        {

        }

        public void HideLine()
        {

        }

        public void ShowLine()
        {

        }

        #endregion

        #region private function

        private void UpdateLine()
        {
            for(int i = 0; i < MenuItems.Count; i++)
            {
                UpdateItemPosition(MenuItems[i], i);
                UpdateItemVisible(MenuItems[i]);
            }
        }

        private void UpdateItemPosition(MenuItem item, int pos)
        {
            item.CurrentRotation = (StartAngle + CurrentLineRotation + AngleForEachSegment * pos); //главный угол
            Vector3 realPosition = new Vector3(Mathf.Cos(item.CurrentRotation * Mathf.Deg2Rad) * LineDistance, Mathf.Sin(Mathf.Deg2Rad * LineAngle) * LineDistance, Mathf.Sin(item.CurrentRotation * Mathf.Deg2Rad) * LineDistance);
            item.transform.localPosition = realPosition;
        }

        private void UpdateItemVisible(MenuItem item)
        {
            if(item.CurrentRotation >= StartAngle && item.CurrentRotation <= FinishAngle)
            {
                item.ShowElement();
                item.transform.LookAt(item.transform.parent);
            }
            else
            {
                item.HideElement();
            }
        }

        private void PlaceInCenterFunction(MonoBehaviour item)
        {
            #region wft
            //dont change this
            Vector3 v = ((SceneChangebleObject)item).GetBundleCenter();
            v.y = 0.006f;
            v.z = -v.z / 2;
            ((SceneChangebleObject)item).gameObject.transform.localPosition = -v * scaleForItems;
            ((SceneChangebleObject)item).DestroyAssetCollider();
            #endregion
        }

        private void CreateMenuObjectAtPosition(MenuItem menuObject, int pos, Transform ZepoPoint)
        {
            menuObject.transform.parent = ZepoPoint; //перемещаем объект в меню. Делаем его дочерним
            menuObject.CurrentRotation = (StartAngle + AngleForEachSegment * pos); //главный угол
            Vector3 realPosition = new Vector3(Mathf.Cos(menuObject.CurrentRotation * Mathf.Deg2Rad) * LineDistance, Mathf.Sin(Mathf.Deg2Rad * LineAngle) * LineDistance, Mathf.Sin(menuObject.CurrentRotation * Mathf.Deg2Rad) * LineDistance);
            menuObject.transform.localPosition = realPosition;

            //создаем префаб объекта и помещаем его MenuObject
            GameObject gTemp = GameObject.Instantiate(PrefabLine, menuObject.transform);
            gTemp.transform.localPosition = new Vector3(0, 0, 0);

            menuObject.ItemInstance.gameObject.transform.parent = menuObject.transform;
            menuObject.ItemInstance.gameObject.transform.localPosition = new Vector3(0, 0, 0);
            menuObject.transform.LookAt(ZepoPoint);

            #region ELEGANTNIY KOSTIL

            if (menuObject.typeOfObject == TypeOfLine.secondLine)
            {
                ((IAssetBundleLoadable)menuObject.ItemInstance).AddListenerLoading(AssetBundleLoaderManager.IAssetBundleLoadableEvent.BundleReady, 
                    new AssetBundleLoaderManager.OnEventFunction(x =>
                    {
                        if (gTemp != null)
                        {
                            gTemp.GetComponent<MeshRenderer>().material = ((LoadedMaterial)menuObject.ItemInstance).loadedMaterial;
                        }
                    }));
                ((IAssetBundleLoadable)menuObject.ItemInstance).StartLoadAssetBundle();
                menuObject.Illumination = gTemp.GetComponent<MenuSecondLineEffects>();
            }

            if (menuObject.typeOfObject == TypeOfLine.firstLine)
            {
                ((IAssetBundleLoadable)menuObject.ItemInstance).AddListenerLoading(AssetBundleLoaderManager.IAssetBundleLoadableEvent.BundleReady, 
                    new AssetBundleLoaderManager.OnEventFunction(x =>
                    {
                        if (menuObject != null)
                        {
                            ((SceneChangebleObject)menuObject.ItemInstance).SpawnBundle();
                        }
                    }));
                ((IAssetBundleLoadable)menuObject.ItemInstance).AddListenerLoading(AssetBundleLoaderManager.IAssetBundleLoadableEvent.BundleReady,
                    new AssetBundleLoaderManager.OnEventFunction(x =>
                    {
                        if (menuObject != null)
                        {
                            PlaceInCenterFunction(menuObject.ItemInstance);
                        }
                    }));
                ((SceneChangebleObject)menuObject.ItemInstance).SpawnBundle();
                ((SceneChangebleObject)menuObject.ItemInstance).gameObject.transform.localScale = new Vector3(scaleForItems, scaleForItems, scaleForItems);

                PlaceInCenterFunction(menuObject.ItemInstance);

                menuObject.Illumination = gTemp.GetComponent<MenuFirstLineEffects>();
            }

            #endregion

        }

        #endregion

    }

    /// <summary>
    /// Class for menu managing
    /// </summary>
    public class MenuManager : MonoBehaviour
    {

        /// <summary>
        /// Singleton
        /// </summary>
        public static MenuManager Instance { get; private set; }

        /// <summary>
        /// Is shown the menu now
        /// </summary>
        public bool IsShown { get; private set; }

        /// <summary>
        /// The object, that select now
        /// </summary>
        public ISceneClickable ObjectSelected { get; private set; }

        [SerializeField]
        private GameObject MenuPosition;

        public Transform menuPosition
        {
            get
            {
                if (!IsShown)
                {
                    MenuPosition.GetComponent<CameraFollower>().UpdatePosition();
                }
                return MenuPosition.transform;
            }
        }

        [SerializeField]
        private List<MenuLine> MenuLines;

        #region Unity functions

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            IsShown = true;
            HideMenu();
        }

        #endregion

        #region public functions

        /// <summary>
        /// Of someone click on clickable object, the click-manager is should call this function. Also, if you want simulate the click of any object,
        /// that have the ISceneClickable interface, call this.
        /// </summary>
        /// <param name="clickableObject"></param>
        public void ClickedOnClickable(ISceneClickable clickableObject)
        {
            ObjectSelected = clickableObject; //назначаем выбранный объект
            CleanMenu();
            List<MenuItem> tempList = ObjectSelected.GetListOfMenuObject(); //получаем от него список объектов для меню            
            RefreshMenu(tempList);            
        }

        public void ClickedOnMenuElement(MenuItem menuItem)
        {
            menuItem.onClick(menuItem.ItemInstance);
        }

        /// <summary>
        /// Show menu, if it hide
        /// </summary>
        public void ShowMenu()
        {
            if (!IsShown) //если меню не показано
            {
                //IsShown = true;
                //Setting.MenuPosition.SetActive(true);
                //Setting.MenuPosition.GetComponent<CameraFollower>().UpdatePosition();
                //Setting.MenuPosition.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                //TODO: animation
            }
        }

        /// <summary>
        /// Hide menu, if it shown
        /// </summary>
        public void HideMenu()
        {
            if (IsShown)
            {
                ObjectSelected = null;
                CleanMenu();
                IsShown = false;
            }
        }

        /// <summary>
        /// Redrawing the menu
        /// </summary>
        public void RefreshMenu(List<MenuItem> tempList)
        {
            if (tempList != null)
            {//рисование меню
                if (tempList.Count > 0)
                {
                    for (int i = 0; i < MenuLines.Count; i++)
                    {
                        MenuLines[i].SetLine(tempList, MenuPosition.transform);
                    }
                }
            }
            IsShown = true;
        }

        public void RotateMenuLine(MenuLine.TypeOfLine typeLine, float angle)
        {
            for(int i = 0; i < MenuLines.Count; i++)
            {
                if(MenuLines[i].typeLine == typeLine)
                {
                    MenuLines[i].ScrollLine(angle);
                }
            }
        }

        #endregion

        #region private functions

        private void CleanMenu()
        {
            for (int i = 0; i < MenuLines.Count; i++)
            {
                MenuLines[i].DestroyAllMenuItems();
            }
        }

        #endregion

    }
}