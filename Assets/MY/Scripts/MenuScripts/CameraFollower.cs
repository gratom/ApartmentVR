using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{

    public GameObject follow;

    private bool IsNeedToCentrate;

	// Update is called once per frame
	void Update ()
    {
		//gameObject.transform.rotation = Quaternion.Euler(0,follow.transform.rotation.eulerAngles.y, 0);
	}
    
    //немного костыльное решение для проблемы центрирования менюшки
    public bool GetCentrateState()
    {
        if (IsNeedToCentrate)
        {
            IsNeedToCentrate = false;
            return true;
        }
        return false;
    }

    public void UpdatePosition()
    {
        if (follow == null)
        {
            if (Camera.main != null)
            {
                follow = Camera.main.gameObject;
            }
        }
        if (follow != null)
        {
            gameObject.transform.position = follow.transform.position - new Vector3(0,0.35f,0);
            gameObject.transform.rotation = Quaternion.Euler(0, follow.transform.rotation.eulerAngles.y, 0);
            IsNeedToCentrate = true;
        }
        
    }

}