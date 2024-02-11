using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Selector
{
    public Vector2 selectorPos;
    public Vector2 prevSelectorPos;
    public Vector2 selection;
    public Vector2[] selections;
    public Vector2 prevSelection;
    public Vector2[] prevSelections;
    public TMP_Text[][] textArray;
    public int width;
    public int height;
    public Selector(int x, int y)
    {
        selectorPos = Vector2.zero;
        selection = Vector2.zero;
        prevSelectorPos = Vector2.zero;
        prevSelection = Vector2.zero;
        textArray = new TMP_Text[y][];
        selections = new Vector2[0];
        prevSelections = new Vector2[0];
        width = x;
        height = y;
        for (int i = 0; i < textArray.Length; i++)
        {
            textArray[i] = new TMP_Text[x];
        }
    }
    public Selector(int[] x, int y)
    {
        selectorPos = Vector2.zero;
        prevSelectorPos = Vector2.zero;
        textArray = new TMP_Text[y][];
        selections = new Vector2[y];
        prevSelections = new Vector2[y];
        height = y;
        for (int i = 0; i < textArray.Length; i++)
        {
            textArray[i] = new TMP_Text[x[i]];
            selections[i] = new Vector2(0, i);
            prevSelections[i] = new Vector2(0, i);
        }
        width = textArray[0].Length;
    }
    public void UpdateSelector()
    {
        textArray[Mathf.RoundToInt(selectorPos.y)][Mathf.RoundToInt(selectorPos.x)].text = "<mark color=#FFFFFF50 padding=15,15,15,15>" + textArray[Mathf.RoundToInt(selectorPos.y)][Mathf.RoundToInt(selectorPos.x)].text;
        if (prevSelectorPos != selectorPos)
        {
            textArray[Mathf.RoundToInt(prevSelectorPos.y)][Mathf.RoundToInt(prevSelectorPos.x)].text = textArray[Mathf.RoundToInt(prevSelectorPos.y)][Mathf.RoundToInt(prevSelectorPos.x)].text.Replace("<mark color=#FFFFFF50 padding=15,15,15,15>", "");
        }
        if (selections.Length == 0)
        {
            textArray[Mathf.RoundToInt(selection.y)][Mathf.RoundToInt(selection.x)].color = Color.yellow;
            textArray[Mathf.RoundToInt(selection.y)][Mathf.RoundToInt(selection.x)].fontStyle = FontStyles.Bold;
            if(selection != prevSelection)
            {
                textArray[Mathf.RoundToInt(prevSelection.y)][Mathf.RoundToInt(prevSelection.x)].color = Color.white;
                textArray[Mathf.RoundToInt(prevSelection.y)][Mathf.RoundToInt(prevSelection.x)].fontStyle = FontStyles.Normal;
            }
        }
        else if (selections.Length > 0)
        {
            textArray[Mathf.RoundToInt(selections[Mathf.RoundToInt(selectorPos.y)].y)][Mathf.RoundToInt(selections[Mathf.RoundToInt(selectorPos.y)].x)].color = Color.yellow;
            if (selections[Mathf.RoundToInt(selectorPos.y)] != prevSelections[Mathf.RoundToInt(selectorPos.y)])
            {
                textArray[Mathf.RoundToInt(prevSelections[Mathf.RoundToInt(selectorPos.y)].y)][Mathf.RoundToInt(prevSelections[Mathf.RoundToInt(selectorPos.y)].x)].color = Color.white;
            }
            width = textArray[Mathf.RoundToInt(selectorPos.y)].Length;
        }
        
    }
}

public class JournalManager : MonoBehaviour
{
    public Selector selector = new(5, 11);
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
        int count = 0;
        for (int i = 0; i < selector.textArray.Length; i++)
        {
            for (int j = 0; j < selector.textArray[i].Length; j++)
            {
                count += 1;
                TMP_Text text = Instantiate(def, TextArray.transform);
                text.rectTransform.localPosition = new Vector2(150 * j, -75 * i);

                text.name = count.ToString();
                selector.textArray[i][j] = text;
            }
        }
        Destroy(def);
    }
    public void OpenJournal()
    {
        journal.GetComponent<Canvas>().enabled = true;
        playerScript.inJournal = true;
        for (int i = 0; i < selector.textArray.Length; i++)
        {
            for (int j = 0; j < selector.textArray[i].Length; j++)
            {
                if (HasNote(i * selector.textArray[i].Length + j))
                {
                    selector.textArray[i][j].text = "#" + selector.textArray[i][j].name;
                }
                else
                {
                    selector.textArray[i][j].text = "???";
                }
            }
        }
        selector.UpdateSelector();
        UpdateRightSide();
    }

    public void CloseJournal()
    {
        journal.GetComponent<Canvas>().enabled = false;
        playerScript.inJournal = false;
    }
    public void UpdateRightSide()
    {
        NoteScript curNote = GetNote(Mathf.RoundToInt(selector.selection.y) * selector.textArray[0].Length + Mathf.RoundToInt(selector.selection.x));
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