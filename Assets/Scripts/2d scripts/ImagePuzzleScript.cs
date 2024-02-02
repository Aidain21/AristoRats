using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImagePuzzleScript : MonoBehaviour
{
    public int width;
    public int height;
    public Sprite[] pieces;
    public string mode;
    public GameObject pushBlock;
    public GameObject item;
    public GameObject floorSwitch;
    public int piecesLeft;
    public float puzzleTimer = 0;
    public bool timerOn;
    public GameObject reward;
    public Texture2D fullImage;
    // Start is called before the first frame update
    void Start()
    {
        reward.SetActive(false);
        timerOn = false;
        piecesLeft = width * height;
        pieces = new Sprite[width * height];
        float pheight = (float) fullImage.height / width;
        float pwidth = (float) fullImage.width / height;
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; i < width; i++)
            {
                pieces[i] = Sprite.Create(fullImage, new Rect(i * pwidth, j * pheight, pwidth, pheight), new Vector2(0.5f, 0.5f));
            }
        }
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
        int[] norm = new int[width*height];
        for (int i = 0; i < width * height; i++)
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
                for (int i = 0; i < width * height; i++)
                {
                    GameObject piece = Instantiate(pushBlock, transform);
                    piece.GetComponent<BlockScript>().id = norm[i];
                    piece.GetComponent<SpriteRenderer>().sprite = pieces[norm[i] - 1];
                    piece.GetComponent<Transform>().localPosition = new Vector2(i % width, i / width);
                    GameObject place = Instantiate(floorSwitch, transform);
                    place.GetComponent<SwitchScript>().switchData = (i+1).ToString();
                    place.GetComponent<SwitchScript>().switchEffect = "insert";
                    place.GetComponent<Transform>().localPosition = new Vector3(i % width, height * 2 - (i/width), 1);
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
