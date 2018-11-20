using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for setting and store data of AbstractObjectConstructable class
/// </summary>
/// <typeparam name="TypeElement">Any of System.Enum</typeparam>
[System.Serializable]
public class AbstractObjectConstructableComponentData<TypeElement> where TypeElement : struct, System.IConvertible
{
    /// <summary>
    /// Any of System.Enum
    /// </summary>
    public TypeElement valueType;

    /// <summary>
    /// ID value of field in JSON 
    /// </summary>
    public int IdField;

    /// <summary>
    /// String value of field in JSON
    /// </summary>
    public string StringValue;
}

[System.Serializable]
public abstract class AbstractObjectConstructable <TypeElement> : MonoBehaviour where TypeElement : struct, System.IConvertible
{

    /// <summary>
    /// unique index in GudHub
    /// </summary>
    public int ID;

    protected delegate void InitFunctions(int num);
    protected Dictionary<TypeElement, InitFunctions> FunctionsDictionary;

    /// <summary>
    /// Data and setting of this class
    /// </summary>
    public List<AbstractObjectConstructableComponentData<TypeElement>> ComponentsDataList;

    /// <summary>
    /// The Initializing functions. Make the initialization by data from ComponentsDataList.
    /// </summary>
    public abstract void InitConstruct();

    /// <summary>
    /// Use this BEFORE initializating. This function make the inner cohesion in class instance 
    /// </summary>
    public abstract void InitDictionary();

    /// <summary>
    /// Get the AbstractObjectConstructableComponentData class by type of data in this class
    /// </summary>
    /// <param name="VType">Type of data in this class</param>
    /// <returns>The AbstractObjectConstructableComponentData class instance</returns>
    public AbstractObjectConstructableComponentData<TypeElement> GetDataByType(TypeElement VType)
    {
        for (int i = 0; i < ComponentsDataList.Count; i++)
        {
            if (ComponentsDataList[i].valueType.Equals(VType))
            {
                return ComponentsDataList[i];
            }
        }
        return null;
    }

}
