using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RainbowEffect : MonoBehaviour
{
    public float rainbowSpeed;
    private float hue;
    private float sat;
    private float bri;
    private Tilemap tileMap;
    // Start is called before the first frame update
    void Start()
    {
        tileMap = GetComponent<Tilemap>();
    }

    // Update is called once per frame
    void Update()
    {
        Color.RGBToHSV(tileMap.color, out hue, out sat, out bri);
        hue += rainbowSpeed / 10000;
        if (hue >= 1)
        {
            hue = 0;
        }
        sat = 1;
        bri = 1;
        tileMap.color = Color.HSVToRGB(hue, sat, bri);

        
    }
}
