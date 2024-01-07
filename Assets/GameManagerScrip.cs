using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerScrip : MonoBehaviour
{
    public Button gameButton;
    public int counter;

    // Start is called before the first frame update
    void Start()
    {
        counter = 0;
        gameButton.interactable = true;
    }

    public void gameStart()
    {
        if(counter == 1)
        {
            Debug.Log("Game");
            counter = 0;
          
        }
        else
        {
            counter += 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
