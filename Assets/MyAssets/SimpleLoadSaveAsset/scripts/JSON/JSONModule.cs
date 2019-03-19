using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#region JSON classes

[Serializable]
public class UserInformation
{
    public int user_id;
    public string accesstoken;
    public string username;
    public string fullname;
    public string avatar_128;
    public string avatar_512;
    public string auth_key;
    public long expirydate;
}

[Serializable]
public class Icon
{
    public int id;
    public string icon_id;
    public string icon_color;
    public string gradient_up;
    public string gradient_down;
}

[Serializable]
public class Field
{
    public int field_id;
    public string field_value;
    public int data_id;
}

[Serializable]
public class ItemsList
{
    public int item_id;
    public object last_update;
    public List<Field> fields;
}

[Serializable]
public class Column
{
    public int column_id;
    public string column_title;
    public string column_class;
    public List<object> fields;
}

[Serializable]
public class Container
{
    public int row_id;
    public string row_title;
    public string row_class;
    public int row_position;
    public List<Column> columns;
}

[Serializable]
public class ViewList
{
    public string name;
    public int view_id;
}

[Serializable]
public class Option
{
    public string name;
    public string value;
    public string color;
}

[Serializable]
public class DataModel
{
    public string image_preview_selected_size;
    public string display_mode;
    public bool image_editor;
    public double app_id;
    public string max_value;
    public List<Option> options;
    public bool multiple_value;
    public List<Ref> refs;
}

[Serializable]
public class Ref
{
    public int app_id;
    public int field_id;
}

[Serializable]
public class FieldList
{
    public int field_id;
    public string name_space;
    public int field_priority;
    public string field_name;
    public string data_type;
    public DataModel data_model;
}

[Serializable]
public class FileList
{
    public int file_id;
    public int app_id;
    public int item_id;
    public string file_name;
    public string url;
    public string extension;
}

[Serializable]
public class App
{
    public int app_id;
    public string app_name;
    public Icon icon;
    public int group_id;
    public bool trash;
    public long last_update;
    public List<ItemsList> items_list;
    public List<ViewList> views_list;
    public List<FieldList> field_list;
    public List<FileList> file_list;
}

[Serializable]
public class tempStorage
{
    public List<App> apps_list;
}

#endregion

/// <summary>
/// Class for  serialization and deserialization
/// </summary>
public static class JSONModule
{
    /// <summary>
    /// Deserialize string to UserInfo
    /// </summary>
    /// <param name="jsonString">json string</param>
    /// <returns>UserInformation object</returns>
    public static UserInformation StringToUserInformation(string jsonString)
    {
        return JsonUtility.FromJson<UserInformation>(jsonString);
    }

    /// <summary>
    /// Serialize UserInformationClass to string
    /// </summary>
    /// <param name="jsonObj">UserInformation object</param>
    /// <returns>JSON string</returns>
    public static string UserInformationToString(UserInformation jsonObj)
    {
        return JsonUtility.ToJson(jsonObj);
    }

    /// <summary>
    /// Deserialize string to App data
    /// </summary>
    /// <param name="jsonString">json string</param>
    /// <returns>UserInformation object</returns>
    public static App StringToAppData(string jsonString)
    {
        return JsonUtility.FromJson<App>(jsonString);
    }

    /// <summary>
    /// Serialize App data to string
    /// </summary>
    /// <param name="jsonObj">UserInformation object</param>
    /// <returns>JSON string</returns>
    public static string AppDataToString(App jsonObj)
    {
        return JsonUtility.ToJson(jsonObj);
    }

    /// <summary>
    /// Deserialize string to tempStorage
    /// </summary>
    /// <param name="jsonString">json string</param>
    /// <returns>UserInformation object</returns>
    public static tempStorage StringToAppList(string jsonString)
    {
        return JsonUtility.FromJson<tempStorage>(jsonString);
    }

    /// <summary>
    /// Serialize tempStorage to string
    /// </summary>
    /// <param name="jsonObj">tempStorage object</param>
    /// <returns>JSON string</returns>
    public static string AppListToString(tempStorage jsonObj)
    {
        return JsonUtility.ToJson(jsonObj);
    }   

}
