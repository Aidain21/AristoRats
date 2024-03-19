using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ImagePuzzleScript : MonoBehaviour
{
    public int width;
    public int height;
    public Sprite[] pieces;
    public string mode;
    public GameObject pushBlock;
    public GameObject item;
    public GameObject sign;
    public GameObject border;
    public GameObject floorSwitch;
    public int piecesLeft;
    public int reward;
    public Texture2D fullImage;
    public GameObject instructionSigns;
    public PlayerScript2D playerScript;
    public bool puzzleAlreadyEnded;

    public void EndPuzzle()
    {
        if (!puzzleAlreadyEnded)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).CompareTag("Block"))
                {
                    transform.GetChild(i).localPosition = new Vector3(transform.GetChild(i).localPosition.x, transform.GetChild(i).localPosition.y, 0.75f);
                }
            }
            piecesLeft = -1;
            playerScript = GameObject.Find("Player").GetComponent<PlayerScript2D>();
            if (int.Parse(playerScript.oldPuzzles[playerScript.oldPuzzles.IndexOf(mode) + 1]) > playerScript.lastPTimerInt)
            {
                playerScript.endMessage = "New Record!!!";
                playerScript.oldPuzzles[playerScript.oldPuzzles.IndexOf(mode) + 1] = playerScript.lastPTimerInt.ToString();
            }
            else
            {
                playerScript.endMessage = "Try Again";
            }
            playerScript.finishedPuzzle = true;
            playerScript.invManager.cheese += reward;
            if (playerScript.inInventory)
            {
                playerScript.invManager.CloseInventory();
            }
            string[] temp = new string[] { "0You:Solved the puzzle in " + playerScript.lastPTimerInt + " seconds, and got " + reward + " cheese!" };
            playerScript.dialogueManager.StartDialogue("Player", temp, 0, playerScript.GetComponent<SpriteRenderer>().sprite);
            playerScript.pTimer = 0;
            playerScript.lastPTimerInt = 0;
            puzzleAlreadyEnded = true;
            
        }
    }
    public void PuzzleSetUp()
    {
        playerScript = GameObject.Find("Player").GetComponent<PlayerScript2D>();
        fullImage = Resize(fullImage, width * 100, height * 100);
        piecesLeft = width * height;
        pieces = new Sprite[width * height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Sprite newSprite = Sprite.Create(fullImage, new Rect(i * 100, (fullImage.height - 100) - (j * 100), 100, 100), new Vector2(0.5f, 0.5f));

                pieces[(j * width) + i] = newSprite;
            }
        }
        int[] norm = new int[width*height];
        int[] randRotation = new int[width * height];
        for (int i = 0; i < width * height; i++)
        {
            norm[i] = i + 1;
            if (mode.Contains("-"))
            {
                randRotation[i] = UnityEngine.Random.Range(0, 4) * 90;
            }
        }
        Shuffle(norm);
        string switchType = "insert";
        bool foundSign = false;
        for (int i = 0; i < instructionSigns.transform.childCount; i++)
        {
            if (instructionSigns.transform.GetChild(i).name == mode)
            {
                foundSign = true;
                if (!playerScript.oldPuzzles.Contains(mode))
                {
                    instructionSigns.transform.GetChild(0).gameObject.SetActive(true);
                    playerScript.oldPuzzles.Add(mode);
                    playerScript.oldPuzzles.Add("999999");
                }
                else
                {
                    instructionSigns.transform.GetChild(i).gameObject.SetActive(true);
                }
                break;
                
            }
        }
        if (!foundSign && !playerScript.oldPuzzles.Contains(mode))
        {
            instructionSigns.transform.GetChild(1).gameObject.SetActive(true);
            playerScript.oldPuzzles.Add(mode);
            playerScript.oldPuzzles.Add("999999");
        }
        if (mode.Contains("+"))
        {
            mode = mode[0..^1];
            switchType = "insert+";
        }
        else if (mode.Contains("-"))
        {
            mode = mode[0..^1];
            switchType = "insert-";
        }
        switch (mode)
        {
            case "Blocks":
                for (int i = 0; i < width * height; i++)
                {
                    GameObject piece = Instantiate(pushBlock, transform);
                    piece.GetComponent<BlockScript>().id = norm[i];
                    piece.GetComponent<SpriteRenderer>().sprite = pieces[norm[i] - 1];
                    piece.GetComponent<Transform>().localPosition = new Vector3(i % width, i / width - height, 0.5f);
                    Instantiate(border, piece.transform).transform.localPosition += new Vector3(0, 0, 0.25f);
                    if (switchType == "insert-")
                    {
                        piece.GetComponent<BlockScript>().rotatable = true;
                        piece.transform.localRotation = Quaternion.Euler(0, 0, randRotation[i]);
                    }
                }
                break;
            case "Items":
                for (int i = 0; i < width * height; i++)
                {
                    GameObject piece = Instantiate(item, transform);
                    piece.name = norm[i].ToString();
                    piece.GetComponent<ItemScript>().itemName = "Puzzle Piece";
                    piece.GetComponent<ItemScript>().itemLore = new string[] { "0You:Piece of the Puzzle." };
                    piece.GetComponent<ItemScript>().itemImage = pieces[norm[i] - 1];
                    piece.GetComponent<SpriteRenderer>().sprite = pieces[norm[i] - 1];
                    piece.GetComponent<Transform>().localPosition = new Vector3(i % width, i / width - height, 0.5f);
                    Instantiate(border, piece.transform).transform.localPosition += new Vector3(0, 0, 0.25f);
                }
                break;
            case "Menu":
                for (int i = 0; i < width * height; i++)
                {
                    GameObject piece = Instantiate(pushBlock, transform);
                    piece.GetComponent<BlockScript>().id = norm[i];
                    piece.GetComponent<BlockScript>().type = "menu";
                    piece.GetComponent<SpriteRenderer>().sprite = pieces[norm[i] - 1];
                    piece.GetComponent<Transform>().localPosition = new Vector3(i % width, i / width - height, 0.5f);
                    Instantiate(border, piece.transform).transform.localPosition += new Vector3(0, 0, 0.25f);
                    if (switchType == "insert-")
                    {
                        piece.GetComponent<BlockScript>().rotatable = true;
                        piece.transform.localRotation = Quaternion.Euler(0, 0, randRotation[i]);
                    }
                }
                break;
            case "Control":
                for (int i = 0; i < width * height; i++)
                {
                    GameObject piece = Instantiate(pushBlock, transform);
                    piece.GetComponent<BlockScript>().id = norm[i]; 
                    piece.GetComponent<BlockScript>().type = "control";
                    piece.GetComponent<SpriteRenderer>().sprite = pieces[norm[i] - 1];
                    piece.GetComponent<Transform>().localPosition = new Vector3(i % width, i / width - height, 0.5f);
                    Instantiate(border, piece.transform).transform.localPosition += new Vector3(0, 0, 0.25f);
                    if (switchType == "insert-")
                    {
                        piece.GetComponent<BlockScript>().rotatable = true;
                        piece.transform.localRotation = Quaternion.Euler(0, 0, randRotation[i]);
                    }
                }
                break;
            case "Shuffle":
                for (int i = 0; i < width * height; i++)
                {
                    GameObject piece = Instantiate(pushBlock, transform);
                    piece.GetComponent<BlockScript>().id = norm[i];
                    piece.GetComponent<BlockScript>().type = "shuffle";
                    piece.GetComponent<SpriteRenderer>().sprite = pieces[norm[i] - 1];
                    piece.GetComponent<Transform>().localPosition = new Vector3(i % width, height - (i / width), 0.5f);
                    Instantiate(border, piece.transform).transform.localPosition += new Vector3(0, 0, 0.25f);
                    if (switchType == "insert-")
                    {
                        piece.GetComponent<BlockScript>().rotatable = true;
                        piece.transform.localRotation = Quaternion.Euler(0, 0, randRotation[i]);
                    }
                }
                break;
            case "Swap":
                for (int i = 0; i < width * height; i++)
                {
                    GameObject piece = Instantiate(pushBlock, transform);
                    piece.GetComponent<BlockScript>().id = norm[i];
                    piece.GetComponent<BlockScript>().type = "swap";
                    piece.GetComponent<SpriteRenderer>().sprite = pieces[norm[i] - 1];
                    piece.GetComponent<Transform>().localPosition = new Vector3(i % width, height - (i / width), 0.5f);
                    piece.AddComponent<Rigidbody2D>();
                    piece.GetComponent<Rigidbody2D>().gravityScale = 0;
                    piece.GetComponent<BoxCollider2D>().isTrigger = true;
                    piece.GetComponent<BoxCollider2D>().size = new Vector2(0.3f, 0.3f);
                    Instantiate(border, piece.transform).transform.localPosition += new Vector3(0, 0, 0.25f);
                    if (switchType == "insert-")
                    {
                        piece.GetComponent<BlockScript>().rotatable = true;
                        piece.transform.localRotation = Quaternion.Euler(0, 0, randRotation[i]);
                    }
                }
                break;
            case "Follow":
                for (int i = 0; i < width * height; i++)
                {
                    GameObject piece = Instantiate(sign, transform);
                    piece.name = "Follower " + norm[i].ToString();
                    string[] temp = new string[] { "0Puzzle Piece:I will follow you!", "1Puzzle Piece: Bye Bye!", "3Puzzle Piece: Yes." };
                    piece.GetComponent<SignTextScript>().dialogue = temp;
                    piece.GetComponent<SpriteRenderer>().sprite = pieces[norm[i] - 1];
                    piece.GetComponent<Transform>().localPosition = new Vector3(i % width, i / width - height, 0.5f);
                    Instantiate(border, piece.transform).transform.localPosition += new Vector3(0, 0, 0.25f);
                }
                break;
            case "Input":
                for (int i = 0; i < width * height; i++)
                {
                    GameObject piece = Instantiate(sign, transform);
                    piece.name = "TypeDude " + norm[i].ToString();
                    string[] temp = new string[] { "0Puzzle Piece: Tell me where to go." , "0Puzzle Piece: Not proper syntax." };
                    piece.GetComponent<SignTextScript>().dialogue = temp;
                    piece.GetComponent<SpriteRenderer>().sprite = pieces[norm[i] - 1];
                    piece.GetComponent<Transform>().localPosition = new Vector3(i % width, i / width - height, 0.5f);
                    Instantiate(border, piece.transform).transform.localPosition += new Vector3(0, 0, 0.25f);
                }
                break;
        }
        for (int i = 0; i < width * height; i++)
        {
            GameObject place = Instantiate(floorSwitch, transform);
            place.GetComponent<SwitchScript>().switchData = (i + 1).ToString();
            place.GetComponent<SwitchScript>().switchEffect = switchType;
            place.GetComponent<Transform>().localPosition = new Vector3(i % width, height - (i / width), 1);
            place.GetComponent<SpriteRenderer>().color = Color.cyan;
        }
        
    }

    public void Shuffle<T>(T[] array)
    {
        int n = array.Length;
        while (n > 1)
        {
            System.Random rng = new();
            int k = rng.Next(n--);
            T temp = array[n];
            array[n] = array[k];
            array[k] = temp;
        }
    }
    public Texture2D Resize(Texture2D source, int newWidth, int newHeight)
    {
        source.filterMode = FilterMode.Point;
        RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight);
        rt.filterMode = FilterMode.Point;
        RenderTexture.active = rt;
        Graphics.Blit(source, rt);
        Texture2D nTex = new(newWidth, newHeight);
        nTex.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
        nTex.Apply();
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);
        return nTex;
    }

    public void ChangePiecesLeft(int change)
    {
        piecesLeft += change;
        playerScript.invManager.UpdateInfo(piecesLeft);
    }
}
