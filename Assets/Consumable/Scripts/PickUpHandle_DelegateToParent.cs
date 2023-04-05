using UnityEngine;


public class PickUpHandle_DelegateToParent : MonoBehaviour, IPickUpHandle
{
    private Consumable consumable;

    void Start()
    {
        consumable = gameObject.transform.parent.GetComponent<Consumable>();
    }

    public void OnConsume()
    {
        consumable.Consume();
    }
}
