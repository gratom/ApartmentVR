using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for interpitate AppData to normal Object
/// </summary>
public static class JSONInterpritator 
{

    public static AbstractObjectConstructable<TypeElement> GetStringValueByID<TypeElement>(App appData, AbstractObjectConstructable<TypeElement> AOC, int ItemID) where TypeElement : struct, System.IConvertible 
    {
        for (int i = 0; i < AOC.ComponentsDataList.Count; i++) // для каждого элемента в объекте
        {
            for (int j = 0; j < appData.items_list[ItemID].fields.Count; j++) // перебор всех полей
            {
                if (AOC.ComponentsDataList[i].IdField == appData.items_list[ItemID].fields[j].field_id)
                {
                    AOC.ComponentsDataList[i].StringValue = appData.items_list[ItemID].fields[j].field_value;
                }
            }
        }
        return AOC;
    }

}
