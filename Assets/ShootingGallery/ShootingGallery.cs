using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/** Control script for the Quantum Shooter mini-game.
 *  USE:
 *      - Assign to game area object
 *      - Correlate the serialized fields with their appropriate UI elements, game objects, and scene indexes
 *  TODO:
 *      - Create end game conditions
 *          + Timer reaches 0
 *          + Target hit points reaches 0
 *          + Player gives up
 *          - Player out of ammo?
 *      - Create/assign methods for UI buttons
 *          + Give up
 *          - Observe
 *          + Reload (in Shooter)
 *      - Add tests
 *      TEST:
 *          - 
 *  BUGS:
 *      - 
 *  CHANGES:
 *      - Shifted code to improve performance. No longer updates HUD every frame:
 *          - Moved target-related display code to Target.cs
 *              - Health display only updates on health decrement
 *              - Speed only updates on speed change
 *              - Vector only updates on vector change
 *          - Moved shooter-related display code to Shooter.cs
 *              - Shots display only updates on shoot
 *              - Reload display only updates on reload
 *          - Added in the index for the ending scenes
 *      
 * @author Joe Shields, Avy Wilford
 * Last Updated: 29 Apr 24 @ 1532
 */

public class ShootingGallery : MonoBehaviour
{
    [SerializeField] private Shooter shooter;
    [SerializeField] private Target target;

    [SerializeField] private Text time;

    //[SerializeField] public int deadEndingSceneIndex;
    //[SerializeField] public int aliveEndingSceneIndex;

    public int deadEndingSceneIndex = 5;
    public int aliveEndingSceneIndex = 4;

    private const int TIME_LIMIT = 30;
    private int timeRemaining;

    // Start is called before the first frame update
    void Start()
    {
        timeRemaining = TIME_LIMIT;
        CountDownTimer();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)) //TODO: Add check for inside game area
        {
            shooter.Shoot();
            if (target.GetHealth() <= 0)
            {
                EndGame();
            }
        }
    }

    /** Decrements timer, updates time display, and checks end game condition
     * Calls itself once every second.
     */
    private void CountDownTimer()
    {
        timeRemaining--;
        time.text = "Time: " + timeRemaining;

        if (timeRemaining <= 0)
        {
            EndGame();
        }
        else
        {
            Invoke("CountDownTimer", 1f);
        }
    }

    /** Loads the appropriate ending scene.
     * Call when game over conditions are met.
     */
    public void EndGame()
    {
        if(target.GetHealth() <= 0)
        {
            SceneManager.LoadScene(deadEndingSceneIndex);
        }
        else
        {
            SceneManager.LoadScene(aliveEndingSceneIndex);
        }
    }
}
