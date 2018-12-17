using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    [Header("Camera Follow Variables")]
    public bool follow = true; // Toggle this to lock camera in place
    public GameObject target; // Tracks the current target
    public float followSpeed = 5f;

    private void Update()
    {
        if (follow)
        {
            transform.position = new Vector3(Mathf.Lerp(transform.position.x, target.transform.position.x, followSpeed),
                                             Mathf.Lerp(transform.position.y, target.transform.position.y, followSpeed),
                                             -10f);
        }
    }

}
