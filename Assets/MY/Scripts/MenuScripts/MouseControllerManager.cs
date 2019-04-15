using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Pointer))]
[RequireComponent(typeof(PointerVisualizer))]
public class MouseControllerManager : MonoBehaviour
{

    #region IControlable property

    public ControlInputData InputData;

    #endregion

    [SerializeField]
    private float Sensitivity;

    private MyVRMenu.MenuItem LastPointedMenuItem;

    private GameObject LastPointedGameObject;

    private Pointer PointerInstance;

    private Coroutine TrackingInputEventCoroutineInstance;

    #region Unity functions

    private void Start()
    {
        Initialize();
    }

    #endregion

    #region public functions

    /// <summary>
    /// Stop controller coroutine, if its work now
    /// </summary>
    public void StopController()
    {
        if (TrackingInputEventCoroutineInstance != null)
        {
            StopCoroutine(TrackingInputEventCoroutineInstance);
            TrackingInputEventCoroutineInstance = null;
        }
    }

    /// <summary>
    /// Start controller coroutine, if its dont work now
    /// </summary>
    public void StartController()
    {
        if (TrackingInputEventCoroutineInstance == null)
        {
            StartCoroutine(TrackingInputEventCoroutine());
        }
    }

    #endregion

    #region private functions

    private void Initialize()
    {
        PointerInstance = GetComponent<Pointer>();
        if (PointerInstance == null)
        {
            Debug.LogError("Can`t find require component typeof<Pointer> for VRControllerManager! VRControllerManager will not work.");
        }

        PointerInstance.AddDelegate(Pointer.PointerEvents.HitActionObject, UpdateLastPointedItem);
        PointerInstance.AddDelegate(Pointer.PointerEvents.HitActionObject, OnSelectButtonClick);

        InputData = new ControlInputData();
        InputData.ControlEventType = ClickManager.ControlEvent.activeActionEvent;
        InputData.Param = 0;
        InputData.interactiveObject = null;

        TrackingInputEventCoroutineInstance = StartCoroutine(TrackingInputEventCoroutine());
    }

    private void UpdateLastPointedItem(InteractiveObject oldBaseActionObject, InteractiveObject newBaseActionObject)
    {
        if (newBaseActionObject.GetComponent<MyVRMenu.MenuItem>() != null)
        {
            LastPointedMenuItem = newBaseActionObject.GetComponent<MyVRMenu.MenuItem>();
        }
    }

    private void OnSelectButtonClick(InteractiveObject oldBaseActionObject, InteractiveObject newBaseActionObject)
    {
        if (Input.GetMouseButtonDown(0))
        {
            SelectButtonClick(newBaseActionObject);
        }
    }

    private void SelectButtonClick(InteractiveObject interactiveObject)
    {
        if (ClickManager.Instance != null)
        {
            InputData.ControlEventType = ClickManager.ControlEvent.activeActionEvent;
            InputData.Param = 0;
            InputData.interactiveObject = interactiveObject;
            ClickManager.Instance.ControlEventHappend(InputData);
        }
    }

    private void ShowDebug(MyVRMenu.MenuItem menuItem)
    {
        if (MenuDebugger.Instance != null)
        {
            if (menuItem != null)
            {
                if (menuItem.typeOfObject == MyVRMenu.MenuLine.TypeOfLine.firstLine)
                {
                    MenuDebugger.Instance.SetText(((SceneChangeableObject)menuItem.AttachedObject).ToString());
                    return;
                }
                if (menuItem.typeOfObject == MyVRMenu.MenuLine.TypeOfLine.secondLine)
                {
                    MenuDebugger.Instance.SetText(((LoadedMaterial)menuItem.AttachedObject).ToString());
                    return;
                }
            }
            MenuDebugger.Instance.EraseText();
        }
    }

    private void MenuRotate(InteractiveObject interactiveObject)
    {
        if (ClickManager.Instance != null)
        {
            InputData.ControlEventType = ClickManager.ControlEvent.menuRotate;
            InputData.Param = Input.mouseScrollDelta.y * Sensitivity;
            if (interactiveObject.transform.parent == null)
            {
                InputData.interactiveObject = null;
            }
            else
            {
                InputData.interactiveObject = interactiveObject;
            }
            ClickManager.Instance.ControlEventHappend(InputData);
        }
    }

    private void ExitToMenu()
    {
        if (ClickManager.Instance != null)
        {
            InputData.ControlEventType = ClickManager.ControlEvent.exitToMainMenu;
            ClickManager.Instance.ControlEventHappend(InputData);
        }
    }

    #endregion

    #region coroutines

    private IEnumerator TrackingInputEventCoroutine()
    {
        while (true)
        {//Tracking the click...                 
            yield return null;

            #region rotate menu
            if (Input.mouseScrollDelta.y != 0 && LastPointedMenuItem != null)
            {
                MenuRotate(LastPointedMenuItem); //TODO переделать
                continue;
            }
            #endregion

            #region show debug
            if (Input.GetMouseButtonDown(1))
            {
                ShowDebug(LastPointedMenuItem);
                continue;
            }
            #endregion
            
            #region exit to menu
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                ExitToMenu();
            }
            #endregion

            //дописать еще другие клики, и события       
        }
    }

    #endregion

}