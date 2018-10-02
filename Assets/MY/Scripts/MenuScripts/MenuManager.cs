using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IClickable
{

    List<MenuObject> GetListOfMenuObject();

}

public class MenuObject : MonoBehaviour
{
    public delegate void OnClickDelegate();

    public enum TypeOfObject
    {
        firstLine,
        secondLine
    }

    public TypeOfObject typeOfObject;
    public OnClickDelegate onClick;
    public MonoBehaviour ItemInstance;
}

public class MenuManager : MonoBehaviour 
{

    /// <summary>
    /// Singleton 
    /// </summary>
    public static MenuManager Instance;

    #region Unity functions
    #endregion

    #region public functions

    void ClickedOnClickable(IClickable clickableObject)
    {
        List<MenuObject> temp = clickableObject.GetListOfMenuObject();
        
    }

    #endregion

    #region private functions
    #endregion

}
