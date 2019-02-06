using System.Collections;
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
    /// Class for lines of menu
    /// </summary>
    [System.Serializable]
    public class MenuLine
    {
        /// <summary>
        /// Line types
        /// </summary>
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

        /// <summary>
        /// Scale for all items in this line
        /// </summary>
        public float scaleForItems;

        /// <summary>
        /// Rotation coroutine instance. That coroutine every frame apply current RotatingImpuls to this line and decreace impuls.
        /// </summary>
        public Coroutine RotationCoroutine;

        /// <summary>
        /// Current rotating impuls
        /// </summary>
        public float RotationImpuls;

        /// <summary>
        /// Decreasing curve for RotationImpuls
        /// </summary>
        public AnimationCurve DecreaseImpulsCurve;
                              
        /// <summary>
        /// Curretn rotation of this line
        /// </summary>
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
        [SerializeField]
        private float _CurrentLineRotation;

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

        /// <summary>
        /// Rotate this line to current angle
        /// </summary>
        /// <param name="angle"></param>
        public void ScrollLine(float angle)
        {
            if (MenuItems != null)
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
        }

        /// <summary>
        /// Updating visibility all items. Need for normal display items
        /// </summary>
        public void UpdateLineVisible()
        {
            for(int i = 0; i < MenuItems.Count; i++)
            {
                UpdateItemVisible(MenuItems[i]);
            }
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

        private void CreateMenuObjectAtPosition(MenuItem menuObject, int pos, Transform ZepoPoint)
        {
            menuObject.onDrawFunction();
            menuObject.transform.parent = ZepoPoint; //перемещаем объект в меню. Делаем его дочерним
            menuObject.CurrentRotation = (StartAngle + AngleForEachSegment * pos); //главный угол
            Vector3 realPosition = new Vector3(Mathf.Cos(menuObject.CurrentRotation * Mathf.Deg2Rad) * LineDistance, Mathf.Sin(Mathf.Deg2Rad * LineAngle) * LineDistance, Mathf.Sin(menuObject.CurrentRotation * Mathf.Deg2Rad) * LineDistance);
            menuObject.transform.localPosition = realPosition;            

            menuObject.AttachedObject.gameObject.transform.parent = menuObject.transform;
            menuObject.AttachedObject.gameObject.transform.localPosition = new Vector3(0, 0, 0);
            menuObject.transform.LookAt(ZepoPoint);
            
                               
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

        /// <summary>
        /// Current position of menu center
        /// </summary>
        public Transform menuPosition
        {
            get
            {
                return MenuPosition.transform;
            }
        }

        [SerializeField]
        private GameObject MenuPosition;        

        [SerializeField]
        private List<MenuLine> MenuLines;

        #region Unity functions

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        #endregion

        #region public functions

        public void Initialize()
        {
            for (int i = 0; i < MenuLines.Count; i++)
            {
                MenuLines[i].RotationCoroutine = StartCoroutine(RotationLineCoroutine(MenuLines[i]));
            }
            IsShown = true;
            HideMenu();
        }

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

        /// <summary>
        /// You can call this funtions if something click to element of menu
        /// </summary>
        /// <param name="menuItem">Item, on which clicked</param>
        public void ClickedOnMenuElement(MenuItem menuItem)
        {
            menuItem.OnActiveAction();
        }

        /// <summary>
        /// Update position of menu center point
        /// </summary>
        public void UpdateMenuPosition()
        {
            MenuPosition.GetComponent<CameraFollower>().UpdatePosition();
        }

        /// <summary>
        /// Show menu, if it hide
        /// </summary>
        public void ShowMenu()
        {

        }

        /// <summary>
        /// Hide menu, if it shown
        /// </summary>
        public void HideMenu()
        {
            ObjectSelected = null;
            if (IsShown)
            {
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
                        MenuLines[i].SetLine(tempList, menuPosition);
                    }
                }
                
            }
            IsShown = true;
        }

        /// <summary>
        /// Rotate one of menu line to current angle
        /// </summary>
        /// <param name="typeLine">Type of line which need to rotate</param>
        /// <param name="angle">Rotation angle</param>
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

        /// <summary>
        /// Rotate one of menu line with impuls
        /// </summary>
        /// <param name="typeLine">Type of line which need to rotate</param>
        /// <param name="Impuls">Rotation impuls</param>
        public void RotateMenuLineWithImpuls(MenuLine.TypeOfLine typeLine, float Impuls)
        {
            for (int i = 0; i < MenuLines.Count; i++)
            {
                if (MenuLines[i].typeLine == typeLine)
                {
                    MenuLines[i].RotationImpuls += Impuls;
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

        #region Coroutines

        private IEnumerator RotationLineCoroutine(MenuLine menuLineObject)
        {
            while (true)
            {
                yield return null;
                if (Mathf.Abs(menuLineObject.RotationImpuls) > 0)
                {
                    //rotate
                    menuLineObject.ScrollLine(menuLineObject.RotationImpuls);
                    //decreace impuls
                    menuLineObject.RotationImpuls *= menuLineObject.DecreaseImpulsCurve.Evaluate(Mathf.Abs(menuLineObject.RotationImpuls));
                }
            }
        }

        #endregion

    }
}