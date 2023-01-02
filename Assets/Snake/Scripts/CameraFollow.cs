using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CameraFollow : MonoBehaviour
{    
    [SerializeField] Camera TargetCamera;

    Vector3 defaultDistance;

    void Start()
    {
        defaultDistance = TargetCamera.transform.position - transform.position;
    }

    void Update()
    {
        TargetCamera.transform.position = transform.position + defaultDistance;
    }
}