using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour

{
    [SerializeField] private Text p1Inventory;
    private string inventoryDisplay;
    private List<string> inventory;
    private const int MAX_ITEMS = 4;
    //old item management system:
    

    public JoystickMovement joystickMovement;

    public Rigidbody2D rb; // Reference to Rigidbody2D component (assign in inspector)
    

    //[SerializeField] private UIInventory uiInventory;// Assign in inspector on both players
    
    void Start()
    {
        inventoryDisplay = "Inventory: \n*\n*\n*\n*";
        inventory = new List<string>();
        
    }

    
    /*private void Awake()
    {
        inventory = new Inventory();
    }*/

    /*public void Move(float horizontalInput, float verticalInput)
    {
       
        // Combine input into a movement direction vector
        Vector2 movement = new Vector2(horizontalInput, verticalInput);

        // Apply movement with speed to Rigidbody2D
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);

    }

    //Separated movement scripts for the two different player classes

    public void Move2(float horizontalInput, float verticalInput)
    {
        // Combine input into a movement direction vector
        Vector2 movement = new Vector2(horizontalInput, verticalInput);

        // Apply movement with speed to Rigidbody2D
        rb.MovePosition(rb.position + movement * moveSpeed2 * Time.fixedDeltaTime);

        
    }*/

    //Old collection function. Moving on to new inventory code:

    void OnTriggerEnter2D(Collider2D collision)
    {
        
        inventory.Add(collision.gameObject.tag);
        UpdateInventoryDisplay();
        Destroy(collision.gameObject);
    
    }

    public void UpdateInventoryDisplay()
    {
        inventoryDisplay = "Inventory:";

        foreach (string item in inventory)
        {
            inventoryDisplay += "\n" + item;
        }
        for (int i = inventory.Count; i < MAX_ITEMS; i ++)
        {
            inventoryDisplay += "\n*";
        }

        p1Inventory.text = inventoryDisplay;
    }



}

