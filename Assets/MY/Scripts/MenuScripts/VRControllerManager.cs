﻿using System.Collections;
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
            return TouchPad.GetAxis(SteamVR_Input_Sources.Any) != new Vector2(0, 0) && !SteamVR_Input._default.inActions.Teleport.GetState(SteamVR_Input_Sources.Any);
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

    private MyVRMenu.MenuItem LastPointedMenuItem;

    private int LastCountOfControllers = -1;

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
        if (SteamVR_Input._default.inActions.GrabPinch.GetStateDown(SteamVR_Input_Sources.Any))
        {
            SelectButtonClick(newBaseActionObject);
        }
    }

    private void SelectButtonClick(InteractiveObject interactiveObject)
    {
        if (CurrentConfirmationInstance != null)
        {
            Destroy(CurrentConfirmationInstance);
        }
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
                LastTouchPosition = TouchPad.GetAxis(SteamVR_Input_Sources.Any);
                CurrentTouchPosition = TouchPad.GetAxis(SteamVR_Input_Sources.Any);
                InputData.Param = 0;
            }
            else
            {
                LastTouchPosition = CurrentTouchPosition;
                CurrentTouchPosition = TouchPad.GetAxis(SteamVR_Input_Sources.Any);
                CurrentImpuls = (LastTouchPosition.x - CurrentTouchPosition.x) * ImpulsMultiplier;
                InputData.Param = CurrentImpuls;
            }
            InputData.interactiveObject = menuItem;
            ClickManager.Instance.ControlEventHappend(InputData);
        }
    }

    private void SwapControl()
    {
        if (hand.trackedObject.inputSource == SteamVR_Input_Sources.LeftHand)
        {
            hand.trackedObject.inputSource = SteamVR_Input_Sources.RightHand;
        }
        else
        {
            hand.trackedObject.inputSource = SteamVR_Input_Sources.LeftHand;
        }
    }

    private IEnumerator TrackingInputEventCoroutine()
    {
        while (true)
        {//Tracking the click...                 
            yield return null;

            #region teleportClick

            if (SteamVR_Input._default.inActions.Teleport.GetStateDown(SteamVR_Input_Sources.Any))
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
            if (SteamVR_Input._default.inActions.Teleport.GetStateUp(SteamVR_Input_Sources.Any))
            {
                pointerVisualizerInstance.IsShowing = true;
            }

            #endregion

            #region toMainMenuClick

            /*
             * ладно, вот варианты
             * 1 - просто переименовать  UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "TestMenuScene" на нужную сцену
             * 2 - как-то брать с ClickManagera имя сцены. Поставить ее типа {get; private set;}
             * 
             * я подумал, что второй вариант лучше, и запилил его, но на самом деле это не решает проблему а просто скрывает ее, 
             * по хорошему нужно вобще запретить переходить в стартовую сцену из нее самой.
             * В самом ClickManagere нужно сделать проверку
             * 
             * сделал проверку
             * 
             * по хорошему, теперь еще нужно сделать вызов такого же события выхода в меню от обычного компьютерного игрока, в MouseControllerManager
             * 
             * ладно, я пойду, мне тут по делам надо, возможно что часов в 4-5 я еще подсяду))
             * удачи с MouseControllerManager
             * 
             * 
             */


            if (MainMenuButton.GetStateDown(SteamVR_Input_Sources.Any) && MyVRMenu.MenuManager.Instance != null && UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != ClickManager.Instance.SceneMainMenuName) // 
            {
                MyVRMenu.MenuManager.Instance.HideMenu();
                if (CurrentConfirmationInstance != null)
                {
                    CurrentConfirmationInstance.transform.position = transform.position;
                    CurrentConfirmationInstance.transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0));
                }
                else
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
                        if (ClickManager.Instance != null)
                        {
                            InputData.ControlEventType = ClickManager.ControlEvent.exitToMainMenu;
                            ClickManager.Instance.ControlEventHappend(InputData);
                        }
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
            }

            #endregion

            #region menu rotate  

            if (IsTouchedNow)
            {
                if (LastPointedMenuItem != null)
                {
                    MenuRotate(LastPointedMenuItem);
                }
            }
            else
            {
                FirstTouchHappend = false;
            }

            #endregion

            string[] arrayOfString = Input.GetJoystickNames();
            List<string> ListOfString = new List<string>();
            for (int i = 0; i < arrayOfString.Length; i++)
            {
                if (arrayOfString[i].IndexOf("Vive.") > 0)
                {
                    ListOfString.Add(arrayOfString[i]);
                }
            }
            if (ListOfString.Count != LastCountOfControllers)
            {
                LastCountOfControllers = ListOfString.Count;
                if (LastCountOfControllers == 2)
                {
                    SwapControl();
                }
                else
                {
                    if (LastCountOfControllers > 0)
                    {
                        if (ListOfString[0].IndexOf("Right") > 0)
                        {
                            hand.trackedObject.inputSource = SteamVR_Input_Sources.RightHand;
                        }
                        if (ListOfString[0].IndexOf("Left") > 0)
                        {
                            hand.trackedObject.inputSource = SteamVR_Input_Sources.LeftHand;
                        }
                        hand.handType = hand.trackedObject.inputSource;
                    }
                }
            }

            if (SteamVR_Input._default.inActions.GrabGrip.GetStateDown(SteamVR_Input_Sources.Any))
            {
                SwapControl();
                hand.handType = hand.trackedObject.inputSource;
            }
        }
    }

    #endregion

}
