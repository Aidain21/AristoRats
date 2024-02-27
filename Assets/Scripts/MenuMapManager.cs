using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuMapManager : MonoBehaviour
{
    public Selector menuSelector = new(1,4);
    public Selector optionSelector = new(new int[] {1,3,5,3,5,5},6);
    public Selector puzzleSelector = new(5,5);
    public List<Image> puzzleImages;
    public string[] menuChoices;
    public Canvas menu;
    public TMP_Text cheeseText;
    public TMP_Text talkedToText;
    public Canvas options;
    //public Canvas map;
    public Canvas puzzle;
    public GameObject menuTextArray;
    public GameObject optionsTextArray;
    public GameObject puzzleTextArray;
    public TMP_Text defOpt;
    public TMP_Text defMenu;
    public TMP_Text defPuzzle;
    public Image defImg;
    public Image curPiece;
    public PlayerScript2D playerScript;
    void Start()
    {
        Canvas.ForceUpdateCanvases();   
        menuChoices = new string[] { "Continue", "Options", "Add 1 Cheese", "Save and Quit" };
        menu.GetComponent<Canvas>().enabled = false;
        options.GetComponent<Canvas>().enabled = false;
        puzzle.GetComponent<Canvas>().enabled = false;
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
        menuChoices = new string[] { "Back", "Hold to Run", "Toggle Between", "Hold to Walk", "Snail", "Slow", "Normal", "Fast", "Cheetah", "Spam Space", "Hold Space", "Disabled", "Very In", "In", "Default", "Out", "Very Out", "0%", "25%", "50%", "75%", "100%" };
        for (int i = 0; i < optionSelector.textArray.Length; i++)
        {
            for (int j = 0; j < optionSelector.textArray[i].Length; j++)
            {
                count += 1;
                TMP_Text text = Instantiate(defOpt, optionsTextArray.transform);
                if (j == 0)
                {
                    text.rectTransform.localPosition = new Vector2(0, -110 * i);
                    if (i != 4 && i != 2 && i != 5)
                    {
                        text.color = Color.yellow;
                    }
                }
                else
                {
                    text.rectTransform.localPosition = new Vector2(optionSelector.textArray[i][j-1].rectTransform.localPosition.x + optionSelector.textArray[i][j - 1].preferredWidth + 30, -110 * i);
                    if ((i == 2 && j == 2) || (i == 4 && j == 2) || (i == 5 && j == 4))
                    {
                        optionSelector.selections[i] = new Vector2(j,i);
                        text.color = Color.yellow;

                    }
                }
                text.name = count.ToString();
                text.text = menuChoices[count - 1];
                optionSelector.textArray[i][j] = text;
                
            }
        }
        MakePuzzleMenu();
        Destroy(defOpt);
        defMenu.gameObject.SetActive(false);
    }
    public void OpenMenu()
    {
        menu.GetComponent<Canvas>().enabled = true;
        playerScript.inMenu = true;
        cheeseText.text = "Cheese: " + playerScript.invManager.cheese.ToString();
        talkedToText.text = "Friends made: " + playerScript.dialogueManager.eventScript.fullyTalkedTo.ToString() + "/" + playerScript.dialogueManager.eventScript.npcsInScene.ToString();
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
    public void OpenPuzzle()
    {
        puzzle.GetComponent<Canvas>().enabled = true;
        curPiece.sprite = playerScript.currentTarget.GetComponent<SpriteRenderer>().sprite;
        playerScript.inPuzzle = true;
        puzzleSelector.UpdateSelector();
    }
    public void MakePuzzleMenu()
    {
        puzzleImages = new List<Image>();
        for (int i = 0; i < puzzleTextArray.transform.childCount; i++)
        {
            Destroy(puzzleTextArray.transform.GetChild(i).gameObject);
        }
        defImg.gameObject.SetActive(true);
        defPuzzle.gameObject.SetActive(true);
        Canvas.ForceUpdateCanvases();
        int count = 0;
        for (int i = 0; i < puzzleSelector.textArray.Length; i++)
        {
            for (int j = 0; j < puzzleSelector.textArray[i].Length; j++)
            {
                count += 1;
                TMP_Text text = Instantiate(defPuzzle, puzzleTextArray.transform);
                text.rectTransform.localPosition = new Vector2(150 * j, -150 * i);
                text.name = count.ToString();
                puzzleSelector.textArray[i][j] = text;

                Image img = Instantiate(defImg, puzzleTextArray.transform);
                img.color = new Color32(0, 0, 0, 0);
                img.rectTransform.localPosition = new Vector2(150 * j, -150 * i);
                img.rectTransform.sizeDelta = new Vector2(150, 150);
                img.name = "Image" + count.ToString();
                puzzleImages.Add(img);
            }
        }
        defPuzzle.gameObject.SetActive(false);
        defImg.gameObject.SetActive(false);
    }

    public void ClosePuzzle()
    {
        puzzle.GetComponent<Canvas>().enabled = false;
        playerScript.inPuzzle = false;
    }
}
