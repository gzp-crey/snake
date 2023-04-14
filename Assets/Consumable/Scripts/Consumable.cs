using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Consumable : NetworkBehaviour
{
    [SerializeField] private NetworkVariable<bool> consumedState = new NetworkVariable<bool>(true);
    public bool isConsumed => consumedState.Value;

    void Start()
    {
        consumedState.OnValueChanged += (s, consumed) => { gameObject.SetActive(!consumed); };
    }

    public override void OnNetworkSpawn()
    {
        //gameObject.SetActive(!consumedState.Value);
    }

    /// Create a new random position in the spawn area relative to this.
    Vector3 GetRandomPosition(Vector2 spawnAreaSize)
    {
        var min = spawnAreaSize * -0.5f;
        var max = spawnAreaSize * 0.5f;
        return new Vector3(UnityEngine.Random.Range(min.x, max.x), 0, UnityEngine.Random.Range(min.y, max.y));
    }

    public bool MoveToRandomLocation(Vector2 spawnAreaSize, int retryCount = 3)
    {
        for (; ; )
        {
            transform.localPosition = GetRandomPosition(spawnAreaSize);
            var dist = spawnAreaSize.y * 2;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit))
            {
                transform.position = hit.point;
                return true;
            }
            retryCount -= 1;
            if (retryCount < 0)
            {
                return false;
            }
        }
    }

    public void SetConsumed(bool consumed)
    {
        consumedState.Value = consumed;
        //gameObject.SetActive(!consumed);
    }

    public void Consume()
    {        
        //todo: add extra effects
        SetConsumed(true);
    }

}
