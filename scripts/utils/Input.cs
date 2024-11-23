using Godot;

public static class InputUtils
{
    public static Vector2 GetVector(string left, string right)
    {
        float x = 0;
        if (Input.IsActionPressed(left))
        {
            x -= 1;
        }

        if (Input.IsActionPressed(right))
        {
            x += 1;
        }

        return new Vector2(x, 0);
    }
}
