using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SceneChangebleObjectTypes
{
    id,
    nameObject,
    typeObject,
    AssetBundleURL
}

/// <summary>
/// Class for changeble object on scene.
/// </summary>
public class SceneChangebleObject : AbstractObjectConstructable <SceneChangebleObjectTypes>
{

    public int ID { get; private set; }
    public string ChangebleObjectName { get; private set; }
    public string ChangebleObjectType { get; private set; }
    public string RealGudHubURL { get; private set; }
    public string URLName { get; private set; }

    private AssetBundle AssetBundleInstance;

    #region public override functions

    public override void InitDictionary()
    {
        FunctionsDictionary = new Dictionary<SceneChangebleObjectTypes, InitFunctions>();
        FunctionsDictionary.Add(SceneChangebleObjectTypes.id, InitID);
        FunctionsDictionary.Add(SceneChangebleObjectTypes.nameObject, InitName);
        FunctionsDictionary.Add(SceneChangebleObjectTypes.AssetBundleURL, LoadAssetBundleFromURL);
        FunctionsDictionary.Add(SceneChangebleObjectTypes.typeObject, InitTypeObject); 
    }

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
#if UNITY_EDITOR
            //Debug.Log("Heh, another one item is initiated!");
#endif
        }
    }

    #endregion

    #region private functions

    private void LoadAssetBundleFromURL(int num)
    {

    }

    private void InitName(int num)
    {

    }

    private void InitID(int num)
    {

    }

    private void InitTypeObject(int num)
    {

    }

    #endregion

}