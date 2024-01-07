using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockScript : MonoBehaviour
{
    public bool moving;
    public IEnumerator move;
    public bool[] walls = { false, false, false, false }; //ULDR
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        moving = false;
    }

    // Update is called once per frame
    void Update()
    {
        walls = WallChecker();
    }
    public IEnumerator Moving(Vector2 goal)
    {
        while (new Vector2(transform.position.x, transform.position.y) != goal)
        {
            moving = true;
            transform.position = Vector2.MoveTowards(transform.position, goal, 2.5f * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), 0);
        moving = false;
    }

    public void Push(Vector3 direction)
    {
        if((!walls[0] && direction == Vector3.up) || (!walls[1] && direction == Vector3.left) || (!walls[2] && direction == Vector3.down) || (!walls[3] && direction == Vector3.right))
        {
            StartCoroutine(Moving(transform.position + direction));
        }
        else if (!player.GetComponent<PlayerScript2D>().moving)
        {
            StartCoroutine(Moving(transform.position - direction));
            StartCoroutine(player.GetComponent<PlayerScript2D>().GridMove(player, player.transform.position - direction, 0.3f));
        }
        
    }
    private bool[] WallChecker()
    {
        RaycastHit2D hitData = Physics2D.Raycast(transform.position + Vector3.up * 0.51f, Vector2.up, 0.5f);
        bool up = hitData.collider != null;
        hitData = Physics2D.Raycast(transform.position + Vector3.left * 0.51f, Vector2.left, 0.5f);
        bool left = hitData.collider != null;
        hitData = Physics2D.Raycast(transform.position + Vector3.down * 0.51f, Vector2.down, 0.5f);
        bool down = hitData.collider != null;
        hitData = Physics2D.Raycast(transform.position + Vector3.right * 0.51f, Vector2.right, 0.5f);
        bool right = hitData.collider != null;
        return new bool[] { up, left, down, right };
    }

}
