using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2 : MonoBehaviour
{

    public JoystickMovement joystickMovement;


    private Controller movement; // Reference to Controller script
    public float moveSpeed = 5f; // Movement speed
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

        /*// Get horizontal and vertical input from keyboard (or alternatives)
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        // Call Move method from Controller script
        movement.Move2(horizontalInput, verticalInput);*/
    }
}

