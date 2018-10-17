using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Inteface for clickable object
/// </summary>
public interface IClickable
{

    List<MenuObject> GetListOfMenuObject();

}

public interface IMenuClickable
{
    void onMenuClick(MonoBehaviour itemInstance);
}

/// <summary>
/// Class for menu object. All menu items are such classes
/// </summary>
public class MenuObject : MonoBehaviour
{

    public delegate void OnClickDelegate(MonoBehaviour item);

    public enum TypeOfObject
    {
        firstLine,
        secondLine
    }

    public TypeOfObject typeOfObject;
    public OnClickDelegate onClick;
    public MonoBehaviour ItemInstance { get; private set; }

    public void SetMenuObject(TypeOfObject menuItemType, MonoBehaviour referenceItem)
    {
        typeOfObject = menuItemType;
        ItemInstance = referenceItem;
        onClick = ((IMenuClickable)referenceItem).onMenuClick;
    }

    public void EraseThis()
    {
        ItemInstance = null;
    }

}

[System.Serializable]
public class MenuLine
{
    /// <summary>
    /// Prefab for line on menu
    /// </summary>
    public GameObject PrefabLine;

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

    public float LineAngle;

    /// <summary>
    /// Type of line
    /// </summary>
    public MenuObject.TypeOfObject typeLine;

}

[System.Serializable]
public class MenuSetting
{

    /// <summary>
    /// Distance to menu
    /// </summary>
    public float MenuDistance;

    /// <summary>
    /// Speed for hide and show menu animation 
    /// </summary>
    public float MenuHideShowAnimationSpeed;

    /// <summary>
    /// Setting for menu lines
    /// </summary>
    public List<MenuLine> MenuLines;

    /// <summary>
    /// Camera for menu
    /// </summary>
    public Camera MenuCamera;
    
    /// <summary>
    /// Return the menu line setting by the type of it
    /// </summary>
    /// <param name="typeLine">Type of needeble menu line setting</param>
    /// <returns>Menu line setting instance</returns>
    public MenuLine GetSettingByType(MenuObject.TypeOfObject typeLine)
    {
        for (int i = 0; i < MenuLines.Count; i++)
        {
            if (MenuLines[i].typeLine == typeLine)
            {
                return MenuLines[i];
            }
        }
        return null;
    }

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
    public IClickable objectSelected { get; private set; }

    [SerializeField]
    private MenuSetting Setting;
    private List<MenuObject> MenuObjects;

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
    /// that have the IClickable interface, call this.
    /// </summary>
    /// <param name="clickableObject"></param>
    public void ClickedOnClickable(IClickable clickableObject)
    {        
        objectSelected = clickableObject; //назначаем выбранный объект
        CleanMenu();
        MenuObjects = objectSelected.GetListOfMenuObject(); //получаем от него список объектов для меню
        RefreshMenu();
        ShowMenu();
    }

    /// <summary>
    /// Show menu, if it hide
    /// </summary>
    public void ShowMenu()
    {
        if (!IsShown) //если меню не показано
        {
            IsShown = true;
            Setting.MenuCamera.transform.localPosition = new Vector3(0, 0, 0);
            //TODO: animation
        }
    }

    /// <summary>
    /// Hide menu, if it shown
    /// </summary>
    public void HideMenu()
    {
        if (IsShown) //если меню показано
        {
            IsShown = false;
            Setting.MenuCamera.transform.localPosition = new Vector3(0, 0, 100);
            //TODO: animation
        }
    }

    /// <summary>
    /// Return the cam that renderer menu
    /// </summary>
    /// <returns>the cam instance</returns>
    public Camera GetMenuCamera()
    {
        return Setting.MenuCamera;
    }

    /// <summary>
    /// Redrawing the menu
    /// </summary>
    public void RefreshMenu()
    {
        if (MenuObjects != null)
        {//рисование меню
            if (MenuObjects.Count > 0)
            {
                Dictionary<MenuObject.TypeOfObject, int> CounterDictionary = new Dictionary<MenuObject.TypeOfObject, int>();
                for (int i = 0; i < Setting.MenuLines.Count; i++)
                {
                    CounterDictionary.Add(Setting.MenuLines[i].typeLine, 0);
                }

                for (int i = 0; i < MenuObjects.Count; i++)
                {
                    CreateMenuObjectAtPosition(MenuObjects[i], CounterDictionary[MenuObjects[i].typeOfObject]);
                    CounterDictionary[MenuObjects[i].typeOfObject]++;
                }
            }
        }
    }

    #endregion

    #region private functions

    private void CleanMenu()
    {
        if (MenuObjects != null)
        {
            for (int i = 0; i < MenuObjects.Count; i++)
            {                
                MenuObjects[i].ItemInstance.transform.parent = null;                
                ((IAssetBundleLoadeble)MenuObjects[i].ItemInstance).BundleHide(); //TODO: подумать, хорошо ли это во всех случаях
                MenuObjects[i].EraseThis();
                Destroy(MenuObjects[i].gameObject);
            }
        }
    }

    private void CreateMenuObjectAtPosition(MenuObject menuObject, int pos)
    {
        MenuLine menuLineTemp = Setting.GetSettingByType(menuObject.typeOfObject);
        if (menuLineTemp != null)
        {
            menuObject.transform.parent = this.transform; //перемещаем объект в меню. Делаем его дочерним
            float mainAngle = Mathf.Deg2Rad * (menuLineTemp.StartAngle + menuLineTemp.AngleForEachSegment * pos); //главный угол
            Vector3 realPosition = new Vector3(Mathf.Cos(mainAngle) * Setting.MenuDistance, Mathf.Sin(Mathf.Deg2Rad * menuLineTemp.LineAngle) * Setting.MenuDistance, Mathf.Sin(mainAngle) * Setting.MenuDistance);
            menuObject.transform.localPosition = realPosition;

            //создаем префаб объекта и помещаем его MenuObject
            GameObject gTemp = Instantiate(menuLineTemp.PrefabLine, menuObject.transform);
            gTemp.transform.localPosition = new Vector3(0, 0, 0);

            //делаем ItemInstance дочерним объектом для gTemp
            menuObject.ItemInstance.gameObject.transform.parent = gTemp.transform;
            menuObject.ItemInstance.gameObject.transform.localPosition = new Vector3(0, 0, 0);

            //пытаемся загрузить для него бандл
            ((IAssetBundleLoadeble)menuObject.ItemInstance).StartLoadAssetBundle();
        }
    }

    #endregion

}