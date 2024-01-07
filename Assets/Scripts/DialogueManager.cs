using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DialogueManager : MonoBehaviour
{
    public TMP_Text dialogueText;
    public TMP_Text nameText;
    public TMP_Text spaceText;
    public bool typing;
    public float typingSpeed = 0.03f;
    public PlayerScript2D playerScript;
    public DialogueEvents eventScript;
    public Canvas textBox;

    public Queue<string> sentences;
    void Start()
    {
        textBox.GetComponent<Canvas>().enabled = false;
        sentences = new Queue<string>();
    }

    public void StartDialogue(string dialogueName, string[] dialogue)
    {
        sentences.Clear();
        playerScript.inDialogue = true;
        eventScript.dialogueData[0] = dialogueName;
        eventScript.dialogueData[1] = "-1";
        textBox.GetComponent<Canvas>().enabled = true;
        foreach (string sentence in dialogue)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        eventScript.dialogueData[1] = (Int32.Parse(eventScript.dialogueData[1]) + 1).ToString();
        spaceText.text = "";
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        nameText.text = sentence.Substring(0,sentence.IndexOf(":")+1);
        eventScript.EventTrigger();
        StartCoroutine(TypeSentence(sentence.Substring(sentence.IndexOf(":")+1)));
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
}