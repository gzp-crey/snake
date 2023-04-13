using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ManualSnakeControl : NetworkBehaviour, ISnakeControl
{
    [SerializeField] float Speed = 100;
    [SerializeField] float TurnSpeed = 180;

    private CharacterController characterController;
    private FloatingJoystick Joystick;
    private bool isRot = false;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        if (IsLocalPlayer)
        {
            Joystick = NamedGameObjects.DirectionJoystick;
            var follow = NamedGameObjects.CameraFollow;
            if (follow != null)
                follow.target = transform;
        }
    }

    void Rotate()
    {
        isRot = false;
        // rotate by input controller
        if (Input.GetAxis("Horizontal") != 0)
        {
            var dx = Input.GetAxis("Horizontal");
            if (dx > 1) dx = 1;
            else if (dx < -1) dx = -1;
            isRot = true;
            var rotation = dx * TurnSpeed;
            transform.Rotate(0, rotation * Time.deltaTime, 0);
            return;
        }

        // rotate by virtual touch
        if (Joystick != null)
        {
            Vector3 target = new Vector3(-Joystick.MovementAmount.y, 0.0f, Joystick.MovementAmount.x);
            if (target.magnitude == 0)
                return;
            isRot = true;
            var rotation = Quaternion.LookRotation(target);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, TurnSpeed * Time.deltaTime);
            return;
        }
    }

    public void Move()
    {
        if (!IsLocalPlayer) return;

        Rotate();
        characterController.SimpleMove(transform.right * Speed * Time.deltaTime);
    }

    void OnGUI()
    {
        DebugHUD.AddLine($"IsLocal {NetworkObjectId}: {IsLocalPlayer}");
        DebugHUD.AddLine($"Rotating {NetworkObjectId}: {isRot}");

    }
}
