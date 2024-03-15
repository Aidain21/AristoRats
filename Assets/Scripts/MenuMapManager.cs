using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class MenuMapManager : MonoBehaviour
{
    public Selector menuSelector = new(1,6);
    public Selector optionSelector = new(new int[] {1,3,5,3,5,5,3},7);
    public Selector puzzleSelector = new(5,5);
    public List<Image> puzzleImages;
    public string[] menuChoices;
    public Canvas menu;
    public Canvas stats;
    public GameObject statsPanel;
    public GameObject puzzlePanel;
    public Canvas input;
    public TMP_InputField typeBox;
    public List<List<object>> collection = new();
    public int trackedSceneNumber = 5;
    public int totalModesNumber;
    public TMP_Text puzzleText;
    public Canvas options;
    //public Canvas map;
    public Canvas puzzle;
    public GameObject menuTextArray;
    public GameObject optionsTextArray;
    public GameObject puzzleTextArray;
    public GameObject statsTextArray;
    public GameObject modesTextArray;
    public TMP_Text defOpt;
    public TMP_Text defMenu;
    public TMP_Text defPuzzle;
    public TMP_Text percent;
    public Image defImg;
    public Image curPiece;
    public PlayerScript2D playerScript;
    void Start()
    {
        trackedSceneNumber = 5;
        totalModesNumber = playerScript.ALL_PUZZLE_MODES.Length;
        Canvas.ForceUpdateCanvases();   
        menuChoices = new string[] { "Continue", "Options", "Progress Stats", "Silly Button", "Respawn", "Quit (Won't Save)" };
        menu.GetComponent<Canvas>().enabled = false;
        stats.GetComponent<Canvas>().enabled = false;
        options.GetComponent<Canvas>().enabled = false;
        puzzle.GetComponent<Canvas>().enabled = false;
        input.GetComponent<Canvas>().enabled = false;
        typeBox.DeactivateInputField(true);
        int count = 0;
        for (int i = 0; i < menuSelector.textArray.Length; i++)
        {
            for (int j = 0; j < menuSelector.textArray[i].Length; j++)
            {
                count += 1;
                TMP_Text text = Instantiate(defMenu, menuTextArray.transform);
                text.rectTransform.localPosition = new Vector2(0 * j, -100 * i + 125);
                text.name = count.ToString();
                text.text = menuChoices[count - 1];
                menuSelector.textArray[i][j] = text;
            }
        }
        count = 0;
        menuChoices = new string[] { "Back", "Hold to Run", "Toggle Between", "Hold to Walk", "Snail", "Slow", "Normal", "Fast", "Cheetah", "Spam Space", "Hold Space", "Disabled", "Very In", "In", "Default", "Out", "Very Out", "0%", "25%", "50%", "75%", "100%", "Show Location", "Show Pieces Left", "None" };
        for (int i = 0; i < optionSelector.textArray.Length; i++)
        {
            for (int j = 0; j < optionSelector.textArray[i].Length; j++)
            {
                count += 1;
                TMP_Text text = Instantiate(defOpt, optionsTextArray.transform);
                if (j == 0)
                {
                    text.rectTransform.localPosition = new Vector2(0, -110 * i);
                    if (i != 4 && i != 2 && i != 5 && i != 3 && i != 6)
                    {
                        text.color = Color.yellow;
                    }
                }
                else
                {
                    text.rectTransform.localPosition = new Vector2(optionSelector.textArray[i][j-1].rectTransform.localPosition.x + optionSelector.textArray[i][j - 1].preferredWidth + 30, -110 * i);
                    if ((i == 2 && j == 3) || (i == 4 && j == 2) || (i == 5 && j == 2) || (i == 3 && j == 2) || (i == 6 && j == 1))
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
        for (int i = 0; i < trackedSceneNumber; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                TMP_Text text = Instantiate(defMenu, statsTextArray.transform);
                text.rectTransform.localPosition = new Vector2(j * 300 - 300, -100 * i + 25);
            }
        }
        for (int i = 0; i < totalModesNumber; i++)
        {
            TMP_Text text = Instantiate(defMenu, modesTextArray.transform);
            text.rectTransform.localPosition = new Vector2((i%3) * 300 - 300, -100 * (i/3) + 125);
        }
        MakePuzzleMenu();
        Destroy(defOpt);
        defMenu.gameObject.SetActive(false);
    }
    public void OpenMenu()
    {
        menu.GetComponent<Canvas>().enabled = true;
        playerScript.inMenu = true;
        puzzleText.text = "Puzzles solved: " + playerScript.completedPuzzles.Count;
        menuSelector.UpdateSelector();
    }
    public void CloseMenu()
    {
        playerScript.invManager.UpdateInfo();
        menu.GetComponent<Canvas>().enabled = false;    
        playerScript.inMenu = false;
    }
    public void OpenInput()
    {
        input.GetComponent<Canvas>().enabled = true;
        playerScript.eventSystem.gameObject.SetActive(true);
        typeBox.enabled = true;
        typeBox.text = "";
        typeBox.ActivateInputField();
        playerScript.typing = true;
    }
    public void CloseInput()
    {
        playerScript.invManager.UpdateInfo();
        typeBox.enabled = false;
        playerScript.eventSystem.gameObject.SetActive(false);
        input.GetComponent<Canvas>().enabled = false;
    }

    public void OpenStats()
    {
        stats.GetComponent<Canvas>().enabled = true;
        playerScript.inStats = true;
        for (int i = 0; i < collection.Count; i++)
        {
            if (collection[i][0].ToString() == SceneManager.GetActiveScene().name)
            {
                collection[i] = new List<object> { SceneManager.GetActiveScene().name, playerScript.dialogueManager.eventScript.fullyTalkedTo, playerScript.dialogueManager.eventScript.npcsInScene, playerScript.dialogueManager.eventScript.collectedNotes, playerScript.dialogueManager.eventScript.notesInScene };
                break;
            }
        }
        int foundObjects = 0;
        int totalObjects = 0;
        for(int i = 0; i < collection.Count; i++)
        {
            foundObjects += (int) collection[i][1] + (int) collection[i][3];
            totalObjects += (int) collection[i][2] + (int) collection[i][4];
            statsTextArray.transform.GetChild(i*3).GetComponent<TMP_Text>().text = collection[i][0].ToString();
            statsTextArray.transform.GetChild(i*3+1).GetComponent<TMP_Text>().text = collection[i][1].ToString() + "/" + collection[i][2].ToString();
            statsTextArray.transform.GetChild(i*3+2).GetComponent<TMP_Text>().text = collection[i][3].ToString() + "/" + collection[i][4].ToString();
        }
        string percentage = ((float)foundObjects / totalObjects * 100).ToString();
        if (percentage.Contains(".") && percentage[(percentage.IndexOf(".") + 1)..].Length > 2)
        {
            percentage = percentage.Substring(0, percentage.IndexOf(".") + 1) + percentage.Substring(percentage.IndexOf(".") + 1, 2);
        }
        percent.text = "Completion: " + percentage + "%";
        int curModes = 0;
        for (int i = 0; i < playerScript.oldPuzzles.Count; i++)
        {
            modesTextArray.transform.GetChild(i).GetComponent<TMP_Text>().text = playerScript.oldPuzzles[i];
            curModes++;
        }
        for (int i = curModes; i < totalModesNumber; i++)
        {
            modesTextArray.transform.GetChild(i).GetComponent<TMP_Text>().text = "???";
        }

        statsPanel.SetActive(true);
        puzzlePanel.SetActive(false);
    }

    public void CloseStats()
    {
        playerScript.invManager.UpdateInfo();
        stats.GetComponent<Canvas>().enabled = false;
        playerScript.inStats = false;
    }

    public void OpenOptions()
    {
        options.GetComponent<Canvas>().enabled = true;
        playerScript.inOptions = true;
        optionSelector.UpdateSelector();
    }
    public void CloseOptions()
    {
        playerScript.invManager.UpdateInfo();
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
                text.rectTransform.localPosition = new Vector2(125 * j, -125 * i);
                text.name = count.ToString();
                puzzleSelector.textArray[i][j] = text;

                Image img = Instantiate(defImg, puzzleTextArray.transform);
                img.color = new Color32(0, 0, 0, 0);
                img.rectTransform.localPosition = new Vector2(125 * j, -125 * i);
                img.rectTransform.sizeDelta = new Vector2(125, 125);
                img.name = "Image" + count.ToString();
                puzzleImages.Add(img);
            }
        }
        defPuzzle.gameObject.SetActive(false);
        defImg.gameObject.SetActive(false);
    }

    public void ClosePuzzle()
    {
        playerScript.invManager.UpdateInfo();
        puzzle.GetComponent<Canvas>().enabled = false;
        playerScript.inPuzzle = false;
    }
}
