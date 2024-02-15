using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TileScreenButtons : MonoBehaviour
{
    public Button startButton;
    public Button optionsButton;
    public Button quitButton;
    public TMP_InputField input;
    public void StartButtonPressed()
    {
        if (input.text.Length > 0)
        {
            PlayerPrefs.SetString("name", input.text);
            PlayerPrefs.Save();
            SceneManager.LoadScene("SampleScene");
        }
        else
        {
            PlayerPrefs.SetString("name", "WhyDidn'tYouEnterAName?");
            PlayerPrefs.Save();
            SceneManager.LoadScene("SampleScene");
        }
    }
    public void QuitButtonPressed()
    {
        UnityEditor.EditorApplication.isPlaying = false;
    }
}
    