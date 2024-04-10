using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShootingGallery : MonoBehaviour
{
    [SerializeField] private Shooter shooter;
    [SerializeField] private Target target;
    [SerializeField] private GameObject vectorArrow;

    [SerializeField] private Text time;
    [SerializeField] private Text targetSpeed;
    [SerializeField] private Text shots;
    [SerializeField] private Text reloads;
    [SerializeField] private Text health;
    [SerializeField] private Text vector;

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
    }

    private void CountDownTimer()
    {
        timeRemaining--;
        
        if(timeRemaining <= 0)
        {
            // End Game
        }
        else
        {
            Invoke("CountDownTimer", 1f);
        }
    }


    private void UpdateHUD()
    {
        time.text = "Time: " + timeRemaining;
        targetSpeed.text = "Speed: " + target.GetSpeed();
        vectorArrow.transform.rotation = Quaternion.Euler(0, 0, target.GetVector());
        vector.text = "Vector:\n " + target.GetVector();
        shots.text = "Shots: " + shooter.GetShotsRemaining();
        reloads.text = "Reload: " + shooter.GetReloadsRemaining();
        health.text = "HEALTH: " + target.GetHealth();
    }
}
