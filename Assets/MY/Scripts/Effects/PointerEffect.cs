using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PointerEffect : MonoBehaviour
{

    public BaseEffect lastBaseEffect { get; private set; }

    public bool IsWorking { get { return lineCoroutineInstance != null; } }

    public bool IsShowing;

    public float MinDistance = 20;

    [SerializeField]
    private LineRenderer LinePointer;

    private Coroutine lineCoroutineInstance;

    private bool IsStateOK = false;

    #region Unity functions

    private void Awake()
    {
        Initialize();
    }

    #endregion

    #region private functions

    private void Initialize()
    {
        lastBaseEffect = new BaseEffect();
        IsStateOK = false;
        if (LinePointer == null)
        {
            LinePointer = GetComponent<LineRenderer>();            
        }
        if (LinePointer == null)
        {
            Debug.LogError("where is my require component type of 'LineRenderer'?");
        }
        else
        {
            IsStateOK = true;
            LinePointer.useWorldSpace = false;
            IsShowing = true;
            StartLineEffect();
        }
    }

    #endregion

    #region public functions

    public void TryToReInit()
    {
        StopLineEffect();
        Initialize();
    }

    public void StartLineEffect()
    {
        if (IsStateOK)
        {
            if(lineCoroutineInstance == null)
            {
                LinePointer.enabled = true;
                lineCoroutineInstance = StartCoroutine(LineCoroutine());
            }
        }
    }

    public void StopLineEffect()
    {
        if (IsStateOK)
        {
            if(lineCoroutineInstance != null)
            {
                StopCoroutine(lineCoroutineInstance);
                lineCoroutineInstance = null;
                LinePointer.enabled = false;
            }
        }
    }

    #endregion

    #region coroutines

    private IEnumerator LineCoroutine()
    {
        while (true)
        {
            yield return null;

            if (IsShowing)
            {
                Ray ray;
                RaycastHit hit;
                ray = new Ray(transform.position, transform.forward * 100);
                if (Physics.Raycast(ray, out hit, 100)) //рисуем лучик
                {
                    LinePointer.SetPosition(1, new Vector3(0, 0, Vector3.Distance(LinePointer.transform.position, hit.point)));
                    BaseEffect tempBaseEffect = hit.collider.gameObject.GetComponent<BaseEffect>();
                    if (tempBaseEffect != null)
                    {
                        if (lastBaseEffect != tempBaseEffect)
                        {
                            lastBaseEffect.OnPointerLeft();
                        }
                        lastBaseEffect = tempBaseEffect;
                        lastBaseEffect.OnPointerHover();
                    }
                    else
                    {
                        lastBaseEffect.OnPointerLeft();
                    }
                }
                else
                {
                    LinePointer.SetPosition(1, new Vector3(0, 0, MinDistance));
                    lastBaseEffect.OnPointerLeft();
                }
            }
            else
            {
                LinePointer.SetPosition(1, new Vector3(0, 0, 0));
                lastBaseEffect.OnPointerLeft();
            }
        }        
    }

    #endregion

}