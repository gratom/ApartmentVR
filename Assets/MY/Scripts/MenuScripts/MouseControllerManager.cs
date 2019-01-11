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

    public void ImitateSelectButtonClick(GameObject clickTo)
    {
        InputData.ControlEventType = ClickManager.ControlEvent.chooseEvent;
        InputData.Param = 0;
        InputData.ClickedObject = clickTo;
        ClickManager.Instance.ControlEventHappend(InputData);
    }

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

    private void ShowDebug(MyVRMenu.MenuItem menuItem)
    {
        if(menuItem != null)
        {
            if(menuItem.typeOfObject == MyVRMenu.MenuLine.TypeOfLine.firstLine)
            {
                MenuDebugger.Instance.SetText(((SceneChangebleObject)menuItem.ItemInstance).ToString());
                return;
            }
            if (menuItem.typeOfObject == MyVRMenu.MenuLine.TypeOfLine.secondLine)
            {
                MenuDebugger.Instance.SetText(((LoadedMaterial)menuItem.ItemInstance).ToString());
                return;
            }
        }
        MenuDebugger.Instance.EraseText();
    }

    private void MenuRotate(GameObject hit)
    {
        InputData.ControlEventType = ClickManager.ControlEvent.menuRotate;
        InputData.Param = Input.mouseScrollDelta.y * Sensitivity;
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
            ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

            if (Physics.Raycast(ray, out hit, 100)) //рисуем лучик
            {
                //LinePointer.SetPosition(1, hit.point);

                if (hit.collider.gameObject.transform.parent != null)
                {
                    if (hit.collider.gameObject.transform.parent.gameObject.GetComponent<MyVRMenu.MenuItem>() != null)
                    {
                        LastPointedItem = hit.collider.gameObject.transform.parent.gameObject.GetComponent<MyVRMenu.MenuItem>();
                        LastPointedItem.onPoint(LastPointedItem, true);

                        LastPointedGameObject = hit.collider.gameObject;
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
            if (Input.GetMouseButtonDown(0))
            {
                SelectButtonClick(hit.collider.gameObject);
                continue;
            }
            #endregion

            #region rotate menu
            if (Input.mouseScrollDelta.y != 0 && LastPointedGameObject != null)
            {
                MenuRotate(LastPointedGameObject);
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

    #endregion

}