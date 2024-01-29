using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerScript2D : MonoBehaviour
{
    //if the player is going between tiles
    public bool moving;
    //a vector of the player's current direciton
    public Vector3 direction;
    //Closer to 0, the faster the move speed (default 0.3)
    public bool sameDir;
    public Vector3 prevDir;
    public float timeBetweenTiles;
    //made so the player can turn in spot without moving
    public float holdTimer;
    //current walls next to player
    public bool[] wallTouchList;
    //lets the player start dialogue
    public DialogueManager dialogueManager;
    public InventoryManager invManager;
    public JournalManager journalManager;
    //Tracks current dialogue instance and place in dialogue. dialogueData[0] is name, dialogueData[1] is position
    public GameObject currentTarget;
    public GameObject backpackMenu;

    public bool inMap;
    public bool inJournal;
    public bool inDialogue;
    public bool inInventory;

    public bool aboveTalker;
    public Vector2 spawnPoint;
    static PlayerScript2D instance;

    public string playerName;
    public Sprite[] idleSprites;

    void Awake()
    {
        if (instance == null)
        {
            instance = this; // In first scene, make us the singleton.
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject); // On reload, singleton already set, so destroy duplicate.
        }
    }
    void Start()
    {
        timeBetweenTiles = 0.3f;
        prevDir = Vector3.zero;
    }
    void Update()
    {
        if (!inDialogue && !inMap && !inJournal && !inInventory) //Controls for overworld
        {
            if (!moving)
            {
                GetPlayerMovement();
            }
            if (Input.GetKey(KeyCode.LeftShift))
            {
                timeBetweenTiles = 0.15f;   
            }
            else
            {
                timeBetweenTiles = 0.3f;
            }
            if (Input.GetKeyDown(KeyCode.J))
            {
                journalManager.OpenJournal();
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
            else if(Input.GetKey(KeyCode.E))
            {
                RaycastHit2D hitData = Physics2D.Raycast(transform.position + direction * 0.51f, direction, 0.5f);
                if (hitData.collider != null && hitData.collider.gameObject.tag == "Block")
                {
                    if (!hitData.collider.gameObject.GetComponent<BlockScript>().moving)
                    {
                        hitData.collider.gameObject.GetComponent<BlockScript>().Push(direction);
                    }
                }
                
            }
        }
        else if (inDialogue) //Controls for in dialogue
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (dialogueManager.typing)
                {
                    dialogueManager.typing = false;
                }
                else
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
            
        }
        else if (inInventory)
        {
            aboveTalker = invManager.selectorPos > 2;
            if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.Escape))
            {
                invManager.CloseInventory();
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                //Use
            }
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S))
            {
                invManager.prevSelect = invManager.selectorPos;
                if (invManager.selectorPos > 2)
                {
                    invManager.selectorPos -= 3;
                }
                else
                {
                    invManager.selectorPos += 3;
                }
                invManager.UpdateSelector();
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                invManager.prevSelect = invManager.selectorPos;
                if (invManager.selectorPos == 0 || invManager.selectorPos == 3)
                {
                    invManager.selectorPos += 2;
                }
                else
                {
                    invManager.selectorPos -= 1;
                }
                invManager.UpdateSelector();
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                invManager.prevSelect = invManager.selectorPos;
                if (invManager.selectorPos == 2 || invManager.selectorPos == 5)
                {
                    invManager.selectorPos -= 2;
                }
                else
                {
                    invManager.selectorPos += 1;
                }
                invManager.UpdateSelector();
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (invManager.selectorPos < invManager.inventory.Count)
                {
                    ItemScript itemScript = invManager.inventory[invManager.selectorPos].GetComponent<ItemScript>();
                    dialogueManager.StartDialogue(itemScript.itemName, itemScript.itemLore, 0, GetComponent<SpriteRenderer>().sprite);
                }
                else
                {
                    string[] temp = new string[] { "0You:There's nothing here :(" };
                    dialogueManager.StartDialogue("Player", temp, 0, GetComponent<SpriteRenderer>().sprite);
                }
                
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                bool[] walls = WallChecker();
                bool frontClear = (!walls[0] && direction == Vector3.up) || (!walls[1] && direction == Vector3.left) || (!walls[2] && direction == Vector3.down) || (!walls[3] && direction == Vector3.right);
                if (invManager.selectorPos < invManager.inventory.Count && frontClear)
                {
                    
                    ItemScript itemScript = invManager.inventory[invManager.selectorPos].GetComponent<ItemScript>();
                    itemScript.gameObject.transform.parent = GameObject.Find("LevelObjects").transform;
                    string[] temp = new string[] { "0You:Dropped the " + itemScript.itemName + "." };
                    itemScript.gameObject.SetActive(true);
                    itemScript.transform.position = transform.position + direction;
                    invManager.inventory.Remove(itemScript.gameObject);
                    invManager.OpenInventory();
                    dialogueManager.StartDialogue("Player", temp, 0, GetComponent<SpriteRenderer>().sprite);
                }
                else if (invManager.selectorPos >= invManager.inventory.Count)
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
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (journalManager.selection != journalManager.selectorPos)
                {
                    journalManager.prevSelection = journalManager.selection;
                    journalManager.selection = journalManager.selectorPos;
                    journalManager.UpdateSelector();
                    journalManager.UpdateRightSide();
                }
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                journalManager.CloseJournal();
                NoteWarp();
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                journalManager.prevSelectorPos = journalManager.selectorPos;
                if (journalManager.selectorPos.y > 0)
                {
                    journalManager.selectorPos.y -= 1;
                }
                else
                {
                    journalManager.selectorPos.y += journalManager.textArray.GetLength(0) - 1;
                }
                journalManager.UpdateSelector();
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                journalManager.prevSelectorPos = journalManager.selectorPos;
                if (journalManager.selectorPos.x > 0)
                {
                    journalManager.selectorPos.x -= 1;
                }
                else
                {
                    journalManager.selectorPos.x += journalManager.textArray.GetLength(1) - 1;
                }
                journalManager.UpdateSelector();
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                journalManager.prevSelectorPos = journalManager.selectorPos;
                if (journalManager.selectorPos.y < journalManager.textArray.GetLength(0) - 1)
                {
                    journalManager.selectorPos.y += 1;
                }
                else
                {
                    journalManager.selectorPos.y -= journalManager.textArray.GetLength(0) - 1;
                }
                journalManager.UpdateSelector();
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                journalManager.prevSelectorPos = journalManager.selectorPos;
                if (journalManager.selectorPos.x < journalManager.textArray.GetLength(1) - 1)
                {
                    journalManager.selectorPos.x += 1;
                }
                else
                {
                    journalManager.selectorPos.x -= journalManager.textArray.GetLength(1) - 1;
                }
                journalManager.UpdateSelector();
            }
        }
        else if (inMap)
        {

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
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
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
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
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
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
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
        else
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
        Vector3 start = mover.transform.position;
        float elapsedTime = 0;
        while (elapsedTime < seconds)
        {
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
                dialogueManager.StartDialogue(signScript.dialogueName, signScript.dialogue, signScript.talkCounter, signScript.talkerImage);
                break;
            case "Block":
                if (!target.GetComponent<BlockScript>().moving)
                {
                    target.GetComponent<BlockScript>().Push(direction);
                }
                break;
            case "OnOff":
                target.GetComponent<SwitchScript>().UseSwitch();
                break;
            case "Item":
                if (invManager.inventory.Count == 6)
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
                Debug.Log(target.name);
                break;
        }
        
    }
    public void NoteWarp()
    {
        NoteScript curNote = journalManager.GetNote(Mathf.RoundToInt(journalManager.selection.y) * journalManager.textArray.GetLength(1) + Mathf.RoundToInt(journalManager.selection.x));
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
                SceneManager.LoadScene(curNote.scene);
            }
            
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
}
