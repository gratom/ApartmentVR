using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Types of field in MaterialGroup
/// </summary>
public enum MaterialGroupClassTypes
{
    nameMaterialGroup
}

/// <summary>
/// Serializeble class for start setting
/// </summary>
[System.Serializable]
public class SettingForFieldInMaterialGroup : AbstractObjectConstructableComponentData<MaterialGroupClassTypes> { }

/// <summary>
/// Class for group materials
/// </summary>
[System.Serializable]
public class MaterialGroup : AbstractObjectConstructable<MaterialGroupClassTypes>
{

    /// <summary>
    /// Name of material group, that used to determite in which material can painted sceneChangableObject 
    /// </summary>
    public string nameOfMaterialGroup;

    [Tooltip("This is a list of settings for the correct operation of the internal functions for initializing an item.\nThese settings are used to determine how to process data from JSON.")]
    public List<SettingForFieldInMaterialGroup> settingFieldList;

    #region public functions

    /// <summary>
    /// The Initializing functions. Make the initialization by data from ComponentsDataList.
    /// </summary>
    public override void InitConstruct()
    {
        if (FunctionsDictionary != null)
        {
            for (int i = 0; i < ComponentsDataList.Count; i++)
            {
                if (FunctionsDictionary.ContainsKey(ComponentsDataList[i].valueType))
                {
                    FunctionsDictionary[ComponentsDataList[i].valueType](i);
                }
            }
        }
    }

    /// <summary>
    /// Use this BEFORE initializating. This function make the inner cohesion in class instance 
    /// </summary>
    public override void InitDictionary()
    {
        FunctionsDictionary = new Dictionary<MaterialGroupClassTypes, InitFunctions>
        {
            { MaterialGroupClassTypes.nameMaterialGroup, InitNameOfGroup }
        };

        if (ComponentsDataList == null)
        {
            SettingListFieldToRealFields();
        }
    }

    /// <summary>
    /// Returns a nicely edited string with information about an object.
    /// </summary>
    /// <returns>Nicely edited string</returns>
    public override string ToString()
    {
        return base.ToString();
    }

    #endregion

    #region private functions

    private void SettingListFieldToRealFields()
    {
        ComponentsDataList = new List<AbstractObjectConstructableComponentData<MaterialGroupClassTypes>>();
        for (int i = 0; i < settingFieldList.Count; i++)
        {
            ComponentsDataList.Add(settingFieldList[i]);
        }
    }

    private void InitNameOfGroup(int num)
    {
        nameOfMaterialGroup = ComponentsDataList[num].StringValue;
    }

    #endregion

}
