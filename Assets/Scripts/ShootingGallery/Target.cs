using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

/** Control script for the Target in the Quantum Shooter mini-game.
 *  USE:
 *      - Create a game object for the Target
 *      - Add this script to the Target game object
 *      - Create a game area as the parent object for the target
 *  TODO:
 *      - Code Oberve() method fuctionality
 *      - Add check for window resize during the game? (EstablishArea(), EstablishTarget())
 *      - Resize target based on window size (EstablishTarget())
 *          - Move call for EstablishTarget() inside EstablishArea()
 *      READY TO TEST:
 *          - Movement
 *              - Object moves? (Expected: yes) YES
 *              - Movement changes speed and direction intermittently? (Expected: yes) YES
 *              - Movement reverses if it reaches the edge of the game area? (Expected: yes) YES
 *  BUGS:
 *      - 
 *  CHANGES:
 *      - Completely redesigned the code for a target built into the Unity Canvas.
 *      - Removed need for boundary colliders. Now uses game area dimensions to directly reassign speeds.
 *      - Added target-related display code from ShootingGallery.cs
 *              - Health display only updates on health decrement
 *              - Speed only updates on speed change
 *              - Vector only updates on vector change
 *      - Added method shell and baseline variables for Observe()
 *      - Changed plan from target sprite to independent raw image
 *      - Removed extraneous component requirements
 *      
 * @author Joe Shields
 * Last Updated: 16 Apr 24 @ 0945
 */

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]

public class Target : MonoBehaviour
{
    //Target Object
    private Rigidbody2D targetBody; // Used to move the target
    private BoxCollider2D targetCollider; // Detects collisions with the boundary
    

    //Target Stats
    [SerializeField] private Text health;
    private const int MAX_HIT_POINTS = 3; // Number of hits needed to destroy the target
    private int hitPoints; // Number of hits remaining before target is destroyed
    private float posX; // Center X position of the target
    private float posY; // Center Y position of the target
    private float height; // Y-dimension of the target
    private float width; // X-dimension of the target

    //Movement
    [SerializeField] private Text speedDisplay;
    [SerializeField] private GameObject vectorArrow;
    private const float MAX_SPEED = 300f; // Sets the upper bound for the randomSpeed on any single axis.
    private const float MIN_SPEED = 30f; // Sets the lower bound for the randomSpeed on any single axis.
    private float speedX; // Horizontal movement speed
    private float speedY; // Vertical movement speed
    private const float MAX_CHANGE_DELAY = 2f; // Sets the maximum time in seconds the target will move before changing directions
    private const float MIN_CHANGE_DELAY = 0.2f; // Sets the minimum time in seconds the target will move before changing directions
    private float changeDelay; // time delay between changes in movement

    //Game Area
    [SerializeField] GameObject gameArea; // The area to which the target is constrained (does not include the side panel)
    private float gameHeight; // height of the game window
    private float gameWidth; // width of the game window (does not include the side panel)

    //Observation Mechanic
    [SerializeField] private GameObject targetImage; // Used to access the sprite flip / visibility
    private const float observationTime = 2f; // Sets the duration for which the display will be fixed after the Observe button is pressed
    private bool isObserved;
    private float observedX;
    private float observedY;

    void Start()
    {
        targetBody = GetComponent<Rigidbody2D>();
        isObserved = false;
        hitPoints = MAX_HIT_POINTS; // Sets the hit point tracker
        health.text = "HEALTH: " + GetHealth();
        EstablishGameArea(); // Gets the size of the game area and sets the BoxCollider2D dimensions to match
        EstablishTarget(); // Instantiates posX, posY, height, and width
        ChangeMovement(); // Instantiates speedX, speedY, and changeDelay. Initiates self-renewing ChangeMovement call cycle.
    }

    void Update()
    {
        Move();
    }

    #region Instantiation methods

    /** Gets/sets the dimensions of the game area and its BoxCollider2D 
     * Call in Start()
     * TODO: Call on window resize
     */
    private void EstablishGameArea()
    {
        gameHeight = gameArea.GetComponent<RectTransform>().rect.yMax - gameArea.GetComponent<RectTransform>().rect.yMin; // Gets the height of the game window
        gameWidth = gameArea.GetComponent<RectTransform>().rect.xMax - gameArea.GetComponent<RectTransform>().rect.xMin; // Gets the width of the game window
        gameArea.GetComponent<BoxCollider2D>().size.Set(gameWidth, gameHeight);
    }

    /** Gets the position and dimensions of the target's box collider
     * Call in Start()
     * TODO: Scale with window size
     * TODO: Call on window resize
     */
    private void EstablishTarget()
    {
        posX = GetComponent<RectTransform>().localPosition.x; // Gets the X position relative to the parent object (Game Area)
        posY = GetComponent<RectTransform>().localPosition.y; // Gets the Y position relative to the parent object (Game Area)
        height = GetComponent<BoxCollider2D>().bounds.size.y; // Gets the target height
        width = GetComponent<BoxCollider2D>().bounds.size.x; // Gets the target width
    }

    #endregion

    #region Control Methods

    /** Changes the X and Y position of the target based on the current speed in both directions
     * Adjusts speed as needed to keep target in bounds
     */
    private void Move()
    {
        // Lateral bounds check
        if ((posX - width / 2) < (-gameWidth / 2))
        {
            speedX = Math.Abs(speedX);
            vectorArrow.transform.rotation = Quaternion.Euler(0, 0, GetVector());
        }
        else if ((posX + width / 2) > (gameWidth / 2))
        {
            speedX = -Math.Abs(speedX);
            vectorArrow.transform.rotation = Quaternion.Euler(0, 0, GetVector());
        }
        // Vertical bounds check
        if ((posY - height / 2) < (-gameHeight / 2))
        {
            speedY = Math.Abs(speedY);
            vectorArrow.transform.rotation = Quaternion.Euler(0, 0, GetVector());
        }
        else if ((posY + height / 2) > (gameHeight / 2))
        {
            speedY = -Math.Abs(speedY);
            vectorArrow.transform.rotation = Quaternion.Euler(0, 0, GetVector());
        }
        
        // Move the target
        targetBody.velocity = new Vector2(speedX, speedY);
        

        // Update the target position vaiables
        posX = GetComponent<RectTransform>().localPosition.x;
        posY = GetComponent<RectTransform>().localPosition.y;
    }

    /** Reassign all movement parameters and updates display.
     * Calls itself after a random delay.
     */
    private void ChangeMovement()
    {
        speedX = RandomSpeed();
        speedY = RandomSpeed();
        changeDelay = RandomDelay();
        speedDisplay.text = "Speed: " + GetSpeed(); // Update speed display
        vectorArrow.transform.rotation = Quaternion.Euler(0, 0, GetVector()); // Update velocity display (also updated in Move() when bounds are reached)
        Invoke("ChangeMovement", changeDelay);
    }

    /** Create a static sprite where the target was at the point the button was pressed.
     * Remove display for speed and velocity.
     * Revert to normal after observeTime seconds.
     */
    public void Observe()
    {
        Debug.Log("Observe called");
        isObserved = true;
        observedX = posX;
        observedY = posY;

        //TODO: Add call to spriteRenderer and set appropriate flipX
    }

    private void resetIsObserved()
    {

    }

    #endregion

    #region Helper Methods

    /** Generates a random value between MIN_ and MAX_SPEED then determines if speed is + or -.
     * @return integer
     */
    private float RandomSpeed()
    {
        float newSpeed = UnityEngine.Random.Range(MIN_SPEED, MAX_SPEED + 1); // Positive speed only, Correcting for non-inclusive max value
        if(UnityEngine.Random.Range(0,2) == 0) // Coin-toss for positive or negative speed.
        {
            newSpeed = -newSpeed;
        }
        return newSpeed;
    }

    /** Generates a random value between MIN_ and MAX_CHANGE_DELAY
     * @return float
     */
    private float RandomDelay()
    {
        return UnityEngine.Random.Range(MIN_CHANGE_DELAY, MAX_CHANGE_DELAY);
    }

    #endregion

    #region Mutators

    /** Returns the absolute speed of the target
     * @return double
     */
    public double GetSpeed()
    {
        double x2 = Math.Pow(speedX, 2);
        double y2 = Math.Pow(speedY, 2);
        double speed = Math.Round(Math.Sqrt(x2 + y2), 2);

        return speed;
    }

    /** Return the degree vector direction of travel of the target
     * @return int
     */
    public int GetVector()
    {
        double radians = Math.Atan2(speedY, speedX);
        double degrees = radians * (180 / Math.PI);

        return (int)degrees;
    }

    /** Return the current health of the target
     * @return int 
     */
    public int GetHealth()
    {
        return hitPoints;
    }

    /** Decrements target health and updates display*/
    public void DamageTarget()
    {
        hitPoints--;
        health.text = "HEALTH: " + GetHealth();
    }

    #endregion
}
