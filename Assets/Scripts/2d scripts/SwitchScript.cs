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
    public Texture2D puzzleImage;
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
                    if (item.CompareTag("RedWall"))
                    {
                        item.SetActive(onValue);
                    }
                    else
                    {
                        item.SetActive(!onValue);
                    } 
                    break;
                case "insert": //for puzzles
                    if (item.CompareTag("Block"))
                    {
                        item.GetComponent<BlockScript>().inserted = true;
                        Destroy(gameObject);
                        break;
                    }
                    else if (item.CompareTag("Item"))
                    {
                        transform.parent.gameObject.GetComponent<ImagePuzzleScript>().piecesLeft -= 1;
                        item.transform.position += new Vector3(0, 0, 1);
                        Destroy(item.GetComponent<BoxCollider2D>());
                        Destroy(item.GetComponent<ItemScript>());
                        Destroy(gameObject);
                        break;
                    }
                    break;
                case "warp":
                    item.GetComponent<PlayerScript2D>().invManager.RemovePuzzleStuff();
                    if (switchData.Length == 0)
                    {
                        item.GetComponent<PlayerScript2D>().StopAllCoroutines();
                        item.GetComponent<PlayerScript2D>().moving = false;
                        item.GetComponent<Animator>().enabled = false;
                        float test;
                        if (item.GetComponent<PlayerScript2D>().direction == Vector3.up || item.GetComponent<PlayerScript2D>().direction == Vector3.down)
                        {
                            test = item.transform.InverseTransformPoint(transform.position).x;
                            item.transform.position = new Vector3(warpEnd.position.x - test, warpEnd.position.y, 0) + item.GetComponent<PlayerScript2D>().direction;
                        }
                        else
                        {
                            test = item.transform.InverseTransformPoint(transform.position).y;
                            item.transform.position = new Vector3(warpEnd.position.x, warpEnd.position.y - test, 0) + item.GetComponent<PlayerScript2D>().direction;
                        }
                        item.GetComponent<PlayerScript2D>().spawnPoint = new Vector3(warpEnd.position.x, warpEnd.position.y, 0) + item.GetComponent<PlayerScript2D>().direction;
                    }
                    else
                    {
                        string scene = switchData.Substring(0, switchData.IndexOf(" "));
                        int x = Int32.Parse(switchData.Substring(switchData.IndexOf(" ") + 1, switchData.IndexOf(",") - switchData.IndexOf(" ") - 1));
                        int y = Int32.Parse(switchData[(switchData.IndexOf(",") + 1)..]);
                        item.GetComponent<PlayerScript2D>().StopAllCoroutines();
                        item.GetComponent<PlayerScript2D>().moving = false;
                        item.GetComponent<Animator>().enabled = false;
                        if (scene == "ImagePuzzle")
                        {
                            item.GetComponent<PlayerScript2D>().direction = Vector3.right;
                        }
                        else if (item.GetComponent<PlayerScript2D>().entryDirection != Vector3.zero)
                        {
                            item.GetComponent<PlayerScript2D>().direction = -item.GetComponent<PlayerScript2D>().entryDirection;
                            item.GetComponent<PlayerScript2D>().entryDirection = Vector3.zero;
                        }
                        item.transform.position = new Vector3(x, y, 0) + item.GetComponent<PlayerScript2D>().direction;
                        item.GetComponent<PlayerScript2D>().spawnPoint = new Vector3(x, y, 0) + item.GetComponent<PlayerScript2D>().direction;
                        item.GetComponent<PlayerScript2D>().SwitchSong(scene);
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
                case "puzzle":
                    PlayerScript2D player = affectedObjects[2].GetComponent<PlayerScript2D>();
                    if (item.name == "Puzzle Box")
                    {
                        item.GetComponent<ImagePuzzleScript>().fullImage = player.puzzleImage;
                        item.GetComponent<ImagePuzzleScript>().width = Mathf.RoundToInt(player.puzzleDims.x);
                        item.GetComponent<ImagePuzzleScript>().height = Mathf.RoundToInt(player.puzzleDims.y);
                        item.GetComponent<ImagePuzzleScript>().reward = player.reward;
                        item.GetComponent<ImagePuzzleScript>().mode = player.puzzleType;
                        item.GetComponent<ImagePuzzleScript>().PuzzleSetUp();
                    }
                    if (item.name == "SceneWarp")
                    {
                        item.GetComponent<SwitchScript>().switchData = player.entryScene + " " + player.entryPos.x + "," + player.entryPos.y;
                    }
                    break;
                case "puzzleData":
                    item.GetComponent<PlayerScript2D>().puzzleImage = puzzleImage;
                    string type = switchData.Substring(0, switchData.IndexOf(" "));
                    int x2 = Int32.Parse(switchData.Substring(switchData.IndexOf(" ") + 1, switchData.IndexOf(",") - switchData.IndexOf(" ") - 1));
                    int y2 = Int32.Parse(switchData[(switchData.IndexOf(",") + 1)..]);
                    item.GetComponent<PlayerScript2D>().puzzleType = type;
                    item.GetComponent<PlayerScript2D>().puzzleDims = new Vector2(x2, y2);
                    item.GetComponent<PlayerScript2D>().reward = null;
                    item.GetComponent<PlayerScript2D>().entryPos = new Vector2(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
                    item.GetComponent<PlayerScript2D>().entryScene = SceneManager.GetActiveScene().name;
                    item.GetComponent<PlayerScript2D>().entryDirection = item.GetComponent<PlayerScript2D>().direction;
                    item.GetComponent<PlayerScript2D>().menuManager.puzzleSelector = new Selector(x2,y2);
                    item.GetComponent<PlayerScript2D>().menuManager.MakePuzzleMenu();
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
        if (collision.CompareTag("Block") && switchType == "floor")
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
        if (collision.CompareTag("Item") && switchType == "floor" && switchData == collision.name)
        {
            affectedObjects[0] = collision.gameObject;
            UseSwitch();
        }
        if (collision.CompareTag("Player") && (switchType == "pressurePlate" || switchType == "pressurePlate+"))
        {
            if (switchEffect == "warp" || switchEffect == "talk" || switchEffect == "puzzleData")
            {
                affectedObjects[0] = collision.gameObject;
            }
            if (switchEffect == "spike")
            {
                affectedObjects[0] = collision.gameObject;
                playerOnSpike = true;
            }
            if (switchEffect == "puzzle")
            {
                affectedObjects = new GameObject[3];
                affectedObjects[0] = GameObject.Find("Puzzle Box");
                affectedObjects[1] = GameObject.Find("SceneWarp");
                affectedObjects[2] = collision.gameObject;
            }
            UseSwitch();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && switchType == "pressurePlate")
        {
            if (switchEffect == "spike")
            {
                affectedObjects[0] = collision.gameObject;
                playerOnSpike = false;
            }
        }
    }
}
