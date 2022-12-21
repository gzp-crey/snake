using UnityEngine;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;



public class PlayerTouchMovement : MonoBehaviour
{
    public enum EScreenArea {
        None,
        All,
        Left,
        Right,        
    }

    [SerializeField] Vector2 JoystickSize = new Vector2(300, 300);
    [SerializeField] bool ShowJoystick = true;
    [SerializeField] EScreenArea ScreenArea = EScreenArea.All;
    [SerializeField] FloatingJoystick Joystick;
    [SerializeField] bool Debug = false;

    ETouch.Finger MovementFinger;
    [HideInInspector] public Vector2 MovementAmount;
    
    void OnEnable()
    {
        MovementFinger = null;
        MovementAmount = Vector2.zero;
        ETouch.EnhancedTouchSupport.Enable();
        ETouch.Touch.onFingerDown += HandleFingerDown;
        ETouch.Touch.onFingerUp += HandleLoseFinger;
        ETouch.Touch.onFingerMove += HandleFingerMove;
    }

    void OnDisable()
    {
        ETouch.Touch.onFingerDown -= HandleFingerDown;
        ETouch.Touch.onFingerUp -= HandleLoseFinger;
        ETouch.Touch.onFingerMove -= HandleFingerMove;
        ETouch.EnhancedTouchSupport.Disable();
        MovementFinger = null;
        MovementAmount = Vector2.zero;
    }

    void HandleFingerDown(ETouch.Finger TouchedFinger)
    {
        if (MovementFinger == null && InActiveArea(TouchedFinger.screenPosition))
        {
            MovementFinger = TouchedFinger;
            MovementAmount = Vector2.zero;
            Joystick.gameObject.SetActive(ShowJoystick);
            Joystick.RectTransform.sizeDelta = JoystickSize;
            Joystick.RectTransform.anchoredPosition = ClampStartPosition(TouchedFinger.screenPosition);
        }
    }

    void HandleFingerMove(ETouch.Finger MovedFinger)
    {
        if (MovedFinger == MovementFinger)
        {
            Vector2 knobPosition;
            float maxMovement = JoystickSize.x / 2f;
            ETouch.Touch currentTouch = MovedFinger.currentTouch;

            if (
                Vector2.Distance(
                    currentTouch.screenPosition,
                    Joystick.RectTransform.anchoredPosition
                ) > maxMovement
            )
            {
                knobPosition =
                    (
                        currentTouch.screenPosition - Joystick.RectTransform.anchoredPosition
                    ).normalized * maxMovement;
            }
            else
            {
                knobPosition =
                    currentTouch.screenPosition - Joystick.RectTransform.anchoredPosition;
            }

            Joystick.Knob.anchoredPosition = knobPosition;
            MovementAmount = knobPosition / maxMovement;
        }
    }

    void HandleLoseFinger(ETouch.Finger LostFinger)
    {
        if (LostFinger == MovementFinger)
        {   
            MovementFinger = null;
            MovementAmount = Vector2.zero;         
            Joystick.Knob.anchoredPosition = Vector2.zero;
            Joystick.gameObject.SetActive(false);            
        }
    }

    bool InActiveArea(Vector2 downPosition) {
        switch(ScreenArea) {
            case EScreenArea.All: return true;
            case EScreenArea.Left: return downPosition.x < Screen.width / 2f;
            case EScreenArea.Right: return downPosition.x > Screen.width / 2f;
        }
        return false;
    }

    Vector2 ClampStartPosition(Vector2 StartPosition)
    {
        if (StartPosition.x < JoystickSize.x / 2)
        {
            StartPosition.x = JoystickSize.x / 2;
        }

        if (StartPosition.y < JoystickSize.y / 2)
        {
            StartPosition.y = JoystickSize.y / 2;
        }
        else if (StartPosition.y > Screen.height - JoystickSize.y / 2)
        {
            StartPosition.y = Screen.height - JoystickSize.y / 2;
        }

        return StartPosition;
    }

    void OnGUI()
    {
        if(!Debug)
            return;

        const float H = 30;
        float y = 35;

        GUIStyle labelStyle = new GUIStyle() 
        {
            fontSize = 24,
            normal = new GUIStyleState() { textColor = Color.white }
        };

        GUI.Label(new Rect(10, y += H, 500, 20), $"Screen Size ({Screen.width}, {Screen.height})", labelStyle);
        
        if (MovementFinger != null)
        {
            GUI.Label(new Rect(10, y += H, 500, 20), $"Finger Start Position: {MovementFinger.currentTouch.startScreenPosition}", labelStyle);
            GUI.Label(new Rect(10, y += H, 500, 20), $"Finger Current Position: {MovementFinger.currentTouch.screenPosition}", labelStyle);
            GUI.Label(new Rect(10, y += H, 500, 20), $"Movement: {MovementAmount}", labelStyle);
        }
        else
        {
            GUI.Label(new Rect(10, y += H, 500, 20), $"No Current Movement Touch ({MovementAmount})", labelStyle);
        }        
    }
}
