using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour {

    public GameObject follow;

	// Update is called once per frame
	void Update ()
    {
		//gameObject.transform.rotation = Quaternion.Euler(0,follow.transform.rotation.eulerAngles.y, 0);
	}

    public void UpdatePosition()
    {
        gameObject.transform.position = follow.transform.position;
        gameObject.transform.rotation = Quaternion.Euler(0, follow.transform.rotation.eulerAngles.y, 0);
    }

}
