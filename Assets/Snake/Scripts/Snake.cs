using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


public class Snake : MonoBehaviour
{
    [Header("Common Settings")]    
    [SerializeField] int PartSize = 200;    
    [SerializeField] bool ShowDebug = true;

    [Header("Body Templates")]
    [SerializeField] GameObject SnakeHeadTemplate;
    [SerializeField] List<GameObject> BodyTemplates;

    public uint BodyCount = 5;
    // body segments including the head (this)
    List<GameObject> bodyParts = new List<GameObject>();    

    void Start()
    {
        Assert.IsTrue(bodyParts.Count == 0);        
        gameObject.AddComponent<SnakePathManager>();
        bodyParts.Add(gameObject);
    }
    
    void Update()
    {
        // KeyDown is not working in FixedUpdate !
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            BodyCount += 1;
        }
        if (Input.GetKeyDown(KeyCode.KeypadMinus) && BodyCount > 0)
        {
            BodyCount -= 1;
        }
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

        var partCount = BodyCount + 1;
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
        var control = GetComponent<SnakeControl>();
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
        var consumable = other.gameObject.GetComponent<Consumable>();
        if(consumable != null) {
            BodyCount += 1;
            consumable.Consume();
        }
    }

    void OnGUI()
    {
        if(!ShowDebug)
            return;

        const float H = 30;
        float y = 35;

        GUIStyle labelStyle = new GUIStyle() 
        {
            fontSize = 24,
            normal = new GUIStyleState() { textColor = Color.white }
        };

        GUI.Label(new Rect(10, y += H, 500, 20), $"Snake length: {bodyParts.Count-1}/{BodyCount}", labelStyle);        
    }
}
