using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/** Control script for the Quantum Shooter mini-game user interface.
 *  USE:
 *      - Assign to any game object (e.g.- main camera)
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
 *      READY TO TEST:
 *          - 
 *  BUGS:
 *      - 
 *  CHANGES:
 *      - 
 *      
 * @author Joe Shields
 * Last Updated: 14 Apr 24 @ 1300
 */

public class ShootingGallery : MonoBehaviour
{
    [SerializeField] private Shooter shooter;
    [SerializeField] private Texture2D crosshairs;
    [SerializeField] private Target target;
    [SerializeField] private GameObject vectorArrow;

    [SerializeField] private Text time;
    [SerializeField] private Text targetSpeed;
    [SerializeField] private Text shots;
    [SerializeField] private Text reloads;
    [SerializeField] private Text health;

    [SerializeField] private int deadEndingSceneIndex;
    [SerializeField] private int aliveEndingSceneIndex;

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
        UpdateHUD();
        if(target.GetHealth() <= 0)
        {
            EndGame();
        }
    }

    private void CountDownTimer()
    {
        timeRemaining--;
        
        if(timeRemaining <= 0)
        {
            EndGame();
        }
        else
        {
            Invoke("CountDownTimer", 1f);
        }
    }

    public void OnMouseEnter()
    {
        Cursor.SetCursor(crosshairs, Vector2.zero, CursorMode.Auto);
        Debug.Log("Mouse over game area");
    }

    public void OnMouseExit()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    /** Calls Getter methods and updates all text fields. Also rotates vector arrow.*/
    private void UpdateHUD()
    {
        time.text = "Time: " + timeRemaining;
        targetSpeed.text = "Speed: " + target.GetSpeed();
        vectorArrow.transform.rotation = Quaternion.Euler(0, 0, target.GetVector());
        shots.text = "Shots: " + shooter.GetShotsRemaining();
        reloads.text = "Reload: " + shooter.GetReloadsRemaining();
        health.text = "HEALTH: " + target.GetHealth();
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
