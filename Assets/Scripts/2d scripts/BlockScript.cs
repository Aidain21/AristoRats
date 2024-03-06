using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockScript : MonoBehaviour
{
    public bool moving;
    public string type = "push";
    public bool inserted;
    public IEnumerator move;
    public bool[] walls = { false, false, false, false }; //ULDR
    public GameObject player;
    public int id = 0;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        moving = false;
    }

    // Update is called once per frame
    public IEnumerator Moving(Vector3 goal, float time)
    {
        
        while (new Vector3(transform.position.x, transform.position.y, transform.position.z) != goal)
        {
            moving = true;
            transform.position = Vector3.MoveTowards(transform.position, goal, time * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        if (id > 0 && GetComponent<SpriteRenderer>().sprite != null && transform.parent.gameObject.GetComponent<ImagePuzzleScript>().piecesLeft == -1)
        {
            transform.position += new Vector3(0, 0, 1);
        }
        transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), Mathf.Round(transform.position.z));
        moving = false;
        if (inserted)
        {
            if (id > 0 && GetComponent<SpriteRenderer>().sprite != null)
            {
                transform.parent.gameObject.GetComponent<ImagePuzzleScript>().piecesLeft -= 1;
            }
            transform.position += new Vector3(0, 0, 1);
            Destroy(GetComponent<BoxCollider2D>());
            Destroy(GetComponent<BlockScript>());
            Destroy(transform.GetChild(0).gameObject);
            if (id > 0 && GetComponent<SpriteRenderer>().sprite != null && transform.parent.gameObject.GetComponent<ImagePuzzleScript>().piecesLeft == 0)
            {
                transform.parent.gameObject.GetComponent<ImagePuzzleScript>().EndPuzzle();
            }
        }        
    }

    public void Push(Vector3 direction)
    {
        if (type == "push")
        {
            walls = WallChecker();
            if ((!walls[0] && direction == Vector3.up) || (!walls[1] && direction == Vector3.left) || (!walls[2] && direction == Vector3.down) || (!walls[3] && direction == Vector3.right))
            {
                StartCoroutine(Moving(transform.position + direction, 2.5f));
            }
            else if (!player.GetComponent<PlayerScript2D>().moving)
            {
                bool[] pwalls = player.GetComponent<PlayerScript2D>().WallChecker();
                if ((!pwalls[0] && direction == Vector3.down) || (!pwalls[1] && direction == Vector3.right) || (!pwalls[2] && direction == Vector3.up) || (!pwalls[3] && direction == Vector3.left))
                {
                    StartCoroutine(Moving(transform.position - direction, 2.5f));
                    string temp;
                    if (direction == Vector3.up)
                    {
                        temp = "Up";
                    }
                    else if (direction == Vector3.left)
                    {
                        temp = "Left";
                    }
                    else if (direction == Vector3.down)
                    {
                        temp = "Down";
                    }
                    else
                    {
                        temp = "Right";
                    }
                    StartCoroutine(player.GetComponent<PlayerScript2D>().GridMove(player, player.transform.position - direction, 0.3f, temp));
                }

            }
        }
        else if (type == "menu")
        {
            player.GetComponent<PlayerScript2D>().menuManager.OpenPuzzle();
        }
        else if (type == "control")
        {
            player.GetComponent<PlayerScript2D>().vcam.Follow = gameObject.transform;
            player.GetComponent<PlayerScript2D>().invManager.blockControlText.GetComponent<Canvas>().enabled = true;
            player.GetComponent<PlayerScript2D>().controllingBlock = true;
        }
        else if (type == "shuffle")
        {
            for (int i = 0; i < transform.parent.childCount; i++) 
            {
                if (transform.parent.GetChild(i).name == "PushBlock(Clone)")
                {
                    if (direction == Vector3.up && transform.parent.GetChild(i).localPosition.x == transform.localPosition.x)
                    {
                        if (transform.parent.GetChild(i).localPosition.y == transform.parent.GetComponent<ImagePuzzleScript>().height)
                        {
                            transform.parent.GetChild(i).localPosition -= new Vector3(0, transform.parent.GetComponent<ImagePuzzleScript>().height - 1, 0);
                        }
                        else
                        {
                            transform.parent.GetChild(i).localPosition += Vector3.up;
                        }
                    }
                    else if (direction == Vector3.left && transform.parent.GetChild(i).localPosition.y == transform.localPosition.y)
                    {
                        if (transform.parent.GetChild(i).localPosition.x == 0)
                        {
                            transform.parent.GetChild(i).localPosition += new Vector3(transform.parent.GetComponent<ImagePuzzleScript>().width -1, 0, 0);
                        }
                        else
                        {
                            transform.parent.GetChild(i).localPosition += Vector3.left;
                        }
                    }
                    else if (direction == Vector3.down && transform.parent.GetChild(i).localPosition.x == transform.localPosition.x)
                    {
                        if (transform.parent.GetChild(i).localPosition.y == 1)
                        {
                            transform.parent.GetChild(i).localPosition += new Vector3(0, transform.parent.GetComponent<ImagePuzzleScript>().height - 1, 0);
                        }
                        else
                        {
                            transform.parent.GetChild(i).localPosition += Vector3.down;
                        }
                    }
                    else if (direction == Vector3.right && transform.parent.GetChild(i).localPosition.y == transform.localPosition.y)
                    {
                        if (transform.parent.GetChild(i).localPosition.x == transform.parent.GetComponent<ImagePuzzleScript>().width - 1)
                        {
                            transform.parent.GetChild(i).localPosition -= new Vector3(transform.parent.GetComponent<ImagePuzzleScript>().width - 1, 0, 0);
                        }
                        else
                        {
                            transform.parent.GetChild(i).localPosition += Vector3.right;
                        }
                    }
                }
            }
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

}

