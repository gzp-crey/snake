using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[DisallowMultipleComponent]
public class ConsumableSpawner : NetworkBehaviour
{
    [SerializeField] private Vector2 spawnAreaSize = Vector2.one * 3f;
    public Vector2 SpawnAreaSize { get => spawnAreaSize; }
    [SerializeField] private int MaxCount;
    [SerializeField] private bool AutoSpawn;
    [SerializeField] private Consumable template;
    [SerializeField] bool ShowDebug;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if(!IsServer) return;

        for (int i = 0; i < MaxCount; i++)
        {
            //var consumable = Instantiate(template, Vector3.zero, Quaternion.identity);
            var consumable = Instantiate(template, Vector3.zero, Quaternion.identity, transform);
            var networkObject = consumable.gameObject.GetComponent<NetworkObject>();
            networkObject.Spawn();
            networkObject.TrySetParent(transform);
        }
    }


    public void Update()
    {
        if (!IsServer) return;

        // reactivate all the spawn point to pick
        foreach (var consumable in gameObject.GetComponentsInChildren<Consumable>(true))
        {
            if (consumable.isConsumed)
            {
                if (consumable.MoveToRandomLocation(SpawnAreaSize))
                {
                    consumable.SetConsumed(false);
                }
            }
        }
    }


    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(SpawnAreaSize.x, 1, SpawnAreaSize.y));
        Gizmos.color = Color.yellow;
    }

    void OnGUI()
    {
        if (!ShowDebug)
            return;

        int active = 0;
        int inactive = 0;
        int consumed = 0;
        foreach (var consumable in gameObject.GetComponentsInChildren<Consumable>(true)) {
            if(consumable.isActiveAndEnabled) active += 1;
            if(!consumable.isActiveAndEnabled) inactive += 1;
            if(!consumable.isConsumed) consumed += 1;
        }
        DebugHUD.AddLine($"Consumables: max:{MaxCount} a:{active} i:{inactive} c:{consumed}");
    }
}

