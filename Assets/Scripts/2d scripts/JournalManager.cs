using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class JournalManager : MonoBehaviour
{
    public Vector2 selectorPos;
    public Vector2 prevSelectorPos;
    public Vector2 selection;
    public Vector2 prevSelection;
    public TMP_Text[,] textArray = new TMP_Text[11, 5];
    public List<NoteScript> notes;
    public GameObject TextArray;
    public TMP_Text def;
    public TMP_Text noteTitle;
    public TMP_Text noteLore;
    public PlayerScript2D playerScript;
    public Canvas journal;

    // Start is called before the first frame update
    void Start()
    {
        journal.GetComponent<Canvas>().enabled = false;

        selectorPos = Vector2.zero;
        selection = Vector2.zero;
        prevSelectorPos = Vector2.up;
        prevSelection = Vector2.up;
        for (int i = 0; i < textArray.GetLength(0); i++)
        {
            for (int j = 0; j < textArray.GetLength(1); j++)
            {
                TMP_Text text = Instantiate(def, TextArray.transform);
                text.rectTransform.localPosition = new Vector2(150 * j, -75 * i);
                text.name = (i * textArray.GetLength(1) + j + 1).ToString();
                textArray[i, j] = text;
            }
        }
        Destroy(def);
    }
    public void OpenJournal()
    {
        journal.GetComponent<Canvas>().enabled = true;
        playerScript.inJournal = true;
        for (int i = 0; i < textArray.GetLength(0); i++)
        {
            for (int j = 0; j < textArray.GetLength(1); j++)
            {
                if (HasNote(i * textArray.GetLength(1) + j))
                {
                    textArray[i, j].text = "#" + textArray[i, j].name;
                }
                else
                {
                    textArray[i, j].text = "???";
                }
            }
        }
        UpdateSelector();
        UpdateRightSide();
    }

    public void CloseJournal()
    {
        journal.GetComponent<Canvas>().enabled = false;
        playerScript.inJournal = false;
    }
    public void UpdateSelector()
    {
        textArray[Mathf.RoundToInt(selectorPos.y), Mathf.RoundToInt(selectorPos.x)].text = "<mark color=#FFFFFF50 padding=15,15,15,15>" + textArray[Mathf.RoundToInt(selectorPos.y), Mathf.RoundToInt(selectorPos.x)].text;
        textArray[Mathf.RoundToInt(prevSelectorPos.y), Mathf.RoundToInt(prevSelectorPos.x)].text = textArray[Mathf.RoundToInt(prevSelectorPos.y), Mathf.RoundToInt(prevSelectorPos.x)].text.Replace("<mark color=#FFFFFF50 padding=15,15,15,15>","");
        textArray[Mathf.RoundToInt(selection.y), Mathf.RoundToInt(selection.x)].color = Color.yellow;
        textArray[Mathf.RoundToInt(selection.y), Mathf.RoundToInt(selection.x)].fontStyle = FontStyles.Bold;
        textArray[Mathf.RoundToInt(prevSelection.y), Mathf.RoundToInt(prevSelection.x)].color = Color.white;
        textArray[Mathf.RoundToInt(prevSelection.y), Mathf.RoundToInt(prevSelection.x)].fontStyle = FontStyles.Normal;
    }
    public void UpdateRightSide()
    {
        NoteScript curNote = GetNote(Mathf.RoundToInt(selection.y) * textArray.GetLength(1) + Mathf.RoundToInt(selection.x));
        if (curNote != null)
        {
            noteTitle.text = "#" + (curNote.noteId + 1) + " - " + curNote.noteTitle;
            noteLore.text = curNote.noteLore;
        }
        else
        {
            noteTitle.text = "#?? - Missing Note";
            noteLore.text = "I don't have this note yet.";
        }
    }
    public bool HasNote(int noteId)
    {
        foreach(NoteScript n in notes)
        {
            if (n.noteId == noteId)
            {
                return true;
            }
        }
        return false;
    }
    public NoteScript GetNote(int noteId)
    {
        foreach (NoteScript n in notes)
        {
            if (n.noteId == noteId)
            {
                return n;
            }
        }
        return null;
    }
}