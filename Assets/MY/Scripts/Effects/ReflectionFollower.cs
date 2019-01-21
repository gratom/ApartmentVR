using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectionFollower : MonoBehaviour {

    public GameObject Follow;

    void Update()
    {
        if (Follow == null)
        {
            if (Camera.main != null)
            {
                Follow = Camera.main.gameObject;
            }
        }
        if (Follow != null)
        {
            transform.position = new Vector3(Follow.transform.position.x, -Follow.transform.position.y, Follow.transform.position.z);
        }
    }
}
