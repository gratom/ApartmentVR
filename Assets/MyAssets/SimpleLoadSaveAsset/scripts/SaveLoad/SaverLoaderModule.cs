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

    /// <summary>
    /// Saves data to a file with the specified name. Remember that this function saves the file in a standard folder.
    /// </summary>
    /// <param name="FileName">Name of file</param>
    /// <param name="StringData">Your string data</param>
    public static void SaveMyDataToFile(string FileName, string StringData)
    {
        try
        {
            string AppPath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/")) + "/Saves";
            FileStream fileStream = new FileStream(AppPath + FileName, FileMode.Create);
            StreamWriter MyFile = new StreamWriter(fileStream);
            MyFile.Write(StringData);
            MyFile.Close();
        }
        catch (ExecutionEngineException e)
        {
            Debug.Log(e);
        }
    }

    /// <summary>
    /// Load string data from file. Remember that this function loads the file from the standard folder.
    /// </summary>
    /// <param name="FileName">Name of file</param>
    /// <returns>String data or "", if file don`t exist</returns>
    public static string LoadMyDataFromFile(string FileName)
    {
        string AppPath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/")) + "/Saves";
        try
        {
            if (File.Exists(AppPath + FileName))
            {
                StreamReader MyFile = new StreamReader(AppPath + FileName);
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

    /// <summary>
    /// Saves data to a file with the specified name. Remember that in this function you must specify the full path to the file.
    /// </summary>
    /// <param name="FullPath">Full path to file</param>
    /// <param name="StringData">Your string data</param>
    public static void SaveMyDataTo(string FullPath, string StringData)
    {
        try
        {
            FileStream fileStream = new FileStream(FullPath, FileMode.Create);
            StreamWriter MyFile = new StreamWriter(fileStream);
            MyFile.Write(StringData);
            MyFile.Close();
        }
        catch (ExecutionEngineException e)
        {
            Debug.Log(e);
        }
    }

    /// <summary>
    /// Load string data from file. Remember that in this function you must specify the full path to the file.
    /// </summary>
    /// <param name="FullPath">Full path to file</param>
    /// <returns>String data or "", if file don`t exist</returns>
    public static string LoadMyDataFrom(string FullPath)
    {
        try
        {
            if (File.Exists(FullPath))
            {
                StreamReader MyFile = new StreamReader(FullPath);
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
