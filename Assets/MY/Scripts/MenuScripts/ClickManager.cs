using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyVRMenu;

public class ControlInputData
{

    public ClickManager.ControlEvent ControlEventType { get; set; }

    public GameObject ClickedObject { get; set; }

    public float Param { get; set; }

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

    public void ImitateClick(GameObject target)
    {
        ControlInputData tempData = new ControlInputData();
        tempData.ClickedObject = target;
        tempData.ControlEventType = ControlEvent.chooseEvent;
        tempData.Param = 0;
        ControlEventHappend(tempData);
    }

    public void ControlEventHappend(ControlInputData controlData)
    {
        EventDelegateDictionary[controlData.ControlEventType](controlData);
    }

    #endregion

    #region private functions

    private void ChooseEventDelegate(ControlInputData controlData)
    {
        if(controlData.ClickedObject != null)
        {
            if (controlData.ClickedObject.GetComponent<MenuItem>() != null)
            {
                MenuManager.Instance.ClickedOnMenuElement(controlData.ClickedObject.GetComponent<MenuItem>());
                return;
            }
            else
            {
                if (controlData.ClickedObject.GetComponent<ISceneClickable>() != null)
                {
                    MenuManager.Instance.ClickedOnClickable(controlData.ClickedObject.GetComponent<ISceneClickable>());
                    return;
                }
                else
                {
                    MenuManager.Instance.HideMenu();
                    MenuDebugger.Instance.EraseText();
                }
            }
        }        
    }

    private void MenuRotateDelegate(ControlInputData controlData)
    {
        if (controlData.ClickedObject != null)
        {
            if (controlData.ClickedObject.GetComponent<MenuItem>() != null)
            {
                MenuManager.Instance.RotateMenuLine(controlData.ClickedObject.GetComponent<MenuItem>().typeOfObject, controlData.Param);
                return;
            }
        }
    }

    private void MenuExitDelegate(ControlInputData controlData)
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
