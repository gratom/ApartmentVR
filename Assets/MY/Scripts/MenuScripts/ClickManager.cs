using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyVRMenu;

public class ControlInputData
{

    public ClickManager.ControlEvent ControlEventType { get; set; }

    public InteractiveObject interactiveObject { get; set; }

    public float Param { get; set; }

    public bool isFakeClick = false;

    public override string ToString()
    {
        return "Control input data:\n__type:" + ControlEventType.ToString() + "\n__param:" + Param.ToString() + "\n__clicked object:" + interactiveObject.name;
    }

}

/// <summary>
/// Class for managing the clicking mouse
/// </summary>
public class ClickManager : MonoBehaviour
{

    /// <summary>
    /// Different events of any control devises
    /// </summary>
    public enum ControlEvent
    {
        activeActionEvent,
        menuRotate,
        menuAddRotatingImpuls,        
        menuClose,
        exitToMainMenu
    }
    
    /// <summary>
    /// Singleton
    /// </summary>
    public static ClickManager Instance { get; private set; }

    private delegate void ControlDelegate(ControlInputData controlData);

    private Dictionary<ControlEvent, ControlDelegate> EventDelegateDictionary;

    #region Unity functions

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        Initialize();        
    }

    #endregion

    #region public functions

    public void ImitateClick(InteractiveObject target)
    {
        ControlInputData tempData = new ControlInputData();
        tempData.interactiveObject = target;
        tempData.ControlEventType = ControlEvent.activeActionEvent;
        tempData.Param = 0;
        tempData.isFakeClick = true;
        ControlEventHappend(tempData);
    }

    public void ControlEventHappend(ControlInputData controlData)
    {
        EventDelegateDictionary[controlData.ControlEventType](controlData);
    }

    #endregion

    #region private functions

    private void ActiveActionsDelegate(ControlInputData controlData)
    {
        if (controlData.interactiveObject != null)
        {
            if (controlData.interactiveObject.GetComponent<MenuItem>() != null) //если элемент меню
            {
                if (MenuManager.Instance != null && MenuManager.Instance.isActiveAndEnabled)
                {
                    MenuManager.Instance.ClickedOnMenuElement(controlData.interactiveObject.GetComponent<MenuItem>());
                }
                return;
            }
            else //если не элемент меню
            {
                if (!controlData.isFakeClick && MenuManager.Instance != null && MenuManager.Instance.isActiveAndEnabled)
                {
                    MenuManager.Instance.UpdateMenuPosition();
                }
                controlData.interactiveObject.OnActiveAction();
            }
        }        
    }

    private void MenuRotateDelegate(ControlInputData controlData)
    {
        if (controlData.interactiveObject != null)
        {
            if (controlData.interactiveObject.GetComponent<MenuItem>() != null)
            {
                MenuManager.Instance.RotateMenuLine(controlData.interactiveObject.GetComponent<MenuItem>().typeOfObject, controlData.Param);
                return;
            }
        }
    }

    private void MenuAddRotatingImpulsDelegate(ControlInputData controlData)
    {
        if (controlData.interactiveObject != null)
        {
            if (controlData.interactiveObject.GetComponent<MenuItem>() != null)
            {
                MenuManager.Instance.RotateMenuLineWithImpuls(controlData.interactiveObject.GetComponent<MenuItem>().typeOfObject, controlData.Param);
                return;
            }
        }
    }

    private void MenuCloseDelegate(ControlInputData controlData)
    {
        MenuManager.Instance.HideMenu();
    }

    private void ExitToMainMenuDelegate(ControlInputData controlData)
    {

    }

    private void Initialize()
    {
        EventDelegateDictionary = new Dictionary<ControlEvent, ControlDelegate>();
        EventDelegateDictionary.Add(ControlEvent.activeActionEvent, ActiveActionsDelegate);
        EventDelegateDictionary.Add(ControlEvent.menuRotate, MenuRotateDelegate);
        EventDelegateDictionary.Add(ControlEvent.menuAddRotatingImpuls, MenuAddRotatingImpulsDelegate);
        EventDelegateDictionary.Add(ControlEvent.menuClose, MenuCloseDelegate);
        EventDelegateDictionary.Add(ControlEvent.exitToMainMenu, ExitToMainMenuDelegate);
    }

    #endregion
    
}
