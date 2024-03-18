using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using UnityEngine.EventSystems;

public class MoveableObject : MonoBehaviour
{
    public bool[] WallChecker(GameObject obj)
    {
        if (obj.name == "Player" && obj.GetComponent<PlayerScript2D>().ignoreWalls)
        {
            return new bool[] { false, false, false, false };
        }
        RaycastHit2D hitData = Physics2D.Raycast(obj.transform.position + Vector3.up * 0.51f, Vector2.up, 0.5f);
        bool up = hitData.collider != null;
        hitData = Physics2D.Raycast(obj.transform.position + Vector3.left * 0.51f, Vector2.left, 0.5f);
        bool left = hitData.collider != null;
        hitData = Physics2D.Raycast(obj.transform.position + Vector3.down * 0.51f, Vector2.down, 0.5f);
        bool down = hitData.collider != null;
        hitData = Physics2D.Raycast(obj.transform.position + Vector3.right * 0.51f, Vector2.right, 0.5f);
        bool right = hitData.collider != null;
        return new bool[] { up, left, down, right };
    }
    public IEnumerator GridMove(GameObject mover, Vector3 end, float seconds, string anim = "", int moveTimes = 1)
    {
        Vector3 dir = end - mover.transform.position;
        if (mover.name == "Player")
        {
            if (anim != "")
            {
                mover.GetComponent<Animator>().enabled = true;
                if (mover.GetComponent<PlayerScript2D>().timeBetweenTiles == 0.15f)
                {
                    mover.GetComponent<Animator>().speed = 2;
                }
                else
                {
                    mover.GetComponent<Animator>().speed = 1;
                }
                if (mover.GetComponent<PlayerScript2D>().sameDir)
                {
                    mover.GetComponent<Animator>().Play(anim, 0, 0.5f);
                }
                else
                {
                    mover.GetComponent<Animator>().Play(anim, 0, 0);
                }
            }
            if (mover.GetComponent<PlayerScript2D>().follower != null)
            {
                StartCoroutine(GridMove(mover.GetComponent<PlayerScript2D>().follower, transform.position, seconds));
            }
            mover.GetComponent<PlayerScript2D>().moving = true;
        }
        if (mover.CompareTag("Block"))
        {
            mover.GetComponent<BlockScript>().moving = true;
        }
        Vector3 start = mover.transform.position;
        float elapsedTime = 0;
        while (elapsedTime < seconds)
        {
            if (GameObject.Find("Player").GetComponent<PlayerScript2D>().isLoading && mover.name != "Player")
            {
                yield break;
            }
            mover.transform.position = Vector3.Lerp(start, end, (elapsedTime / seconds));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        mover.transform.position = new Vector3(Mathf.Round(mover.transform.position.x), Mathf.Round(mover.transform.position.y), mover.transform.position.z);
        if (mover.name == "Player")
        {
            mover.GetComponent<Animator>().enabled = false;
            mover.GetComponent<PlayerScript2D>().moving = false;
            mover.GetComponent<PlayerScript2D>().noControl = false;
            if (mover.GetComponent<PlayerScript2D>().follower != null && mover.GetComponent<PlayerScript2D>().follower.GetComponent<SignTextScript>() == null)
            {
                mover.GetComponent<PlayerScript2D>().follower.transform.position += new Vector3(0, 0, 1);
                mover.GetComponent<PlayerScript2D>().follower = null;
            }
        }
        
        if (moveTimes > 1)
        {
            bool[] walls = WallChecker(mover);
            if ((!walls[0] && dir == Vector3.up) || (!walls[1] && dir == Vector3.left) || (!walls[2] && dir == Vector3.down) || (!walls[3] && dir == Vector3.right))
            {
                StartCoroutine(GridMove(mover, mover.transform.position + dir, 0.25f, "", moveTimes - 1));
            }
        }

        if (mover.CompareTag("Block") && mover.GetComponent<BlockScript>() != null)
        {
            mover.GetComponent<BlockScript>().moving = false;
            if (mover.GetComponent<BoxCollider2D>() == null)
            {
                if (mover.GetComponent<BlockScript>().type == "control")
                {
                    GameObject.Find("Player").GetComponent<PlayerScript2D>().vcam.Follow = GameObject.Find("Player").transform;
                    GameObject.Find("Player").GetComponent<PlayerScript2D>().invManager.blockControlText.GetComponent<Canvas>().enabled = false;
                    GameObject.Find("Player").GetComponent<PlayerScript2D>().controllingBlock = false;
                }
                
                if (SceneManager.GetActiveScene().name == "ImagePuzzle" && mover.GetComponent<BlockScript>().id > 0 && mover.transform.parent.gameObject.GetComponent<ImagePuzzleScript>().piecesLeft <= 0)
                {
                    mover.transform.parent.gameObject.GetComponent<ImagePuzzleScript>().EndPuzzle();
                }
                mover.transform.position += new Vector3(0, 0, 1);
                Destroy(mover.GetComponent<BlockScript>());
                GameObject.Find("Player").GetComponent<PlayerScript2D>().currentTarget = null;
            }
        }
    }
}
public class PlayerScript2D : MoveableObject
{
    public readonly string[] ALL_PUZZLE_MODES = new string[] { "Blocks", "Items", "Control", "Shuffle+", "Menu", "Blocks+", "Control+", "Swap+", "Items+", "Follow", "Follow+", "Input", "Input+", "Blocks-", "Control-" };
    //if the player is going between tiles
    public bool moving;
    public float spinTimer;
    public bool spinning;
    //a vector of the player's current direciton
    public Vector3 direction;
    //Closer to 0, the faster the move speed (default 0.3)
    public bool sameDir;
    public Vector3 prevDir;
    public bool running;
    public float timeBetweenTiles;
    public float swapy;
    //made so the player can turn in spot without moving
    public float holdTimer;
    //current walls next to player
    public bool[] wallTouchList;
    public bool ignoreWalls;
    //lets the player start dialogue
    public DialogueManager dialogueManager;
    public InventoryManager invManager;
    public JournalManager journalManager;
    public MenuMapManager menuManager;
    //Tracks current dialogue instance and place in dialogue. dialogueData[0] is name, dialogueData[1] is position
    public GameObject currentTarget;
    public GameObject follower = null;
    public GameObject backpackMenu;

    public CinemachineVirtualCamera vcam;
    public Texture2D recentCapture;
    public EventSystem eventSystem;

    public bool isLoading;
    public bool inMap;
    public bool inJournal;
    public bool selectingItem;
    public GameObject selection = null;
    public string input = "gQprk73vInHt51GHQNA8rTtilfRaNiNTxjm00IUBFd3yeplTPJ";
    public bool typing;
    public bool inDialogue;
    public bool inInventory;
    public bool inMenu;
    public bool inOptions;
    public bool inPuzzle;
    public bool inStats;
    public bool controllingBlock;
    public bool noControl;

    public string roomName = "hi ther";

    public bool aboveTalker;
    public Vector2 spawnPoint;
    static PlayerScript2D instance;

    public string playerName;
    public Sprite[] idleSprites;

    //data for puzzle generation
    public Texture2D puzzleImage;
    public Vector2 puzzleDims;
    public string puzzleType;
    public int reward;
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
    public int sillyPress;

    public string oldScene;
    void Awake()
    {
        //Cursor.visible = false;
        if (instance == null)
        {

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            instance = this;
            if (PlayerPrefs.HasKey("name"))
            {
                playerName = PlayerPrefs.GetString("name");
                PlayerPrefs.DeleteAll();
            }
            DontDestroyOnLoad(gameObject);
            spawnPoint = transform.position;
        }
        else if (instance != this)
        {
            Destroy(gameObject); 
        }
    }
    void Start()
    {
        eventSystem.gameObject.SetActive(false);
        oldScene = SceneManager.GetActiveScene().name;
        timeBetweenTiles = 0.3f;
        swapy = 0.15f;
        prevDir = Vector3.zero;
        SwitchSong(SceneManager.GetActiveScene().name);
        dialogueManager.eventScript.RunPastEvents();
        invManager.UpdateInfo();
        input = "gQprk73vInHt51GHQNA8rTtilfRaNiNTxjm00IUBFd3yeplTPJ";
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
        if (!isLoading && !moving && !inInventory && !inJournal && !inMap && !inDialogue && !inOptions && !inMenu && !inPuzzle && !controllingBlock && !inStats && !typing && holdTimer == 0)
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
        if (!isLoading && !inDialogue && !inMap && !inJournal && !inInventory && !inOptions && !inMenu && !inPuzzle && !controllingBlock && !inStats && !noControl && !typing) //Controls for overworld
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
            if (Input.GetKey(KeyCode.LeftShift) && menuManager.optionSelector.selections[1] != new Vector2(1, 1))
            {

                if (menuManager.optionSelector.selections[1] == new Vector2(0, 1))
                {
                    running = true;
                    timeBetweenTiles = 0.15f;
                }
                else if (menuManager.optionSelector.selections[1] == new Vector2(2, 1))
                {
                    running = false;
                    timeBetweenTiles = 0.3f;
                }
            }
            else
            {
                if (menuManager.optionSelector.selections[1] == new Vector2(0, 1))
                {
                    running = false;
                    timeBetweenTiles = 0.3f;
                }
                else if (menuManager.optionSelector.selections[1] == new Vector2(2, 1))
                {
                    running = true;
                    timeBetweenTiles = 0.15f;
                }
            }
            if (Input.GetKeyDown(KeyCode.LeftShift) && menuManager.optionSelector.selections[1] == new Vector2(1, 1))
            {
                running = !running;
                float temp = swapy;
                swapy = timeBetweenTiles;
                timeBetweenTiles = temp;
            }
            if (Input.GetKeyDown(KeyCode.J))
            {
                journalManager.OpenJournal();
            }
            else if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.M))
            {
                menuManager.OpenMenu();
            }
            else if (Input.GetKeyDown(KeyCode.B))
            {
                backpackMenu.SetActive(true);
            }
            else if (Input.GetKeyDown(KeyCode.I))
            {
                invManager.OpenInventory();
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                RaycastHit2D hitData = Physics2D.Raycast(transform.position + direction * 0.51f, direction, 0.5f);
                if (hitData.collider != null)
                {
                    currentTarget = hitData.collider.gameObject;
                    Interact(currentTarget);
                }
                else if (currentTarget != null && currentTarget.CompareTag("Block") && currentTarget.GetComponent<BlockScript>() != null && currentTarget.GetComponent<BlockScript>().type == "swap")
                {
                    Interact(currentTarget);
                }
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                RaycastHit2D hitData = Physics2D.Raycast(transform.position + direction * 0.51f, direction, 0.5f);
                if (hitData.collider != null)
                {
                    currentTarget = hitData.collider.gameObject;
                    Interact(currentTarget, true);
                }
            }
            else if (Input.GetKey(KeyCode.E))
            {
                RaycastHit2D hitData = Physics2D.Raycast(transform.position + direction * 0.51f, direction, 0.5f);
                if (hitData.collider != null && hitData.collider.gameObject.CompareTag("Block"))
                {
                    if (!hitData.collider.gameObject.GetComponent<BlockScript>().moving && hitData.collider.gameObject.GetComponent<BlockScript>().type == "push")
                    {
                        hitData.collider.gameObject.GetComponent<BlockScript>().Push(direction);
                    }
                }

            }
        }
        else if (inDialogue && !selectingItem && !typing) //Controls for in dialogue
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
                    dialogueManager.StartDialogue(itemScript.gameObject.name, itemScript.itemLore, 0, GetComponent<SpriteRenderer>().sprite);
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
                bool[] walls = WallChecker(gameObject);
                bool frontClear = (!walls[0] && direction == Vector3.up) || (!walls[1] && direction == Vector3.left) || (!walls[2] && direction == Vector3.down) || (!walls[3] && direction == Vector3.right);
                if (invManager.selector.selection.y * invManager.selector.width + invManager.selector.selection.x < invManager.inventory.Count && frontClear)
                {

                    ItemScript itemScript = invManager.inventory[Mathf.RoundToInt(invManager.selector.selection.y) * invManager.selector.width + Mathf.RoundToInt(invManager.selector.selection.x)].GetComponent<ItemScript>();
                    itemScript.gameObject.transform.parent = GameObject.Find("LevelObjects").transform;
                    if (itemScript.transform.childCount > 0)
                    {
                        itemScript.gameObject.transform.parent = GameObject.Find("Puzzle Box").transform;
                    }
                    itemScript.gameObject.SetActive(true);
                    itemScript.transform.position = transform.position + direction;
                    invManager.inventory.Remove(itemScript.gameObject);
                    invManager.OpenInventory();
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
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.M))
            {
                menuManager.CloseMenu();
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                invManager.cheese += 1;
                menuManager.CloseMenu();
                menuManager.OpenMenu();
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
                        menuManager.OpenStats();
                        break;
                    case 3:
                        sillyPress += 1;
                        string[] temp = sillyPress switch
                        {
                            1 => new string[] { "0Silly Button: You have pressed the silly button 1 time." },
                            3 => new string[] { "0Silly Button: You have pressed the silly button 3 times. Don't spam if you want to see secrets!" },
                            10 => new string[] { "0Silly Button: You have pressed the silly button 10 times. Keep it up!" },
                            21 => new string[] { "0Silly Button: Whats  9 + 10? 21." },
                            42 => new string[] { "0Silly Button: I couldn't think of anything to put for 42..." },
                            50 => new string[] { "0Silly Button: Halfway to 100!!! You are on a roll!" },
                            69 => new string[] { "0Silly Button: 69 lol." },
                            100 => new string[] { "0Silly Button: Great job reaching 100! Nothing beyond this point!" },
                            101 => new string[] { "0#Silly Button: Or is there..." },
                            151 => new string[] { "0Silly Button: To decode, use a Caesar Cipher with a shift of 16." },
                            171 => new string[] { "0Silly Button: You may have missed something important. Time to reset if you want it!" },
                            999999999 => new string[] { "0Silly Button: No. There is no way you pressed the silly button nine hundred ninety nine million nine hundred ninety nine thousand nine hundred ninety nine times. You are either cheating in some way or looking at the code. If you are looking at the code, read the comment below this." },
                            //Hello its ya boy
                            _ => new string[] { "0Silly Button: You have pressed the silly button " + sillyPress + " times." },
                        };
                        dialogueManager.StartDialogue("SillyButton", temp, 0, dialogueManager.hasHiddenText);
                        break;
                    case 4:
                        transform.position = spawnPoint;
                        break;
                    case 5:
                        Cursor.lockState = CursorLockMode.None;
                        Cursor.visible = true;
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
                    vcam.m_Lens.OrthographicSize = menuManager.optionSelector.selectorPos.x + 3;
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
                                currentTarget.transform.parent.gameObject.GetComponent<ImagePuzzleScript>().ChangePiecesLeft(-1);
                                invManager.UpdateInfo();
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
                                currentTarget = null;

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
                menuManager.puzzleImages[Mathf.RoundToInt(menuManager.puzzleSelector.prevSelectorPos.y) * menuManager.puzzleSelector.width + Mathf.RoundToInt(menuManager.puzzleSelector.prevSelectorPos.x)].rectTransform.sizeDelta = new Vector2(125, 125);
            }
        }
        else if (controllingBlock && currentTarget != null && currentTarget.GetComponent<BoxCollider2D>() != null)
        {
            if (!currentTarget.GetComponent<BlockScript>().moving)
            {

                if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                {
                    holdTimer += Time.deltaTime;
                    direction = Vector3.up;
                    wallTouchList = WallChecker(currentTarget);
                    if (!wallTouchList[0] && holdTimer > 0.1f)
                    {
                        StartCoroutine(GridMove(currentTarget, currentTarget.transform.position + direction, timeBetweenTiles));
                    }
                }
                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                {
                    holdTimer += Time.deltaTime;
                    direction = Vector3.left;
                    wallTouchList = WallChecker(currentTarget);
                    if (!wallTouchList[1] && holdTimer > 0.1f)
                    {
                        StartCoroutine(GridMove(currentTarget, currentTarget.transform.position + direction, timeBetweenTiles));
                    }
                }
                if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                {
                    holdTimer += Time.deltaTime;
                    direction = Vector3.down;
                    wallTouchList = WallChecker(currentTarget);
                    if (!wallTouchList[2] && holdTimer > 0.1f)
                    {
                        StartCoroutine(GridMove(currentTarget, currentTarget.transform.position + direction, timeBetweenTiles));
                    }
                }
                if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                {
                    holdTimer += Time.deltaTime;
                    direction = Vector3.right;
                    wallTouchList = WallChecker(currentTarget);
                    if (!wallTouchList[3] && holdTimer > 0.1f)
                    {
                        StartCoroutine(GridMove(currentTarget, currentTarget.transform.position + direction, timeBetweenTiles));
                    }

                }

            }
            if (!Input.anyKey)
            {
                holdTimer = 0;
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                vcam.Follow = gameObject.transform;
                invManager.blockControlText.GetComponent<Canvas>().enabled = false;
                controllingBlock = false;
            }
            if (Input.GetKeyDown(KeyCode.R) && currentTarget.GetComponent<BlockScript>().rotatable)
            {
                currentTarget.GetComponent<BlockScript>().Rotate();
            }
        }
        else if (inStats)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.M))
            {
                menuManager.CloseStats();
                menuManager.OpenMenu();
            }
            if (Input.GetKeyDown(KeyCode.Equals) && menuManager.puzzlePanel.activeSelf)
            {
                ignoreWalls = !ignoreWalls;
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (menuManager.puzzlePanel.activeSelf)
                {
                    menuManager.puzzlePanel.SetActive(false);
                    menuManager.statsPanel.SetActive(true);
                }
                else
                {
                    menuManager.puzzlePanel.SetActive(true);
                    menuManager.statsPanel.SetActive(false);
                }
                
            }
        }
        else if (typing)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.Escape))
            {
                menuManager.typeBox.ActivateInputField();
            }
            if (Input.GetKeyDown(KeyCode.Return))
            {
                typing = false;
                input = menuManager.typeBox.text;
                menuManager.CloseInput();
                dialogueManager.eventScript.EndEventTrigger();
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
            wallTouchList = WallChecker(gameObject);
            if ((!wallTouchList[0] || (follower != null && follower.transform.position == transform.position + direction)) && holdTimer > 0.1f)
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
            wallTouchList = WallChecker(gameObject);
            if ((!wallTouchList[1] || (follower != null && follower.transform.position == transform.position + direction)) && holdTimer > 0.1f)
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
            wallTouchList = WallChecker(gameObject);
            if ((!wallTouchList[2] || (follower != null && follower.transform.position == transform.position + direction)) && holdTimer > 0.1f)
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
            wallTouchList = WallChecker(gameObject);
            if ((!wallTouchList[3] || (follower != null && follower.transform.position == transform.position + direction)) && holdTimer > 0.1f)
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
    public void Interact(GameObject target, bool rotate = false)
    {
        switch ((target.tag, rotate))
        {
            case ("Sign",false):
                aboveTalker = transform.position.y > target.transform.position.y;
                SignTextScript signScript = target.GetComponent<SignTextScript>();
                dialogueManager.StartDialogue(signScript.name, signScript.dialogue, signScript.talkCounter, signScript.talkerImage);
                break;
            case ("Block",false):
                if (!target.GetComponent<BlockScript>().moving)
                {
                    target.GetComponent<BlockScript>().Push(direction);
                }
                if (target.GetComponent<BlockScript>().id != 0 && menuManager.optionSelector.selections[6].x == 0)
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
            case ("Block", true):
                if (!target.GetComponent<BlockScript>().moving && target.GetComponent<BlockScript>().rotatable && target.GetComponent<BlockScript>().type != "control")
                {
                    target.GetComponent<BlockScript>().Rotate();
                }
                break;
            case ("Switch", false):
                target.GetComponent<SwitchScript>().UseSwitch();
                break;
            case ("Item", false):
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
                    dialogueManager.StartDialogue(itemScript.gameObject.name, itemScript.pickupText, 0, itemScript.itemImage);
                    target.SetActive(false);
                }
                break;
            case ("Cheese", false):
                target.transform.parent = transform;
                invManager.cheese += target.GetComponent<CheeseScript>().amount;
                string[] cheesy = new string[] { "0You:I just found " + target.GetComponent<CheeseScript>().amount + " pieces of cheese."};
                dialogueManager.StartDialogue("Player", cheesy, 0, GetComponent<SpriteRenderer>().sprite);
                target.SetActive(false);
                break;
            case ("Note", false):
                if (journalManager.HasNote(target.GetComponent<NoteScript>().noteId))
                {
                    string[] temp = new string[] { "0Aidan:Uh-oh. You can't have multiple notes with the same Id. Probably should change one of them." };
                    dialogueManager.StartDialogue("Player", temp, 0, GetComponent<SpriteRenderer>().sprite);
                }
                else
                {
                    journalManager.notes.Add(target.GetComponent<NoteScript>());
                    dialogueManager.eventScript.collectedNotes += 1;
                    StartCoroutine(RecordFrame());
                    target.transform.parent = transform;
                    target.SetActive(false);
                }
                break;
            default:
                currentTarget = null;
                break;
        }
        invManager.UpdateInfo();

    }
    public void NoteWarp()
    {
        NoteScript curNote = journalManager.GetNote(Mathf.RoundToInt(journalManager.selector.selection.y) * journalManager.selector.textArray[0].Length + Mathf.RoundToInt(journalManager.selector.selection.x));
        if (curNote == null)
        {
            string[] temp = new string[] { "0You:I can't travel to a place that I haven't found yet." };
            dialogueManager.StartDialogue("Player", temp, 0, GetComponent<SpriteRenderer>().sprite);
        }
        else
        {
            roomName = curNote.noteTitle;
            StopAllCoroutines();
            moving = false;
            transform.position = curNote.pos;
            spawnPoint = curNote.pos;
            invManager.UpdateInfo();
            if(curNote.scene != SceneManager.GetActiveScene().name)
            {
                invManager.RemovePuzzleStuff();
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
            "Forest" => songs[3],
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

    IEnumerator RecordFrame()
    {
        invManager.infoDisplay.enabled = false;
        yield return new WaitForEndOfFrame();
        var texture = ScreenCapture.CaptureScreenshotAsTexture();
        NoteScript note = journalManager.notes[journalManager.notes.IndexOf(currentTarget.GetComponent<NoteScript>())];
        note.image = texture;
        invManager.infoDisplay.enabled = true;
        string[] temp = new string[] { "0You:Found Note #" + (note.noteId + 1).ToString() + " - " + note.noteTitle + ". I should read it in my Journal later." };
        dialogueManager.StartDialogue("Player", temp, 0, GetComponent<SpriteRenderer>().sprite);
    }
    public IEnumerator SwitchScene(string sceneName)
    {
        follower = null;
        oldScene = SceneManager.GetActiveScene().name;
        yield return null;
        isLoading = true;
        var op = SceneManager.LoadSceneAsync(sceneName);
        op.completed += (x) =>
        {
            isLoading = false;
            SwitchSong(sceneName);
            if (finishedPuzzle)
            {
                if (puzzleName != "rand" && puzzleName != "make")
                {
                    completedPuzzles.Add(puzzleName);
                }
                finishedPuzzle = false;
            }
            dialogueManager.eventScript.RunPastEvents();
            invManager.UpdateInfo();
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

