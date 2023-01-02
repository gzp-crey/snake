using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ManualSnakeControl : SnakeControl
{
    [SerializeField] float Speed = 100;
    [SerializeField] float TurnSpeed = 180;

    CharacterController characterController;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }
    
    void Rotate()
    {
        // rotate by input controller
        if (Input.GetAxis("Horizontal") != 0)
        {            
            var dx = Input.GetAxis("Horizontal");
            if(dx > 1) dx = 1;
            else if(dx < -1) dx = -1;
            var rotation = dx * TurnSpeed;
            transform.Rotate(0, rotation * Time.deltaTime, 0);
            return;
        }

        // rotate by virtual touch
        var touchMovement = GetComponent<PlayerTouchMovement>();        
        if (touchMovement != null) 
        {
            Vector3 target = new Vector3(-touchMovement.MovementAmount.y, 0.0f, touchMovement.MovementAmount.x);
            if(target.magnitude == 0) 
                return;
            var rotation = Quaternion.LookRotation(target);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, TurnSpeed * Time.deltaTime);
            return;
        }
    }

    public override void Move()
    {
        Rotate();
        characterController.SimpleMove(transform.right * Speed * Time.deltaTime);
    }
}
