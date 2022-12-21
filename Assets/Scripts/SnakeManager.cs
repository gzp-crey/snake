using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeManager : MonoBehaviour
{
    [Header("Common Settings")]
    [SerializeField] float Speed = 100;
    [SerializeField] float TurnSpeed = 180;
    [SerializeField] float PartSize = 0.2f;    
    [SerializeField] bool ShowDebug = true;

    [Header("Body Templates")]
    [SerializeField] GameObject SnakeHeadTemplate;
    [SerializeField] List<GameObject> BodyTemplates;

    public int PartCount = 5;
    List<GameObject> parts = new List<GameObject>();

    void Start()
    {
        var head = CreatePartObject(SnakeHeadTemplate, transform.position, transform.rotation);
    }
    
    void Update()
    {
        // KeyDown is not working in FixedUpdate !
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            PartCount += 1;
        }
        if (Input.GetKeyDown(KeyCode.KeypadMinus) && PartCount > 1)
        {
            PartCount -= 1;
        }
    }

    void FixedUpdate()
    {
        Move();
        CreatePart();
    }

    GameObject CreatePartObject(GameObject template, Vector3 position, Quaternion rotation)
    {
        var part = Instantiate(template, position, rotation, transform);

        if (part.GetComponent<Rigidbody2D>() == null)
            part.AddComponent<Rigidbody2D>();

        if (part.GetComponent<SnakePathManager>() == null)
            part.AddComponent<SnakePathManager>();

        parts.Add(part);
        return part;
    }

    void CreatePart()
    {
        var tail = parts[parts.Count - 1];
        var tailPath = tail.GetComponent<SnakePathManager>();
        var tailMarker = tailPath.TakeTailAt(PartSize);
        if (parts.Count < PartCount && tailMarker != null)
        {
            var template = BodyTemplates[0]; // select body
            var part = CreatePartObject(template, tailMarker.position, tailMarker.rotation);
        }

        while (parts.Count > PartCount)
        {
            var part = parts[parts.Count - 1];
            parts.RemoveAt(parts.Count - 1);
            Destroy(part);
        }
    }

    void Move()
    {
        var head = parts[0];
        
        // rotate snake head
        float rotation = 0;        

        var touchMovement = GetComponent<PlayerTouchMovement>();        
        if (Input.GetAxis("Horizontal") != 0)
        {
            var dx = Input.GetAxis("Horizontal");
            if(dx > 1) dx = 1;
            else if(dx < -1) dx = -1;
            rotation = -dx * TurnSpeed;
        }
        else if (touchMovement != null && touchMovement.MovementAmount != Vector2.zero) 
        {
            Vector2 target = touchMovement.MovementAmount;
            Vector2 source = head.transform.right;
            rotation = Vector2.SignedAngle(source, target); 

            // max rotation toward the target
            if(rotation > 0) rotation = TurnSpeed;
            else if(rotation < 0) rotation = -TurnSpeed;

            // clamp speed
            //if(rotation > 0 && rotation > turnSpeed) rotation = turnSpeed;
            //else if(rotation < 0 && -rotation > turnSpeed) rotation = -turnSpeed;
        }

        if(rotation != 0) {
            head.transform.Rotate(0, 0, rotation * Time.deltaTime);
        }

        // move snake in the new direction
        head.GetComponent<Rigidbody2D>().velocity = head.transform.right * Speed * Time.deltaTime;

        // set the position of each part to the tail of the path of the prev part
        for (int i = 1; i < parts.Count; i++)
        {
            var prevPath = parts[i - 1].GetComponent<SnakePathManager>();
            var marker = prevPath.TakeTail();
            if (marker != null)
            {
                var part = parts[i];
                part.transform.position = marker.position;
                part.transform.rotation = marker.rotation;
            }
            else
            {
                Debug.LogError("Path is empty, could not update part position");
            }
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

        GUI.Label(new Rect(10, y += H, 500, 20), $"Snake length: {parts.Count}/{PartCount}", labelStyle);        
    }
}
