using Godot;
using System;

public class HealthBar : Label
{
    [Export]
    bool Left = false; 
    int MarginFromCenter = 100;
    int MarginFromTop = 20;
    public override void _Ready()
    {
        float ViewportX = GetViewport().Size.x;
        if (Left)
        {
            SetPosition(new Vector2((ViewportX/2) - MarginFromCenter, MarginFromTop));
            GD.Print("My left position is x: " + RectPosition.x + " y: " + RectPosition.y);
        }
        else
        {
            SetPosition(new Vector2((ViewportX/2) + MarginFromCenter, MarginFromTop));
            GD.Print("My right position is x: " + RectPosition.x + " y: " + RectPosition.y);
        }
    }
}
