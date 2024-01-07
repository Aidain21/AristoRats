using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImagePuzzleScript : MonoBehaviour
{
    public Sprite[] pieces;
    public GameObject pushBlock;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PuzzleSetUp()
    {
        
        int[] norm = new int[16];
        for (int i = 0; i < 16; i++)
        {
            norm[i] = i+1;
        }
        for (int t = 0; t < norm.Length; t++)
        {
            int tmp = norm[t];
            int r = Random.Range(t, norm.Length);
            norm[t] = norm[r];
            norm[r] = tmp;
        }
        for (int i = 0; i < 16; i++)
        { 
            GameObject piece = Instantiate(pushBlock, transform.position, Quaternion.identity);
            piece.GetComponent<BlockScript>().id = norm[i];
            piece.GetComponent<SpriteRenderer>().sprite = pieces[norm[i]-1];
            piece.GetComponent<Transform>().position = new Vector2(transform.position.x + (i-(6+(i%4)))/2, transform.position.y+(i%4)*2+3);
        }
        Debug.Log(string.Join(", ", norm));
    }
}
