using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

/// <summary>
/// Class 
/// </summary>
public class VRControllerManager : MonoBehaviour, IControlable
{

    /// <summary>
    /// Singleton
    /// </summary>
    public static VRControllerManager Instance { get; private set; }

    /// <summary>
    /// Type of control event
    /// </summary>
    public ClickManager.ControlEvent ControlEventType { get; private set; }

    /// <summary>
    /// Object, on which cursor was be
    /// </summary>
    public GameObject ClickedObject { get; private set; }

    /// <summary>
    /// Parametr of control event
    /// </summary>
    public float Param { get; private set; }

    [SerializeField]
    private Hand hand;

    private Coroutine TrackingInputEventCoroutineInstance;

    #region Unity functions

    void Awake()
    {
        Initialize();
    }

    #endregion

    #region public functions

    public void ImitateSelectButtonClick(GameObject clickTo)
    {
        ControlEventType = ClickManager.ControlEvent.chooseEvent;
        Param = 0;
        ClickedObject = clickTo;
        ClickManager.Instance.ControlEventHappend(this);
    }

    #endregion

    #region private functions

    private void Initialize()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        ControlEventType = ClickManager.ControlEvent.chooseEvent;
        Param = 0;
        ClickedObject = null;
        TrackingInputEventCoroutineInstance = StartCoroutine(TrackingInputEventCoroutine());
    }

    private void SelectButtonClick()
    {
        ControlEventType = ClickManager.ControlEvent.chooseEvent;
        Param = 0;

        Ray ray;
        RaycastHit hit;
        ray = new Ray(hand.gameObject.transform.position, hand.gameObject.transform.forward * 100); // 100 - это просто дальность
        if (Physics.Raycast(ray, out hit, 100)) //рисуем лучик
        {
            if (hit.collider.gameObject.transform.parent == null)
            {
                ClickedObject = null;
            }
            else
            {
                ClickedObject = hit.collider.gameObject.transform.parent.gameObject;
            }
            ClickManager.Instance.ControlEventHappend(this);
        }
    }

    #endregion

    #region coroutines

    private IEnumerator TrackingInputEventCoroutine()
    {
        while (true)
        {//Tracking the click...                 

            #region select button click
            if (SteamVR_Input._default.inActions.GrabPinch.GetStateDown(hand.handType))
            {
                SelectButtonClick();
            }
            #endregion

            //дописать еще другие клики, и события
            yield return null;
        }
    }

    #endregion

}
