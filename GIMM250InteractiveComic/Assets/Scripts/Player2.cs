using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2 : MonoBehaviour
{
    private Controller movement; // Reference to Controller script
    

    void Start()
    {
        movement = GetComponent<Controller>(); // Get Controller component
        
    }
    void Update()
    {
        // Get horizontal and vertical input from keyboard (or alternatives)
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        // Call Move method from Controller script
        movement.Move2(horizontalInput, verticalInput);
    }
}

