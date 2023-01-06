using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ConsumableSpawner: MonoBehaviour
{
    [SerializeField] Vector3 spawnAreaSize = Vector3.one * 3f;
    [SerializeField] Consumable template;

    [Range(0,10)]
    [SerializeField] int count;

    List<GameObject> liveConsumables = new List<GameObject>();

    void Start()
    {
        while(liveConsumables.Count < count) {
            var consumable = Instantiate(template, transform);
            consumable.spawner = this;
            consumable.transform.localPosition  = GetRandomPosition(); 
            consumable.transform.rotation = Quaternion.identity;       
            liveConsumables.Add(consumable.gameObject);
        }
    }   
  
    void OnDrawGizmos() {        
        Gizmos.DrawWireCube(transform.position, spawnAreaSize);
        Gizmos.color = Color.yellow;
    }

    /// Create a new random poistion in the spawn area relative to this.
    Vector3 GetRandomPosition() {
        var min = spawnAreaSize * -0.5f;
        var max = spawnAreaSize * 0.5f;
        return new Vector3(UnityEngine.Random.Range(min.x, max.x), UnityEngine.Random.Range(min.y, max.y), UnityEngine.Random.Range(min.z, max.z));
    }

     public void Consumed(Consumable consumable) {
        Assert.IsTrue(liveConsumables.Contains(consumable.gameObject));
        consumable.transform.localPosition = GetRandomPosition();        
    }
}

