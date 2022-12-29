using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Vector3 FollowDistance = new Vector3(6.0f, 14.0f, -16.0f);
    [SerializeField] Camera TargetCamera;

    void Update()
    {
        TargetCamera.transform.position = transform.position + FollowDistance;// Quaternion.Euler(new Vector3(30.0f, 0.0f,0.0f));
    }
}
