using UnityEngine;

public class CameraFollowFixedRotation : MonoBehaviour, ICameraFollow
{
    public Transform target;
    public float smoothSpeed = 0.125f;    
    private Vector3 locationOffset;

    Transform ICameraFollow.target { get => target; set => target = value; }

    void Start()
    {
        locationOffset = transform.position;
    }

    void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 desiredPosition = target.position + locationOffset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}