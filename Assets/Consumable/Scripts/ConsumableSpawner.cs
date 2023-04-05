using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ConsumableSpawner : MonoBehaviour
{
    [SerializeField] Vector3 spawnAreaSize = Vector3.one * 3f;
    [SerializeField] Consumable template;

    [Range(0, 30)]
    [SerializeField] int MaxCount;
    [SerializeField] bool ShowDebug = true;

    List<GameObject> liveConsumables = new List<GameObject>();

    void Start()
    {
        for (var i = 0; i < MaxCount; ++i)
        {
            var consumable = Instantiate(template, transform);
            consumable.spawner = this;
            consumable.transform.rotation = Quaternion.identity;
            if (MoveToRandomLocation(consumable))
            {
                liveConsumables.Add(consumable.gameObject);
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, spawnAreaSize);
        Gizmos.color = Color.yellow;
    }

    /// Create a new random position in the spawn area relative to this.
    Vector3 GetRandomPosition()
    {
        var min = spawnAreaSize * -0.5f;
        var max = spawnAreaSize * 0.5f;
        return new Vector3(UnityEngine.Random.Range(min.x, max.x), max.y, UnityEngine.Random.Range(min.z, max.z));
    }

    bool MoveToRandomLocation(Consumable consumable, int retryCount = 3)
    {
        for (; ; )
        {
            consumable.transform.localPosition = GetRandomPosition();
            var dist = spawnAreaSize.y * 2;
            RaycastHit hit;
            if (Physics.Raycast(consumable.transform.position, Vector3.down, out hit, spawnAreaSize.y * 2))
            {
                consumable.transform.position = hit.point;
                return true;
            }
            retryCount -= 1;
            if (retryCount < 0)
            {
                return false;
            }
        }
    }

    public void Consumed(Consumable consumable)
    {
        Assert.IsTrue(liveConsumables.Contains(consumable.gameObject));
        if (!MoveToRandomLocation(consumable))
        {
            liveConsumables.Remove(consumable.gameObject);
        }
    }

    void OnGUI()
    {
        if (!ShowDebug)
            return;

        DebugHUD.AddLine($"Consumables: {MaxCount}/{liveConsumables.Count}");
    }
}

