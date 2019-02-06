using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseControllerManager : MonoBehaviour
{

    #region IControlable property

    public ControlInputData InputData;

    #endregion

    [SerializeField]
    private float Sensitivity;

    private MyVRMenu.MenuItem LastPointedItem;

    private GameObject LastPointedGameObject;

    private Coroutine TrackingInputEventCoroutineInstance;

    #region Unity functions

    void OnEnable()
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
        if(TrackingInputEventCoroutineInstance != null)
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
        if(TrackingInputEventCoroutineInstance == null)
        {
            StartCoroutine(TrackingInputEventCoroutine());
        }
    }

    #endregion
    
    #region private functions

    private void Initialize()
    {
        InputData = new ControlInputData();
        InputData.ControlEventType = ClickManager.ControlEvent.activeActionEvent;
        InputData.Param = 0;
        InputData.interactiveObject = null;
        TrackingInputEventCoroutineInstance = StartCoroutine(TrackingInputEventCoroutine());
    }

    private void SelectButtonClick(InteractiveObject interactiveObject)
    {
        InputData.ControlEventType = ClickManager.ControlEvent.activeActionEvent;
        InputData.Param = 0;
        if (interactiveObject)
        {
            InputData.interactiveObject = null;
        }
        else
        {
            InputData.interactiveObject = interactiveObject;
        }
        ClickManager.Instance.ControlEventHappend(InputData);
    }

    private void ShowDebug(MyVRMenu.MenuItem menuItem)
    {
        if(menuItem != null)
        {
            if(menuItem.typeOfObject == MyVRMenu.MenuLine.TypeOfLine.firstLine)
            {
                MenuDebugger.Instance.SetText(((SceneChangebleObject)menuItem.AttachedObject).ToString());
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

    private void MenuRotate(InteractiveObject interactiveObject)
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

    #endregion

    #region coroutines

    private IEnumerator TrackingInputEventCoroutine()
    {
        while (true)
        {//Tracking the click...                 
            yield return null;

            if (Camera.main != null)
            {
                Ray ray;
                RaycastHit hit;
                ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

                if (Physics.Raycast(ray, out hit, 100)) //рисуем лучик
                {
                    //LinePointer.SetPosition(1, hit.point);

                    if (hit.collider.gameObject.transform.parent != null)
                    {
                        if (hit.collider.gameObject.transform.parent.gameObject.GetComponent<MyVRMenu.MenuItem>() != null)
                        {
                            LastPointedItem = hit.collider.gameObject.transform.parent.gameObject.GetComponent<MyVRMenu.MenuItem>();
                            //LastPointedItem.onPoint(LastPointedItem, true);

                            LastPointedGameObject = hit.collider.gameObject;
                        }
                        else
                        {
                            if (LastPointedItem != null)
                            {
                                //LastPointedItem.onPoint(LastPointedItem, false);
                            }
                        }
                    }

                }
                else
                {
                    continue;
                }

                #region select button click
                if (Input.GetMouseButtonDown(0))
                {
                    SelectButtonClick(hit.collider.gameObject.GetComponent<InteractiveObject>()); //TODO переделать
                    continue;
                }
                #endregion

                #region rotate menu
                if (Input.mouseScrollDelta.y != 0 && LastPointedGameObject != null)
                {
                    MenuRotate(LastPointedGameObject.GetComponent<InteractiveObject>()); //TODO переделать
                    continue;
                }
                #endregion

                #region show debug
                if (Input.GetMouseButtonDown(1))
                {
                    ShowDebug(LastPointedItem);
                    continue;
                }
                #endregion

                //дописать еще другие клики, и события       
            }
        }
    }

    #endregion

}