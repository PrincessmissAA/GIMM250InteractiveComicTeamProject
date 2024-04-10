using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEditor.UIElements;
using UnityEngine;

/** Control script for the Target in the Quantum Shooter mini-game.
 *  USE:
 *      - Create a game object for the Target
 *      - Add this script to the Target game object
 *      - Create four (4) game objects for the boundaries
 *          - Include component: BoxCollider2D
 *          - Set tags: top/bottom = "vertical", left/right = "horizontal"
 *          - Within BoxCollider2D component, check box labeled "Is Trigger"
 *          - Place and size colliders to form a continuous box around the target area
 *  TODO:
 *      - Create damage method(s)
 *      - Add sprite renderer call to Move() method
 *      TEST:
 *          - Movement
 *              - Object moves? (Expected: yes) YES
 *              - Movement changes speed and direction intermittently? (Expected: yes) YES
 *              - Movement reverses if it encounters a boundary? (Expected: yes) YES
 *          - Sprite
 *              - Does toggling the sprite visiblity completely screw up the color pallette? (Expected: no)
 *              - Does toggling the sprite visibility toggle the sprite visibility? (Expected: yes)
 *  BUGS:
 *      - Has not been tested
 *  CHANGES:
 *      - Added component variables for Target game object
 *      - Removed internal references to boundary colliders. 
 *        (Uses OnTriggerEnter2D to reference tags.)
 *      - Organized Methods by region: Control, Helper, Mutator.
 *      - Removed "position" variables. Call Vector2 constructor directly.
 *      - Added visibility mutators: ToggleVisible() and IsVisible
 *        Uses alpha component of the sprite's color.
 *      - Converted speed to float
 *      - Added RequireComponent for necessary attributes
 *      - Added BoxCollider2D
 *      
 * @author Joe Shields
 * Last Updated: 6 Mar 24 @ 4:10p
 */

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]

public class Target : MonoBehaviour
{
    //Target Object
    private Rigidbody2D targetBody; // Used to move the target
    private BoxCollider2D targetCollider; // Detects collisions with the boundary
    private SpriteRenderer targetSprite; // Used to access the sprite flip / visibility
    private Color visible;
    private Color invisible;
    private Animator targetAnimator; // Used to animate target
    private const int MAX_HIT_POINTS = 3;
    private int hitPoints;

    //Movement
    private const float MAX_SPEED = 3f; // Sets the maximum bounds for the randomSpeed on any single axis.
    private float speedX;
    private float speedY;
    private const float MAX_CHANGE_DELAY = 2f; // Sets the maximum time in seconds the target will move before changing directions
    private const float MIN_CHANGE_DELAY = 0.2f; // Sets the minimum time in seconds the target will move before changing directions
    private float changeDelay;

    void Start()
    {
        targetBody = GetComponent<Rigidbody2D>();
        targetSprite = GetComponent<SpriteRenderer>();
        visible = new Color(targetSprite.color.r, targetSprite.color.g, targetSprite.color.b, 1);
        invisible = new Color(targetSprite.color.r, targetSprite.color.g, targetSprite.color.b, 0);
        targetAnimator = GetComponent<Animator>();
        hitPoints = MAX_HIT_POINTS;
        ChangeMovement(); // Instantiates speedX, speedY, and changeDelay. Initiates self-renewing ChangeMovement call cycle.
    }

    void Update()
    {
        Move();
    }

    #region Control Methods

    /** Ensure target is in-bounds.
     * Correct target movement if necessary.
     */
    private void OnTriggerEnter2D(Collider2D collision)
    {
        String boundary = collision.gameObject.tag;
        Debug.Log(boundary + " collision");

        switch (boundary)
        {
            //TODO: Build boundary trigger colliders and assign tags: top/bottom = "vertical", left/right = "horizontal"
            case "vertical":
                speedY = -speedY;
                Debug.Log("Reverse Y");
                break;
            case "horizontal":
                speedX = -speedX;
                Debug.Log("Reverse X");
                break;
            default: // Fail-safe reverses both directions if collider tag is not caught.
                speedX = -speedX;
                speedY = -speedY;
                Debug.Log("Reverse Both: BAD BAD BAD!");
                break;
        }

    }

    /** Changes the X and Y position of the target based on the current speed in both directions
    * 
    */
    private void Move()
    {
        //TODO: Add calls to sprite renderer
        targetBody.velocity = new Vector2(speedX, speedY);
    }

    /** Reassign all movement parameters. Speed is random.
     * 
     */
    private void ChangeMovement()
    {
        Debug.Log("Change Movement");
        speedX = RandomSpeed();
        speedY = RandomSpeed();
        changeDelay = RandomDelay();
        Invoke("ChangeMovement", changeDelay);
    }

    #endregion

    #region Helper Methods

    /** Generates a random value between +/-MAX_SPEED
     * @return integer
     */
    private float RandomSpeed()
    {
        return UnityEngine.Random.Range(-MAX_SPEED, MAX_SPEED + 1); // Correcting for non-inclusive max value
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

        return Math.Floor(speed*10)/10;
    }

    /** Return the degree vector direction of travel of the target
     * @return int
     */
    public int GetVector()
    {
        double radians = 0;
        
        if (speedX != 0)
        {
            radians = Math.Atan2(Math.Abs(speedY), Math.Abs(speedX));
        }
        else
        {
            radians = Math.PI / 2;
        }

        double degrees = radians * (180 / Math.PI);

        if(speedX < 0)
        {
            if(speedY < 0)
            {
                degrees += 180;
            }
            else
            {
                degrees = 180 - degrees;
            }
        }
        else
        {
            if(speedY < 0)
            {
                degrees = 360 - degrees;
            }
        }
        return (int)degrees;
    }

    /** Toggle the visibility of the target sprite.
     * Modifies the alpha value of the sprite's color
     * @return boolean
     */
    public bool ToggleVisible()
    {
        if(targetSprite.color.Equals(visible))
        {
            targetSprite.color = invisible;
            return false;
        }
        else
        {
            targetSprite.color = visible;
            return true;
        }
    }

    /** Return true if target is visible or false if it is not.
     * @return boolean
     */
    public bool IsVisible()
    {
        if (targetSprite.color.Equals(visible))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /** Return remaining hit points
     * @return int
     */
    public int GetHealth()
    {
        return hitPoints;
    }

    #endregion
}
