using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuDebugger : MonoBehaviour
{

    /// <summary>
    /// Singleton
    /// </summary>
    public static MenuDebugger Instance { get; private set; }

    [SerializeField]
    private Text debugText;

    [SerializeField]
    private GameObject PanelForText;

    #region Unity functions

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    #endregion

    #region public functions

    public void SetText(string text)
    {
        PanelForText.SetActive(true);
        debugText.text = text;
    }

    public void EraseText()
    {
        PanelForText.SetActive(false);
        debugText.text = "";
    }

    #endregion

    #region private functions
    #endregion

}
