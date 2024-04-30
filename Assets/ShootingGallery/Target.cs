using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using Unity.Collections;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

/** Control script for the Target in the Quantum Shooter mini-game.
 *  USE:
 *      - Create a game area as the parent object for the target
 *      - Create a game object for the Target
 *      - Add this script to the Target game object
 *  TODO:
 *      - NANCY: Code Oberve() method fuctionality
 *      - Add check for window resize during the game? (EstablishArea(), EstablishTarget())
 *      - Resize target based on window size (EstablishTarget())
 *          - Move call for EstablishTarget() inside EstablishArea()
 *      - Add remaining tests
 *      - Remove target image (should be free-moving non-visible collider only)
 *      TEST:
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
 * @author Joe Shields, Nancy Gonzalez
 * Last Updated: 23 Apr 24 @ 10:40a
 */

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]

public class Target : MonoBehaviour
{
    //Target Object
    private Rigidbody targetBody; // Used to move the target
    private BoxCollider targetCollider; // Detects collisions with the boundary
    

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
    private const float OBSERVATION_TIME = 2f; // Sets the duration for which the display will be fixed after the Observe button is pressed
    private bool isObserved;
    private float observedX;
    private float observedY;
    //NANCY TODO: Set constant baseline position for target image when not observed? Ensure using correct parent reference.
    private float UNOBSERVED_X;
    private float UNOBSERVED_Y;
    private float arrowX;
    private float arrowY;

    void Start()
    {
        targetBody = GetComponent<Rigidbody>();
        isObserved = false;
        hitPoints = MAX_HIT_POINTS; // Sets the hit point tracker
        health.text = "HEALTH: " + GetHealth();
        EstablishGameArea(); // Gets the size of the game area and sets the BoxCollider dimensions to match
        EstablishTarget(); // Instantiates posX, posY, height, and width
        UNOBSERVED_X = targetImage.GetComponent<RectTransform>().localPosition.x; // Gets the X position relative to the parent object (Game Area)
        UNOBSERVED_Y = targetImage.GetComponent<RectTransform>().localPosition.y; // Gets the Y position relative to the parent object (Game Area)
        ChangeMovement(); // Instantiates speedX, speedY, and changeDelay. Initiates self-renewing ChangeMovement call cycle.
        arrowX = vectorArrow.GetComponent<RectTransform>().localPosition.x;
        arrowY = vectorArrow.GetComponent<RectTransform>().localPosition.y;
    }

    void Update()
    {
        Move();
    }

    #region Instantiation methods

    /** Gets/sets the dimensions of the game area and its BoxCollider
     * Call in Start()
     * TODO: Call on window resize
     */
    private void EstablishGameArea()
    {
        gameHeight = gameArea.GetComponent<RectTransform>().rect.yMax - gameArea.GetComponent<RectTransform>().rect.yMin; // Gets the height of the game window
        gameWidth = gameArea.GetComponent<RectTransform>().rect.xMax - gameArea.GetComponent<RectTransform>().rect.xMin; // Gets the width of the game window
        gameArea.GetComponent<BoxCollider>().size.Set(gameWidth, gameHeight, 0f);
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
        height = GetComponent<BoxCollider>().bounds.size.y; // Gets the target height
        width = GetComponent<BoxCollider>().bounds.size.x; // Gets the target width
    }

    #endregion

    #region Control Methods

    /** Changes the X and Y position of the target based on the current speed in both directions
     * Adjusts +/-speed as needed to keep target in bounds
     */
    private void Move()
    {
        bool vectorChange = false;

        // Lateral bounds check
        if ((posX - width / 2) < (-gameWidth / 2))
        {
            speedX = Math.Abs(speedX);
            vectorChange = true;
        }
        else if ((posX + width / 2) > (gameWidth / 2))
        {
            speedX = -Math.Abs(speedX);
            vectorChange = true;
        }
        // Vertical bounds check
        if ((posY - height / 2) < (-gameHeight / 2))
        {
            speedY = Math.Abs(speedY);
            vectorChange = true;
        }
        else if ((posY + height / 2) > (gameHeight / 2))
        {
            speedY = -Math.Abs(speedY);
            vectorChange = true;
        }

        // Change vector arrow display if needed
        // NANCY TODO: Do not modify the display if the target is observed
        if (vectorChange && !isObserved)
        {
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

        // NANCY TODO: Do not modify the display if the target is observed
        if (!isObserved)
        {
         speedDisplay.text = "Speed: " + GetSpeed(); // Update speed display
        vectorArrow.transform.rotation = Quaternion.Euler(0, 0, GetVector()); // Update velocity display (also updated in Move() when bounds are reached)
        }
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
        targetImage.transform.localPosition = new Vector3(observedX, observedY, 0);
        speedDisplay.text = "Speed: ???";
        vectorArrow.transform.localPosition = new Vector3(UNOBSERVED_X, UNOBSERVED_Y, 0);
        Invoke("resetIsObserved", OBSERVATION_TIME);
        // NANCY TODO: Move the target image to the observed position
        // NANCY TODO: Hide the speed and vector arrow readouts
        // NANCY TODO: Call resetIsObserved after appropriate delay
    }

    private void resetIsObserved()
    {
        isObserved = false;
        targetImage.transform.localPosition = new Vector3(UNOBSERVED_X, UNOBSERVED_Y,0);
        speedDisplay.text = "Speed: " + GetSpeed(); // Update speed display
        vectorArrow.transform.localPosition = new Vector3(arrowX, arrowY, 0);
        // NANCY TODO: Move the target image to the unobserved position
        // NANCY TODO: Show the speed and vector arrow readouts
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
