using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for managing the clicking mouse
/// </summary>
public class ClickManager : MonoBehaviour 
{

    /// <summary>
    /// Singleton
    /// </summary>
    public static ClickManager Instance { get; private set; }

    /// <summary>
    /// The object, that was clicked last
    /// </summary>
    public GameObject clickTarget { get; private set; }

    private Coroutine TrackingClikableCoroutineInstance;

    private bool IsFrozen;

    #region Unity functions

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        Initialize();
    }

    #endregion

    #region public functions

    

    #endregion

    #region private functions

    private void StartClickableTracking()
    {
        if (TrackingClikableCoroutineInstance == null)
        {
            TrackingClikableCoroutineInstance = StartCoroutine(TrackingClikableCoroutine());
        }
#if UNITY_EDITOR
        else
        {
            Debug.Log("Are you really want to create another one coroutine?? I think no. So - dont do that again.");
        }
#endif
    }

    private void StopClickableTracking()
    {
        if (TrackingClikableCoroutineInstance != null)
        {
            StopCoroutine(TrackingClikableCoroutineInstance);
            TrackingClikableCoroutineInstance = null;
        }
#if UNITY_EDITOR
        else
        {
            Debug.Log("You try to stop null coroutine. Dont do that. That sucks.");
        }
#endif
    }

    private void Initialize()
    {
        //Start managing the clicks
        IsFrozen = false;
        StartClickableTracking();
    }

    private void ClickableClick(IClickable objectClicked)
    {
        MenuManager.Instance.ClickedOnClickable(objectClicked);
    }

    private void MenuObjectClick(MenuObject menuObject)
    {
        menuObject.onClick(menuObject.ItemInstance);
    }

    #endregion

    #region Coroutines

    private IEnumerator TrackingClikableCoroutine()
    {
        while (true)
        {//Tracking the click...            
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray;
                RaycastHit hit;
                
                //если клик произошел, то мы проверяем меню
                if (MenuManager.Instance.IsShown) // если меню показано, то...
                {
                    //создание луча
                    ray = MenuManager.Instance.GetMenuCamera().ScreenPointToRay(Input.mousePosition);
                }
                else
                {
                    ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
                }

                #region menu click
                //пробуем меню
                if (Physics.Raycast(ray, out hit, 100)) //рисуем лучик
                {
                    clickTarget = hit.collider.gameObject; //назначаем объект клика
                    if (clickTarget.transform.parent.gameObject.GetComponent<MenuObject>() != null) //пытаемся нажать на него, как на объект на сцене
                    {
                        MenuObjectClick(clickTarget.transform.parent.gameObject.GetComponent<MenuObject>());
                        yield return null;
                        continue;
                    }
                }

                #endregion
                #region scene object click
                //если мы дошли сюда, значит с меню что-то пошло не так, либо меню не показанно, либо не кликнули
                //пытаемся кликнуть, как на обычный объект                
                if (Physics.Raycast(ray, out hit, 100)) //пытаеся кликнуть как на объект сцены
                {
                    clickTarget = hit.collider.gameObject; //назначаем объект клика
                    if (clickTarget.transform.parent.gameObject.GetComponent<IClickable>() != null) //пытаемся нажать на него, как на объект на сцене
                    {
                        ClickableClick(clickTarget.transform.parent.gameObject.GetComponent<IClickable>());
                        Froze();
                        yield return null;
                        continue;
                    }
                }
                #endregion

                UnFroze();
            }
            yield return null;
        }
    }

    /// <summary>
    /// Stop controller, free cursor
    /// </summary>
    private void Froze()
    {
        if (!IsFrozen)
        {
            FirstPersonController.Instance.StopController();
            Cursor.lockState = CursorLockMode.None;
            MenuManager.Instance.ShowMenu();
            IsFrozen = true;
        }
    }

    /// <summary>
    /// Run the controller, lock cursor
    /// </summary>
    private void UnFroze()
    {
        if (IsFrozen)
        {
            FirstPersonController.Instance.PlayController();
            Cursor.lockState = CursorLockMode.Locked;
            MenuManager.Instance.HideMenu();
            IsFrozen = false;
        }
    }

    #endregion

}
