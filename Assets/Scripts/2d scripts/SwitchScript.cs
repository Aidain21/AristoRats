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
        if (switchType == "pressurePlate++")
        {
            for (int i = 0; i < GameObject.Find("Player").transform.childCount; i++)
            {
                if (GameObject.Find("Player").transform.GetChild(i).name.Equals(name))
                {
                    Destroy(gameObject);
                }
            }
        }
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
                        Destroy(item.transform.GetChild(0).gameObject);
                        Destroy(gameObject);
                        if (transform.parent.gameObject.GetComponent<ImagePuzzleScript>().piecesLeft == 0)
                        {
                            transform.parent.gameObject.GetComponent<ImagePuzzleScript>().EndPuzzle();
                        }
                        break;
                    }
                    break;
                case "insert+":
                    if (item.CompareTag("Block"))
                    {
                        transform.parent.gameObject.GetComponent<ImagePuzzleScript>().piecesLeft -= 1;
                        if (transform.parent.gameObject.GetComponent<ImagePuzzleScript>().piecesLeft == 0)
                        {
                            for (int i = 0; i < transform.parent.childCount; i++)
                            {
                                if (transform.parent.GetChild(i).name.Equals("PushBlock(Clone)"))
                                {
                                    transform.parent.GetChild(i).position += new Vector3(0, 0, 1);
                                    Destroy(transform.parent.GetChild(i).GetComponent<BoxCollider2D>());
                                    Destroy(transform.parent.GetChild(i).GetChild(0).gameObject);
                                }
                            }
                            for (int i = 0; i < transform.parent.childCount; i++)
                            {
                                if (transform.parent.GetChild(i).name.Equals("FloorSwitch(Clone)"))
                                {
                                    Destroy(transform.parent.GetChild(i).gameObject);
                                }
                            }
                            transform.parent.gameObject.GetComponent<ImagePuzzleScript>().EndPuzzle();
                            break;
                        }
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
                        if (name.Contains("-"))
                        {
                            item.GetComponent<PlayerScript2D>().roomName = name[(name.IndexOf("-") + 2)..];
                        }
                        else
                        {
                            item.GetComponent<PlayerScript2D>().roomName = name;
                        }
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
                        item.transform.position = new Vector3(x, y, 0) + item.GetComponent<PlayerScript2D>().direction;
                        item.GetComponent<PlayerScript2D>().spawnPoint = new Vector3(x, y, 0) + item.GetComponent<PlayerScript2D>().direction;
                        
                        item.GetComponent<PlayerScript2D>().currentTarget = null;
                        StartCoroutine(item.GetComponent<PlayerScript2D>().SwitchScene(scene));
                        
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
                    item.GetComponent<PlayerScript2D>().dialogueManager.StartDialogue(signScript.name, signScript.dialogue, signScript.talkCounter, signScript.talkerImage);
                    break;
                case "puzzle":
                    PlayerScript2D player = affectedObjects[2].GetComponent<PlayerScript2D>();
                    GameObject signs = GameObject.Find("InstructionSigns");
                    if (item.name == "Puzzle Box")
                    {
                        item.GetComponent<ImagePuzzleScript>().fullImage = player.puzzleImage;
                        item.GetComponent<ImagePuzzleScript>().width = Mathf.RoundToInt(player.puzzleDims.x);
                        item.GetComponent<ImagePuzzleScript>().height = Mathf.RoundToInt(player.puzzleDims.y);
                        item.GetComponent<ImagePuzzleScript>().reward = player.reward;
                        item.GetComponent<ImagePuzzleScript>().mode = player.puzzleType;
                        item.GetComponent<ImagePuzzleScript>().PuzzleSetUp();
                        item.GetComponent<ImagePuzzleScript>().playerScript = player;
                    }
                    if (item.name == "SceneWarp")
                    {
                        player.StopAllCoroutines();
                        player.moving = false;
                        player.GetComponent<Animator>().enabled = false;
                        item.GetComponent<SwitchScript>().switchData = player.entryScene + " " + player.entryPos.x + "," + player.entryPos.y;
                        if (player.entryDirection == Vector3.up)
                        {
                            item.transform.position = new Vector3(-3, -13, 1);
                            item.transform.Rotate(new Vector3(0, 0, 90));
                            player.transform.position = new Vector3(-3, -12, 0);
                            signs.transform.position = new Vector3(-5, -12, 0);

                        }
                        else if (player.entryDirection == Vector3.left)
                        {
                            item.transform.position = new Vector3(6, 0, 1);
                            player.transform.position = new Vector3(5, 0, 0);
                            signs.transform.position = new Vector3(5, -2, 0);
                        }
                        else if (player.entryDirection == Vector3.down)
                        {
                            item.transform.position = new Vector3(-3, 13, 1);
                            item.transform.Rotate(new Vector3(0, 0, 90));
                            player.transform.position = new Vector3(-3, 12, 0);
                            signs.transform.position = new Vector3(-1, 12, 0);
                        }
                        else if (player.entryDirection == Vector3.right)
                        {
                            item.transform.position = new Vector3(-12, 0, 1);
                            player.transform.position = new Vector3(-11, 0, 0);
                            signs.transform.position = new Vector3(-11, 2, 0);
                        }
                    }
                    break;
                case "puzzleData":
                    item.GetComponent<PlayerScript2D>().puzzleImage = puzzleImage;
                    string type = switchData.Substring(0, switchData.IndexOf(" "));
                    int x2 = Int32.Parse(switchData.Substring(switchData.IndexOf(" ") + 1, switchData.IndexOf(",") - switchData.IndexOf(" ") - 1));
                    int y2 = Int32.Parse(switchData[(switchData.IndexOf(",") + 1)..]);
                    item.GetComponent<PlayerScript2D>().puzzleType = type;
                    item.GetComponent<PlayerScript2D>().puzzleDims = new Vector2(x2, y2);
                    item.GetComponent<PlayerScript2D>().reward = 0;
                    if (name.Contains("L"))
                    {
                        item.GetComponent<PlayerScript2D>().reward = 3;
                    }
                    else if (name != "rand")
                    {
                        item.GetComponent<PlayerScript2D>().reward = 1;
                    }
                    item.GetComponent<PlayerScript2D>().entryPos = new Vector2(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
                    item.GetComponent<PlayerScript2D>().entryScene = SceneManager.GetActiveScene().name;
                    item.GetComponent<PlayerScript2D>().entryDirection = item.GetComponent<PlayerScript2D>().direction;
                    item.GetComponent<PlayerScript2D>().puzzleName = name;
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
            case "pressurePlate++":
                transform.parent = affectedObjects[0].GetComponent<PlayerScript2D>().transform;
                gameObject.SetActive(false);
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
        if (collision.CompareTag("Player") && (switchType == "pressurePlate" || switchType == "pressurePlate+" || switchType == "pressurePlate++"))
        {
            if (switchEffect == "warp" || switchEffect == "talk" || switchEffect == "puzzleData" || switchEffect == "activate")
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
        if (collision.CompareTag("Block") && switchType == "floor" && switchEffect == "insert+")
        {
            if (switchData == collision.GetComponent<BlockScript>().id.ToString() && transform.parent.gameObject.GetComponent<ImagePuzzleScript>().piecesLeft > 0)
            {
                transform.parent.gameObject.GetComponent<ImagePuzzleScript>().piecesLeft += 1;
            }
        }
    }
}
