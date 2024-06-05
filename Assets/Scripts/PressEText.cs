using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressEText : MonoBehaviour
{
    public Transform chest;
    public Vector3 offset;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        gameObject.SetActive(false);     
    }

    private void Update()
    {
        transform.position = mainCamera.WorldToScreenPoint(chest.transform.position + offset);
    }
}
