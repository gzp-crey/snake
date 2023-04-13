using UnityEngine;

public class DebugHUD : MonoBehaviour
{
    [SerializeField] private bool ShowDebug = true;

    const float H = 30;

    private static DebugHUD instance;
    private GUIStyle labelStyle;
    private float y = 35;

    void Awake()
    {
        instance = this;
        labelStyle = new GUIStyle()
        {
            fontSize = 24,
            normal = new GUIStyleState() { textColor = Color.blue }
        };
    }

    void LateUpdate()
    {
        if (instance == null) return;

        instance.y = 0;
        instance.ShowDebug = enabled;
    }

    public static void AddLine(string line)
    {        
        if (instance == null || !instance.ShowDebug)
            return;

        GUI.Label(new Rect(10, instance.y += H, 500, 20), line, instance.labelStyle);
    }
}
