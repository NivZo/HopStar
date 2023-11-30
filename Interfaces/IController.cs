using Godot;

public interface IController
{
    bool DidStart { get; set; }

    Vector2 GetVelocity();

    float GetRotation();

     
}