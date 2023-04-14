using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;

public class Snake : NetworkBehaviour
{
    [Header("Common Settings")]    
    [SerializeField] int PartSize = 200;    
    [SerializeField] bool ShowDebug = true;

    [Header("Body Templates")]
    [SerializeField] List<GameObject> BodyTemplates;

    public NetworkVariable<uint> BodyCount = new NetworkVariable<uint>(5);
    // body segments including the head (this)
    List<GameObject> bodyParts = new List<GameObject>();    

    void Start()
    {
        Assert.IsTrue(bodyParts.Count == 0);        
        gameObject.AddComponent<SnakePathManager>();
        bodyParts.Add(gameObject);
    }
    
    void FixedUpdate()
    {
        Move();
        CreatePart();
    }

    GameObject CreatePartObject(int bodyTemplateId, Vector3 position, Quaternion rotation)
    {
        Assert.IsTrue(BodyTemplates.Count > bodyTemplateId);
        var template = BodyTemplates[bodyTemplateId]; // select body

        var part = Instantiate(template, position, rotation, transform);
        part.AddComponent<SnakePathManager>();
        bodyParts.Add(part);
        return part;
    }

    void CreatePart()
    {
        Assert.IsTrue(bodyParts.Count > 0 && bodyParts[0] == gameObject);

        var tail = bodyParts[bodyParts.Count - 1];
        var tailPath = tail.GetComponent<SnakePathManager>();        
        var tailMarker = tailPath.TrimAtCount(PartSize);

        var partCount = BodyCount.Value + 1;
        if (bodyParts.Count < partCount && tailMarker != null)
        {                        
            var part = CreatePartObject(0, tailMarker.position, tailMarker.rotation);
        }

        while (bodyParts.Count > partCount)
        {
            var part = bodyParts[bodyParts.Count - 1];
            bodyParts.RemoveAt(bodyParts.Count - 1);
            Destroy(part);
        }

        Assert.IsTrue(bodyParts.Count > 0 && bodyParts[0] == gameObject);
    }

    void Move()
    {
        var control = GetComponent<ISnakeControl>();
        if( control != null)
            control.Move();

        // set the position of each part to the tail of the path of the prev part
        for (int i = 1; i < bodyParts.Count; i++)
        {
            var prevPath = bodyParts[i - 1].GetComponent<SnakePathManager>();
            var marker = prevPath.TakeTail();
            if (marker != null)
            {
                var part = bodyParts[i];
                part.transform.position = marker.position;
                part.transform.rotation = marker.rotation;
            }
            else
            {
                Debug.LogError("Path is empty, could not update part position");
            }
        }
    }

    //Upon collision with another GameObject, this GameObject will reverse direction
    void OnTriggerEnter(Collider other)
    {
        if(IsLocalPlayer) return;
        
        var consumable = other.gameObject.GetComponent<IPickUpHandle>();
        if(consumable != null) {
            
            BodyCount.Value += 1;
            consumable.OnConsume();
        }
    }

    void OnGUI()
    {
        if(!ShowDebug)
            return;

        DebugHUD.AddLine( $"Snake length: {bodyParts.Count-1}/{BodyCount.Value}");        
    }
}
