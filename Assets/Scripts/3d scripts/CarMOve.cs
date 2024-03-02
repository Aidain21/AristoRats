using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMOve : MonoBehaviour
{
    
    public float deathTimer;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        deathTimer += Time.deltaTime;
        transform.Translate(-0.6f, 0, 0);
        if(deathTimer >= 10f)
        {
            Destroy(gameObject);
        }

        
    }

    
}
