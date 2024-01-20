using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScript : MonoBehaviour
{
    public string switchType;
    public string switchEffect;
    public string switchData;
    public Transform warpEnd;
    public bool onValue;
    public bool playerOnSpike = false;
    public GameObject[] affectedObjects = new GameObject[1];
    // Start is called before the first frame update
    void Start()
    {
        if (switchType == "onoff")
        {
            onValue = true;
            UseSwitch();
        }
        if (switchEffect == "spike")
        {
            StartCoroutine(SpikeAction());
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (switchEffect == "spike" && playerOnSpike)
        {
            UseSwitch();
        }
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
                    if (item.gameObject.tag == "RedWall")
                    {
                        item.gameObject.SetActive(onValue);
                    }
                    else
                    {
                        item.gameObject.SetActive(!onValue);
                    } 
                    break;
                case "insert": //for puzzles
                    item.GetComponent<BlockScript>().inserted = true;
                    Destroy(gameObject);
                    break;
                case "warp":
                    if (switchData.Length == 0)
                    {
                        item.GetComponent<PlayerScript2D>().StopAllCoroutines();
                        item.GetComponent<PlayerScript2D>().moving = false;
                        item.transform.position = new Vector3(warpEnd.position.x, warpEnd.position.y, 0) + item.GetComponent<PlayerScript2D>().direction;
                        item.GetComponent<PlayerScript2D>().spawnPoint = new Vector3(warpEnd.position.x, warpEnd.position.y, 0) + item.GetComponent<PlayerScript2D>().direction;
                    }
                    else
                    {
                        string scene = switchData.Substring(0, switchData.IndexOf(" "));
                        int x = Int32.Parse(switchData.Substring(switchData.IndexOf(" ") + 1, switchData.IndexOf(",") - switchData.IndexOf(" ") - 1));
                        int y = Int32.Parse(switchData[(switchData.IndexOf(",") + 1)..]);
                        item.GetComponent<PlayerScript2D>().StopAllCoroutines();
                        item.GetComponent<PlayerScript2D>().moving = false;
                        item.transform.position = new Vector3(x, y, 0) + item.GetComponent<PlayerScript2D>().direction;
                        item.GetComponent<PlayerScript2D>().spawnPoint = new Vector3(x, y, 0) + item.GetComponent<PlayerScript2D>().direction;
                        SceneManager.LoadScene(scene);
                    }
                    break;
                case "spike":
                    if (GetComponent<SpriteRenderer>().color == Color.red)
                    {
                        item.GetComponent<PlayerScript2D>().StopAllCoroutines();
                        item.GetComponent<PlayerScript2D>().moving = false;
                        item.transform.position = item.GetComponent<PlayerScript2D>().spawnPoint;
                    }
                    break;
                case "talk":
                    SignTextScript signScript = GetComponent<SignTextScript>();
                    item.GetComponent<PlayerScript2D>().dialogueManager.StartDialogue(signScript.dialogueName, signScript.dialogue, signScript.talkCounter, signScript.talkerImage);
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
            case "pressurePlate+":
                Destroy(gameObject);
                break;
        }
    }

    public IEnumerator SpikeAction()
    {
        while (true)
        {
            GetComponent<SpriteRenderer>().color = Color.red;
            yield return new WaitForSeconds(1);
            GetComponent<SpriteRenderer>().color = Color.green;
            yield return new WaitForSeconds(1);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Block" && switchType == "floor")
        {
            if(switchData == "")
            {
                UseSwitch();
            }
            else if (switchData == collision.GetComponent<BlockScript>().id.ToString())
            {
                affectedObjects[0] = collision.gameObject;
                UseSwitch();
            }
        }
        if (collision.tag == "Player" && (switchType == "pressurePlate" || switchType == "pressurePlate+"))
        {
            if (switchEffect == "warp")
            {
                affectedObjects[0] = collision.gameObject;
            }
            if (switchEffect == "spike")
            {
                affectedObjects[0] = collision.gameObject;
                playerOnSpike = true;
            }
            if (switchEffect == "talk")
            {
                affectedObjects[0] = collision.gameObject;
            }
            UseSwitch();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player" && switchType == "pressurePlate")
        {
            if (switchEffect == "spike")
            {
                affectedObjects[0] = collision.gameObject;
                playerOnSpike = false;
            }
        }
    }
}
