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
    public List<string[]> storedTalks = new();
    public List<Sprite> talkerStats = new();
    public int fullyTalkedTo;
    public int npcsInScene;
    public Sprite norm;

    public void RunPastEvents()
    {
        fullyTalkedTo = 0;
        dontAdd = true;
        for (int i = 0; i < storedEvents.Count; i++)
        {
            playerScript.currentTarget = GameObject.Find(storedEvents[i][0]);
            if (playerScript.currentTarget != null)
            {
                storedEvents[i].CopyTo(dialogueData, 0);
                EventTrigger();
                EndEventTrigger();
            }
        }
        for (int i = 0; i < storedTalks.Count; i++)
        {
            playerScript.currentTarget = GameObject.Find(storedTalks[i][0]);
            if (playerScript.currentTarget != null)
            {
                playerScript.currentTarget.GetComponent<SignTextScript>().talkCounter = Int32.Parse(storedTalks[i][1]);
                playerScript.currentTarget.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = talkerStats[i];
                if (talkerStats[i] == playerScript.dialogueManager.noMoreText)
                {
                    fullyTalkedTo += 1;
                }
            }
        }
        npcsInScene = GameObject.FindGameObjectsWithTag("Sign").Length;
        dontAdd = false;
    }
    public void EventTrigger() //use:   else if (Enumerable.SequenceEqual(dialogueData, new string[] { "NPC Object's name", "Talk Counter", "Current Line" }))
    {
        // NPC in test room runs away from you.
        if (Enumerable.SequenceEqual(dialogueData, new string[] { "Testy", "2", "1" }))
        {
            StartCoroutine(playerScript.GridMove(playerScript.currentTarget, playerScript.currentTarget.transform.position + Vector3.up * 9, 4f));
            if (!dontAdd)
            {
                storedEvents.Add((string[])dialogueData.Clone());
            }
        }

        // Hidden NPC changes to red
        else if (Enumerable.SequenceEqual(dialogueData, new string[] { "RedSign", "2", "0" }))
        {
            playerScript.currentTarget.GetComponent<SpriteRenderer>().color = Color.red;
        }

        // First guard moves out of the way
        else if (Enumerable.SequenceEqual(dialogueData, new string[] { "FirstGuard", "3", "1" }))
        {
            StartCoroutine(playerScript.GridMove(playerScript.currentTarget, playerScript.currentTarget.transform.position + Vector3.down * 3, 1f));
            if (!dontAdd)
            {
                storedEvents.Add((string[])dialogueData.Clone());
            }
        }
    }
    public void EndEventTrigger()
    {
        if (SceneManager.GetActiveScene().name == "Castle")
        {
            //Guard Rat 2 (Basement) Takes Cheese and tells riddle.
            if (Enumerable.SequenceEqual(dialogueData, new string[] { "GuardRat2", "0", "2" }) || Enumerable.SequenceEqual(dialogueData, new string[] { "GuardRat2", "1", "0" }))
            {
                if (playerScript.invManager.cheese > 0)
                {
                    playerScript.dialogueManager.ChangeDialogue(3, true);
                    playerScript.invManager.cheese -= 1;
                }
            }
            //Maid Rat 1 (Basement) Takes Cheese and tells riddle.
            else if (Enumerable.SequenceEqual(dialogueData, new string[] { "MaidRat1", "1", "0" }))
            {
                if (playerScript.invManager.cheese > 0)
                {
                    playerScript.dialogueManager.ChangeDialogue(3, true);
                    playerScript.invManager.cheese -= 1;
                }
            }
            //Chef Rat 1 (Main Floor) Takes Cheese and tells riddle.
            else if (Enumerable.SequenceEqual(dialogueData, new string[] { "ChefRat1", "1", "0" }) || Enumerable.SequenceEqual(dialogueData, new string[] { "ChefRat1", "2", "0" }))
            {
                if (playerScript.invManager.cheese > 1)
                {
                    playerScript.dialogueManager.ChangeDialogue(4, true);
                    playerScript.invManager.cheese -= 2;
                }
            }
            //Maid Rat 3 (Main floor) Takes Medicine and gives Cheese.
            else if (Enumerable.SequenceEqual(dialogueData, new string[] { "MaidRat3", "1", "0" }))
            {
                if (SelectItem("Medicine", 3, 1))
                {
                    playerScript.invManager.cheese += 5;
                }
            }
            //Guard Rat 7 (2nd floor) Takes 2 cheese and gives riddle.
            else if (Enumerable.SequenceEqual(dialogueData, new string[] { "GuardRat7", "1", "0" }))
            {
                if (playerScript.invManager.cheese > 1)
                {
                    playerScript.dialogueManager.ChangeDialogue(3, true);
                    playerScript.invManager.cheese -= 2;
                }
            }
            //Maid Rat 4 (2nd floor) Takes Proof and gives Cheese.
            else if (Enumerable.SequenceEqual(dialogueData, new string[] { "MaidRat4", "1", "0" }))
            {
                if (SelectItem("Photo of a Made Bed", 3, 1))
                {
                    playerScript.invManager.cheese += 5;
                }
            }
            //Maid Rat 6 (2nd floor) Takes 2 cheese and gives riddle.
            else if (Enumerable.SequenceEqual(dialogueData, new string[] { "MaidRat6", "1", "0" }))
            {
                if (playerScript.invManager.cheese > 1)
                {
                    playerScript.dialogueManager.ChangeDialogue(3, true);
                    playerScript.invManager.cheese -= 2;
                }
            }
        }
        else if (SceneManager.GetActiveScene().name == "Forest")
        {
            //First Guard takes key if player has one
            if (Enumerable.SequenceEqual(dialogueData, new string[] { "FirstGuard", "0", "0" }) || Enumerable.SequenceEqual(dialogueData, new string[] { "FirstGuard", "1", "0" }))
            {
                SelectItem("Key", 3, Int32.Parse(dialogueData[1]));
            }
        }
        else if (SceneManager.GetActiveScene().name == "PuzzleTest")
        {
            // Darkness Renderer takes key from player
            if (Enumerable.SequenceEqual(dialogueData, new string[] { "DarkDude", "1", "0" }))
            {
                SelectItem("TestKey", 3, 1);
            }
            // funny infinite talk!
            else if (Enumerable.SequenceEqual(dialogueData, new string[] { "Infinite", "1", "5" }))
            {
                playerScript.dialogueManager.ChangeDialogue(1, true);
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

    }
    public bool SelectItem(string wantedItem, int successDialogueCounter, int failDialogueCounter)
    {
        if (playerScript.selection == null)
        {
            playerScript.dialogueManager.ChangeDialogue(failDialogueCounter, false);
            playerScript.selectingItem = true;
            playerScript.invManager.OpenInventory();
        }
        else if (playerScript.selection.CompareTag("Item") && playerScript.selection.GetComponent<ItemScript>().itemName == wantedItem)
        {
            playerScript.currentTarget.GetComponent<SignTextScript>().talkCounter = successDialogueCounter;
            playerScript.dialogueManager.StartDialogue(playerScript.currentTarget.name, playerScript.currentTarget.GetComponent<SignTextScript>().dialogue, successDialogueCounter, playerScript.currentTarget.GetComponent<SignTextScript>().talkerImage);
            playerScript.invManager.inventory.Remove(playerScript.GetItem(wantedItem));
            playerScript.selection = null;
            return true;
        }
        else if (playerScript.selection.CompareTag("Item") && playerScript.selection.GetComponent<ItemScript>().itemName != wantedItem)
        {
            
            playerScript.selection = null;
            playerScript.dialogueManager.ChangeDialogue(failDialogueCounter, true, dialogueData[2]);
        }
        else if (playerScript.selection == playerScript.currentTarget)
        {
            playerScript.selection = null;
            playerScript.dialogueManager.ChangeDialogue(failDialogueCounter, true, dialogueData[2]);
        }
        return false;
    }
}
