using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemScript : MonoBehaviour
{
    public string itemName;
    public string[] pickupText;
    public string[] itemLore;
    public Sprite itemImage;

    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = itemImage;
        for (int i=0; i < GameObject.Find("Player").transform.childCount; i++)
        {
            if (GameObject.Find("Player").transform.GetChild(i).name.Equals(name))
            {
                Destroy(gameObject);
            }
        }
    }

}
