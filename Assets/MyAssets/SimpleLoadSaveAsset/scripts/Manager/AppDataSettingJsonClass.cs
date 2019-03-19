using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 
/// </summary>
namespace SettingFromJson
{

    [Serializable]
    public class AppDataSettingJsonClass
    {
        public List<App> Apps;
        public string AuthKey;
    }

    [Serializable]
    public class App
    {
        public string AppType;
        public int AppID;
        public List<Field> Fields;

        public App(string appType, int appID)
        {
            AppType = appType;
            AppID = appID;
        }
    }

    [Serializable]
    public class Field
    {
        public string Name;
        public int ID;

        public Field(int id, string name)
        {
            ID = id;
            Name = name;
        }
    }

}
