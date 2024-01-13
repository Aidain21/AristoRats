using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public List<GameObject> inventory;
    public Canvas inventoryBox;
    public TMP_Text[] texts = new TMP_Text[6];
    public Image[] images = new Image[6];
    public PlayerScript2D playerScript;
    public int selectorPos = 0;
    public int prevSelect = 5;

    // Start is called before the first frame update
    void Start()
    {
        inventoryBox.GetComponent<Canvas>().enabled = false;
        prevSelect = 5;
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
            texts[i].text = inventory[i].GetComponent<ItemScript>().itemName;
            curItems++;
        }
        for (int i = curItems; i < 6; i++)
        {
            images[i].sprite = null;
            images[i].color = new Color32(0,0,0,0);
            texts[i].text = "";
        }
        UpdateSelector();
    }
    public void UpdateSelector()
    {
        images[selectorPos].color = new Color32(255, 255, 255, 200);
        texts[selectorPos].color = new Color32(255, 255, 0, 255);
        texts[selectorPos].fontStyle = FontStyles.Bold;
        if (prevSelect < inventory.Count)
        {
            images[prevSelect].color = Color.white; 
        }
        else
        {
            images[prevSelect].color = new Color32(0, 0, 0, 0);
        }
        texts[prevSelect].color = Color.white;
        texts[prevSelect].fontStyle = FontStyles.Normal;
    }
    public void CloseInventory()
    {
        inventoryBox.GetComponent<Canvas>().enabled = false;
        playerScript.inInventory = false;
    }
}
