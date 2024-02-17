using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherCarMove : MonoBehaviour
{

    public float deathTimer;
    void Start()
    {
        transform.Rotate(0, 180, 0);
    }

    // Update is called once per frame
    void Update()
    {
        deathTimer += Time.deltaTime;
        transform.Translate(-0.8f, 0, 0);
        
        if (deathTimer >= 10f)
        {
            Destroy(gameObject);
        }


    }


}
