using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Controller movement; // Reference to Controller script

    public JoystickMovement joystickMovement;

    public float moveSpeed = 5f; // Movement speed
    [SerializeField] private float _rotationSpeed;
    public Rigidbody2D rb; // Reference to Rigidbody2D component (assign in inspector)

    void Start()
    {
        movement = GetComponent<Controller>(); // Get Controller component
    }

    void FixedUpdate()
    {
        
        if (joystickMovement.joystickVec.y != 0)
        {
            rb.velocity = new Vector2(joystickMovement.joystickVec.x * moveSpeed, joystickMovement.joystickVec.y * moveSpeed);
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
        
        if (rb.velocity != Vector2.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(transform.forward, rb.velocity);
            Quaternion rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);

            rb.MoveRotation(rotation);
        }
    }
    
}

