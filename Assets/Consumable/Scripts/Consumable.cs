using UnityEngine;

public class Consumable : MonoBehaviour
{
    [HideInInspector]
    public ConsumableSpawner spawner;

    public void Consume()
    {
        if (spawner != null)
        {
            spawner.Consumed(this);
        }
        else
        {
            Destroy(this);
        }
    }
}
