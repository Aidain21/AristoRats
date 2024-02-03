using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class DialogueEvents : MonoBehaviour
{
    public GameObject player;
    public PlayerScript2D playerScript;
    public string[] dialogueData = new string[3];
    public Sprite norm;
    //current targeted item 
    // Start is called before the first frame update

    // Update is called once per frame
    public void EventTrigger()
    {
        if (Enumerable.SequenceEqual(dialogueData, new string[] { "Testy", "2", "1" }))
        {
            StartCoroutine(playerScript.GridMove(playerScript.currentTarget, playerScript.currentTarget.transform.position + Vector3.up * 10, 4f));
        }
        else if (Enumerable.SequenceEqual(dialogueData, new string[] { "RedSign", "2", "0" }))
        {
            playerScript.currentTarget.GetComponent<SpriteRenderer>().color = Color.red;
        }
        else if (Enumerable.SequenceEqual(dialogueData, new string[] { "tre", "1", "2" }))
        {
            playerScript.currentTarget.GetComponent<SpriteRenderer>().sprite = norm;
            StartCoroutine(Grow());
        }
        else if (Enumerable.SequenceEqual(dialogueData, new string[] { "FirstGuard", "3", "1" }))
        {
            StartCoroutine(playerScript.GridMove(playerScript.currentTarget, playerScript.currentTarget.transform.position + Vector3.left * 3, 1f));
        }
    }
    public void EndEventTrigger()
    {
        if (Enumerable.SequenceEqual(dialogueData, new string[] { "darkDude", "1", "0" }))
        {
            if (playerScript.HasItem("TestKey"))
            {
                playerScript.dialogueManager.ChangeDialogue(5, true);
                playerScript.invManager.inventory.Remove(playerScript.GetItem("TestKey"));
            }
        }
        else if (Enumerable.SequenceEqual(dialogueData, new string[] { "darkDude", "2", "0" }))
        {
            playerScript.dialogueManager.ChangeDialogue(0, false);
        }
        else if (Enumerable.SequenceEqual(dialogueData, new string[] { "FirstGuard", "0", "0" }) || Enumerable.SequenceEqual(dialogueData, new string[] { "FirstGuard", "1", "0" }))
        {
            if (playerScript.HasItem("Key"))
            { 
                playerScript.dialogueManager.ChangeDialogue(3, true);
                playerScript.invManager.inventory.Remove(playerScript.GetItem("Key"));
            }
        }
        else if (Enumerable.SequenceEqual(dialogueData, new string[] { "trash", "0", "3" }))
        {
            Destroy(playerScript.currentTarget);
        }
    }

    public IEnumerator Grow()
    {
        while (true)
        {
            playerScript.currentTarget.transform.localScale += new Vector3(0.01f, 0.01f, 0);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
