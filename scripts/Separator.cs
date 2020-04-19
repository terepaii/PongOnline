using Godot;
using System;

public class Separator : Sprite
{

    public override void _Ready()
    {
        Position = new Vector2(GetViewport().Size.x/2, GetViewport().Size.y/2);
        GD.Print("My separator position is x: " + Position.x + " y: " + Position.y);
    }

}
