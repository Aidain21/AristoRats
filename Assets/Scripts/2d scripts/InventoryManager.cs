using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public Selector selector;
    public List<GameObject> inventory;
    public Canvas inventoryBox;
    public Canvas blockControlText;
    public Canvas infoDisplay;
    public TMP_Text itemsText;
    public TMP_Text progressText;
    public TMP_Text locationText;
    public int cheese;
    public TMP_Text titleText;
    public TMP_Text controlsText;
    public List<Image> images;
    public PlayerScript2D playerScript;
    public GameObject TextArray;
    public TMP_Text def;
    public Image def2;
    public int lastPieceCount;

    // Start is called before the first frame update
    void Start()
    {
        selector = new Selector(5, 3);
        images = new List<Image>();
        inventoryBox.GetComponent<Canvas>().enabled = false;
        blockControlText.GetComponent<Canvas>().enabled = false;
        cheese = 0;
        for (int i = 0; i < selector.textArray.Length; i++)
        {
            for (int j = 0; j < selector.textArray[i].Length; j++)
            {
                Image img = Instantiate(def2, TextArray.transform);
                img.rectTransform.localPosition = new Vector2(325* j, -225 * i);
                img.rectTransform.sizeDelta = new Vector2(100, 100);
                img.name = "Image" + (i * selector.textArray[i].Length + j + 1).ToString();
                images.Add(img);

                TMP_Text text = Instantiate(def, TextArray.transform);
                text.rectTransform.localPosition = new Vector2(325 * j + 150, -225 * i);
                text.name = "Text" + (i * selector.textArray[i].Length + j + 1).ToString();
                selector.textArray[i][j] = text;
            }
        }
        Destroy(def);
        Destroy(def2);
    }

    public void UpdateInfo(int pieces = -2)
    {
        itemsText.text = "Cheese: " + cheese + "\nInventory: " + inventory.Count + "/15" + "\nNot in Puzzle";
        locationText.text = SceneManager.GetActiveScene().name + " - " + playerScript.roomName;
        if (SceneManager.GetActiveScene().name == "ImagePuzzle")
        {
            if (pieces != -2)
            {
                lastPieceCount = pieces;
            }
            locationText.text = "Mode: (" + playerScript.puzzleType + ")";
            if (playerScript.puzzleType != "Shuffle+" && playerScript.puzzleType != "Swap+" && (playerScript.menuManager.optionSelector.selections[6].x == 0 || playerScript.menuManager.optionSelector.selections[6].x == 1))
            {
                locationText.text += " - Pieces Left: " + lastPieceCount;
            }
            if (!playerScript.finishedPuzzle)
            {
                itemsText.text = "Cheese: " + cheese + "\nInventory: " + inventory.Count + "/15" + "\nTime: " + playerScript.lastPTimerInt + " sec";
            }
            else
            {
                itemsText.text = "Cheese: " + cheese + "\nInventory: " + inventory.Count + "/15" + "\n" + playerScript.endMessage;
            }
        }
        progressText.text = "Friends: " + playerScript.dialogueManager.eventScript.fullyTalkedTo + "/" + playerScript.dialogueManager.eventScript.npcsInScene + "\nNotes: " + playerScript.dialogueManager.eventScript.collectedNotes + "/" + playerScript.dialogueManager.eventScript.notesInScene + "\nPuzzles: " + playerScript.dialogueManager.eventScript.completedPuzzlesInScene + "/" + playerScript.dialogueManager.eventScript.puzzlesInScene;
    }
    public void OpenInventory()
    {
        inventoryBox.GetComponent<Canvas>().enabled = true;
        playerScript.inInventory = true;
        int curItems = 0;
        for (int i = 0; i < inventory.Count; i++)
        {
            images[i].color = Color.white;
            images[i].sprite = inventory[i].GetComponent<ItemScript>().itemImage;
            selector.textArray[i / selector.width][i % selector.width].text = inventory[i].GetComponent<ItemScript>().itemName;
            curItems++;
        }
        for (int i = curItems; i < selector.width * selector.height; i++)
        {
            images[i].sprite = null;
            images[i].color = new Color32(0,0,0,0);
            selector.textArray[i / selector.width][i % selector.width].text = "Empty";
        }
        if (playerScript.selectingItem)
        {
            titleText.text = "Select an Item:";
            controlsText.text = "WASD - Select              E - Choose Item";
        }
        else
        {
            titleText.text = "Inventory";
            controlsText.text = "WASD - Select         E - Info          R - Drop       I/Esc - Close";
        }
        selector.UpdateSelector();
    }
    public void CloseInventory()
    {
        UpdateInfo();
        inventoryBox.GetComponent<Canvas>().enabled = false;
        playerScript.inInventory = false;
    }

    public void RemovePuzzleStuff()
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].GetComponent<ItemScript>().itemName == "Puzzle Piece")
            {
                inventory.Remove(inventory[i]);
                i--;
            }
            
        }
        for (int i = 0; i < playerScript.transform.childCount; i++)
        {
            if (playerScript.transform.GetChild(i).CompareTag("Item"))
            {
                if (playerScript.transform.GetChild(i).GetComponent<ItemScript>().itemName == "Puzzle Piece")
                {
                    Destroy(playerScript.transform.GetChild(i).gameObject);
                }
            }
        }
    }
}
