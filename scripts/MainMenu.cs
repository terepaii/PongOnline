using Godot;
using System;
using System.Collections;

public class MainMenu : MarginContainer
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.

    int CurrentPosInMenu;
    
    TextureRect SinglePlayerItem;
    Texture SinglePlayerItemTexture;
    Texture SinglePlayerItemSelectedTexture;

    TextureRect MultiplayerItem;
    Texture MultiplayerItemTexture;
    Texture MultiplayerItemSelectedTexture;

    TextureRect OptionsItem;
    Texture OptionsItemTexture;
    Texture OptionsItemSeletedTexture;

    AudioStreamPlayer MenuMoveSound;

    [Signal]
    public delegate void SetSinglePlayer();
    public override void _Ready()
    {
        // Keep track of the currently selected menu item
        CurrentPosInMenu = 0;
        // Load main menu items
        SinglePlayerItem = GetNode("/root/MainMenu/MainContainer/LeftSideContainer/MenuOptions/Singleplayer") as TextureRect;
        MultiplayerItem = GetNode("/root/MainMenu/MainContainer/LeftSideContainer/MenuOptions/Multiplayer") as TextureRect;
        OptionsItem = GetNode("/root/MainMenu/MainContainer/LeftSideContainer/MenuOptions/Options") as TextureRect;

        // Load main menu textures
        SinglePlayerItemTexture = ResourceLoader.Load("res://MainMenu/label_single_player.png") as Texture;
        SinglePlayerItemSelectedTexture = ResourceLoader.Load("res://MainMenu/label_single_player_selected.png") as Texture;
        MultiplayerItemTexture = ResourceLoader.Load("res://MainMenu/label_multi_player.png") as Texture;
        MultiplayerItemSelectedTexture = ResourceLoader.Load("res://MainMenu/label_multi_player_selected.png") as Texture;
        OptionsItemTexture = ResourceLoader.Load("res://MainMenu/label_options.png") as Texture;
        OptionsItemSeletedTexture = ResourceLoader.Load("res://MainMenu/label_options_selected.png") as Texture;

        // Load menu sounds
        MenuMoveSound = GetNode<AudioStreamPlayer>("menu_move");
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
     public override void _Process(float delta)
     {
         //var playerVariables = (PongCommon)GetNode("/root/PlayerVariables");
         //playerVariables.Health -= 10; // Instance field.
         // Check if the SinglePlayer item is selected
         if (CurrentPosInMenu == 0)
         {
             
             SinglePlayerItem.Texture = SinglePlayerItemSelectedTexture;
         }
         else
         {
             SinglePlayerItem.Texture = SinglePlayerItemTexture;
         }
         
         // Check if the Multiplayer item is selected
         if (CurrentPosInMenu == 1)
         {
             MultiplayerItem.Texture = MultiplayerItemSelectedTexture;
         }
         else
         {
             MultiplayerItem.Texture = MultiplayerItemTexture;
         }
         
         // Check if the Options items is selected
         if (CurrentPosInMenu == 2)
         {
             OptionsItem.Texture = OptionsItemSeletedTexture;
         }
         else
         {
             OptionsItem.Texture = OptionsItemTexture;
         }
     }

     public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_up"))
        {
            if(CurrentPosInMenu >= 1)
                CurrentPosInMenu--;
        }
        if (@event.IsActionPressed("ui_down"))
        {
            if(CurrentPosInMenu <= 1)
            CurrentPosInMenu++;
        }
        if (@event.IsActionPressed("ui_accept"))
        {
            // Kick off the singleplayer portion of the game
            if(CurrentPosInMenu == 0)
            {
                PackedScene PongScene = ResourceLoader.Load<PackedScene>("res://pong.tscn");
                Node2D Pong = PongScene.Instance() as Node2D;    
                this.AddChild(Pong);
                Hide();
                EmitSignal(nameof(SetSinglePlayer));
            }
            // Kick off the multiplayer portion of the game
            if(CurrentPosInMenu == 1)
            {
                PackedScene LobbyScene = ResourceLoader.Load<PackedScene>("res://lobby.tscn");
                Control Lobby = LobbyScene.Instance() as Control;
                GetTree().Root.AddChild(Lobby);
                Hide();
            }
        }
    }
}
