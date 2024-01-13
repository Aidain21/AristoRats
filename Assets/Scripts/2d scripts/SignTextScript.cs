using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignTextScript : MonoBehaviour
{
    public string dialogueName;
    public string[] dialogue;
    public int talkCounter = 0;
    public Sprite talkerImage;

    void Start()
    {
        talkerImage = GetComponent<SpriteRenderer>().sprite;
    }

    //edit within unity plz :)
    //put name:text in strings of dialogue for individual signs.
    //make sure to save scene and checkin using plastic/Devops NOT unity window after
}
