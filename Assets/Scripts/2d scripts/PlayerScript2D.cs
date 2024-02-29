using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;


public class PlayerScript2D : MonoBehaviour
{
    //if the player is going between tiles
    public bool moving;
    public float spinTimer;
    public bool spinning;
    //a vector of the player's current direciton
    public Vector3 direction;
    //Closer to 0, the faster the move speed (default 0.3)
    public bool sameDir;
    public Vector3 prevDir;
    public float timeBetweenTiles;
    public float swapy;
    //made so the player can turn in spot without moving
    public float holdTimer;
    //current walls next to player
    public bool[] wallTouchList;
    //lets the player start dialogue
    public DialogueManager dialogueManager;
    public InventoryManager invManager;
    public JournalManager journalManager;
    public MenuMapManager menuManager;
    //Tracks current dialogue instance and place in dialogue. dialogueData[0] is name, dialogueData[1] is position
    public GameObject currentTarget;
    public GameObject backpackMenu;

    public CinemachineVirtualCamera cam;

    public bool isLoading;
    public bool inMap;
    public bool inJournal;
    public bool selectingItem;
    public GameObject selection = null;
    public bool inDialogue;
    public bool inInventory;
    public bool inMenu;
    public bool inOptions;
    public bool inPuzzle;
    public bool controllingBlock;

    public bool aboveTalker;
    public Vector2 spawnPoint;
    static PlayerScript2D instance;

    public string playerName;
    public Sprite[] idleSprites;

    //data for puzzle generation
    public Texture2D puzzleImage;
    public Vector2 puzzleDims;
    public string puzzleType;
    public GameObject reward;
    public string entryScene;
    public Vector2 entryPos;
    public Vector3 entryDirection;
    public string puzzleName;
    public bool finishedPuzzle;
    public List<string> completedPuzzles;
    public GameObject wall;
    public List<string> oldPuzzles;

    public List<AudioClip> sfx;
    public List<AudioClip> songs;
    public int sillyDude;
    public bool funnyCheck;
    public float funnyTimer;
    void Awake()
    {
        //Cursor.visible = false;
        if (instance == null)
        {
            instance = this; 
            if (PlayerPrefs.HasKey("name"))
            {
                playerName = PlayerPrefs.GetString("name");
                PlayerPrefs.DeleteAll();
            }
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject); 
        }
    }
    void Start()
    {
        timeBetweenTiles = 0.3f;
        swapy = 0.15f;
        prevDir = Vector3.zero;
        SwitchSong(SceneManager.GetActiveScene().name);
        dialogueManager.eventScript.RunPastEvents();
    }
    void Update()
    {
        if (sillyDude > 0)
        {
            funnyTimer += Time.deltaTime;
        }
        if (funnyCheck)
        {
            funnyTimer = 0;
            sillyDude += 1;
            funnyCheck = false;
        }
        if (funnyTimer > 2.5f)
        {
            sillyDude = 0;
        }
        if (sillyDude > 10 && transform.position != new Vector3(23, 1, 0))
        {
            StopAllCoroutines();
            moving = false;
            GetComponent<Animator>().enabled = false;
            StartCoroutine(SwitchScene("PuzzleTest"));
            transform.position = new Vector3(23, 1, 0);
            sillyDude = 0;
        }
        if (!isLoading && !moving && !inInventory && !inJournal && !inMap && !inDialogue && !inOptions && !inMenu && !inPuzzle && !controllingBlock && holdTimer == 0)
        {
            spinTimer += Time.deltaTime;
        }
        else
        {
            spinTimer = 0;
            if (spinning)
            {
                StopCoroutine(IdleSpin());
                spinning = false;
            }
            
        }
        if (spinTimer > 10 && !spinning)
        {
            StartCoroutine(IdleSpin());
        }
        if (!isLoading && !inDialogue && !inMap && !inJournal && !inInventory && !inOptions && !inMenu && !inPuzzle && !controllingBlock) //Controls for overworld
        {
            if (!moving)
            {
                if (direction == Vector3.up)
                {
                    GetComponent<SpriteRenderer>().sprite = idleSprites[0];
                }
                else if (direction == Vector3.left)
                {
                    GetComponent<SpriteRenderer>().sprite = idleSprites[1];
                }
                else if (direction == Vector3.down)
                {
                    GetComponent<SpriteRenderer>().sprite = idleSprites[2];
                }
                else if (direction == Vector3.right)
                {
                    GetComponent<SpriteRenderer>().sprite = idleSprites[3];
                }
                GetPlayerMovement();
            }
            if (Input.GetKey(KeyCode.LeftShift) && menuManager.optionSelector.selections[1] != new Vector2(1,1))
            {
                if (menuManager.optionSelector.selections[1] == new Vector2(0,1))
                {
                    timeBetweenTiles = 0.15f;
                }
                else if (menuManager.optionSelector.selections[1] == new Vector2(2, 1))
                {
                    timeBetweenTiles = 0.3f;
                } 
            }
            else
            {
                if (menuManager.optionSelector.selections[1] == new Vector2(0, 1))
                {
                    timeBetweenTiles = 0.3f;
                }
                else if (menuManager.optionSelector.selections[1] == new Vector2(2, 1))
                {
                    timeBetweenTiles = 0.15f;
                }
            }
            if (Input.GetKeyDown(KeyCode.LeftShift) && menuManager.optionSelector.selections[1] == new Vector2(1, 1))
            {
                float temp = swapy;
                swapy = timeBetweenTiles;
                timeBetweenTiles = temp;
            }
            if (Input.GetKeyDown(KeyCode.J))
            {
                journalManager.OpenJournal();
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                menuManager.OpenMenu();
            }
            if (Input.GetKeyDown(KeyCode.B))
            {
                backpackMenu.SetActive(true);
            }
            if (Input.GetKeyDown(KeyCode.I))
            {
                invManager.OpenInventory();
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                //map
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                RaycastHit2D hitData = Physics2D.Raycast(transform.position + direction * 0.51f, direction, 0.5f);
                if (hitData.collider != null)
                {
                    currentTarget = hitData.collider.gameObject;
                    Interact(currentTarget);
                }
            }
            else if (Input.GetKey(KeyCode.E))
            {
                RaycastHit2D hitData = Physics2D.Raycast(transform.position + direction * 0.51f, direction, 0.5f);
                if (hitData.collider != null && hitData.collider.gameObject.CompareTag("Block"))
                {
                    if (!hitData.collider.gameObject.GetComponent<BlockScript>().moving && hitData.collider.gameObject.GetComponent<BlockScript>().type == "push")
                    {
                        GetComponent<AudioSource>().PlayOneShot(sfx[0]);
                        hitData.collider.gameObject.GetComponent<BlockScript>().Push(direction);
                    }
                }

            }
        }
        else if (inDialogue && !selectingItem) //Controls for in dialogue
        {
            if (Input.GetKeyDown(KeyCode.Space) && menuManager.optionSelector.selections[3] != new Vector2(1, 3))
            {
                if (dialogueManager.typing && menuManager.optionSelector.selections[3] != new Vector2(2, 3))
                {
                    dialogueManager.typing = false;
                }
                else if (dialogueManager.typing == false)
                {
                    if (currentTarget != null)
                    {
                        aboveTalker = transform.position.y > currentTarget.transform.position.y;
                    }
                    dialogueManager.sentences.RemoveAt(0);
                    dialogueManager.eventScript.EndEventTrigger();
                    if (!dialogueManager.changed)
                    {
                        dialogueManager.DisplayNextSentence();
                    }
                    dialogueManager.changed = false;
                }
            }
            if (Input.GetKey(KeyCode.Space) && menuManager.optionSelector.selections[3] == new Vector2(1, 3))
            {
                dialogueManager.typing = false;
                if (currentTarget != null)
                {
                    aboveTalker = transform.position.y > currentTarget.transform.position.y;
                }
                dialogueManager.sentences.RemoveAt(0);
                dialogueManager.eventScript.EndEventTrigger();
                if (!dialogueManager.changed)
                {
                    dialogueManager.DisplayNextSentence();
                }
                dialogueManager.changed = false;
            }

        }
        else if (inInventory)
        {
            aboveTalker = invManager.selector.selectorPos.y != 0;
            if ((Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.Escape)) && !selectingItem)
            {
                invManager.CloseInventory();
            }
            GetSelectorMovement(invManager.selector);
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                invManager.selector.prevSelection = invManager.selector.selection;
                invManager.selector.selection = invManager.selector.selectorPos;
                invManager.selector.UpdateSelector();
            }
            if (Input.GetKeyDown(KeyCode.E) && !selectingItem)
            {
                if (invManager.selector.selection.y * invManager.selector.width + invManager.selector.selection.x < invManager.inventory.Count)
                {
                    ItemScript itemScript = invManager.inventory[Mathf.RoundToInt(invManager.selector.selection.y) * invManager.selector.width + Mathf.RoundToInt(invManager.selector.selection.x)].GetComponent<ItemScript>();
                    dialogueManager.StartDialogue(itemScript.itemName, itemScript.itemLore, 0, GetComponent<SpriteRenderer>().sprite);
                }
                else
                {
                    string[] temp = new string[] { "0You:There's nothing here :(" };
                    dialogueManager.StartDialogue("Player", temp, 0, GetComponent<SpriteRenderer>().sprite);
                }
            }
            if (Input.GetKeyDown(KeyCode.E) && selectingItem)
            {
                selectingItem = false;
                if (invManager.selector.selection.y * invManager.selector.width + invManager.selector.selection.x < invManager.inventory.Count)
                {
                    selection = invManager.inventory[Mathf.RoundToInt(invManager.selector.selection.y) * invManager.selector.width + Mathf.RoundToInt(invManager.selector.selection.x)];
                }
                else
                {
                    selection = currentTarget;
                }
                invManager.CloseInventory();
                dialogueManager.eventScript.EndEventTrigger();
            }
            if (Input.GetKeyDown(KeyCode.R) && !selectingItem)
            {
                bool[] walls = WallChecker();
                bool frontClear = (!walls[0] && direction == Vector3.up) || (!walls[1] && direction == Vector3.left) || (!walls[2] && direction == Vector3.down) || (!walls[3] && direction == Vector3.right);
                if (invManager.selector.selection.y * invManager.selector.width + invManager.selector.selection.x < invManager.inventory.Count && frontClear)
                {

                    ItemScript itemScript = invManager.inventory[Mathf.RoundToInt(invManager.selector.selection.y) * invManager.selector.width + Mathf.RoundToInt(invManager.selector.selection.x)].GetComponent<ItemScript>();
                    itemScript.gameObject.transform.parent = GameObject.Find("LevelObjects").transform;
                    string[] temp = new string[] { "0You:Dropped the " + itemScript.itemName + "." };
                    itemScript.gameObject.SetActive(true);
                    itemScript.transform.position = transform.position + direction;
                    invManager.inventory.Remove(itemScript.gameObject);
                    invManager.OpenInventory();
                    dialogueManager.StartDialogue("Player", temp, 0, GetComponent<SpriteRenderer>().sprite);
                }
                else if (invManager.selector.selection.y * invManager.selector.width + invManager.selector.selection.x >= invManager.inventory.Count)
                {
                    string[] temp = new string[] { "0You:Can't drop nothing :(" };
                    dialogueManager.StartDialogue("Player", temp, 0, GetComponent<SpriteRenderer>().sprite);
                }
                else
                {
                    string[] temp = new string[] { "0You:Not enough space to drop this. I should move to a better spot." };
                    dialogueManager.StartDialogue("Player", temp, 0, GetComponent<SpriteRenderer>().sprite);
                }

            }
        }
        else if (inJournal) //yeah you get it
        {
            if (Input.GetKeyDown(KeyCode.J) || Input.GetKeyDown(KeyCode.Escape))
            {
                journalManager.CloseJournal();
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                journalManager.CloseJournal();
                NoteWarp();
            }
            GetSelectorMovement(journalManager.selector);
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                journalManager.selector.prevSelection = journalManager.selector.selection;
                journalManager.selector.selection = journalManager.selector.selectorPos;
                journalManager.selector.UpdateSelector();
                journalManager.UpdateRightSide();
            }
        }

        else if (inMenu)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                menuManager.CloseMenu();
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                menuManager.CloseMenu();
                switch (Mathf.RoundToInt(menuManager.menuSelector.selectorPos.y))
                {
                    case 0:
                        break;
                    case 1:
                        menuManager.OpenOptions();
                        break;
                    case 2:
                        invManager.cheese += 1;
                        menuManager.OpenMenu();
                        break;
                    case 3:  
                        SceneManager.LoadScene("TitleScreen");
                        Destroy(gameObject);
                        break;
                }
            }
            GetSelectorMovement(menuManager.menuSelector);
        }
        else if (inOptions)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                menuManager.CloseOptions();
                menuManager.OpenMenu();
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (menuManager.optionSelector.selections[Mathf.RoundToInt(menuManager.optionSelector.selectorPos.y)] != menuManager.optionSelector.selectorPos)
                {
                    menuManager.optionSelector.prevSelections[Mathf.RoundToInt(menuManager.optionSelector.selectorPos.y)] = menuManager.optionSelector.selections[Mathf.RoundToInt(menuManager.optionSelector.selectorPos.y)];
                    menuManager.optionSelector.selections[Mathf.RoundToInt(menuManager.optionSelector.selectorPos.y)] = menuManager.optionSelector.selectorPos;
                    menuManager.optionSelector.UpdateSelector();
                }
                if (menuManager.optionSelector.selectorPos == Vector2.zero)
                {
                    menuManager.CloseOptions();
                    menuManager.OpenMenu();
                }
                if (menuManager.optionSelector.selectorPos.y == 4)
                {
                    cam.m_Lens.OrthographicSize = menuManager.optionSelector.selectorPos.x + 3;
                }
                if (menuManager.optionSelector.selectorPos.y == 5)
                {
                    GetComponent<AudioSource>().volume = menuManager.optionSelector.selectorPos.x * 0.25f;
                }
            }
            GetSelectorMovement(menuManager.optionSelector);
        }
        else if (inPuzzle)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                menuManager.ClosePuzzle();
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                for (int i = 0; i < currentTarget.transform.parent.childCount; i++)
                {
                    if (currentTarget.transform.parent.GetChild(i).name.Equals("FloorSwitch(Clone)"))
                    {
                        if (Int32.Parse(currentTarget.transform.parent.GetChild(i).GetComponent<SwitchScript>().switchData) == menuManager.puzzleSelector.selectorPos.y * menuManager.puzzleSelector.width + menuManager.puzzleSelector.selectorPos.x + 1)
                        {
                            currentTarget.transform.position = currentTarget.transform.parent.GetChild(i).position;
                            currentTarget.transform.position += new Vector3(0, 0, -0.5f);
                            if (currentTarget.GetComponent<BlockScript>().id == Int32.Parse(currentTarget.transform.parent.GetChild(i).GetComponent<SwitchScript>().switchData))
                            {
                                currentTarget.transform.parent.gameObject.GetComponent<ImagePuzzleScript>().piecesLeft -= 1;
                                currentTarget.transform.position += new Vector3(0, 0, 0.5f);
                                menuManager.puzzleImages[currentTarget.GetComponent<BlockScript>().id - 1].sprite = currentTarget.GetComponent<SpriteRenderer>().sprite;
                                menuManager.puzzleImages[currentTarget.GetComponent<BlockScript>().id - 1].color = Color.white;
                                menuManager.puzzleSelector.textArray[(currentTarget.GetComponent<BlockScript>().id - 1) / menuManager.puzzleSelector.width][(currentTarget.GetComponent<BlockScript>().id - 1) % menuManager.puzzleSelector.width].text = "";
                                Destroy(currentTarget.GetComponent<BoxCollider2D>());
                                Destroy(currentTarget.GetComponent<BlockScript>());
                                Destroy(currentTarget.transform.GetChild(0).gameObject);
                                Destroy(currentTarget.transform.parent.GetChild(i).gameObject);
                                if (currentTarget.transform.parent.gameObject.GetComponent<ImagePuzzleScript>().piecesLeft == 0)
                                {
                                    currentTarget.transform.parent.gameObject.GetComponent<ImagePuzzleScript>().EndPuzzle();
                                }
                                
                            }
                            break;
                        }
                    }
                }
                menuManager.ClosePuzzle();
            }
            GetSelectorMovement(menuManager.puzzleSelector);
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                menuManager.puzzleImages[Mathf.RoundToInt(menuManager.puzzleSelector.selectorPos.y) * menuManager.puzzleSelector.width + Mathf.RoundToInt(menuManager.puzzleSelector.selectorPos.x)].rectTransform.sizeDelta = new Vector2(100, 100);
                menuManager.puzzleImages[Mathf.RoundToInt(menuManager.puzzleSelector.prevSelectorPos.y) * menuManager.puzzleSelector.width + Mathf.RoundToInt(menuManager.puzzleSelector.prevSelectorPos.x)].rectTransform.sizeDelta = new Vector2(150, 150);
            }
        }
        else if (controllingBlock && !currentTarget.GetComponent<BlockScript>().moving)
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                holdTimer += Time.deltaTime;
                direction = Vector3.up;
                wallTouchList = currentTarget.GetComponent<BlockScript>().WallChecker();
                if (!wallTouchList[0] && holdTimer > 0.1f)
                {
                    StartCoroutine(GridMove(currentTarget, currentTarget.transform.position + direction, timeBetweenTiles));
                }
            }
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                holdTimer += Time.deltaTime;
                direction = Vector3.left;
                wallTouchList = currentTarget.GetComponent<BlockScript>().WallChecker();
                if (!wallTouchList[1] && holdTimer > 0.1f)
                {
                    StartCoroutine(GridMove(currentTarget, currentTarget.transform.position + direction, timeBetweenTiles));
                }
            }
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                holdTimer += Time.deltaTime;
                direction = Vector3.down;
                wallTouchList = currentTarget.GetComponent<BlockScript>().WallChecker();
                if (!wallTouchList[2] && holdTimer > 0.1f)
                {
                    StartCoroutine(GridMove(currentTarget, currentTarget.transform.position + direction, timeBetweenTiles));
                }
            }
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                holdTimer += Time.deltaTime;
                direction = Vector3.right;
                wallTouchList = currentTarget.GetComponent<BlockScript>().WallChecker();
                if (!wallTouchList[3] && holdTimer > 0.1f)
                {
                    StartCoroutine(GridMove(currentTarget, currentTarget.transform.position + direction, timeBetweenTiles));
                }
                
            }
            if (!Input.anyKey)
            {
                holdTimer = 0;
            }
            if(Input.GetKeyDown(KeyCode.E))
            {
                cam.Follow = gameObject.transform;
                invManager.blockControlText.GetComponent<Canvas>().enabled = false;
                controllingBlock = false;
            }
        }
        
    }
    public void GetSelectorMovement(Selector selector)
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            selector.prevSelectorPos = selector.selectorPos;
            if (selector.selectorPos.y > 0)
            {
                selector.selectorPos.y -= 1;
            }
            else
            {
                selector.selectorPos.y += selector.textArray.Length - 1;
            }
            if (selector.textArray[Mathf.RoundToInt(selector.selectorPos.y)].Length - 1 < selector.selectorPos.x)
            {
                selector.selectorPos.x = selector.textArray[Mathf.RoundToInt(selector.selectorPos.y)].Length - 1;
            }
            selector.UpdateSelector();
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            selector.prevSelectorPos = selector.selectorPos;
            if (selector.selectorPos.x > 0)
            {
                selector.selectorPos.x -= 1;
            }
            else
            {
                selector.selectorPos.x += selector.textArray[Mathf.RoundToInt(selector.selectorPos.y)].Length - 1;
            }
            selector.UpdateSelector();
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            selector.prevSelectorPos = selector.selectorPos;
            if (selector.selectorPos.y < selector.textArray.Length - 1)
            {
                selector.selectorPos.y += 1;
            }
            else
            {
                selector.selectorPos.y -= selector.textArray.Length - 1;
            }
            if (selector.textArray[Mathf.RoundToInt(selector.selectorPos.y)].Length - 1 < selector.selectorPos.x)
            {
                selector.selectorPos.x = selector.textArray[Mathf.RoundToInt(selector.selectorPos.y)].Length - 1;
            }
            selector.UpdateSelector();
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            selector.prevSelectorPos = selector.selectorPos;
            
            if (selector.selectorPos.x < selector.textArray[Mathf.RoundToInt(selector.selectorPos.y)].Length - 1)
            {
                selector.selectorPos.x += 1;
            }
            else
            {
                selector.selectorPos.x -= selector.textArray[Mathf.RoundToInt(selector.selectorPos.y)].Length - 1;
            }
            selector.UpdateSelector();
        }
    }
    public void GetPlayerMovement()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            holdTimer += Time.deltaTime;
            direction = Vector3.up;
            wallTouchList = WallChecker();
            if (!wallTouchList[0] && holdTimer > 0.1f)
            {
                sameDir = prevDir == Vector3.up && !sameDir;
                prevDir = direction;
                StartCoroutine(GridMove(gameObject,transform.position + direction, timeBetweenTiles, "Up"));
            }
            else
            {
                GetComponent<SpriteRenderer>().sprite = idleSprites[0];
            }
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            holdTimer += Time.deltaTime;
            direction = Vector3.left;
            wallTouchList = WallChecker();
            if (!wallTouchList[1] && holdTimer > 0.1f)
            {
                sameDir = prevDir == Vector3.left && !sameDir;
                prevDir = direction;
                StartCoroutine(GridMove(gameObject,transform.position + direction, timeBetweenTiles, "Left"));
            }
            else
            {
                GetComponent<SpriteRenderer>().sprite = idleSprites[1];
            }
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            holdTimer += Time.deltaTime;
            direction = Vector3.down;
            wallTouchList = WallChecker();
            if (!wallTouchList[2] && holdTimer > 0.1f)
            {
                sameDir = prevDir == Vector3.down && !sameDir;
                prevDir = direction;
                StartCoroutine(GridMove(gameObject,transform.position + direction, timeBetweenTiles, "Down"));
            }
            else
            {
                GetComponent<SpriteRenderer>().sprite = idleSprites[2];
            }
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            holdTimer += Time.deltaTime;
            direction = Vector3.right;
            wallTouchList = WallChecker();
            if (!wallTouchList[3] && holdTimer > 0.1f)
            {
                sameDir = prevDir == Vector3.right && !sameDir;
                prevDir = direction;
                StartCoroutine(GridMove(gameObject,transform.position + direction, timeBetweenTiles, "Right"));
            }
            else
            {
                GetComponent<SpriteRenderer>().sprite = idleSprites[3];
            }
        }
        if(!Input.anyKey)
        {
            holdTimer = 0;
        }
    }
    public IEnumerator GridMove(GameObject mover, Vector3 end, float seconds, string anim = "")
    {
        if (mover.name == "Player")
        {
            GetComponent<Animator>().enabled = true;
            if (timeBetweenTiles == 0.15f)
            {
                GetComponent<Animator>().speed = 2;
            }
            else
            {
                GetComponent<Animator>().speed = 1;
            }
            if (sameDir)
            {
                GetComponent<Animator>().Play(anim, 0, 0.5f);
            }
            else
            {
                GetComponent<Animator>().Play(anim, 0, 0);
            }
            moving = true;
        }
        if (mover.CompareTag("Block"))
        {
            mover.GetComponent<BlockScript>().moving = true;
        }
        Vector3 start = mover.transform.position;
        float elapsedTime = 0;
        while (elapsedTime < seconds)
        {
            if (isLoading && mover.name != "Player")
            {
                yield break;
            }
            mover.transform.position = Vector3.Lerp(start, end, (elapsedTime / seconds));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        mover.transform.position = new Vector3(Mathf.Round(mover.transform.position.x), Mathf.Round(mover.transform.position.y), 0);
        if (mover.name == "Player")
        {
            GetComponent<Animator>().enabled = false;
            moving = false;
        }
        if (mover.CompareTag("Block"))
        {
            mover.GetComponent<BlockScript>().moving = false;
            if (mover.GetComponent<BlockScript>().inserted)
            {
                if (mover.GetComponent<BlockScript>().id > 0)
                {
                    mover.transform.parent.gameObject.GetComponent<ImagePuzzleScript>().piecesLeft -= 1;
                }
                mover.transform.position += new Vector3(0, 0, 1);
                Destroy(mover.GetComponent<BoxCollider2D>());
                Destroy(mover.GetComponent<BlockScript>());
                Destroy(mover.transform.GetChild(0).gameObject);
                cam.Follow = gameObject.transform;
                invManager.blockControlText.GetComponent<Canvas>().enabled = false;
                controllingBlock = false;
                if (mover.GetComponent<BlockScript>().id > 0 && mover.transform.parent.gameObject.GetComponent<ImagePuzzleScript>().piecesLeft == 0)
                {
                    mover.transform.parent.gameObject.GetComponent<ImagePuzzleScript>().EndPuzzle();
                }
            }
            else if (mover.GetComponent<BlockScript>().id > 0 && mover.transform.parent.gameObject.GetComponent<ImagePuzzleScript>().piecesLeft == -1)
            {
                mover.transform.position += new Vector3(0, 0, 1);
                cam.Follow = gameObject.transform;
                invManager.blockControlText.GetComponent<Canvas>().enabled = false;
                controllingBlock = false;
            }
        }
    }
    public IEnumerator IdleSpin()
    {
        spinning = true;
        
        while (spinning)
        {
            if (direction == Vector3.up)
            {
                direction = Vector3.left;
            }
            else if (direction == Vector3.left)
            {
                direction = Vector3.down;
            }
            else if (direction == Vector3.down)
            {
                direction = Vector3.right;

            }
            else if (direction == Vector3.right)
            {
                direction = Vector3.up;
            }
            yield return new WaitForSeconds(0.5f);
            
        }
    }
    public bool[] WallChecker()
    {
        RaycastHit2D hitData = Physics2D.Raycast(transform.position + Vector3.up * 0.51f, Vector2.up, 0.5f);
        bool up = hitData.collider != null;
        hitData = Physics2D.Raycast(transform.position + Vector3.left * 0.51f, Vector2.left, 0.5f);
        bool left = hitData.collider != null;
        hitData = Physics2D.Raycast(transform.position + Vector3.down * 0.51f, Vector2.down, 0.5f);
        bool down = hitData.collider != null;
        hitData = Physics2D.Raycast(transform.position + Vector3.right * 0.51f, Vector2.right, 0.5f);
        bool right = hitData.collider != null;
        return new bool[] { up, left, down, right };
    }

    public void Interact(GameObject target)
    {
        switch (target.tag)
        {
            case "Sign":
                aboveTalker = transform.position.y > target.transform.position.y;
                SignTextScript signScript = target.GetComponent<SignTextScript>();
                dialogueManager.StartDialogue(signScript.name, signScript.dialogue, signScript.talkCounter, signScript.talkerImage);
                break;
            case "Block":
                if (!target.GetComponent<BlockScript>().moving)
                {
                    GetComponent<AudioSource>().PlayOneShot(sfx[0]);
                    target.GetComponent<BlockScript>().Push(direction);
                }
                if (target.GetComponent<BlockScript>().id != 0)
                {
                    for(int i = 0; i < target.transform.parent.childCount; i++)
                    {
                        if (target.transform.parent.GetChild(i).name.Equals("FloorSwitch(Clone)"))
                        {
                            if (target.transform.parent.GetChild(i).GetComponent<SwitchScript>().switchData == target.GetComponent<BlockScript>().id.ToString() && target.transform.parent.GetChild(i).GetComponent<SpriteRenderer>().color == Color.cyan)
                            {
                                target.transform.parent.GetChild(i).GetComponent<SpriteRenderer>().color = Color.red;
                            }
                            else if(target.transform.parent.GetChild(i).GetComponent<SwitchScript>().switchData != target.GetComponent<BlockScript>().id.ToString() && target.transform.parent.GetChild(i).GetComponent<SpriteRenderer>().color == Color.red)
                            {
                                target.transform.parent.GetChild(i).GetComponent<SpriteRenderer>().color = Color.cyan;
                            }
                        }
                    }
                }
                break;
            case "OnOff":
                target.GetComponent<SwitchScript>().UseSwitch();
                break;
            case "Item":
                if (invManager.inventory.Count == invManager.selector.width * invManager.selector.height)
                {
                    string[] temp = new string[] { "0You:I don't have any room left to pick items up. I should drop or use one." };
                    dialogueManager.StartDialogue("Player", temp, 0, GetComponent<SpriteRenderer>().sprite);
                }
                else
                {
                    target.transform.parent = transform;
                    invManager.inventory.Add(target);
                    ItemScript itemScript = target.GetComponent<ItemScript>();
                    dialogueManager.StartDialogue(itemScript.itemName, itemScript.pickupText, 0, itemScript.itemImage);
                    target.SetActive(false);
                }
                break;
            case "Cheese":
                target.transform.parent = transform;
                invManager.cheese += target.GetComponent<CheeseScript>().amount;
                string[] cheesy = new string[] { "0You:I just found " + target.GetComponent<CheeseScript>().amount + " pieces of cheese."};
                dialogueManager.StartDialogue("Player", cheesy, 0, GetComponent<SpriteRenderer>().sprite);
                target.SetActive(false);
                break;
            case "Note":
                if (journalManager.HasNote(target.GetComponent<NoteScript>().noteId))
                {
                    string[] temp = new string[] { "0Aidan:Uh-oh. You can't have multiple notes with the same Id. Probably should change one of them." };
                    dialogueManager.StartDialogue("Player", temp, 0, GetComponent<SpriteRenderer>().sprite);
                }
                else
                {
                    journalManager.notes.Add(target.GetComponent<NoteScript>());
                    NoteScript note = journalManager.notes[journalManager.notes.IndexOf(target.GetComponent<NoteScript>())];
                    target.transform.parent = transform;
                    target.SetActive(false);
                    string[] temp = new string[] { "0You:Found Note #" + (note.noteId + 1).ToString() + " - " + note.noteTitle + ". I should read it in my Journal later." };
                    dialogueManager.StartDialogue("Player", temp, 0, GetComponent<SpriteRenderer>().sprite);
                }
                break;
            default:
                currentTarget = null;
                break;
        }
        
    }
    public void NoteWarp()
    {
        NoteScript curNote = journalManager.GetNote(Mathf.RoundToInt(journalManager.selector.selection.y) * journalManager.selector.textArray[0].Length + Mathf.RoundToInt(journalManager.selector.selection.x));
        if (curNote == null)
        {
            string[] temp = new string[] { "0You:I can't travel to a place that I haven't found yet." };
            dialogueManager.StartDialogue("Player", temp, 0, GetComponent<SpriteRenderer>().sprite);
        }
        else if (curNote.scene == SceneManager.GetActiveScene().name && Vector3.Distance(transform.position, curNote.pos) <= 10)
        {
            string[] temp = new string[] { "0You:I'm too close to where I found the note to justify traveling there." };
            dialogueManager.StartDialogue("Player", temp, 0, GetComponent<SpriteRenderer>().sprite);
        }
        else
        {
            StopAllCoroutines();
            moving = false;
            transform.position = curNote.pos;
            spawnPoint = curNote.pos;
            if(curNote.scene != SceneManager.GetActiveScene().name)
            {
                StartCoroutine(SwitchScene(curNote.scene));
            }
            
        }
    }
    public void SwitchSong(string scene)
    {
        AudioClip prevSong = GetComponent<AudioSource>().clip;
        GetComponent<AudioSource>().clip = scene switch
        {
            "ImagePuzzle" => songs[1],
            "PuzzleTest" => songs[0],
            "Castle" => songs[2],
            _ => songs[0],
        };
        if (prevSong != GetComponent<AudioSource>().clip)
        {
            GetComponent<AudioSource>().Play();
        }
    }
    public bool HasItem(string name)
    {
        foreach (GameObject g in invManager.inventory)
        {
            if (g.GetComponent<ItemScript>().itemName == name)
            {
                return true;
            }
        }
        return false;
    }
    public GameObject GetItem(string name)
    {
        foreach (GameObject g in invManager.inventory)
        {
            if (g.GetComponent<ItemScript>().itemName == name)
            {
                return g;
            }
        }
        return null;
    }

    public IEnumerator SwitchScene(string sceneName)
    {
        yield return null;
        isLoading = true;
        var op = SceneManager.LoadSceneAsync(sceneName);
        op.completed += (x) =>
        {
            isLoading = false;
            SwitchSong(sceneName);
            dialogueManager.eventScript.RunPastEvents();
            if (finishedPuzzle)
            {
                if (puzzleName != "rand")
                {
                    completedPuzzles.Add(puzzleName);
                }
                finishedPuzzle = false;
            }
            foreach (string puz in completedPuzzles)
            {
                GameObject puzzle = GameObject.Find(puz);
                if (puzzle != null)
                {
                    GameObject newWall = Instantiate(wall, new Vector2(Mathf.RoundToInt(puzzle.transform.position.x), Mathf.RoundToInt(puzzle.transform.position.y)), Quaternion.identity);
                    newWall.transform.localScale = new Vector3(Mathf.RoundToInt(puzzle.transform.localScale.x + 0.5f), Mathf.RoundToInt(puzzle.transform.localScale.y + 0.5f), 0);
                    if (newWall.transform.localScale.x % 2 == 0)
                    {
                        newWall.transform.localScale = new Vector3(newWall.transform.localScale.x + 1, 1, 0);
                    }
                    else if (newWall.transform.localScale.y % 2 == 0)
                    {
                        newWall.transform.localScale = new Vector3(1, newWall.transform.localScale.y + 1, 0);
                    }
                    Destroy(puzzle);
                }
            }
        };
        funnyCheck = true;
        
    }
}
