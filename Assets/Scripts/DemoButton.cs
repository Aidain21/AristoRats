using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;



public class DemoButton : MonoBehaviour
{
    public TMP_InputField input;

    public int counter;
    public Button gameButton;
    public Button loginButton;
    


    // Start is called before the first frame update
    void Start()
    {
        loginButton.interactable = false;
        gameButton.interactable = true;
        counter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (input.text.Length > 2 && input.text.Length < 11)
        {
            loginButton.interactable = true;
        }
        else
        {
            loginButton.interactable = false;
        }
    }

    public void ButtonPressed()
    {
        if (counter == 1)
        {
            SceneManager.LoadScene("Forest");
            counter = 0;
        }
        else
        {
            counter += 1;
        }

    }

    
    public void LoginButtonPressed()
    {
        PlayerPrefs.SetString("name", input.text);
        PlayerPrefs.Save();

    }




}
