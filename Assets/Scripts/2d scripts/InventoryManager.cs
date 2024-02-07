using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public Selector selector = new Selector(3, 2);
    public List<GameObject> inventory;
    public Canvas inventoryBox;
    public int cheese;
    public TMP_Text cheeseText;
    public Image[] images = new Image[6];
    public PlayerScript2D playerScript;
    public GameObject TextArray;
    public TMP_Text def;

    // Start is called before the first frame update
    void Start()
    {
        inventoryBox.GetComponent<Canvas>().enabled = false;
        cheese = 0;
        for (int i = 0; i < selector.textArray.GetLength(0); i++)
        {
            for (int j = 0; j < selector.textArray[i].GetLength(0); j++)
            {
                TMP_Text text = Instantiate(def, TextArray.transform);
                text.rectTransform.localPosition = new Vector2(150 * j, -75 * i);
                text.name = (i * selector.textArray[i].GetLength(0) + j + 1).ToString();
                selector.textArray[i][j] = text;
            }
        }
        Destroy(def);
    }

    // Update is called once per frame
    public void OpenInventory()
    {
        inventoryBox.GetComponent<Canvas>().enabled = true;
        playerScript.inInventory = true;
        int curItems = 0;
        for (int i = 0; i < inventory.Count; i++)
        {
            images[i].color = Color.white;
            images[i].sprite = inventory[i].GetComponent<ItemScript>().itemImage;
            selector.textArray[i%selector.width][i/selector.width].text = inventory[i].GetComponent<ItemScript>().itemName;
            curItems++;
        }
        for (int i = curItems; i < 6; i++)
        {
            images[i].sprite = null;
            images[i].color = new Color32(0,0,0,0);
            selector.textArray[i % selector.width][i / selector.width].text = "Empty";
        }
        cheeseText.text = "Cheese: " + cheese.ToString();
        selector.UpdateSelector();
    }
    public void CloseInventory()
    {
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
            if (playerScript.transform.GetChild(i).tag == "Item")
            {
                if (playerScript.transform.GetChild(i).GetComponent<ItemScript>().itemName == "Puzzle Piece")
                {
                    Destroy(playerScript.transform.GetChild(i).gameObject);
                }
            }
        }
    }
}
