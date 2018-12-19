using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAction : MonoBehaviour
{

    public delegate void SimpleActionDelegate();

    public SimpleActionDelegate simpleActionDelegate;

}
