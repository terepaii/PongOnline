using Godot;
using System;

public class Game : Node2D
{

    public override void _Ready()
    {
        // Load Save File?

        // Load Main Menu
        GD.Print("I am here!");
        PackedScene MainMenuScene = ResourceLoader.Load<PackedScene>("res://MainMenu.tscn");
        GD.Print(MainMenuScene.ToString());
        MarginContainer MainMenu = MainMenuScene.Instance() as MarginContainer;    
        GetTree().Root.CallDeferred("AddChild", MainMenu);
        Hide();
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {

    }
}
