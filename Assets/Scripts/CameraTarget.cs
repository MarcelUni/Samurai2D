using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    public Transform target;
    
    public Vector3 offset;
    public Vector3 minYPos;
    private Vector3 followVelocity = Vector3.zero;
    public float followTime;

    private void LateUpdate()
    {
        Vector3 nextTarget = target.position + offset;
        Vector3 nextposition = Vector3.SmoothDamp(transform.position, nextTarget, ref followVelocity, followTime);

        transform.position = nextposition;
    }
}
