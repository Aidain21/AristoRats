using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TileScreenButtons : MonoBehaviour
{


    public Button startButton;
    public Button optionsButton;
    public Button quitButton;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        startButton.interactable = true;
    }

    public void StartButtonPressed()
    {
        Debug.Log("Start");
        SceneManager.LoadScene("NameInputScene");
    }

    

    public void ButtonPressed()
    {
        SceneManager.LoadScene("TextingScene");
          
    }




}
