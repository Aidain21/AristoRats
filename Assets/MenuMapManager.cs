using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuMapManager : MonoBehaviour
{
    public Selector menuSelector = new(1,4);
    public Selector optionSelector;
    public string[] menuChoices;
    public Canvas menu;
    public Canvas options;
    public Canvas map;
    public GameObject TextArray;
    public TMP_Text def;
    public PlayerScript2D playerScript;
    void Start()
    {
        menuChoices = new string[] { "Continue", "Options", "god mode (real)", "Save and Quit" };
        menu.GetComponent<Canvas>().enabled = false;
        //options.GetComponent<Canvas>().enabled = false;
        int count = 0;
        for (int i = 0; i < menuSelector.textArray.Length; i++)
        {
            for (int j = 0; j < menuSelector.textArray[i].Length; j++)
            {
                count += 1;
                TMP_Text text = Instantiate(def, TextArray.transform);
                text.rectTransform.localPosition = new Vector2(0 * j, -100 * i);
                text.name = count.ToString();
                text.text = menuChoices[count - 1];
                menuSelector.textArray[i][j] = text;
            }
        }
        Destroy(def);
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
