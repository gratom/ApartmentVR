using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for making the request to server and loading JSON
/// </summary>
public class RequestsModule : MonoBehaviour 
{

    /// <summary>
    /// Type of events in this class
    /// </summary>
    public enum RequestEvents
    {
        NoInternet,
        FindInternet,
        CheckInternet,
        StartAuthentification,
        AuthentificationOK,
        AuthentificationFailed,
        StartGettingAppList,
        GettingAppListOK,
        GettingAppListFailed,
        StartAppData,
        AppDataOK,
        AppDataFailed,
        ResponceReady
    }

    /// <summary>
    /// Is ready this manager
    /// </summary>
    public bool IsReady { get; private set; }

    /// <summary>
    /// Type of delegate for events on this class
    /// </summary>
    public delegate void RequestEventDelegate(string Responce);    

    private Dictionary<RequestEvents, List<RequestEventDelegate>> EventsDictionary;

    #region Unity Functions

    

    #endregion

    #region Public Functions

    /// <summary>
    /// Adding the new function to event
    /// </summary>
    /// <param name="eventName">The name of the event in which the function will be executed</param>
    /// <param name="REDelegate">The function</param>
    public void AddDelegate(RequestEvents eventName, RequestEventDelegate REDelegate)
    {
        EventsDictionary[eventName].Add(REDelegate);
    }

    /// <summary>
    /// Removing function from event
    /// </summary>
    /// <param name="eventName">The name of the event in which the function will be deleted</param>
    /// <param name="REDelegate">The function</param>
    public void RemoveDelegate(RequestEvents eventName, RequestEventDelegate REDelegate)
    {
        EventsDictionary[eventName].Remove(REDelegate);
    }

    /// <summary>
    /// Starting the coroutine InternetCheck
    /// </summary>
    public void CheckInternetStart()
    {
        StartCoroutine(CheckInternetCoroutine());
    }

    /// <summary>
    /// Starting the coroutine Authentification
    /// </summary>
    public void AuthentificationStart(string key)
    {
        StartCoroutine(AuthentificationCoroutine(key));
    }

    /// <summary>
    /// Starting the coroutine GetAppList
    /// </summary>
    public void GetAppListStart(string accessToken)
    {
        StartCoroutine(GetAppListCoroutine(accessToken));
    }

    /// <summary>
    /// Starting the coroutine AppData
    /// </summary>
    public void AppDataStart(int AppID, string accessToken)
    {
        StartCoroutine(GetAppDataCoroutine(AppID, accessToken));
    }

    public void InitEventsDictionary()
    {
        if (EventsDictionary == null)
        {
            EventsDictionary = new Dictionary<RequestEvents, List<RequestEventDelegate>>();
            for (int i = 0; i < System.Enum.GetNames(typeof(RequestEvents)).Length; i++)
            {
                EventsDictionary.Add((RequestEvents)i, new List<RequestEventDelegate>());
            }
        }
    }

    #endregion

    #region Coroutines

    private IEnumerator CheckInternetCoroutine()
    {
        RequestEventHappend(RequestEvents.CheckInternet, "Event:RequestEvents.CheckInternet");
        WWW www = new WWW("https://gudhub.com/GudHub");
        yield return www;
        if (www.error != null)
        {
            RequestEventHappend(RequestEvents.NoInternet, "Event:RequestEvents.NoInternet:" + www.error);
        }
        else
        {
            RequestEventHappend(RequestEvents.FindInternet, "Event:RequestEvents.FindInternet");
        }
    }

    private IEnumerator AuthentificationCoroutine(string authKey)
    {
        RequestEventHappend(RequestEvents.StartAuthentification, "Event:RequestEvents.StartAuthentification");
        WWWForm wwwForm = new WWWForm();
        wwwForm.AddField("authkey", authKey);
        string url = "https://gudhub.com/GudHub/auth/login?auth_key=" + authKey;

        WWW AuthRequest = new WWW(url, wwwForm);
        yield return AuthRequest;

        if (AuthRequest.error == null) // no errors
        {
            RequestEventHappend(RequestEvents.AuthentificationOK, AuthRequest.text);
            RequestEventHappend(RequestEvents.ResponceReady, AuthRequest.text);
        }
        else
        {
            RequestEventHappend(RequestEvents.AuthentificationFailed, "Event:RequestEvents.AuthentificationFailed:" + AuthRequest.error);
            RequestEventHappend(RequestEvents.ResponceReady, "Event:RequestEvents.AuthentificationFailed:" + AuthRequest.error);
        }
    }

    private IEnumerator GetAppListCoroutine(string accessToken)
    {
        RequestEventHappend(RequestEvents.StartGettingAppList, "Event:RequestEvents.StartGettingAppList");
        string url = "https://gudhub.com/GudHub/api/applist/get?token=" + accessToken;
        WWW AppListRequest = new WWW(url);
        yield return AppListRequest;
        
        if (AppListRequest.error == null) // no errors
        {
            RequestEventHappend(RequestEvents.GettingAppListOK, AppListRequest.text);
            RequestEventHappend(RequestEvents.ResponceReady, AppListRequest.text);
        }
        else
        {
            RequestEventHappend(RequestEvents.GettingAppListFailed, "Event:RequestEvents.GettingAppListFailed:" + AppListRequest.error);
            RequestEventHappend(RequestEvents.ResponceReady, "Event:RequestEvents.GettingAppListFailed:" + AppListRequest.error);
        }
    }

    private IEnumerator GetAppDataCoroutine(int appID, string accessToken)
    {
        RequestEventHappend(RequestEvents.StartAppData, "Event:RequestEvents.StartAppData");        
        string url = "https://gudhub.com/GudHub/api/app/get?app_id=" + appID + "&token=" + accessToken;
        WWW AppDataRequest = new WWW(url);
        yield return AppDataRequest;
        if (AppDataRequest.error == null) // no errors
        {
            RequestEventHappend(RequestEvents.AppDataOK, AppDataRequest.text);
            RequestEventHappend(RequestEvents.ResponceReady, AppDataRequest.text);
        }
        else
        {
            RequestEventHappend(RequestEvents.AppDataFailed, "Event:RequestEvents.AppDataFailed:" + AppDataRequest.error);
            RequestEventHappend(RequestEvents.ResponceReady, "Event:RequestEvents.AppDataFailed:" + AppDataRequest.error);
        }
    }

    #endregion

    #region Private Functions

    private void RequestEventHappend(RequestEvents eventName, string Responce)
    {
        RequestEventDelegate[] ExecutingArray = new RequestEventDelegate[EventsDictionary[eventName].Count];
        EventsDictionary[eventName].CopyTo(ExecutingArray);
        for (int i = 0; i < ExecutingArray.Length; i++)
        {
            ExecutingArray[i](Responce);
        }
    }

    #endregion

}