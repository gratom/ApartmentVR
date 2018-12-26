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

    #region IControlable property

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

    #endregion

    [SerializeField]
    private LineRenderer LinePointer;

    private MyVRMenu.MenuItem LastPointedItem;

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
        ControlEventType = ClickManager.ControlEvent.chooseEvent;
        Param = 0;
        ClickedObject = null;
        TrackingInputEventCoroutineInstance = StartCoroutine(TrackingInputEventCoroutine());
    }

    private void SelectButtonClick(GameObject hit)
    {
        ControlEventType = ClickManager.ControlEvent.chooseEvent;
        Param = 0;
        if (hit.transform.parent == null)
        {
            ClickedObject = null;
        }
        else
        {
            ClickedObject = hit.transform.parent.gameObject;
        }
        ClickManager.Instance.ControlEventHappend(this);
    }

    #endregion

    #region coroutines

    private IEnumerator TrackingInputEventCoroutine()
    {
        while (true)
        {//Tracking the click...                 
            yield return null;

            Ray ray;
            RaycastHit hit;
            ray = new Ray(hand.gameObject.transform.position, hand.gameObject.transform.forward * 100); // 100 - это просто дальность

            if (Physics.Raycast(ray, out hit, 100)) //рисуем лучик
            {
                LinePointer.SetPosition(1, new Vector3(0, 0, Vector3.Distance(LinePointer.transform.position, hit.point)));
                
                if(hit.collider.gameObject.transform.parent != null)
                {
                    if (hit.collider.gameObject.transform.parent.gameObject.GetComponent<MyVRMenu.MenuItem>() != null)
                    {
                        LastPointedItem = hit.collider.gameObject.transform.parent.gameObject.GetComponent<MyVRMenu.MenuItem>();
                        LastPointedItem.onPoint(LastPointedItem, true);
                    }
                    else
                    {
                        if (LastPointedItem != null)
                        {
                            LastPointedItem.onPoint(LastPointedItem, false);
                        }
                    }
                }

            }
            else
            {
                continue;
            }

            #region select button click
            if (SteamVR_Input._default.inActions.GrabPinch.GetStateDown(hand.handType))
            {
                SelectButtonClick(hit.collider.gameObject);
            }
            #endregion

            //дописать еще другие клики, и события            
        }
    }

    #endregion

}
