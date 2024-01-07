using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchScript : MonoBehaviour
{
    public string switchType;
    public string switchEffect;
    public int switchId;
    public bool onValue;
    public GameObject[] affectedObjects = new GameObject[1];
    // Start is called before the first frame update
    void Start()
    {
        if (switchType == "onoff")
        {
            onValue = true;
            UseSwitch();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UseSwitch()
    {
        //what the switch does
        foreach (GameObject item in affectedObjects)
        {
            switch (switchEffect)
            {
                case "activate":
                    item.SetActive(true);
                    break;
                case "onoff":
                    foreach(Transform child in item.transform)
                    {
                        if (child.gameObject.tag == "RedWall")
                        {
                            child.gameObject.SetActive(onValue);
                        }
                        else
                        {
                            child.gameObject.SetActive(!onValue);
                        } 
                    }
                    break;
                case "insert": //for puzzles
                    item.GetComponent<BlockScript>().inserted = true;
                    break;
            }
        }
        //what happens to the switch
        switch (switchType)
        {
            case "onoff":
                if (!onValue)
                {
                    onValue = !onValue;
                    GetComponent<SpriteRenderer>().color = new Color32(100, 0, 255, 255);
                }
                else
                {
                    onValue = !onValue;
                    GetComponent<SpriteRenderer>().color = new Color32(255, 0, 100, 255);
                }
                break;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Block" && switchType == "floor")
        {
            if(switchId == 0)
            {
                UseSwitch();
            }
            else if (switchId == collision.GetComponent<BlockScript>().id)
            {
                affectedObjects[0] = collision.gameObject;
                UseSwitch();
            }
        }
    }
}
