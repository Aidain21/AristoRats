using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueEvents : MonoBehaviour
{
    public GameObject player;
    public PlayerScript2D playerScript;
    public string[] dialogueData = new string[] { " ", "0" };
    //current targeted item 
    // Start is called before the first frame update

    // Update is called once per frame
    public void EventTrigger()
    {
        if (dialogueData[0] == "Testy" && dialogueData[1] == "3")
        {
            StartCoroutine(playerScript.GridMove(playerScript.currentTarget, playerScript.currentTarget.transform.position + Vector3.up*10, 4f));
        }
        else if (dialogueData[0] == "RedSign" && dialogueData[1] == "4")
        {
            playerScript.currentTarget.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }
}
