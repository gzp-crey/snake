using UnityEngine;

public class CameraFollow : MonoBehaviour, ICameraFollow
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    private Vector3 locationOffset;
    private Quaternion rotationOffset;

    Transform ICameraFollow.target { get => target; set => target = value; }

    void Start()
    {
        locationOffset = transform.position;
        rotationOffset = transform.rotation;
    }

    void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        Vector3 desiredPosition = target.position + target.rotation * locationOffset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        Quaternion desiredRotation = target.rotation * rotationOffset;
        Quaternion smoothedRotation = Quaternion.Lerp(transform.rotation, desiredRotation, smoothSpeed);
        transform.rotation = smoothedRotation;
    }
}