using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class for visualizing loading process
/// </summary>
public class LoadingVisualizer : MonoBehaviour 
{

    [SerializeField]
    private Text TextProcess;

    private int Counter;

    public int counter
    {
        get
        {
            return this.Counter;
        }
        set
        {
            this.Counter = value < 0 ? 0 : value > 100 ? 100 : value;
            TextProcess.text = this.Counter.ToString() + "%";
        }
    }

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

    }

    #endregion

    #region Coroutines

    private IEnumerator LoaderCoroutine()
    {
        yield return null;
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
            yield return new WaitForSeconds(0.1f);
        }
        yield return null;
    }

    #endregion

}
