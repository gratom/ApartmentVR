using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class for visualizing loading process
/// </summary>
public class LoadingVisualizer : MonoBehaviour 
{

    /// <summary>
    /// Singleton
    /// </summary>
    public static LoadingVisualizer Instance { get; private set; }

    [SerializeField]
    private Text TextProcess;

    [SerializeField]
    private Text StatusText;

    private int Counter;

    /// <summary>
    /// counter - is the int value of step loading
    /// </summary>
    public int counter
    {
        get { return this.Counter; }
        set
        {
            this.Counter = value < 0 ? 0 : value > 100 ? 100 : value;
            TextProcess.text = this.Counter.ToString() + "%";
        }
    }

    public string StatusBarText
    {
        get { return this.StatusText.text; }
        set { this.StatusText.text = value; }
    }

    #region Unity functions

    void Awake()
    {
        Initialize();
        //TestLoadingPersentCoroutine();
    }
    
    #endregion

    #region public functions

    #endregion

    #region private functions

    private void Initialize()
    {
        counter = 0;
        if (Instance == null)
        {
            Instance = this;
        }
    }

    #endregion

    #region testing

    private void TestValue1()
    {
        counter = 0;
        Debug.Log(counter == 0);
        counter++;
        Debug.Log(counter == 1);
        counter = 55;
        Debug.Log(counter == 55);
        counter = -10;
        Debug.Log(counter == 0);
        counter = 110;
        Debug.Log(counter == 100);
    }

    private void TestLoadingPersentCoroutine()
    {
        StartCoroutine(LoadingPersentCoroutine());
    }

    private IEnumerator LoadingPersentCoroutine()
    {
        while (counter < 100)
        {
            counter++;
            yield return new WaitForSeconds(0.2f);
            if (counter == 10)
            {
                StatusBarText += "\nCheck the internet...";
            }
            if (counter == 30)
            {
                StatusBarText += "\nInternet OK.\nLoading data...";
            }
            if (counter == 50)
            {
                StatusBarText += "\nData was load.\nLoading images and textures...";
            }
            if (counter == 70)
            {
                StatusBarText += "\nUnpacking...";
            }
            if (counter == 80)
            {
                StatusBarText += "\nUnpacking complete.\nCreate the objects...";
            }
            if (counter == 95)
            {
                StatusBarText += "\nComplete.\nLoad main menu...";
            }
        }
        yield return null;
    }

    #endregion

}
