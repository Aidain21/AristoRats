using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            string effect;
            string otherData;
            if (switchEffect.IndexOf(" ") != -1)
            {
                otherData = switchEffect[(switchEffect.IndexOf(" ") + 1)..];
                effect = switchEffect.Substring(0, switchEffect.IndexOf(" "));
            }
            else
            {
                otherData = "";
                effect = switchEffect;
            }
            
            switch (effect)
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
                    Destroy(gameObject);
                    break;
                case "warp":
                    int x = Int32.Parse(otherData.Substring(0, otherData.IndexOf(" ")));
                    int y = Int32.Parse(otherData[(otherData.IndexOf(" ") + 1)..]);
                    item.GetComponent<PlayerScript2D>().StopAllCoroutines();
                    item.GetComponent<PlayerScript2D>().moving = false;
                    item.transform.position = new Vector3(x, y, 0) + item.GetComponent<PlayerScript2D>().direction;
                    SceneManager.LoadScene(switchId);
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
        if (collision.tag == "Player" && switchType == "pressurePlate")
        {
            if (switchEffect.Substring(0, switchEffect.IndexOf(" ")) == "warp")
            {
                affectedObjects[0] = collision.gameObject;
            }
            UseSwitch();
        }
    }
}
