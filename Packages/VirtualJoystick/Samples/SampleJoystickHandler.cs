using UnityEngine;

public class SampleJoystickHandler : MonoBehaviour
{
    [SerializeField] string Text;
    [SerializeField] float TextPosY;

    [SerializeField] FloatingJoystick Joystick;

    void OnGUI()
    {
        GUIStyle labelStyle = new GUIStyle()
        {
            fontSize = 24,
            normal = new GUIStyleState() { textColor = Color.white }
        };

        if (Joystick != null)
        {
            Vector3 target = new Vector3(-Joystick.MovementAmount.y, 0.0f, Joystick.MovementAmount.x);
            GUI.Label(new Rect(10, TextPosY, 500, 20), $"{Text} - target: {target}", labelStyle);
        }
        else
        {
            GUI.Label(new Rect(10, TextPosY, 500, 20), $"{Text} - Missing joystick controller", labelStyle);
        }
    }
}
