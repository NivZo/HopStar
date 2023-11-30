using Godot;

public class CameraUtils
{
    public static bool IsTargetInScreen(Camera2D camera, Node2D target) => target.GlobalPosition == CameraClamped(camera, target.GlobalPosition);

    public static Vector2 CameraClamped(Camera2D camera, Vector2 vector)
    {
        var center = GetCenter(camera);
        var rectSize = camera.GetViewportRect().Size;
        var margin = new Vector2(rectSize.X * 0.04f, rectSize.Y * 0.04f);
        var halfX = rectSize.X / 2 - margin.X;
        var halfY = rectSize.Y / 2 - margin.Y;

        var top = center + new Vector2(0, halfY);
        var bottom = center - new Vector2(0, halfY) + new Vector2(0, 100);
        var right = center + new Vector2(halfX, 0);
        var left = center - new Vector2(halfX, 0);
        var clamped = new Vector2(
            Mathf.Clamp(vector.X, left.X, right.X),
            Mathf.Clamp(vector.Y, bottom.Y, top.Y));
        return clamped;
    }

    public static Vector2 GetCenter(Camera2D camera)
    {
        var rectSize = camera.GetViewportRect().Size;
        var cameraPos = camera.GlobalPosition;
        cameraPos.X = Mathf.Max(camera.LimitLeft + rectSize.X / 2, cameraPos.X);
        cameraPos.X = Mathf.Min(camera.LimitRight - rectSize.X / 2, cameraPos.X);
        cameraPos.Y = Mathf.Max(camera.LimitTop + rectSize.Y / 2, cameraPos.Y);
        cameraPos.Y = Mathf.Min(camera.LimitBottom - rectSize.Y / 2, cameraPos.Y);
        return cameraPos;
    }
}