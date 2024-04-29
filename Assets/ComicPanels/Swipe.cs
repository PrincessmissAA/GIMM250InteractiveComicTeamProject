using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Swipe : MonoBehaviour
{
    //public Sprite ComicPanel;
    public List<Sprite> Numbers;
    public int PageNumber;
    public Image PageImage;

    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            startTouchPosition = Input.GetTouch(0).position;
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            endTouchPosition = Input.GetTouch(0).position;

            if (endTouchPosition.x < startTouchPosition.x)
            {
                NextPage();
                Debug.Log("this is nextpage");
            }

            if(endTouchPosition.x > startTouchPosition.x)
            {
                PreviousPage();
                Debug.Log("this is previouspage");
            }
        }

    }

    private void PreviousPage()
    {

        PageNumber--;
        //SceneManager.LoadScene(PageNumber);
        PageImage.sprite = Numbers[PageNumber];
    }

    private void NextPage()
    {
        PageNumber++;
        //SceneManager.LoadScene(PageNumber);
        PageImage.sprite = Numbers[PageNumber];
    }

}
