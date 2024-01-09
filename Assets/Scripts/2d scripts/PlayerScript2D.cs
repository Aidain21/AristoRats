using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript2D : MonoBehaviour
{
    //if the player is going between tiles
    public bool moving;
    //a vector of the player's current direciton
    public Vector3 direction;
    //Closer to 0, the faster the move speed (default 0.3)
    public float timeBetweenTiles;
    //made so the player can turn in spot without moving
    public float holdTimer;
    //current walls next to player
    public bool[] wallTouchList;
    //we can remove this once we have a character with different facing directions.
    public GameObject directionTracker;
    //lets the player start dialogue
    public DialogueManager dialogueManager;
    //Tracks current dialogue instance and place in dialogue. dialogueData[0] is name, dialogueData[1] is position
    public GameObject currentTarget;

    public bool inMap;
    public bool inJournal;
    public bool inDialogue;
    public bool inInventory;

    public bool aboveTalker;

    void Start()
    {
        directionTracker = transform.GetChild(0).gameObject;
        timeBetweenTiles = 0.3f;
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
                //journal
            }
            if (Input.GetKeyDown(KeyCode.I))
            {
                //inventory
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
                    aboveTalker = transform.position.y > currentTarget.transform.position.y;
                    dialogueManager.DisplayNextSentence();
                }
                
            }
            
        }
        else if (inJournal) //yeah you get it
        {

        }
        else if (inMap)
        {

        }
        else if (inInventory)
        {

        }
    }
    public void GetPlayerMovement()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            holdTimer += Time.deltaTime;
            direction = Vector3.up;
            directionTracker.transform.localPosition = new Vector3(0, 0.25f, -1);
            wallTouchList = WallChecker();
            if (!wallTouchList[0] && holdTimer > 0.1f)
            {
                StartCoroutine(GridMove(gameObject,transform.position + direction, timeBetweenTiles));
            }
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            holdTimer += Time.deltaTime;
            direction = Vector3.left;
            directionTracker.transform.localPosition = new Vector3(-0.25f, 0, -1);
            wallTouchList = WallChecker();
            if (!wallTouchList[1] && holdTimer > 0.1f)
            {
                StartCoroutine(GridMove(gameObject,transform.position + direction, timeBetweenTiles));
            }
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            holdTimer += Time.deltaTime;
            direction = Vector3.down;
            directionTracker.transform.localPosition = new Vector3(0, -0.25f, -1);
            wallTouchList = WallChecker();
            if (!wallTouchList[2] && holdTimer > 0.1f)
            {
                StartCoroutine(GridMove(gameObject,transform.position + direction, timeBetweenTiles));
            }
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            holdTimer += Time.deltaTime;
            direction = Vector3.right;
            directionTracker.transform.localPosition = new Vector3(0.25f, 0, -1);
            wallTouchList = WallChecker();
            if (!wallTouchList[3] && holdTimer > 0.1f)
            {
                StartCoroutine(GridMove(gameObject,transform.position + direction, timeBetweenTiles));
            }
        }
        else
        {
            holdTimer = 0;
        }
    }
    public IEnumerator GridMove(GameObject mover, Vector3 end, float seconds)
    {
        if (mover.name == "Player")
        {
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
                if (dialogueManager.hasMoreText)
                {
                    target.GetComponent<SignTextScript>().talkCounter += 1;
                }
                break;
            case "Block":
                if (target.GetComponent<BlockScript>().id != 0) 
                {
                    Debug.Log(target.GetComponent<BlockScript>().id);
                }
                if (!target.GetComponent<BlockScript>().moving)
                {
                    target.GetComponent<BlockScript>().Push(direction);
                }
                break;
            case "OnOff":
                target.GetComponent<SwitchScript>().UseSwitch();
                break;
            default:
                Debug.Log(target.name);
                break;
        }
        
    }
}
