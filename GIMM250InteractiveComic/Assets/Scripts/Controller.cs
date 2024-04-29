using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour

{
    //public List<string> items;
    public float moveSpeed = 5f; // Movement speed
    public Rigidbody2D rb; // Reference to Rigidbody2D component (assign in inspector)
    private Inventory inventory;

    [SerializeField] private UIInventory uiInventory;
    /*
    void Start()
    {
        items = new List<string>();
    }
    */
    private void Awake()
    {
        inventory = new Inventory();
    }

    public void Move(float horizontalInput, float verticalInput)
    {
       
        // Combine input into a movement direction vector
        Vector2 movement = new Vector2(horizontalInput, verticalInput);

        // Apply movement with speed to Rigidbody2D
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
    public void Move2(float horizontalInput, float verticalInput)
    {
        // Combine input into a movement direction vector
        Vector2 movement = new Vector2(horizontalInput, verticalInput);

        // Apply movement with speed to Rigidbody2D
        rb.MovePosition(rb.position - movement * moveSpeed * Time.fixedDeltaTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if colliding with a collectable object (using tag)
        /*if (collision.gameObject.CompareTag("Collectable"))
        {
            // Handle item collection logic (e.g., destroy, add to inventory)
            
            string itemType = collision.gameObject.GetComponent<CollectableScript>().itemType;
            print(itemType + " Collected!");
            items.Add(itemType);
            print("Inventory Count:" + items.Count);
            Destroy(collision.gameObject); // Example: Destroy collected item
            
        }*/
    }
   


}

