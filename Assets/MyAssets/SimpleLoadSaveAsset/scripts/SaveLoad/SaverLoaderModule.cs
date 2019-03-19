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
    #region public functions

    /// <summary>
    /// Saves data to a file with the specified name. Remember that this function saves the file in a standard folder.
    /// </summary>
    /// <param name="FileName">Name of file</param>
    /// <param name="StringData">Your string data</param>
    public static void SaveMyDataToFile(string FileName, string StringData)
    {
        try
        {
            FileName = Normalizer(FileName);
            string AppPath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/")) + "/Standart_Saves";
            OverGeneratePath(AppPath + FileName);
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
        try
        {
            FileName = Normalizer(FileName);
            string AppPath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/")) + "/Standart_Saves";            
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
            OverGeneratePath(FullPath);
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

    #endregion

    #region private functions

    private static string Normalizer(string fileName)
    {
        if (fileName.IndexOf('/') == 0)
        {
            return fileName;
        }
        else
        {
            return "/" + fileName;
        }
    }

    private static string OverGeneratePath(string fileName)
    {
        string[] tempArray = fileName.Split('/');
        List<string> finalListOfPathParts = new List<string>();

        #region delete extra "/"

        for (int i = 0; i < tempArray.Length; i++)
        {
            if (tempArray[i] != "")
            {
                finalListOfPathParts.Add(tempArray[i]);
            }
        }

        #endregion

        #region create returned string and overGenerating path

        string returnedPath = "";
        for(int i = 0; i < finalListOfPathParts.Count - 1; i++)
        {
            returnedPath += finalListOfPathParts[i] + "/";
            if (!Directory.Exists(returnedPath))
            {
                try
                {
                    Directory.CreateDirectory(returnedPath);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }
        returnedPath += finalListOfPathParts[finalListOfPathParts.Count - 1];

        #endregion

        return returnedPath;
    }

    #endregion

}
