using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Collider))]
public class Snake : NetworkBehaviour, ISnakePart
{
    [SerializeField] private float BodyLength = 1;
    public float Length => BodyLength;

    [SerializeField] List<SnakeBody> BodyTemplates;
    [SerializeField] private bool ShowDebug = true;

    public NetworkVariable<uint> BodyCount = new NetworkVariable<uint>(5);

    class BodyPart
    {
        public GameObject gameObject;
        public SnakePathManager path;
        public float length;
    }

    // body segments including the head (this)
    List<BodyPart> bodyParts = new List<BodyPart>();

    void Start()
    {
        Assert.IsTrue(bodyParts.Count == 0);
        var bodyPart = new BodyPart
        {
            gameObject = gameObject,
            length = gameObject.GetComponent<ISnakePart>().Length,
            path = gameObject.AddComponent<SnakePathManager>(),
        };
        bodyParts.Add(bodyPart);
    }

    void FixedUpdate()
    {
        MoveBodyParts();
        UpdateBodyParts();
    }

    SnakeBody CreatePartObject(int bodyTemplateId, Vector3 position, Quaternion rotation)
    {
        Assert.IsTrue(BodyTemplates.Count > bodyTemplateId);
        var template = BodyTemplates[bodyTemplateId]; // select body

        var part = Instantiate(template, position, rotation/*, transform*/); // spawn as root, so head transform/interpolation won't effect us
        var bodyPart = new BodyPart
        {
            gameObject = part.gameObject,
            path = part.gameObject.AddComponent<SnakePathManager>(),
            length = part.Length,
        };
        bodyPart.path.Segment = bodyParts.Count;
        bodyParts.Add(bodyPart);
        return part;
    }

    void MoveBodyParts()
    {
        // set the position of each part to the tail of the path of the prev part
        for (int i = 1; i < bodyParts.Count; i++)
        {
            var prevPath = bodyParts[i - 1].path;
            var marker = prevPath.TakeTail();
            if (marker != null)
            {
                var part = bodyParts[i].path;
                part.transform.position = marker.position;
                part.transform.rotation = marker.rotation;
            }
            else
            {
                Debug.LogError("Path is empty, could not update part position");
            }
        }
    }

    void UpdateBodyParts()
    {
        Assert.IsTrue(bodyParts.Count > 0 && bodyParts[0].gameObject == gameObject);

        var tail = bodyParts[bodyParts.Count - 1];
        var tailMarker = tail.path.TrimAtLength(tail.length);
        //var tailMarker = tail.path.TrimAtCount(8);

        var partCount = BodyCount.Value + 1;
        if (bodyParts.Count < partCount && tailMarker != null)
        {
            var part = CreatePartObject(0, tailMarker.position, tailMarker.rotation);
        }

        while (bodyParts.Count > partCount)
        {
            var part = bodyParts[bodyParts.Count - 1].gameObject;
            bodyParts.RemoveAt(bodyParts.Count - 1);
            Destroy(part);
        }

        Assert.IsTrue(bodyParts.Count > 0 && bodyParts[0].gameObject == gameObject);
    }

    //Upon collision with another GameObject, this GameObject will reverse direction
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("colliding with another GameObject");

        if (!IsServer) return;

        var consumable = other.gameObject.GetComponent<IPickUpHandle>();
        if (consumable != null)
        {

            BodyCount.Value += 1;
            consumable.OnConsume();
        }
    }

    void OnGUI()
    {
        if (!ShowDebug)
            return;

        DebugHUD.AddLine($"Snake length: {bodyParts.Count - 1}/{BodyCount.Value}");
    }
}
