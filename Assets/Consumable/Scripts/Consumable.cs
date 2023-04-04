using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumable : MonoBehaviour
{
    public ConsumableSpawner spawner;
    public float pivotY;

    public void Consume() 
    {
        if(spawner != null) {
            spawner.Consumed(this);
        } else {
            Destroy(this);
        }
    }
}
