using System.Collections;
using System.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogueEvents : MonoBehaviour
{
    public bool dontAdd;
    public GameObject player;
    public PlayerScript2D playerScript;
    public string[] dialogueData = new string[3];
    public List<string[]> storedEvents = new();
    public List<Sprite> talkerStats = new();
    public Sprite norm;

    public void RunPastEvents()
    {
        dontAdd = true;
        for (int i = 0; i < storedEvents.Count; i++)
        {
            playerScript.currentTarget = GameObject.Find(storedEvents[i][0]);
            if (playerScript.currentTarget != null)
            {
                playerScript.currentTarget.GetComponent<SignTextScript>().talkCounter = Int32.Parse(storedEvents[i][1]) + 1;
                playerScript.currentTarget.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = talkerStats[i];
                storedEvents[i].CopyTo(dialogueData, 0);
                EventTrigger();
                EndEventTrigger();
            }
        }
        dontAdd = false;
    }
    public void EventTrigger() //use:   else if (Enumerable.SequenceEqual(dialogueData, new string[] { "NPC Object's name", "Talk Counter", "Current Line" }))
    {
        // NPC in test room runs away from you.
        if (Enumerable.SequenceEqual(dialogueData, new string[] { "Testy", "2", "1" }))
        {
            StartCoroutine(playerScript.GridMove(playerScript.currentTarget, playerScript.currentTarget.transform.position + Vector3.up * 10, 4f));
            if (!dontAdd)
            {
                storedEvents.Add((string[])dialogueData.Clone());
                talkerStats.Add(playerScript.currentTarget.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite);
            }
        }

        // Hidden NPC changes to red
        else if (Enumerable.SequenceEqual(dialogueData, new string[] { "RedSign", "2", "0" }))
        {
            playerScript.currentTarget.GetComponent<SpriteRenderer>().color = Color.red;
        }

        //NPC behind tree starts growing and has funny normal face
        else if (Enumerable.SequenceEqual(dialogueData, new string[] { "TreeGuy", "1", "2" }))
        {
            playerScript.currentTarget.GetComponent<SpriteRenderer>().sprite = norm;
            StartCoroutine(Grow());
        }

        // First guard moves out of the way
        else if (Enumerable.SequenceEqual(dialogueData, new string[] { "FirstGuard", "3", "1" }))
        {
            StartCoroutine(playerScript.GridMove(playerScript.currentTarget, playerScript.currentTarget.transform.position + Vector3.down * 3, 1f));
            if (!dontAdd)
            {
                storedEvents.Add((string[])dialogueData.Clone());
                talkerStats.Add(playerScript.dialogueManager.storedStatus);
            }
        }
    }
    public void EndEventTrigger()
    {
        // Darkness Renderer takes key from player
        if (Enumerable.SequenceEqual(dialogueData, new string[] { "DarkDude", "1", "0" }))
        {
            if (playerScript.HasItem("TestKey"))
            {
                playerScript.dialogueManager.ChangeDialogue(5, true);
                playerScript.invManager.inventory.Remove(playerScript.GetItem("TestKey"));
            }
        }

        // Darkness Renderer resets dialogue
        else if (Enumerable.SequenceEqual(dialogueData, new string[] { "DarkDude", "2", "0" }))
        {
            playerScript.dialogueManager.ChangeDialogue(0, false);
        }

        // The First Guard takes the key from the player, if they had one, and changes dialogue
        else if (Enumerable.SequenceEqual(dialogueData, new string[] { "FirstGuard", "0", "0" }) || Enumerable.SequenceEqual(dialogueData, new string[] { "FirstGuard", "1", "0" }))
        {
            if (playerScript.HasItem("Key"))
            { 
                playerScript.dialogueManager.ChangeDialogue(3, true);
                playerScript.invManager.inventory.Remove(playerScript.GetItem("Key"));
            }
        }

        // Stickman death
        else if (Enumerable.SequenceEqual(dialogueData, new string[] { "Trash", "0", "3" }))
        {
            Destroy(playerScript.currentTarget);
        }

        // Funny Stuff start
        else if (Enumerable.SequenceEqual(dialogueData, new string[] { "Normal", "0", "0" }))
        {
            GameObject.Find("LevelObjects/NPCs").transform.Find("Easy").gameObject.SetActive(true);
        }
        else if (Enumerable.SequenceEqual(dialogueData, new string[] { "Easy", "0", "0" }))
        {
            GameObject.Find("LevelObjects/NPCs").transform.Find("Harder").gameObject.SetActive(true);
        }
        else if (Enumerable.SequenceEqual(dialogueData, new string[] { "Harder", "0", "0" }))
        {
            GameObject.Find("LevelObjects/NPCs").transform.Find("Insane").gameObject.SetActive(true);
        }
        else if (Enumerable.SequenceEqual(dialogueData, new string[] { "Insane", "0", "0" }))
        {
            GameObject.Find("LevelObjects/NPCs").transform.Find("Auto").gameObject.SetActive(true);
        }
        // Funny stuff end
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
