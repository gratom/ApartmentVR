using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Inteface for clickable object
/// </summary>
public interface IClickable
{

    List<MenuObject> GetListOfMenuObject();

}

/// <summary>
/// Class for menu object. All menu items are such classes
/// </summary>
public class MenuObject : MonoBehaviour
{

    public delegate void OnClickDelegate(MonoBehaviour item);

    public enum TypeOfObject
    {
        firstLine,
        secondLine
    }

    public TypeOfObject typeOfObject;
    public OnClickDelegate onClick;
    public MonoBehaviour ItemInstance;

    public MenuObject(TypeOfObject menuItemType, MonoBehaviour referenceItem, OnClickDelegate onClickDelegate = null)
    {
        typeOfObject = menuItemType;
        ItemInstance = referenceItem;
        onClick = onClickDelegate;
    }
    
}

/// <summary>
/// Class for menu managing
/// </summary>
public class MenuManager : MonoBehaviour 
{

    /// <summary>
    /// Singleton
    /// </summary>
    public static MenuManager Instance { get; private set; }

    #region Unity functions

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    #endregion

    #region public functions

    /// <summary>
    /// Of someone click on clickable object, the click-manager is should call this function. Also, if you want simulate the click of any object,
    /// that have the IClickable interface, call this.
    /// </summary>
    /// <param name="clickableObject"></param>
    public void ClickedOnClickable(IClickable clickableObject)
    {
        List<MenuObject> temp = clickableObject.GetListOfMenuObject();
        
        //TODO: сделать графику, и функцию, которая заполняет заспавненные ячейки тем, что прислали...

        //примерно так
        //сделать ячейки по количеству присланного
        //запихать туда объекты
        //ждать пока ClickableMAnager не скажет спрятаться или что-то еще.
    }

    #endregion

    #region private functions
    #endregion

}
