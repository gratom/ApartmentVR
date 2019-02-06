using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for pointing current objects, in which added script BaseEffect
/// </summary>
public class Pointer : MonoBehaviour
{

    public enum PointerEvents
    {
        HitAny,
        HitEmpty,
        HitActionObject,
        HitNewActionObject,        
        NoHit       
    }

    public delegate void PointerAction(InteractiveObject oldBaseActionObject, InteractiveObject newBaseActionObject);

    public InteractiveObject lastBaseEffect
    {
        get
        {
            if (_lastBaseEffect == null)
            {
                if (DefaultInteractive == null)
                {
                    GameObject gTemp = new GameObject("BaseEffectNullObject");
                    DefaultInteractive = gTemp.AddComponent<InteractiveObject>();
                }
                return DefaultInteractive;
            }    
            return _lastBaseEffect;                        
        }
        private set
        {
            _lastBaseEffect = value;
        }
    }
    private InteractiveObject _lastBaseEffect;

    private InteractiveObject DefaultInteractive;

    public Vector3 LastHitPoint { get; private set; }

    public Vector3 LastHitNormal { get; private set; }

    public bool IsCoroutineWorking { get { return lineCoroutineInstance != null; } }

    public bool IsWorking = true;

    private Dictionary<PointerEvents, List<PointerAction>> EventsDictionary;    

    private Coroutine lineCoroutineInstance;

    #region Unity functions

    private void Awake()
    {
        Initialize();
    }

    #endregion

    #region private functions

    private void Initialize()
    {        
        EventsDictionary = new Dictionary<PointerEvents, List<PointerAction>>();
        for (int i = 0; i < System.Enum.GetNames(typeof(PointerEvents)).Length; i++)
        {
            EventsDictionary.Add((PointerEvents)i, new List<PointerAction>());
        }
        StartPointerProcess();
    }

    private void PointerEventHappend(PointerEvents eventName, InteractiveObject newBaseActionObject)
    {
        PointerAction[] ExecutingArray = new PointerAction[EventsDictionary[eventName].Count];
        EventsDictionary[eventName].CopyTo(ExecutingArray);
        for (int i = 0; i < ExecutingArray.Length; i++)
        {
            ExecutingArray[i](lastBaseEffect, newBaseActionObject);
        }
    }

    #endregion

    #region public functions

    public void TryToReInit()
    {
        StopPointerProcess();
        Initialize();
    }
    
    public void StartPointerProcess()
    {
        if (lineCoroutineInstance == null)
        {
            lineCoroutineInstance = StartCoroutine(LineCoroutine());
        }
    }

    public void StopPointerProcess()
    {
        if (lineCoroutineInstance != null)
        {
            StopCoroutine(lineCoroutineInstance);
            lineCoroutineInstance = null;
        }
    }

    public void AddDelegate(PointerEvents eventName, PointerAction pointerAction)
    {
        if (EventsDictionary != null)
        {
            EventsDictionary[eventName].Add(pointerAction);
        }
    }

    public void RemoveDelegate(PointerEvents eventName, PointerAction pointerAction)
    {
        if (EventsDictionary != null)
        {
            EventsDictionary[eventName].Remove(pointerAction);
        }
    }

    #endregion

    #region coroutines

    private IEnumerator LineCoroutine()
    {
        while (true)
        {
            yield return null;
            if (IsWorking)
            {
                Ray ray;
                RaycastHit hit;
                ray = new Ray(transform.position, transform.forward * 100);
                if (Physics.Raycast(ray, out hit, 100))
                {
                    LastHitPoint = hit.point;
                    LastHitNormal = hit.normal;
                    PointerEventHappend(PointerEvents.HitAny, null);
                    InteractiveObject tempBaseEffect = hit.collider.gameObject.GetComponent<InteractiveObject>();  
                    if (tempBaseEffect != null)
                    {
                        PointerEventHappend(PointerEvents.HitActionObject, tempBaseEffect);
                        if (lastBaseEffect != tempBaseEffect)
                        {
                            PointerEventHappend(PointerEvents.HitNewActionObject, tempBaseEffect);
                            lastBaseEffect = tempBaseEffect;
                        }
                    }
                    else
                    {
                        PointerEventHappend(PointerEvents.HitEmpty, null);
                    }
                }
                else
                {
                    PointerEventHappend(PointerEvents.NoHit, null);
                }
            }            
        }
    }

    #endregion

}