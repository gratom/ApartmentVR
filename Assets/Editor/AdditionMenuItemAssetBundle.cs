using UnityEditor;
using UnityEngine;
using System.Collections;

public class AdditionMenuItemAssetBundle
{
    [MenuItem("Simple Bundles/Build")]
    static void BuildBundles()
    {
        string path = EditorUtility.SaveFolderPanel("Save Bundle", "", "");  //Отображает диалоговое окно "Сохранить Папку" и возвращается
        if (path.Length != 0)
        {
            BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
        }
    }
}

class SelectAllOfTag : ScriptableWizard
{

    [MenuItem("Simple Bundles/Remove all")]
    static void SelectAllOfTagWizard()
    {

        string [] arrayNames = AssetDatabase.GetAllAssetBundleNames();
        for(int i = 0; i < arrayNames.Length; i++)
        {
            AssetDatabase.RemoveAssetBundleName(arrayNames[i], true);
        }

    }

}