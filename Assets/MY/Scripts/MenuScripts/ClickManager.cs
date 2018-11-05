using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyVRMenu;

public interface IControlable
{

    ClickManager.ControlEvent ControlEventType { get; }

    GameObject ClickedObject { get; }

    float Param { get; }

}

/// <summary>
/// Class for managing the clicking mouse
/// </summary>
public class ClickManager : MonoBehaviour
{

    public enum ControlEvent
    {
        chooseEvent,
        menuRotate,
        menuExit
    }
    
    /// <summary>
    /// Singleton
    /// </summary>
    public static ClickManager Instance { get; private set; }

    private delegate void ControlDelegate(IControlable controlInstance);

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

    public void ControlEventHappend(IControlable controlInstance)
    {
        EventDelegateDictionary[controlInstance.ControlEventType](controlInstance);
    }

    #endregion

    #region private functions

    private void ChooseEventDelegate(IControlable controlInstance)
    {
        if(controlInstance.ClickedObject != null)
        {
            if (controlInstance.ClickedObject.GetComponent<MenuItem>() != null)
            {
                MenuManager.Instance.ClickedOnMenuElement(controlInstance.ClickedObject.GetComponent<MenuItem>());
                return;
            }
            //try to clickable...
            if (controlInstance.ClickedObject.GetComponent<ISceneClickable>() != null)
            {
                MenuManager.Instance.ClickedOnClickable(controlInstance.ClickedObject.GetComponent<ISceneClickable>());
                return;
            }
        }
        else
        {
            MenuManager.Instance.HideMenu();
        }
    }

    private void MenuRotateDelegate(IControlable controlInstance)
    {
        // if ( clicked on menu zone ) 
    }

    private void MenuExitDelegate(IControlable controlInstance)
    {
        MenuManager.Instance.HideMenu();
    }

    private void Initialize()
    {
        EventDelegateDictionary = new Dictionary<ControlEvent, ControlDelegate>();
        EventDelegateDictionary.Add(ControlEvent.chooseEvent, ChooseEventDelegate);
        EventDelegateDictionary.Add(ControlEvent.menuRotate, MenuRotateDelegate);
        EventDelegateDictionary.Add(ControlEvent.menuExit, MenuExitDelegate);
    }

    #endregion
    
}
