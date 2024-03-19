using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BlockScript : MoveableObject
{
    public bool moving;
    public bool rotatable;
    public string type = "push";
    public IEnumerator move;
    public bool[] walls = { false, false, false, false }; //ULDR
    public GameObject player;
    public GameObject currentCollider;
    public int id = 0;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        moving = false;
    }

    // Update is called once per frame
    public void Push(Vector3 direction)
    {
        if (type == "push")
        {
            walls = WallChecker(gameObject);
            if (((!walls[0] && direction == Vector3.up) || (!walls[1] && direction == Vector3.left) || (!walls[2] && direction == Vector3.down) || (!walls[3] && direction == Vector3.right)) && !moving)
            {
                player.GetComponent<AudioSource>().PlayOneShot(player.GetComponent<PlayerScript2D>().sfx[0]);
                StartCoroutine(GridMove(gameObject, transform.position + direction, 0.3f));
            }
            else if (!player.GetComponent<PlayerScript2D>().moving)
            {
                bool[] pwalls = WallChecker(player);
                if (((!pwalls[0] && direction == Vector3.down) || (!pwalls[1] && direction == Vector3.right) || (!pwalls[2] && direction == Vector3.up) || (!pwalls[3] && direction == Vector3.left)) && !moving)
                {
                    player.GetComponent<AudioSource>().PlayOneShot(player.GetComponent<PlayerScript2D>().sfx[0]);
                    StartCoroutine(GridMove(gameObject, transform.position - direction, 0.3f));
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
                    StartCoroutine(GridMove(player, player.transform.position - direction, 0.3f, temp));
                }

            }
        }
        else if (type == "menu")
        {
            player.GetComponent<PlayerScript2D>().menuManager.OpenPuzzle();
        }
        else if (type == "swap")
        {
            for (int i = 0; i < transform.parent.childCount; i++)
            {
                if (transform.parent.GetChild(i).name == "PushBlock(Clone)" && transform.parent.GetChild(i).position == transform.position + player.GetComponent<PlayerScript2D>().direction)
                {
                    player.GetComponent<AudioSource>().PlayOneShot(player.GetComponent<PlayerScript2D>().sfx[0]);
                    Vector3 temp = transform.parent.GetChild(i).position;
                    transform.parent.GetChild(i).position = transform.position;
                    transform.position = temp;
                    break;
                }
            }
        }
        else if (type == "control")
        {
            player.GetComponent<AudioSource>().PlayOneShot(player.GetComponent<PlayerScript2D>().sfx[0]);
            player.GetComponent<PlayerScript2D>().vcam.Follow = gameObject.transform;
            player.GetComponent<PlayerScript2D>().invManager.blockControlText.GetComponent<Canvas>().enabled = true;
            player.GetComponent<PlayerScript2D>().controllingBlock = true;
        }
        else if (type == "shuffle")
        {
            player.GetComponent<AudioSource>().PlayOneShot(player.GetComponent<PlayerScript2D>().sfx[0]);
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

    public void Rotate()
    {
        if (type == "push" || type == "control")
        {
            transform.localRotation *= Quaternion.Euler(0, 0, 90);
            transform.localRotation = Quaternion.Euler(0, 0, Mathf.RoundToInt(transform.localRotation.eulerAngles.z));
        }


        if (currentCollider != null && id != 0)
        {
            if (id.ToString() == currentCollider.GetComponent<SwitchScript>().switchData && transform.localRotation == currentCollider.transform.localRotation)
            {
                rotatable = false;
                currentCollider.GetComponent<SwitchScript>().affectedObjects[0] = gameObject;
                currentCollider.GetComponent<SwitchScript>().UseSwitch();
                
                if (type == "control")
                {
                    player.GetComponent<PlayerScript2D>().vcam.Follow = player.transform;
                    player.GetComponent<PlayerScript2D>().invManager.blockControlText.GetComponent<Canvas>().enabled = false;
                    player.GetComponent<PlayerScript2D>().controllingBlock = false;
                }

                if (SceneManager.GetActiveScene().name == "ImagePuzzle" && transform.parent.gameObject.GetComponent<ImagePuzzleScript>().piecesLeft <= 0)
                {
                    transform.parent.gameObject.GetComponent<ImagePuzzleScript>().EndPuzzle();
                }
                player.GetComponent<PlayerScript2D>().currentTarget = null;
                Destroy(this);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && type == "swap")
        {
            collision.GetComponent<PlayerScript2D>().currentTarget = gameObject;
        }
        if (collision.GetComponent<SwitchScript>() != null && collision.GetComponent<SwitchScript>().switchType == "floor")
        {
            currentCollider = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && type == "swap")
        {
            if (collision.GetComponent<PlayerScript2D>().currentTarget == gameObject)
            {
                collision.GetComponent<PlayerScript2D>().currentTarget = null;
            }
        }
        if (collision.GetComponent<SwitchScript>() != null && collision.GetComponent<SwitchScript>().switchType == "floor" && currentCollider == collision.gameObject)
        {
            currentCollider = null;
        }
    }

}

