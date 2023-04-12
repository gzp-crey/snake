using UnityEngine;

public static class NamedGameObjects
{
    public static FloatingJoystick DirectionJoystick => GameObject.Find("DirectionJoystick")?.GetComponent<FloatingJoystick>();
    public static GameObject TouchCanvas => GameObject.Find("TouchCanvas");
    public static ICameraFollow CameraFollow => Camera.main?.GetComponent<ICameraFollow>();
}
