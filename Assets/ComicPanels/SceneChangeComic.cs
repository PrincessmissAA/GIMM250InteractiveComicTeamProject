using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeComic : MonoBehaviour
{
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void LoadOnClick(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
        Debug.Log("This is scenechange");
    }

}
