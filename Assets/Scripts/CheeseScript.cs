using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheeseScript : MonoBehaviour
{
    public Sprite[] cheeseImages;
    public int amount;
    void Start()
    {
        
        for (int i = 0; i < GameObject.Find("Player").transform.childCount; i++)
        {
            if (GameObject.Find("Player").transform.GetChild(i).name.Equals(name))
            {
                Destroy(gameObject);
            }
        }
        GetComponent<SpriteRenderer>().sprite = cheeseImages[amount - 1];
    }
}
