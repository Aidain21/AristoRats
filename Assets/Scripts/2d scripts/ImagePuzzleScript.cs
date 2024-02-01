using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImagePuzzleScript : MonoBehaviour
{
    public Sprite[] pieces;
    public string mode;
    public GameObject pushBlock;
    public GameObject item;
    public GameObject floorSwitch;
    public int piecesLeft;
    public float puzzleTimer = 0;
    public bool timerOn;
    public GameObject reward;
    // Start is called before the first frame update
    void Start()
    {
        reward.SetActive(false);
        timerOn = false;
        piecesLeft = 16;
    }

    // Update is called once per frame
    void Update()
    {
        if (timerOn)
        {
            puzzleTimer += Time.deltaTime;
        }
        if (piecesLeft == 0)
        {
            if (reward != null)
            {
                reward.SetActive(true);
            }
            timerOn = false;
            piecesLeft = -1;
        }
    }

    public void PuzzleSetUp()
    {
        int[] norm = new int[16];
        for (int i = 0; i < 16; i++)
        {
            norm[i] = i + 1;
        }
        for (int t = 0; t < norm.Length; t++)
        {
            int tmp = norm[t];
            int r = Random.Range(t, norm.Length);
            norm[t] = norm[r];
            norm[r] = tmp;
        }
        switch (mode)
        {
            case "Blocks":
                for (int i = 0; i < 16; i++)
                {
                    GameObject piece = Instantiate(pushBlock, transform);
                    piece.GetComponent<BlockScript>().id = norm[i];
                    piece.GetComponent<SpriteRenderer>().sprite = pieces[norm[i] - 1];
                    piece.GetComponent<Transform>().localPosition = new Vector2(i%4,(i-(i%4))/4);
                    GameObject place = Instantiate(floorSwitch, transform);
                    place.GetComponent<SwitchScript>().switchData = (i+1).ToString();
                    place.GetComponent<SwitchScript>().switchEffect = "insert";
                    place.GetComponent<Transform>().localPosition = new Vector3(i % 4, 8 - ((i - (i % 4)) / 4), 1);
                }
                timerOn = true;
                break;
            case "Items":
                for (int i = 0; i < 16; i++)
                {
                    GameObject piece = Instantiate(item, transform);
                    piece.name = norm[i].ToString();
                    piece.GetComponent<ItemScript>().itemName = "Puzzle Piece";
                    piece.GetComponent<ItemScript>().itemLore = new string[] { "0You:Piece of the Puzzle. What? Did you expect me to say anything else? How rude of you.", "0You:The Rat King is trying to steal my hair, my friend is missing, and I have to do this stupid puzzle, and yet you have the nerve to ask me to describe the puzzle piece in more detail.", "0You:You know what? Fine. I'll tell you more. " + piece.name + ". That's all I got, now leave me alone."};
                    piece.GetComponent<ItemScript>().itemImage = pieces[norm[i] - 1];
                    piece.GetComponent<SpriteRenderer>().sprite = pieces[norm[i] - 1];
                    piece.GetComponent<Transform>().localPosition = new Vector2(i % 4, (i - (i % 4)) / 4);
                    GameObject place = Instantiate(floorSwitch, transform);
                    place.GetComponent<SwitchScript>().switchData = (i + 1).ToString();
                    place.GetComponent<SwitchScript>().switchEffect = "insert";
                    place.GetComponent<Transform>().localPosition = new Vector3(i % 4, 8 - ((i - (i % 4)) / 4), 1);
                }
                break;

        }
        
    }
}
