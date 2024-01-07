using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    public GameObject Car;

    public Vector3 CarArea;
    
    public float timer;
    public float CarSpawn;

    
    void Start()
    {
        CarSpawn = Random.Range(6, 15);
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        CarArea.x = 157;
        CarArea.z = 2;
        

        if(timer >= CarSpawn)
        {
            Instantiate(Car, CarArea, Quaternion.identity);
            CarSpawn = Random.Range(6, 15);
            timer = 0;
        }
    }
}
