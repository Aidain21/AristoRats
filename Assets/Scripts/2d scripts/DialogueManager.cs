using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DialogueManager : MonoBehaviour
{
    public TMP_Text dialogueText;
    public TMP_Text nameText;
    public TMP_Text spaceText;
    public bool typing;
    public bool changed;
    public bool keepTalkerInfo;
    public bool talkingToNPC;
    public float typingSpeed = 0.03f;
    public bool hasMoreText;
    public bool hiddenText;
    public bool isRiddle;
    public PlayerScript2D playerScript;
    public DialogueEvents eventScript;
    public Canvas textBox;
    public GameObject background;
    public Image imageFrame;
    public Sprite currentImage; //just the talker for now

    public Sprite hasNewText;
    public Sprite hasHiddenText;
    public Sprite noMoreText;

    public Sprite storedStatus;

    public List<string> sentences;
    void Start()
    {
        textBox.GetComponent<Canvas>().enabled = false;
        sentences = new List<string>();
    }

    public void StartDialogue(string name, string[] dialogue, int talkCounter, Sprite talkerImage)
    {
        talkingToNPC = playerScript.currentTarget != null && playerScript.currentTarget.CompareTag("Sign");
        typingSpeed = 2 * ((5 - playerScript.menuManager.optionSelector.selections[2].x)  / 100) - 0.01f; 
        hasMoreText = false;
        hiddenText = false;
        sentences.Clear();
        playerScript.inDialogue = true;
        eventScript.dialogueData[0] = name;
        eventScript.dialogueData[1] = talkCounter.ToString();
        if (!changed)
        {
            eventScript.dialogueData[2] = "-1";
        }
        textBox.GetComponent<Canvas>().enabled = true;
        PositionBox();
        currentImage = talkerImage;
        foreach (string sentence in dialogue)
        {
            int index = 0;
            for (int ctr = 0; ctr < sentence.Length; ctr++)
            {
                if (!Char.IsDigit(sentence[ctr]))
                {
                    index = ctr;
                    break;
                }
            }
            if (index == 0)
            {
                sentences.Add("Aidan:Fix up the formatting of the text plz something is missing the talk counter :P");
                break;
            }
            else if (Int32.Parse(sentence.Substring(0, index)) == talkCounter)
            {
                sentences.Add(sentence[index..]);
            }
            if (Int32.Parse(sentence.Substring(0, index)) == talkCounter + 1)
            {
                hasMoreText = true;
                hiddenText = false;
            }
            else if (!hasMoreText && Int32.Parse(sentence.Substring(0, index)) > talkCounter + 1)
            {
                hiddenText = true;
            }
        }
        if (changed && eventScript.dialogueData[2] != "-1")
        {
            int num = Int32.Parse(eventScript.dialogueData[2]);
            for (int i = 0; i < num + 1; i++)
            {
                sentences.RemoveAt(0);
            }
            changed = false;
        }
        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        
        eventScript.dialogueData[2] = (Int32.Parse(eventScript.dialogueData[2]) + 1).ToString();
        spaceText.text = "";
        
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences[0];
        if (sentence[0] == '#')
        {
            sentence = sentence[1..];
            dialogueText.color = Color.magenta;
            dialogueText.fontStyle = FontStyles.Bold;
        }
        else
        {
            dialogueText.color = Color.white;
            dialogueText.fontStyle = FontStyles.Normal;
        }
        StopAllCoroutines();

        if (hasMoreText)
        {
            storedStatus = hasNewText;
        }
        else if (hiddenText)
        {
            storedStatus = hasHiddenText;
        }
        else
        {
            storedStatus = noMoreText;
        }

        nameText.text = sentence.Substring(0,sentence.IndexOf(":")+1);
        if (nameText.text == "")
        {
            nameText.text = "Aidan:";
            StartCoroutine(TypeSentence("Fix up the formatting of the text plz its missing the semicolon after the name :P"));
        }
        else if (nameText.text == "You:")
        {
            nameText.text = playerScript.playerName + ":";
            imageFrame.sprite = playerScript.GetComponent<SpriteRenderer>().sprite;
            eventScript.EventTrigger();
            if (sentence[sentence.IndexOf(":") + 1] == ' ')
            {
                StartCoroutine(TypeSentence(sentence[(sentence.IndexOf(":") + 2)..]));
            }
            else
            {
                StartCoroutine(TypeSentence(sentence[(sentence.IndexOf(":") + 1)..]));
            }  
        }
        else
        {
            if (playerScript.currentTarget != null && playerScript.currentTarget.CompareTag("Sign") && playerScript.currentTarget.GetComponent<Animator>().runtimeAnimatorController != null)
            {
                playerScript.currentTarget.GetComponent<Animator>().enabled = true;
                playerScript.currentTarget.GetComponent<Animator>().Play("Talk");
            }
            imageFrame.sprite = currentImage;
            eventScript.EventTrigger();
            if (sentence[sentence.IndexOf(":") + 1] == ' ')
            {
                StartCoroutine(TypeSentence(sentence[(sentence.IndexOf(":") + 2)..]));
            }
            else
            {
                StartCoroutine(TypeSentence(sentence[(sentence.IndexOf(":") + 1)..]));
            }
        }
    }
    public void ChangeDialogue(int talkCounter, bool keepTalking, string continueLine = "-1")
    {
        keepTalkerInfo = true;
        EndDialogue();
        playerScript.currentTarget.GetComponent<SignTextScript>().talkCounter = talkCounter;
        eventScript.dialogueData[2] = continueLine;
        if (keepTalking)
        {
            changed = true;
            StartDialogue(eventScript.dialogueData[0], playerScript.currentTarget.GetComponent<SignTextScript>().dialogue, talkCounter, currentImage);
        }
    }


    public IEnumerator TypeSentence(string sentence)
    {
        typing = true;
        dialogueText.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            if (!typing)
            {
                break;
            }
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        dialogueText.text = sentence;
        typing = false;
        yield return new WaitForSeconds(1.5f);
        spaceText.text = "Space to Continue";
    }

    void EndDialogue()
    {
        if (playerScript.currentTarget != null && playerScript.currentTarget.CompareTag("Sign") && talkingToNPC)
        {
            if (playerScript.currentTarget.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite != noMoreText && storedStatus == noMoreText)
            {
                eventScript.fullyTalkedTo += 1;
            }
            playerScript.currentTarget.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = storedStatus;
            int alreadyTracked = -1;
            for (int i = 0; i < eventScript.storedTalks.Count; i++)
            {
                if (eventScript.storedTalks[i][0] == eventScript.dialogueData[0])
                {
                    alreadyTracked = i;
                }
            }
            if (alreadyTracked != -1)
            {
                eventScript.storedTalks[alreadyTracked] = (string[])eventScript.dialogueData.Clone();
                eventScript.talkerStats[alreadyTracked] = playerScript.currentTarget.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
            }
            else
            {
                eventScript.storedTalks.Add((string[])eventScript.dialogueData.Clone());
                eventScript.talkerStats.Add(playerScript.currentTarget.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite);
            }
            
            talkingToNPC = false;
        }
        if (hasMoreText)
        {
            playerScript.currentTarget.GetComponent<SignTextScript>().talkCounter += 1;
        }
        playerScript.inDialogue = false;
        textBox.GetComponent<Canvas>().enabled = false;
        if (!keepTalkerInfo)
        {
            playerScript.currentTarget = null;
        }
        keepTalkerInfo = false;
    }

    public void PositionBox()
    {
        if (playerScript.aboveTalker)
        {
            background.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 75, 300);
            imageFrame.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 75, 300);
        }
        else
        {
            background.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 75, 300);
            imageFrame.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 75, 300);
        }
    }
}