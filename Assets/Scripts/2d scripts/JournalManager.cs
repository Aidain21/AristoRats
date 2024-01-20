using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class JournalManager : MonoBehaviour
{
    public Vector2 selectorPos;
    public Vector2 prevSelect;
    public TMP_Text[,] textArray = new TMP_Text[5,5];
    public bool[] hasNote = new bool[25];
    public GameObject TextArray;
    public TMP_Text def;
    public PlayerScript2D playerScript;
    public Canvas journal;

    // Start is called before the first frame update
    void Start()
    {
        journal.GetComponent<Canvas>().enabled = false;

        selectorPos = Vector2.zero;
        for (int i = 0; i < textArray.GetLength(0); i++)
        {
            for (int j = 0; j < textArray.GetLength(1); j++)
            {
                TMP_Text text = Instantiate(def, TextArray.transform);
                text.rectTransform.localPosition = new Vector2(55 * j, -55 * i);
                text.name = (i * textArray.GetLength(0) + j + 1).ToString();
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
                if (hasNote[i * textArray.GetLength(0) + j])
                {
                    textArray[i, j].text = "#" + (i * textArray.GetLength(0) + j + 1).ToString();
                }
                else
                {
                    textArray[i, j].text = "???";
                }
            }
        }
        UpdateSelector();
    }

    public void CloseJournal()
    {
        journal.GetComponent<Canvas>().enabled = false;
        playerScript.inJournal = false;
    }
    public void UpdateSelector()
    {
        textArray[Mathf.RoundToInt(selectorPos.y), Mathf.RoundToInt(selectorPos.x)].color = Color.yellow;
        textArray[Mathf.RoundToInt(prevSelect.y), Mathf.RoundToInt(prevSelect.x)].color = Color.white;
    }
}
