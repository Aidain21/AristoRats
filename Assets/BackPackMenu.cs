using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackPackMenu : MonoBehaviour
{
    public GameObject backpackMenu;
    public DialogueManager dialogueManager;
    public InventoryManager invManager;
    public JournalManager journalManager;

    public bool inMap;
    public bool inJournal;
    public bool inDialogue;
    public bool inInventory;
    // Start is called before the first frame update
    void Start()
    {
        backpackMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            backpackMenu.SetActive(false);
        }
    }

    public void backpackButton()
    {
        backpackMenu.SetActive(true);
    }

    public void inventory()
    {
        invManager.OpenInventory();
    }

    public void JournelButton()
    {
        journalManager.OpenJournal();
    }
}
