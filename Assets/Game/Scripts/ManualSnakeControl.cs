using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ManualSnakeControl : NetworkBehaviour, ISnakeControl
{
    [SerializeField] float Speed = 100;
    [SerializeField] float TurnSpeed = 180;

    private CharacterController characterController;
    private FloatingJoystick Joystick;
    private NetworkVariable<Vector2> targetDirection = new NetworkVariable<Vector2>(Vector2.right, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

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
        // rotate by input controller
        if (Input.GetAxis("Horizontal") != 0)
        {
            var dx = Input.GetAxis("Horizontal");
            if (dx > 1) dx = 1;
            else if (dx < -1) dx = -1;
            dx = dx * TurnSpeed * Time.deltaTime;
            if (dx != 0)
                targetDirection.Value = Quaternion.Euler(0, 0, -dx) * targetDirection.Value;
            return;
        }

        // rotate by virtual touch
        if (Joystick != null)
        {
            var target = Joystick.MovementAmount;
            if (target.magnitude > 0)
                targetDirection.Value = target;
            return;
        }
    }

    public void Move()
    {
        if (IsLocalPlayer)
        {
            // local player only sets the target direction, but let the server decide the movement.
            Rotate();
        }

        var direction = targetDirection.Value;
        var lookAtDirection = Quaternion.LookRotation(new Vector3(-direction.y, 0.0f, direction.x));
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookAtDirection, TurnSpeed * Time.deltaTime);
        characterController.SimpleMove(transform.right * Speed * Time.deltaTime);
    }

    void OnGUI()
    {
        DebugHUD.AddLine($"Control {NetworkObjectId}, l: {IsLocalPlayer} r: {targetDirection.Value}");
    }
}
