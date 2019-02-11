using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Pointer))]
[RequireComponent(typeof(PointerVisualizer))]
/// <summary>
/// Class for managing events from phisical VRControllers
/// </summary>
public class VRControllerManager : MonoBehaviour
{

    #region IControlable property

    public ControlInputData InputData;

    #endregion    

    #region Touchpad variables 

    private bool IsTouchedNow
    {
        get
        {
            return TouchPad.GetAxis(hand.handType) != new Vector2(0, 0) && !SteamVR_Input._default.inActions.Teleport.GetState(hand.handType);
        }
    }

    private bool FirstTouchHappend = false;

    private Vector2 LastTouchPosition;
    private Vector2 CurrentTouchPosition;

    private float CurrentImpuls;

    [SerializeField]
    private float ImpulsMultiplier;

    #endregion

    [Space(10)]
    [Header("-------VR setting---------")]
    [SerializeField]
    private Hand hand;

    private Pointer PointerInstance;

    private PointerVisualizer pointerVisualizerInstance;

    [SerializeField]
    private GameObject ExitConfirmation;
    private GameObject CurrentConfirmationInstance;

    [SerializeField]
    private SteamVR_Action_Vector2 TouchPad;
    [SerializeField]
    private SteamVR_ActionSet TouchPadGroupAction;

    [SerializeField]
    private SteamVR_Action_Boolean MainMenuButton;
    [SerializeField]
    private SteamVR_ActionSet MainMenuButtonGroupAction;

    private MyVRMenu.MenuItem LastPointedMenuItem; //for rotation only

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

        pointerVisualizerInstance = GetComponent<PointerVisualizer>();
        if (pointerVisualizerInstance == null)
        {
            Debug.LogError("Can`t find require component typeof<PointerVisualizer> for VRControllerManager! VRControllerManager will not work.");
        }

        InputData = new ControlInputData();
        InputData.ControlEventType = ClickManager.ControlEvent.activeActionEvent;
        InputData.Param = 0;
        InputData.interactiveObject = null;
        SteamVR_Input.platformer.Initialize();
        SteamVR_Input.buggy.Initialize();
        MainMenuButtonGroupAction.ActivatePrimary();
        TouchPadGroupAction.ActivatePrimary();
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
        if (SteamVR_Input._default.inActions.GrabPinch.GetStateDown(hand.handType))
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

    private void MenuRotate(MyVRMenu.MenuItem menuItem)
    {
        if (ClickManager.Instance != null)
        {
            InputData.ControlEventType = ClickManager.ControlEvent.menuAddRotatingImpuls;
            if (!FirstTouchHappend)
            {//при первом касании сбрасываем значения.
                FirstTouchHappend = true; //первое касание произошло
                LastTouchPosition = TouchPad.GetAxis(hand.handType);
                CurrentTouchPosition = TouchPad.GetAxis(hand.handType);
                InputData.Param = 0;
            }
            else
            {
                LastTouchPosition = CurrentTouchPosition;
                CurrentTouchPosition = TouchPad.GetAxis(hand.handType);
                CurrentImpuls = (LastTouchPosition.x - CurrentTouchPosition.x) * ImpulsMultiplier;
                InputData.Param = CurrentImpuls;
            }
            InputData.interactiveObject = menuItem;            
            ClickManager.Instance.ControlEventHappend(InputData);
        }
    }

    private IEnumerator TrackingInputEventCoroutine()
    {
        while (true)
        {//Tracking the click...                 
            yield return null;

            #region teleportClick

            if (SteamVR_Input._default.inActions.Teleport.GetStateDown(hand.handType))
            {
                if (MyVRMenu.MenuManager.Instance != null)
                {
                    MyVRMenu.MenuManager.Instance.HideMenu();
                }
                if (CurrentConfirmationInstance != null)
                {
                    Destroy(CurrentConfirmationInstance);
                }
                pointerVisualizerInstance.IsShowing = false;
            }
            if (SteamVR_Input._default.inActions.Teleport.GetStateUp(hand.handType))
            {
                pointerVisualizerInstance.IsShowing = true;
            }

            #endregion

            #region toMainMenuClick

            if (MainMenuButton.GetStateDown(hand.handType) && MyVRMenu.MenuManager.Instance != null && CurrentConfirmationInstance == null && UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "TestMenuScene")
            {
                CurrentConfirmationInstance = Instantiate(ExitConfirmation, transform.position, Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0)));
                ExtendedInteractive ExtInt0 = CurrentConfirmationInstance.transform.GetChild(0).GetComponent<ExtendedInteractive>();
                ExtInt0.OnPointerHover = () =>
                {
                    if (!ExtInt0.effectGameObject.activeSelf)
                    {
                        ExtInt0.effectGameObject.SetActive(true);
                    }
                };
                ExtInt0.OnPointerLeft = () =>
                {
                    if (ExtInt0.effectGameObject.activeSelf)
                    {
                        ExtInt0.effectGameObject.SetActive(false);
                    }
                };
                ExtInt0.OnActiveAction = () =>
                {
                    MyVRMenu.MenuManager.Instance.HideMenu();
                    UnityEngine.XR.XRSettings.enabled = false;
                    GameObject.FindGameObjectWithTag("SceneManagers").SetActive(false);
                    MyPlayerControllers.PlayerManager.Instance.DestroyCurrentController();
                    UnityEngine.SceneManagement.SceneManager.LoadScene("TestMenuScene");
                };

                ExtendedInteractive ExtInt1 = CurrentConfirmationInstance.transform.GetChild(1).GetComponent<ExtendedInteractive>();
                ExtInt1.OnPointerHover = () =>
                {
                    if (!ExtInt1.effectGameObject.activeSelf)
                    {
                        ExtInt1.effectGameObject.SetActive(true);
                    }
                };
                ExtInt1.OnPointerLeft = () =>
                {
                    if (ExtInt1.effectGameObject.activeSelf)
                    {
                        ExtInt1.effectGameObject.SetActive(false);
                    }
                };
                ExtInt1.OnActiveAction = () =>
                {
                    Destroy(CurrentConfirmationInstance);
                };
            }

            #endregion

            #region menu rotate  

            if (IsTouchedNow)
            {
                if (LastPointedMenuItem != null)
                {
                    MenuRotate(LastPointedMenuItem); // просто брать объект из Pointer`а и проверять, не объект меню ли. В целом просто переписать с использованием Pointer`а
                }
            }
            else
            {
                FirstTouchHappend = false;
            }           

            #endregion

        }
    }

    #endregion

}
