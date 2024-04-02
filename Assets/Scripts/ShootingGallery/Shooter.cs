using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/** Control script for the Target in the Quantum Shooter mini-game.
 *  USE:
 *      - Create a game object for the shooter's crosshairs
 *          - Include components: RigidBody2D, Animator, SpriteRenderer
 *      - Add this script to the shooter game object
 *  TODO:
 *      - HUD to display health and ammo (Or move to ShootingGallery class)
 *      - Write Aim method to move the crosshairs
 *      - Finish Shoot() method that looks for target and decrements ammo
 *      TEST:
 *          
 *  BUGS:
 *      - Has not been tested
 *  CHANGES:
 *      
 *      
 * @author Joe Shields
 * Last Updated: 5 Mar 24 @ 1:00p
 */

public class Shooter : MonoBehaviour
{
    private Rigidbody2D aimBody; // Used to move the crosshairs
    private SpriteRenderer aimSprite; // Show's the crosshairs
    private Animator aimAnimator; // call to shooting/reloading animation (2D graphics should not require separate projectile animation)
    
    // Gun Attributes
    private const int MAX_SHOTS = 6;
    private int shots;
    private const int MAX_RELOADS = 2;
    private int reloads;
    private float shotDelay;
    private bool canShoot;
    private float reloadTime;
    private bool isReloading;


    // Start is called before the first frame update
    void Start()
    {
        aimBody = GetComponent<Rigidbody2D>();
        aimSprite = GetComponent<SpriteRenderer>();
        aimAnimator = GetComponent<Animator>();
        shots = MAX_SHOTS;
        reloads = MAX_RELOADS;
        shotDelay = 1f;
        canShoot = true;
        reloadTime = 3f;
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
        if(shots > 0 && !isReloading)
        {
            shots--;
            canShoot = false;
            // TODO: check for hit
            // TODO: call animation
            if(shots > 0)
            {
                Invoke("ReadyNextShot", shotDelay);
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
            isReloading = true;
            // TODO: call animation
            Invoke("ReloadComplete", reloadTime);
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
