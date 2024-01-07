using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawnNearMansion : MonoBehaviour
{
    public GameObject Car;

    public Vector3 CarArea;
    
    public float timer;
    public float CarSpawn;


    void Start()
    {
        CarSpawn = Random.Range(10, 20);

    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        CarArea.x = -125;
        CarArea.z = -16;
        

        if (timer >= CarSpawn)
        {
            Instantiate(Car, CarArea, Quaternion.identity);
            CarSpawn = Random.Range(10, 20);
            timer = 0;
        }
    }
}
