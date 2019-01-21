using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

/// <summary>
/// Class 
/// </summary>
public class VRControllerManager : MonoBehaviour
{

    #region IControlable property

    public ControlInputData InputData;

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

    #endregion

    #region private functions

    private void Initialize()
    {
        InputData = new ControlInputData();
        InputData.ControlEventType = ClickManager.ControlEvent.chooseEvent;
        InputData.Param = 0;
        InputData.ClickedObject = null;
        TrackingInputEventCoroutineInstance = StartCoroutine(TrackingInputEventCoroutine());
    }

    private void SelectButtonClick(GameObject hit)
    {
        InputData.ControlEventType = ClickManager.ControlEvent.chooseEvent;
        InputData.Param = 0;
        if (hit.transform.parent == null)
        {
            InputData.ClickedObject = null;
        }
        else
        {
            InputData.ClickedObject = hit.transform.parent.gameObject;
        }
        ClickManager.Instance.ControlEventHappend(InputData);
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

                if (hit.collider.gameObject.transform.parent != null)
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
