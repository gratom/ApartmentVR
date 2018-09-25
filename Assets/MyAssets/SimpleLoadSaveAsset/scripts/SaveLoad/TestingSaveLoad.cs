﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestingSaveLoad : MonoBehaviour
{

    public InputField textTest;

    public void SaveText()
    {
        SaverLoaderModule.SaveMyDataTo("/Mdata.txt", textTest.text);
    }

    public void LoadText()
    {
        textTest.text = SaverLoaderModule.LoadMyDataFrom("/Mdata.txt");
    }



}
