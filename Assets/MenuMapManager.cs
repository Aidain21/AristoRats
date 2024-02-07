using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuMapManager : MonoBehaviour
{
    public Vector2 selectorPos;
    public Vector2 prevSelectorPos;
    public Vector2 selection;
    public Vector2 prevSelection;
    public int[] settings;
    public Canvas menu;
    public Canvas options;
    public Canvas map;
    public TMP_Text[,] textArray;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    
    public void UpdateSelector()
    {
        textArray[Mathf.RoundToInt(selectorPos.y), Mathf.RoundToInt(selectorPos.x)].text = "<mark color=#FFFFFF50 padding=15,15,15,15>" + textArray[Mathf.RoundToInt(selectorPos.y), Mathf.RoundToInt(selectorPos.x)].text;
        textArray[Mathf.RoundToInt(prevSelectorPos.y), Mathf.RoundToInt(prevSelectorPos.x)].text = textArray[Mathf.RoundToInt(prevSelectorPos.y), Mathf.RoundToInt(prevSelectorPos.x)].text.Replace("<mark color=#FFFFFF50 padding=15,15,15,15>", "");
        textArray[Mathf.RoundToInt(selection.y), Mathf.RoundToInt(selection.x)].color = Color.yellow;
        textArray[Mathf.RoundToInt(selection.y), Mathf.RoundToInt(selection.x)].fontStyle = FontStyles.Bold;
        textArray[Mathf.RoundToInt(prevSelection.y), Mathf.RoundToInt(prevSelection.x)].color = Color.white;
        textArray[Mathf.RoundToInt(prevSelection.y), Mathf.RoundToInt(prevSelection.x)].fontStyle = FontStyles.Normal;
    }
}
