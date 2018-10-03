using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{

    public KeyCode keyForward;
    public KeyCode keyBack;
    public KeyCode keyLeft;
    public KeyCode keyRight;

    public float speedMouse;
    public float speedWalking;

	// Use this for initialization
	void Start () 
    {
        Cursor.lockState = CursorLockMode.Locked;
	}
	
	// Update is called once per frame
	void Update () 
    {
        MouseTracker();
        ObjectTracker();
	}

    private void ObjectTracker()
    {
        if (Input.GetKey(keyForward))
        {
            this.gameObject.transform.position += this.gameObject.transform.forward * speedWalking * Time.deltaTime;
        }
        if (Input.GetKey(keyBack))
        {
            this.gameObject.transform.position += this.gameObject.transform.forward * -speedWalking * Time.deltaTime;
        }
        if (Input.GetKey(keyLeft))
        {
            this.gameObject.transform.position += this.gameObject.transform.right * -speedWalking * Time.deltaTime;
        }
        if (Input.GetKey(keyRight))
        {
            this.gameObject.transform.position += this.gameObject.transform.right * speedWalking * Time.deltaTime;
        }
    }

    private void MouseTracker()
    {
        float f = Input.GetAxis("Mouse X") * -speedMouse * Time.deltaTime;
        this.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, f + this.gameObject.transform.rotation.eulerAngles.y, 0));
        f = Input.GetAxis("Mouse Y") * speedMouse * Time.deltaTime;
        Camera.main.transform.localRotation = Quaternion.Euler(new Vector3(f + Camera.main.transform.localRotation.eulerAngles.x, 0, 0));
    }

}
