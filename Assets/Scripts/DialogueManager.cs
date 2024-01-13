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
    public float typingSpeed = 0.03f;
    public bool hasMoreText;
    public PlayerScript2D playerScript;
    public DialogueEvents eventScript;
    public Canvas textBox;
    public GameObject background;
    public Image imageFrame;
    public Sprite currentImage; //just the talker for now

    public Queue<string> sentences;
    void Start()
    {
        textBox.GetComponent<Canvas>().enabled = false;
        sentences = new Queue<string>();
    }

    public void StartDialogue(string dialogueName, string[] dialogue, int talkCounter, Sprite talkerImage)
    {
        hasMoreText = false;
        sentences.Clear();
        playerScript.inDialogue = true;
        eventScript.dialogueData[0] = dialogueName;
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
                sentences.Enqueue("Aidan:Fix up the formatting of the text plz something is missing the talk counter :P");
                break;
            }
            else if (Int32.Parse(sentence.Substring(0, index)) == talkCounter)
            {
                sentences.Enqueue(sentence.Substring(index));
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

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        imageFrame.sprite = currentImage;
        nameText.text = sentence.Substring(0,sentence.IndexOf(":")+1);
        if (nameText.text == "")
        {
            nameText.text = "Aidan:";
            StartCoroutine(TypeSentence("Fix up the formatting of the text plz its missing the semicolon after the name :P"));
        }
        else
        {
            eventScript.EventTrigger();
            StartCoroutine(TypeSentence(sentence.Substring(sentence.IndexOf(":") + 1)));
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
        playerScript.inDialogue = false;
        textBox.GetComponent<Canvas>().enabled = false;
    }

    public void PositionBox()
    {
        if (playerScript.aboveTalker)
        {
            background.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 25 , 125);
        }
        else
        {
            background.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 25, 125);
        }
    }
}