using System.Collections;
using System.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogueEvents : MoveableObject
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
    public int collectedNotes;
    public int notesInScene;
    public string tempData;
    public string data2;
    public Texture2D tempImage;
    public void RunPastEvents()
    {
        bool statsExist = false;
        for (int i = 0; i < playerScript.menuManager.collection.Count; i++)
        {
            if (playerScript.menuManager.collection[i][0].ToString() == playerScript.oldScene)
            {
                playerScript.menuManager.collection[i] = new List<object> { playerScript.oldScene, fullyTalkedTo, npcsInScene, collectedNotes, notesInScene };
                statsExist = true;
                break;
            }
        }
        if (!statsExist && playerScript.oldScene != "ImagePuzzle")
        {
            playerScript.menuManager.collection.Add(new List<object> { playerScript.oldScene, fullyTalkedTo, npcsInScene, collectedNotes, notesInScene });
        }
        
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
                if (talkerStats[i] != playerScript.dialogueManager.noMoreText)
                {
                    playerScript.currentTarget.GetComponent<SignTextScript>().talkCounter += 1;
                }
                playerScript.currentTarget.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = talkerStats[i];
                if (talkerStats[i] == playerScript.dialogueManager.noMoreText)
                {
                    fullyTalkedTo += 1;
                }
            }
        }
        npcsInScene = GameObject.FindGameObjectsWithTag("Sign").Length;
        collectedNotes = 0;
        notesInScene = GameObject.FindGameObjectsWithTag("Note").Length;
        foreach (NoteScript note in playerScript.journalManager.notes)
        {
            if (note.scene == SceneManager.GetActiveScene().name)
            {
                collectedNotes += 1;
            }
        }
        dontAdd = false;
        playerScript.currentTarget = null;

        bool newStatsExist = false;
        for (int i = 0; i < playerScript.menuManager.collection.Count(); i++)
        {
            if (playerScript.menuManager.collection[i][0].ToString() == SceneManager.GetActiveScene().name)
            {
                playerScript.menuManager.collection[i] = new List<object> { SceneManager.GetActiveScene().name, fullyTalkedTo, npcsInScene, collectedNotes, notesInScene };
                newStatsExist = true;
                break;
            }
        }
        if (!newStatsExist && SceneManager.GetActiveScene().name != "ImagePuzzle")
        {
            playerScript.menuManager.collection.Add(new List<object> { SceneManager.GetActiveScene().name, fullyTalkedTo, npcsInScene, collectedNotes, notesInScene });
        }
    }
    public void EventTrigger() //use:   else if (Enumerable.SequenceEqual(dialogueData, new string[] { "NPC Object's name", "Talk Counter", "Current Line" }))
    {
        // NPC in test room runs away from you.
        if (Enumerable.SequenceEqual(dialogueData, new string[] { "Testy", "2", "1" }))
        {
            StartCoroutine(GridMove(playerScript.currentTarget, playerScript.currentTarget.transform.position + Vector3.up * 9, 4f));
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
            StartCoroutine(GridMove(playerScript.currentTarget, playerScript.currentTarget.transform.position + Vector3.down * 3, 1f));
            if (!dontAdd)
            {
                storedEvents.Add((string[])dialogueData.Clone());
            }
        }

        else if (Enumerable.SequenceEqual(dialogueData, new string[] { "Normal", "1", "0" }) || Enumerable.SequenceEqual(dialogueData, new string[] { "TreeGuy", "1", "2" }))
        {
            playerScript.GetComponent<AudioSource>().PlayOneShot(playerScript.sfx[1]);
        }
    }
    public void EndEventTrigger()
    {
        // For Follow puzzle mdoe
        if (Enumerable.SequenceEqual(dialogueData, new string[] { "Follower", "1", "0" }))
        {
            playerScript.dialogueManager.ChangeDialogue(0, false);
            playerScript.follower = null;
        }

        //For input Puzzle mode
        else if (Enumerable.SequenceEqual(dialogueData, new string[] { "TypeDude", "0", "0" }))
        {
            data2 = GetPlayerText(0);
            bool success = false;
            int distance = 0;
            string[] commands = new string[] { "n", "s", "w", "e", "u", "d", "l", "r", "north", "east", "south", "west", "up", "down", "left", "right" };
            if (data2 != "gQprk73vInHt51GHQNA8rTtilfRaNiNTxjm00IUBFd3yeplTPJ" && data2.Contains(" "))
            {
                if (commands.Contains(data2.Substring(0, data2.IndexOf(" "))))
                {
                    if (int.TryParse(data2[(data2.IndexOf(" ") + 1)..], out int x))
                    {
                        distance = x;
                        success = true;
                    }
                }
            }
            if (success)
            {
                string temp = data2.Substring(0, data2.IndexOf(" "));
                Vector3 dir = temp switch
                { 
                    "n" or "u" or "north" or "up" => Vector3.up,
                    "e" or "r" or "east" or "right" => Vector3.right,
                    "s" or "d" or "south" or "down" => Vector3.down,
                    "w" or "l" or "west" or "left" => Vector3.left,
                    _ => Vector3.zero,
                };
                bool[] walls = WallChecker(playerScript.currentTarget);
                if ((!walls[0] && dir == Vector3.up) || (!walls[1] && dir == Vector3.left) || (!walls[2] && dir == Vector3.down) || (!walls[3] && dir == Vector3.right))
                {
                    StartCoroutine(GridMove(playerScript.currentTarget, playerScript.currentTarget.transform.position + dir, 0.25f, "", distance));
                }
                playerScript.dialogueManager.ChangeDialogue(0, false);
                playerScript.currentTarget = null;
            }
            else
            {
                playerScript.dialogueManager.ChangeDialogue(0, true, dialogueData[2]);
            }
        }

        if (Enumerable.SequenceEqual(dialogueData, new string[] { "SillyButton", "0", "0" }))
        {
            playerScript.menuManager.OpenMenu();
        }
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
                if (SelectItem("Medicine", 3, 1).Item1)
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
                if (SelectItem("Photo of a Made Bed", 3, 1).Item1)
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
            else if (Enumerable.SequenceEqual(dialogueData, new string[] { "Infinite", "2", "5" }))
            {
                playerScript.dialogueManager.ChangeDialogue(2, true);
            }
            // Stickman death
            else if (Enumerable.SequenceEqual(dialogueData, new string[] { "Trash", "0", "3" }))
            {
                Destroy(playerScript.currentTarget);
            }
            // ???
            else if (Enumerable.SequenceEqual(dialogueData, new string[] { "VerySecret", "0", "6" }))
            {
                Destroy(playerScript.currentTarget);
            }

            // Funny Stuff start
            else if (Enumerable.SequenceEqual(dialogueData, new string[] { "Normal", "0", "0" }))
            {
                GameObject.Find("LevelObjects/NPCs").transform.Find("Easy").position = new Vector3(25, 0, 0);
                if (!dontAdd)
                {
                    storedEvents.Add((string[])dialogueData.Clone());
                }
            }
            else if (Enumerable.SequenceEqual(dialogueData, new string[] { "Easy", "0", "0" }))
            {
                GameObject.Find("LevelObjects/NPCs").transform.Find("Harder").position = new Vector3(25, 1, 0);
                if (!dontAdd)
                {
                    storedEvents.Add((string[])dialogueData.Clone());
                }
            }
            else if (Enumerable.SequenceEqual(dialogueData, new string[] { "Harder", "0", "0" }))
            {
                GameObject.Find("LevelObjects/NPCs").transform.Find("Insane").position = new Vector3(25, 2, 0);
                if (!dontAdd)
                {
                    storedEvents.Add((string[])dialogueData.Clone());
                }
            }
            else if (Enumerable.SequenceEqual(dialogueData, new string[] { "Insane", "0", "0" }))
            {
                GameObject.Find("LevelObjects/NPCs").transform.Find("Auto").position = new Vector3(25, 3, 0);
                if (!dontAdd)
                {
                    storedEvents.Add((string[])dialogueData.Clone());
                }
            }
            // Funny stuff end

            //Randomized Puzzle!!!
            else if (Enumerable.SequenceEqual(dialogueData, new string[] { "PuzzleRando", "0", "0" }))
            {
                UnityEngine.Object[] assets = Resources.LoadAll("Sprites", typeof(Texture2D));
                SwitchScript rand = GameObject.Find("rand").GetComponent<SwitchScript>();
                rand.switchData = playerScript.ALL_PUZZLE_MODES[UnityEngine.Random.Range(1, playerScript.ALL_PUZZLE_MODES.Length)] + " " + UnityEngine.Random.Range(1, 8) + "," + UnityEngine.Random.Range(1, 8);
                rand.puzzleImage = (Texture2D) assets[UnityEngine.Random.Range(1, assets.Length)];
            }

            //Create Puzzle!!!! It took a lot of code ;-;
            else if (Enumerable.SequenceEqual(dialogueData, new string[] { "PuzzleMaker", "0", "0" }))
            {
                tempData = GetPlayerText(0);
                bool success = false;
                if (tempData != "gQprk73vInHt51GHQNA8rTtilfRaNiNTxjm00IUBFd3yeplTPJ" && tempData.Contains(" "))
                {
                    if (playerScript.ALL_PUZZLE_MODES.Contains(tempData.Substring(0, tempData.IndexOf(" "))))
                    {
                        if (int.TryParse(tempData.Substring(tempData.IndexOf(" ") + 1, 1), out int x) && tempData.Substring(tempData.IndexOf(" ") + 2, 1) == "," && int.TryParse(tempData.Substring(tempData.IndexOf(" ") + 3, 1), out int y))
                        {
                            if (x > 0 && x < 8 && y > 0 && y < 8)
                            {
                                success = true;
                            }
                        }
                    }
                }
                if (success)
                {
                    playerScript.currentTarget.GetComponent<SignTextScript>().talkCounter = 2;
                    playerScript.dialogueManager.StartDialogue(playerScript.currentTarget.name, playerScript.currentTarget.GetComponent<SignTextScript>().dialogue, 2, playerScript.currentTarget.GetComponent<SignTextScript>().talkerImage);
                }
                else
                {
                    playerScript.dialogueManager.ChangeDialogue(0, true, dialogueData[2]);
                }
            }
            else if (Enumerable.SequenceEqual(dialogueData, new string[] { "PuzzleMaker", "2", "0" }))
            {
                tempImage = SelectItem("", 4, 0, true, true).Item2;
            }
            else if (Enumerable.SequenceEqual(dialogueData, new string[] { "PuzzleMaker", "0", "1" }) || Enumerable.SequenceEqual(dialogueData, new string[] { "PuzzleMaker", "2", "1" }))
            {
                playerScript.dialogueManager.ChangeDialogue(0, false);
            }
            else if (Enumerable.SequenceEqual(dialogueData, new string[] { "PuzzleMaker", "4", "0" }))
            {
                SwitchScript make = GameObject.Find("make").GetComponent<SwitchScript>();
                make.switchData = tempData;
                make.puzzleImage = tempImage;
                playerScript.dialogueManager.ChangeDialogue(0, false);
            }
        }

    }
    public (bool,Texture2D) SelectItem(string wantedItem, int successDialogueCounter, int failDialogueCounter, bool dontRemoveItem = false, bool takeAnyItem = false) 
    {
        if (playerScript.selection == null)
        {
            playerScript.dialogueManager.ChangeDialogue(failDialogueCounter, false);
            playerScript.selectingItem = true;
            playerScript.invManager.OpenInventory();
        }
        else if (playerScript.selection.CompareTag("Item") && (playerScript.selection.GetComponent<ItemScript>().itemName == wantedItem || takeAnyItem))
        {
            playerScript.currentTarget.GetComponent<SignTextScript>().talkCounter = successDialogueCounter;
            playerScript.dialogueManager.StartDialogue(playerScript.currentTarget.name, playerScript.currentTarget.GetComponent<SignTextScript>().dialogue, successDialogueCounter, playerScript.currentTarget.GetComponent<SignTextScript>().talkerImage);
            if (!dontRemoveItem)
            {
                playerScript.invManager.inventory.Remove(playerScript.GetItem(wantedItem));playerScript.selection = null;
                playerScript.selection = null;
                return (true, null);
            }
            else
            {
                Sprite sprite = playerScript.selection.GetComponent<ItemScript>().itemImage;
                Texture2D croppedTexture = new((int)sprite.rect.width, (int)sprite.rect.height);
                croppedTexture.SetPixels32(sprite.texture.GetPixels32());
                croppedTexture.Apply();
                playerScript.selection = null;
                return (true, croppedTexture);
            }
            
            
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
        return (false, null);
    }

    public string GetPlayerText(int failDialogueCounter)
    {
        if (playerScript.input == "gQprk73vInHt51GHQNA8rTtilfRaNiNTxjm00IUBFd3yeplTPJ")
        {
            playerScript.dialogueManager.ChangeDialogue(failDialogueCounter, false);
            playerScript.menuManager.OpenInput();
        }
        else
        {
            string temp = playerScript.input;
            playerScript.input = "gQprk73vInHt51GHQNA8rTtilfRaNiNTxjm00IUBFd3yeplTPJ";
            return temp;
        }
        return "";
    }
}

