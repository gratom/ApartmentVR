using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneClickTracker : MonoBehaviour
{

    private void Start()
    {
        StartCoroutine(TrackingCoroutine());
    }

    private IEnumerator TrackingCoroutine()
    {
        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray;
                RaycastHit hit;
                ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

                if (Physics.Raycast(ray, out hit, 100))
                {
                    if (hit.collider.gameObject.GetComponent<SimpleAction>() != null)
                    {
                        hit.collider.gameObject.GetComponent<SimpleAction>().simpleActionDelegate();
                        break;
                    }
                }
            }
            yield return null;
        }
    }

}
