using UnityEngine;


public interface ISnakePart
{
    float Length { get; }
}

public class SnakeBody : MonoBehaviour, ISnakePart
{
    public float BodyLength = 1;
    public float Length => BodyLength;
}
