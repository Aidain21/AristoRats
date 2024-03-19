using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

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
        if (switchEffect == "barrier")
        {
            transform.GetChild(0).localScale = new Vector3(1/transform.localScale.x, 1 / transform.localScale.y);
            affectedObjects[0] = GameObject.Find("Player");
            if (switchData[0] == 'T')
            {
                GetComponent<SpriteRenderer>().color = new Color32(250, 170, 0, 255);
            }
            if (switchData[1] == 'F')
            {
                transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Rat");
            }
            else if (switchData[1] == 'N')
            {
                transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/NotePaper");
            }
            else
            {
                transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Normal");
            }
            transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = switchData[2..];
        }
        if (switchType == "pressurePlate++" || switchType == "physical++")
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
        if (switchEffect == "speed" && playerOnSpike)
        {
            if (affectedObjects[0].GetComponent<PlayerScript2D>().running)
            {
                affectedObjects[0].GetComponent<PlayerScript2D>().timeBetweenTiles = 0.05f;
            }
            else
            {
                affectedObjects[0].GetComponent<PlayerScript2D>().timeBetweenTiles = 0.1f;
            }
            
        }
        if (switchEffect == "shift" && playerOnSpike && affectedObjects[0] != null && !affectedObjects[0].GetComponent<PlayerScript2D>().moving)
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
                case "insert" or "insert-": //for puzzles
                    if (item.CompareTag("Block"))
                    {
                        if (GameObject.Find("Puzzle Box") != null)
                        {
                            transform.parent.gameObject.GetComponent<ImagePuzzleScript>().ChangePiecesLeft(-1);
                        }
                        GameObject.Find("Player").GetComponent<PlayerScript2D>().invManager.UpdateInfo();
                        Destroy(item.GetComponent<BoxCollider2D>());
                        Destroy(item.transform.GetChild(0).gameObject);
                        Destroy(gameObject);
                        GameObject.Find("Player").GetComponent<PlayerScript2D>().currentTarget = null;
                        break;
                    }
                    else if (item.CompareTag("Item"))
                    {
                        transform.parent.gameObject.GetComponent<ImagePuzzleScript>().ChangePiecesLeft(-1);
                        GameObject.Find("Player").GetComponent<PlayerScript2D>().invManager.UpdateInfo();
                        Destroy(item.GetComponent<BoxCollider2D>());
                        Destroy(item.GetComponent<ItemScript>());
                        Destroy(item.transform.GetChild(0).gameObject);
                        Destroy(gameObject);
                        if (transform.parent.gameObject.GetComponent<ImagePuzzleScript>().piecesLeft == 0)
                        {
                            transform.parent.gameObject.GetComponent<ImagePuzzleScript>().EndPuzzle();
                        }
                        GameObject.Find("Player").GetComponent<PlayerScript2D>().currentTarget = null;
                        break;
                    }
                    else if (item.CompareTag("Sign"))
                    {
                        transform.parent.gameObject.GetComponent<ImagePuzzleScript>().ChangePiecesLeft(-1);
                        GameObject.Find("Player").GetComponent<PlayerScript2D>().invManager.UpdateInfo();
                        GameObject.Find("Player").GetComponent<PlayerScript2D>().dialogueManager.StartDialogue(item.GetComponent<SignTextScript>().name, item.GetComponent<SignTextScript>().dialogue, 3, item.GetComponent<SignTextScript>().talkerImage);
                        Destroy(item.GetComponent<BoxCollider2D>());
                        Destroy(item.GetComponent<SignTextScript>());
                        Destroy(item.GetComponent<Animator>());
                        Destroy(item.transform.GetChild(0).gameObject);
                        Destroy(item.transform.GetChild(1).gameObject);
                        Destroy(gameObject);
                        if (transform.parent.gameObject.GetComponent<ImagePuzzleScript>().piecesLeft == 0)
                        {
                            transform.parent.gameObject.GetComponent<ImagePuzzleScript>().EndPuzzle();
                        }
                        GameObject.Find("Player").GetComponent<PlayerScript2D>().currentTarget = null;
                        break;
                    }
                    break;
                case "insert+":
                    if (item.CompareTag("Block") || item.CompareTag("Item") || item.CompareTag("Sign"))
                    {
                        string blockType = "";
                        transform.parent.gameObject.GetComponent<ImagePuzzleScript>().ChangePiecesLeft(-1);
                        GameObject.Find("Player").GetComponent<PlayerScript2D>().invManager.UpdateInfo();
                        if (transform.parent.gameObject.GetComponent<ImagePuzzleScript>().piecesLeft == 0)
                        {
                            for (int i = 0; i < transform.parent.childCount; i++)
                            {
                                if (transform.parent.GetChild(i).name.Equals("PushBlock(Clone)"))
                                {
                                    blockType = transform.parent.GetChild(i).GetComponent<BlockScript>().type;
                                    Destroy(transform.parent.GetChild(i).GetComponent<BoxCollider2D>());
                                    Destroy(transform.parent.GetChild(i).GetChild(0).gameObject);
                                }
                                else if (transform.parent.GetChild(i).CompareTag("Item"))
                                {
                                    Destroy(transform.parent.GetChild(i).GetComponent<ItemScript>());
                                    Destroy(transform.parent.GetChild(i).GetComponent<BoxCollider2D>());
                                    Destroy(transform.parent.GetChild(i).GetChild(0).gameObject);
                                }
                                else if (transform.parent.GetChild(i).CompareTag("Sign"))
                                {
                                    Destroy(transform.parent.GetChild(i).GetComponent<SignTextScript>());
                                    Destroy(transform.parent.GetChild(i).GetComponent<BoxCollider2D>());
                                    Destroy(transform.parent.GetChild(i).GetChild(0).gameObject);
                                    Destroy(transform.parent.GetChild(i).GetChild(1).gameObject);
                                }
                                else if (transform.parent.GetChild(i).name.Equals("FloorSwitch(Clone)"))
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
                        item.GetComponent<PlayerScript2D>().invManager.UpdateInfo();
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
                        
                        if (!name.Contains("SceneWarp"))
                        {
                            if (name.Contains("-"))
                            {
                                item.GetComponent<PlayerScript2D>().roomName = name[(name.IndexOf("-") + 2)..];
                            }
                            else
                            {
                                item.GetComponent<PlayerScript2D>().roomName = name;
                            }
                        }
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
                    player.pTimer = 0;
                    player.lastPTimerInt = 0;
                    player.invManager.UpdateInfo();
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
                            item.transform.position = new Vector3(-5, -11, 1);
                            item.transform.Rotate(new Vector3(0, 0, 90));
                            player.transform.position = new Vector3(-5, -10, 0);
                            signs.transform.position = new Vector3(-3, -10, 0);

                        }
                        else if (player.entryDirection == Vector3.left)
                        {
                            item.transform.position = new Vector3(2, 0, 1);
                            player.transform.position = new Vector3(1, 0, 0);
                            signs.transform.position = new Vector3(1, -2, 0);
                        }
                        else if (player.entryDirection == Vector3.down)
                        {
                            item.transform.position = new Vector3(-5, 11, 1);
                            item.transform.Rotate(new Vector3(0, 0, 90));
                            player.transform.position = new Vector3(-5, 10, 0);
                            signs.transform.position = new Vector3(-3, 10, 0);
                        }
                        else if (player.entryDirection == Vector3.right)
                        {
                            item.transform.position = new Vector3(-12, 0, 1);
                            player.transform.position = new Vector3(-11, 0, 0);
                            signs.transform.position = new Vector3(-11, 2, 0);
                        }
                    }
                    player.invManager.UpdateInfo((int)(player.puzzleDims.x * player.puzzleDims.y));
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
                    else if (!name.Contains("#"))
                    {
                        item.GetComponent<PlayerScript2D>().reward = 1;
                    }
                    item.GetComponent<PlayerScript2D>().entryPos = new Vector2(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
                    item.GetComponent<PlayerScript2D>().entryScene = SceneManager.GetActiveScene().name;
                    item.GetComponent<PlayerScript2D>().entryDirection = item.GetComponent<PlayerScript2D>().direction;
                    item.GetComponent<PlayerScript2D>().puzzleName = name;
                    item.GetComponent<PlayerScript2D>().menuManager.puzzleSelector = new Selector(x2,y2);
                    if (type == "Menu")
                    {
                        item.GetComponent<PlayerScript2D>().menuManager.MakePuzzleMenu();
                    }
                    break;
                case "shift":
                    if (!item.GetComponent<PlayerScript2D>().moving)
                    {
                        item.GetComponent<PlayerScript2D>().noControl = true;
                        item.GetComponent<PlayerScript2D>().direction = transform.up;
                        if (transform.up == Vector3.up)
                        {
                            item.GetComponent<SpriteRenderer>().sprite = item.GetComponent<PlayerScript2D>().idleSprites[0];
                        }
                        else if (transform.up == Vector3.left)
                        {
                            item.GetComponent<SpriteRenderer>().sprite = item.GetComponent<PlayerScript2D>().idleSprites[1];
                        }
                        else if (transform.up == Vector3.down)
                        {
                            item.GetComponent<SpriteRenderer>().sprite = item.GetComponent<PlayerScript2D>().idleSprites[2];
                        }
                        else if (transform.up == Vector3.right)
                        {
                            item.GetComponent<SpriteRenderer>().sprite = item.GetComponent<PlayerScript2D>().idleSprites[3];
                        }
                        StartCoroutine(item.GetComponent<PlayerScript2D>().GridMove(item, item.transform.position - -transform.up, 0.5f));
                    }
                    break;
                case "barrier":
                    item.GetComponent<PlayerScript2D>().menuManager.OpenStats();
                    item.GetComponent<PlayerScript2D>().menuManager.CloseStats();
                    int required = Int32.Parse(switchData[2..]);
                    int has = 0;
                    string[] info = { SceneManager.GetActiveScene().name, "" , ""};
                    if (switchData[0] == 'S')
                    {
                        if (switchData[1] == 'F')
                        {
                            info[1] = "make";
                            info[2] = "friends";
                            has = item.GetComponent<PlayerScript2D>().dialogueManager.eventScript.fullyTalkedTo;

                        }
                        else if (switchData[1] == 'N')
                        {
                            info[1] = "find";
                            info[2] = "notes";
                            has = item.GetComponent<PlayerScript2D>().dialogueManager.eventScript.collectedNotes;
                        }
                        else if (switchData[1] == 'P')
                        {
                            info[1] = "solve";
                            info[2] = "puzzles";
                            has = item.GetComponent<PlayerScript2D>().dialogueManager.eventScript.completedPuzzlesInScene;
                        }
                    }
                    else if (switchData[0] == 'T')
                    {
                        info[0] = "total";
                        if (switchData[1] == 'F')
                        {
                            info[1] = "make";
                            info[2] = "friends";
                            for (int i = 0; i < item.GetComponent<PlayerScript2D>().menuManager.collection.Count; i++)
                            {
                                has += (int)item.GetComponent<PlayerScript2D>().menuManager.collection[i][1];
                            }
                        }
                        else if (switchData[1] == 'N')
                        {
                            info[1] = "find";
                            info[2] = "notes";
                            for (int i = 0; i < item.GetComponent<PlayerScript2D>().menuManager.collection.Count; i++)
                            {
                                has += (int)item.GetComponent<PlayerScript2D>().menuManager.collection[i][3];
                            }
                        }
                        else if (switchData[1] == 'P')
                        {
                            info[1] = "solve";
                            info[2] = "puzzles";
                            for (int i = 0; i < item.GetComponent<PlayerScript2D>().menuManager.collection.Count; i++)
                            {
                                has += (int)item.GetComponent<PlayerScript2D>().menuManager.collection[i][5];
                            }
                        }
                    }
                    if (has >= required)
                    {
                        transform.parent = affectedObjects[0].GetComponent<PlayerScript2D>().transform;
                        gameObject.SetActive(false);
                    }
                    else
                    {
                        string[] temp = new string[] { "0You:I need to " + info[1] + " " + required + " " + info[2] + " in " + info[0] + " to get rid of this barrier." };
                        item.GetComponent<PlayerScript2D>().dialogueManager.StartDialogue("Player", temp, 0, item.GetComponent<SpriteRenderer>().sprite);
                    }
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
            if (switchData == "")
            {
                UseSwitch();
            }
            else if (switchData == collision.GetComponent<BlockScript>().id.ToString() && (switchEffect != "insert-" || transform.localRotation == collision.transform.localRotation))
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
        if (collision.CompareTag("Sign") && switchType == "floor" && collision.name[9..] == switchData)
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
            if (switchEffect == "spike" || switchEffect == "shift" || switchEffect == "speed")
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
            if (switchEffect == "spike" || switchEffect == "shift" || switchEffect == "speed")
            {
                affectedObjects[0] = collision.gameObject;
                playerOnSpike = false;
                
            }
            if (switchEffect == "speed")
            {
                if (affectedObjects[0].GetComponent<PlayerScript2D>().running)
                {
                    affectedObjects[0].GetComponent<PlayerScript2D>().timeBetweenTiles = 0.15f;
                }
                else
                {
                    affectedObjects[0].GetComponent<PlayerScript2D>().timeBetweenTiles = 0.3f;
                }
            }
        }
        if (collision.CompareTag("Block") && switchType == "floor" && switchEffect == "insert+")
        {
            if (switchData == collision.GetComponent<BlockScript>().id.ToString() && transform.parent.gameObject.GetComponent<ImagePuzzleScript>().piecesLeft > 0)
            {
                transform.parent.gameObject.GetComponent<ImagePuzzleScript>().ChangePiecesLeft(1);
            }
        }
        if (collision.CompareTag("Item") && switchType == "floor" && switchEffect == "insert+")
        {
            if (switchData == collision.name && transform.parent.gameObject.GetComponent<ImagePuzzleScript>().piecesLeft > 0)
            {
                transform.parent.gameObject.GetComponent<ImagePuzzleScript>().ChangePiecesLeft(1);
            }
        }
        if (collision.CompareTag("Sign") && switchType == "floor" && switchEffect == "insert+")
        {
            if (collision.name[9..] == switchData && transform.parent.gameObject.GetComponent<ImagePuzzleScript>().piecesLeft > 0)
            {
                transform.parent.gameObject.GetComponent<ImagePuzzleScript>().ChangePiecesLeft(1);
            }
        }
    }
}
