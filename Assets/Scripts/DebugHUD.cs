using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugHUD
{
    const float H = 30;    
    static DebugHUD instance = new DebugHUD();

    bool showDebug = true;
    int currentFrame = 0;
    float y = 35;
    GUIStyle labelStyle = new GUIStyle()
    {
        fontSize = 24,
        normal = new GUIStyleState() { textColor = Color.white }
    };

    public static void StartFrame(bool enabled) {
        instance.y = 0;
        instance.showDebug = enabled;
    }

    public static void AddLine(string line)
    {
        if(!instance.showDebug)
            return;

        GUI.Label(new Rect(10, instance.y += H, 500, 20), line, instance.labelStyle);
    }
}
