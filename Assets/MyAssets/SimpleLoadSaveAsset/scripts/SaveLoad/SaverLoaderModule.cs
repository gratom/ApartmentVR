using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

/// <summary>
/// Universal module for saving and loading any text information
/// </summary>
public static class SaverLoaderModule 
{
    public static void SaveMyDataTo(string DPath, string DString)
    {
        try
        {
            FileStream file1 = new FileStream(Application.persistentDataPath + "/" + DPath, FileMode.Create);
            StreamWriter MyFile = new StreamWriter(file1);
            MyFile.Write(DString);
            MyFile.Close();
        }
        catch (ExecutionEngineException e)
        {
            Debug.Log(e);
        }
    }

    public static string LoadMyDataFrom(string DPath)
    {
        try
        {
            if (File.Exists(Application.persistentDataPath + "/" + DPath))
            {
                StreamReader MyFile = new StreamReader(Application.persistentDataPath + "/" + DPath);
                string DString = MyFile.ReadToEnd();
                MyFile.Close();                
                return DString;
            }
        }
        catch (ExecutionEngineException e)
        {
            Debug.Log(e);
        }
        return "";
    }   
}
