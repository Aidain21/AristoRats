using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuMapManager : MonoBehaviour
{
    public Selector menuSelector = new(1,4);
    public Selector optionSelector = new(new int[] {1,3,5,3,4},5);
    public string[] menuChoices;
    public Canvas menu;
    public Canvas options;
    public Canvas map;
    public GameObject menuTextArray;
    public GameObject optionsTextArray;
    public TMP_Text defOpt;
    public TMP_Text defMenu;
    public PlayerScript2D playerScript;
    void Start()
    {
        Canvas.ForceUpdateCanvases();   
        menuChoices = new string[] { "Continue", "Options", "god mode (real)", "Save and Quit" };
        menu.GetComponent<Canvas>().enabled = false;
        options.GetComponent<Canvas>().enabled = false;
        int count = 0;
        for (int i = 0; i < menuSelector.textArray.Length; i++)
        {
            for (int j = 0; j < menuSelector.textArray[i].Length; j++)
            {
                count += 1;
                TMP_Text text = Instantiate(defMenu, menuTextArray.transform);
                text.rectTransform.localPosition = new Vector2(0 * j, -100 * i);
                text.name = count.ToString();
                text.text = menuChoices[count - 1];
                menuSelector.textArray[i][j] = text;
            }
        }
        count = 0;
        menuChoices = new string[] { "Back", "Hold to Run", "Toggle Between", "Hold to Walk", "Snail", "Slow", "Normal", "Fast", "Cheetah", "Spam Space", "Hold Space", "Disabled", "1", "2", "3", "4" };
        for (int i = 0; i < optionSelector.textArray.Length; i++)
        {
            for (int j = 0; j < optionSelector.textArray[i].Length; j++)
            {
                count += 1;
                TMP_Text text = Instantiate(defOpt, optionsTextArray.transform);
                if (j == 0)
                {
                    text.rectTransform.localPosition = new Vector2(0, -110 * i);
                    if (i != 2 && j != 2)
                    {
                        text.color = Color.yellow;
                    }
                }
                else
                {
                    text.rectTransform.localPosition = new Vector2(optionSelector.textArray[i][j-1].rectTransform.localPosition.x + optionSelector.textArray[i][j - 1].preferredWidth + 30, -110 * i);
                }
                text.name = count.ToString();
                text.text = menuChoices[count - 1];
                optionSelector.textArray[i][j] = text;
                if (i == 2 && j == 2)
                {
                    optionSelector.selections[2] = new Vector2(2, 2);
                    text.color = Color.yellow;

                }
            }
        }
        Destroy(defOpt);
        Destroy(defMenu);
    }
    public void OpenMenu()
    {
        menu.GetComponent<Canvas>().enabled = true;
        playerScript.inMenu = true;
        menuSelector.UpdateSelector();
    }
    public void CloseMenu()
    {
        menu.GetComponent<Canvas>().enabled = false;
        playerScript.inMenu = false;
    }

    public void OpenOptions()
    {
        options.GetComponent<Canvas>().enabled = true;
        playerScript.inOptions = true;
        optionSelector.UpdateSelector();
    }
    public void CloseOptions()
    {
        options.GetComponent<Canvas>().enabled = false;
        playerScript.inOptions = false;
    }
}
