using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;



public class DemoBu : MonoBehaviour
{
    public TMP_InputField input;

    public int counter;
    public Button gameButton;
    


    // Start is called before the first frame update
    void Start()
    {
        counter = 0;
      
    }

    // Update is called once per frame
    void Update()
    {
        gameButton.interactable = true;
    }

    public void ButtonPressed()
    {
        if (counter == 1)
        {
            Debug.Log("Game");
            SceneManager.LoadScene("Forest");
            counter = 0;

        }
        else{
            counter += 1;
        }

    }

    
    public void LoginButtonPressed()
    {
        PlayerPrefs.SetString("name", input.text);
        PlayerPrefs.Save();

    }




}
