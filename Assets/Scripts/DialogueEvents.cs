using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class DialogueEvents : MonoBehaviour
{
    public GameObject player;
    public PlayerScript2D playerScript;
    public string[] dialogueData = new string[] { " ", "0" , "0" };
    //current targeted item 
    // Start is called before the first frame update

    // Update is called once per frame
    public void EventTrigger()
    {
        if (Enumerable.SequenceEqual(dialogueData, new string[]{"Testy", "0", "3"}))
        {
            StartCoroutine(playerScript.GridMove(playerScript.currentTarget, playerScript.currentTarget.transform.position + Vector3.up*10, 4f));
        }
        else if (Enumerable.SequenceEqual(dialogueData, new string[] { "RedSign", "0", "4" }))
        {
            playerScript.currentTarget.GetComponent<SpriteRenderer>().color = Color.red;
        }
        else if (Enumerable.SequenceEqual(dialogueData, new string[] { "PuzzleTrigger", "0", "4" }))
        {
            playerScript.currentTarget.GetComponent<ImagePuzzleScript>().PuzzleSetUp();
        }
    }
}
