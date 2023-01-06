using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumable : MonoBehaviour
{
    public ConsumableSpawner spawner;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Consume() 
    {
        if(spawner != null) {
            spawner.Consumed(this);
        } else {
            Destroy(this);
        }
    }
}
