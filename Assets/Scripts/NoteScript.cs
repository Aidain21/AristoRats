using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class NoteScript : MonoBehaviour
{
    public string noteTitle;
    public string noteLore;
    public int noteId;
    public string scene;
    public Vector3 pos;
    public Texture2D image;

    void Start()
    {
        foreach (NoteScript n in GameObject.Find("JournalManager").GetComponent<JournalManager>().notes)
        {
            if (n.noteId == noteId)
            {
                Destroy(gameObject);
            }
        }
        scene = SceneManager.GetActiveScene().name;
        pos = transform.position;
    }
}
