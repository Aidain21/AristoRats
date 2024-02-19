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
    public float typingSpeed = 0.03f;
    public bool hasMoreText;
    public PlayerScript2D playerScript;
    public DialogueEvents eventScript;
    public Canvas textBox;
    public GameObject background;
    public Image imageFrame;
    public Sprite currentImage; //just the talker for now

    public List<string> sentences;
    void Start()
    {
        textBox.GetComponent<Canvas>().enabled = false;
        sentences = new List<string>();
    }

    public void StartDialogue(string name, string[] dialogue, int talkCounter, Sprite talkerImage)
    {
        typingSpeed = 2 * ((5 - playerScript.menuManager.optionSelector.selections[2].x)  / 100) - 0.01f; 
        hasMoreText = false;
        sentences.Clear();
        playerScript.inDialogue = true;
        eventScript.dialogueData[0] = name;
        eventScript.dialogueData[1] = talkCounter.ToString();
        eventScript.dialogueData[2] = "-1";
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
            }
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
        StopAllCoroutines();
        
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
    public void ChangeDialogue(int talkCounter, bool keepTalking)
    {
        EndDialogue();
        playerScript.currentTarget.GetComponent<SignTextScript>().talkCounter = talkCounter;
        eventScript.dialogueData[2] = "-1";
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
        if (hasMoreText)
        {
            playerScript.currentTarget.GetComponent<SignTextScript>().talkCounter += 1;
            if (playerScript.currentTarget != null && playerScript.currentTarget.CompareTag("Sign") && playerScript.currentTarget.GetComponent<SpriteRenderer>().sprite.name == "Circle")
            {
                playerScript.currentTarget.GetComponent<SpriteRenderer>().color = Color.yellow;
            }
        }
        else
        {
            if (playerScript.currentTarget != null && playerScript.currentTarget.CompareTag("Sign") && playerScript.currentTarget.GetComponent<SpriteRenderer>().sprite.name == "Circle")
            {
                playerScript.currentTarget.GetComponent<SpriteRenderer>().color = Color.blue;
            }
        }
        playerScript.inDialogue = false;
        textBox.GetComponent<Canvas>().enabled = false;
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