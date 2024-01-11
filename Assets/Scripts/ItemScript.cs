using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemScript : MonoBehaviour
{
    public string itemName;
    public string[] pickupText;
    public string[] itemLore;
    public int itemId;
    public Sprite itemImage;

    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = itemImage;
    }

}
