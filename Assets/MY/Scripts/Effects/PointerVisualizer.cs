using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Pointer))]
[RequireComponent(typeof(LineRenderer))]
/// <summary>
/// Class for visualizing action and events of pointer
/// </summary>
public class PointerVisualizer : MonoBehaviour
{

    /// <summary>
    /// Minimum distance of showing ray
    /// </summary>
    public float MinDistance = 20;

    /// <summary>
    /// Animation, that playing, if pointer point something interactive
    /// </summary>
    public GameObject pointingAnimation;

    public bool IsShowing = true;

    private Pointer PointerInstance;

    private LineRenderer LinePointer;

    public bool IsReady { get; private set; }

    #region Unity functions

    private void Start()
    {
        IsReady = Initialize();
    }

    #endregion

    #region public functions

    public void TryToReInit()
    {
        if (!IsReady)
        {
            IsReady = Initialize();
        }
    }

    #endregion

    #region private functions

    private bool Initialize()
    {
        PointerInstance = GetComponent<Pointer>();
        if (PointerInstance == null)
        {
            Debug.LogError("Can`t find require component typeof<Pointer> for PointerVisualizer! PointerVisualizer will not work.");
            return false;   
        }
        LinePointer = GetComponent<LineRenderer>();
        if (LinePointer == null)
        {
            Debug.LogError("Can`t find require component typeof<LineRenderer> for PointerVisualizer! PointerVisualizer will not work.");
            return false;
        }
        LinePointer.useWorldSpace = false;

        PointerInstance.AddDelegate(Pointer.PointerEvents.HitActionObject, OnHoverAction);
        PointerInstance.AddDelegate(Pointer.PointerEvents.HitEmpty, OnLeftAction);
        PointerInstance.AddDelegate(Pointer.PointerEvents.NoHit, NoHitAction);
        return true;
    }

    private void OnHoverAction(InteractiveObject oldBaseActionObject, InteractiveObject newBaseActionObject)
    {
        if (IsShowing)
        {
            if (oldBaseActionObject != null)
            {
                if (oldBaseActionObject != newBaseActionObject)
                {
                    oldBaseActionObject.OnPointerLeft();
                }
            }
            if (newBaseActionObject != null)
            {
                newBaseActionObject.OnPointerHover();
                if (newBaseActionObject.GetComponent<MyVRMenu.MenuItem>() == null && newBaseActionObject.GetComponent<ExtendedInteractive>() == null)
                {
                    pointingAnimation.transform.position = PointerInstance.LastHitPoint;
                    pointingAnimation.transform.LookAt(PointerInstance.LastHitPoint - PointerInstance.LastHitNormal);
                    pointingAnimation.SetActive(true);
                }
                else
                {                    
                    pointingAnimation.SetActive(false);
                }
            }
            LinePointer.SetPosition(1, new Vector3(0, 0, Vector3.Distance(PointerInstance.LastHitPoint, LinePointer.transform.position)));
        }
        else
        {
            OnLeftAction(oldBaseActionObject, newBaseActionObject);
        }
    }

    private void OnLeftAction(InteractiveObject oldBaseActionObject, InteractiveObject newBaseActionObject)
    {
        if (oldBaseActionObject != null)
        {
            oldBaseActionObject.OnPointerLeft();
        }
        if (IsShowing)
        {
            LinePointer.SetPosition(1, new Vector3(0, 0, Vector3.Distance(PointerInstance.LastHitPoint, LinePointer.transform.position)));
        }
        else
        {
            LinePointer.SetPosition(1, new Vector3(0, 0, 0));
        }
        pointingAnimation.SetActive(false);
    }

    private void NoHitAction(InteractiveObject oldBaseActionObject, InteractiveObject newBaseActionObjec)
    {
        if (oldBaseActionObject != null)
        {
            oldBaseActionObject.OnPointerLeft();
        }
        if (IsShowing)
        {
            LinePointer.SetPosition(1, new Vector3(0, 0, MinDistance));
        }
        else
        {
            LinePointer.SetPosition(1, new Vector3(0, 0, 0));
        }
        pointingAnimation.SetActive(false);
    }

    #endregion

}
