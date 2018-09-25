using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AbstractObjectConstructableComponentData
{
    public enum ValueType
    {
        type1,
        type2,
        type3,
        type4
    }    
    public ValueType valueType;
    public int IdField;
    public string StringValue;
}

public abstract class AbstractObjectConstructable : MonoBehaviour
{

    public List<AbstractObjectConstructableComponentData> ComponentsDataList;

    public abstract void InitConstruct();

}
