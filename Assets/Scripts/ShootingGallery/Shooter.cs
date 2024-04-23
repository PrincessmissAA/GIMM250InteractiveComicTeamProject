using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

/** Control script for the shooter in the Quantum Shooter mini-game.
 *  USE:
 *      x Create a game object for the shooter's crosshairs
 *          x Include components: RigidBody2D, Animator, SpriteRenderer
 *      - Add this script to the shooter game object
 *      - Add the game area object to the appropriate serialized field
 *  TODO:
 *      - Write Aim method to move the crosshairs
 *          - Cursor update in ShootingGallery.cs or sprite update here
 *              - Remove unused code when finished
 *      - Finish Shoot() method 
 *          - Check for hit
 *          - Only functions in game area
 *      - Add banner display to indicate when reload is required or in-progress
 *      TEST:
 *          - UI text Updates
 *              - Initiates with [6] shots (Expected: yes)
 *              - Initiates with [2] reloads (Expected: yes)
 *              - Shooting
 *                  - Decrements shots when mouse is clicked (Expected: yes)
 *                      - Only decrements when clicked in shooting area (Expected: yes)
 *                      - Only decrements after appropriate delay (Expected: yes)
 *                      - Decrements below zero (Expected: no)
 *                  - Shooting animation triggers when valid shot (Expected: not implemented)
 *              - Reloading
 *                  - Decrements reloads when button is clicked (Expected: yes)
 *                  - Restores shots to [6] after delay (Expected: yes)
 *                  - Decrements below zero (Expected: no)
 *                  - Reloading animation triggers when valid (Expected: not implemented)
 *          - Crosshair rendering
 *              - Render Crosshairs at mouse location (Expected: yes)
 *              - Crosshairs follow mouse movement (Expected: yes)
 *                  - Crosshairs follow mouse movement into side panel (Expected: no)
 *          - Playtesting
 *              - Is shot delay reasonable?
 *              - Is reload delay reasonable?
 *              - Is number of shots reasonable?
 *              - Is number of reloads reasonable
 *              (Note: (1 sec shot delay x 6 shots) x 3 + (3 sec reload time x 2 reloads) = 6 sec x3 + 6 sec = 24 sec. Game duration is 30 sec.)
 *  BUGS:
 *      - 
 *  CHANGES:
 *      - Added shooter-related display code from ShootingGallery.cs
 *              - Shots display only updates on shoot
 *              - Reload display only updates on reload
 *      - Fixed bug where shooter could rapid-fire
 *      - Made shot delay and reload time constants
 *      
 * @author Joe Shields
 * Last Updated: 23 Apr 24 @ 10:30a
 */

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]

public class Shooter : MonoBehaviour
{
    [SerializeField] GameObject gameArea;
    [SerializeField] private Text shotsDisplay;
    [SerializeField] private Text reloadsDisplay;
    private Rigidbody2D aimBody; // Used to move the crosshairs
    private SpriteRenderer aimSprite; // Show's the crosshairs
    private Animator aimAnimator; // call to shooting/reloading animation (2D graphics should not require separate projectile animation)
    
    // Gun Attributes
    private const int MAX_SHOTS = 6;
    private int shots;
    private const int MAX_RELOADS = 2;
    private int reloads;
    private const float SHOT_DELAY = 1f;
    private bool canShoot;
    private const float RELOAD_TIME = 3f;
    private bool isReloading;


    // Start is called before the first frame update
    void Start()
    {
        aimBody = GetComponent<Rigidbody2D>();
        aimSprite = GetComponent<SpriteRenderer>();
        aimAnimator = GetComponent<Animator>();
        shots = MAX_SHOTS;
        shotsDisplay.text = "Shots: " + GetShotsRemaining();
        reloads = MAX_RELOADS;
        reloadsDisplay.text = "Reloads: " + GetReloadsRemaining();
        canShoot = true;
        isReloading = false;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Control Methods

    public void Aim()
    {
        //MouseMoveEvent

    }

    /** Checks if shooter can shoot.
     * If true, decrements shots, sets canShoot to false, checks for hit, then
     * starts timer to call ReadyNextShot() if there are still shots remaining.
     * 
     */
    public void Shoot()
    {
        if(shots > 0 && !isReloading && canShoot)
        {
            shots--;
            shotsDisplay.text = "Shots: " + GetShotsRemaining();
            canShoot = false;

            // TODO: check for hit
            
            // TODO: call animation
            if(shots > 0)
            {
                Invoke("ReadyNextShot", SHOT_DELAY);
            }
            // TODO: else {//prompt for reload}?
        }
    }

    /** Starts the process of reloading the weapon if reloads remain.
     * Sets isReloading to true to prevent shooting while reloading is in progress.
     * Calls the ReloadComplete() method after specified time has elapsed.
     */
    public void Reload()
    {
        if(reloads > 0 && !isReloading)
        {
            reloads--;
            reloadsDisplay.text = "Reloads: " + GetReloadsRemaining();
            isReloading = true;
            // TODO: call animation
            Invoke("ReloadComplete", RELOAD_TIME);
        }
    }

    #endregion

    #region Helper Methods

    /** Change state of canShoot to true.
     * Call via Invoke() after shooting and reloading.
     * Prevents rapid fire.
     */
    private void ReadyNextShot()
    {
        canShoot = true;
    }

    /** Completes process of reloading.
     * Call via Invoke() after reloading.
     * Resets shots to maximum, resets canShoot to true, resets isReloading to false.
     */
    private void ReloadComplete()
    {
        shots = MAX_SHOTS;
        shotsDisplay.text = "Shots: " + GetShotsRemaining();
        isReloading = false;
        ReadyNextShot();
    }

    #endregion

    #region Mutator Methods

    /** Get number of shots currently loaded in the weapon.
     * @return integer
     */
    public int GetShotsRemaining()
    {
        return shots;
    }

    /** Get number of reloads remaining.
     * @return integer
     */
    public int GetReloadsRemaining()
    {
        return reloads;
    }

    /** Returns true if shooter is out of shots and reloads
     * @return boolean
     */
    public bool IsOutOfAmmo()
    {
        if(shots <= 0 && reloads <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion
}
