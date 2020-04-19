using Godot;
using System;


public class pong : Node2D
{
    const int HEALTH_TO_LOSE = 0;
    float BATTING_RANGE = 50;
    int HealthLeft = 10000;
    int HealthRight = 10000;

    float Shake = 0;
    float ShakeMagnitude = 15;
    
    [Signal]
    public delegate void GameFinished();

    AudioStreamPlayer BoostStreamPlayer;

    int GameMode = -1;

    MarginContainer MainMenu;

    public override void _Ready()
    {
        GD.Print("Game Mode: " + this.GameMode);
        BoostStreamPlayer = GetNode("BoostSound") as AudioStreamPlayer;
        if (GetTree().IsNetworkServer())
        {
            GetNode("Player2").SetNetworkMaster(GetTree().GetNetworkConnectedPeers()[0]);
        }
        else
        {
            GetNode("Player2").SetNetworkMaster(GetTree().GetNetworkUniqueId());
        }
        Console.WriteLine("UniqueID: ", GetTree().GetNetworkUniqueId());
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("exit"))
        {
            EmitSignal("GameFinished");
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        ShakeTheScreen();
        if (Input.IsActionJustPressed("boost"))
        {
            
            Node2D Player1 = GetNode("Player1") as Node2D;
            Node2D Player2 = GetNode("Player2") as Node2D;
            Node2D Ball = GetNode("Ball") as Node2D;
            if(IsWithinRange(Player1, Ball, BATTING_RANGE))
            {
                Shake = ShakeMagnitude;
                Ball.Rpc("Bounce", true, 10);
                BoostStreamPlayer.Play();
            }
            else if(IsWithinRange(Player2, Ball, BATTING_RANGE))
            {
                Shake = ShakeMagnitude;
                Ball.Rpc("Bounce", false, 10);
                BoostStreamPlayer.Play();
            }
        }
    }

    private void ShakeTheScreen()
    {
        Camera2D Camera = GetNode<Camera2D>("Camera2D");
        RandomNumberGenerator rng = new RandomNumberGenerator();
        rng.Seed = (ulong)DateTime.Now.Second;        
        Camera.Position = new Vector2(rng.RandfRange(-Shake, Shake), rng.RandfRange(-Shake, Shake));

        // Decrease the shake by 10% every physics step
        Shake *= 0.9f;
    }
    private bool IsWithinRange(Node2D A, Node2D B, float Distance)
    {
        return A.Position.DistanceTo(B.Position) < Distance;
    }

    [Sync]
    public void SetGameMode(int GameMode)
    {
        this.GameMode = GameMode;
    }
    [Sync]
    public void UpdateHealth(bool SubtractHealthFromLeft, int Damage)
    {
        if (SubtractHealthFromLeft)
        {
            HealthLeft -= Damage;
            Label LeftHealthLabel = GetNode("HealthLeft") as Label;
            LeftHealthLabel.Text = HealthLeft.ToString();
        }
        else
        {
            HealthRight -= Damage;
            Label RightHealthLabel = GetNode("HealthRight") as Label;
            RightHealthLabel.Text = HealthRight.ToString();
        }

        // Check if the game has ended
        bool GameEnded = false;
        if (HealthLeft <= HEALTH_TO_LOSE)
        {
            GameEnded = true;
            Label WinnerLeftLabel = GetNode("WinnerLeft") as Label;
            WinnerLeftLabel.Show();
        }
        else if (HealthRight <= HEALTH_TO_LOSE)
        {
            GameEnded = true;
            Label WinnerRightLabel = GetNode("WinnerRight") as Label;
            WinnerRightLabel.Show();
        }

        if (GameEnded)
        {
            Button ExitGameButton = GetNode("ExitGame") as Button;
            ExitGameButton.Show();

            Area2D Ball = GetNode("Ball") as Area2D;
            Ball.Rpc("Stop");
        }   
    }

    private void OnExitGamePressed()
    {
        EmitSignal("GameFinished");
    }
}


/*
extends Node2D

const SCORE_TO_WIN = 10

var score_left = 0
var score_right = 0

signal game_finished()

onready var player2 = $Player2
onready var score_left_node = $ScoreLeft
onready var score_right_node = $ScoreRight
onready var winner_left = $WinnerLeft
onready var winner_right = $WinnerRight

func _ready():
	# By default, all nodes in server inherit from master,
	# while all nodes in clients inherit from puppet.
	# set_network_master is tree-recursive by default.
	if get_tree().is_network_server():
		# For the server, give control of player 2 to the other peer. 
		player2.set_network_master(get_tree().get_network_connected_peers()[0])
	else:
		# For the client, give control of player 2 to itself.
		player2.set_network_master(get_tree().get_network_unique_id())
	print("unique id: ", get_tree().get_network_unique_id())


sync func update_score(add_to_left):
	if add_to_left:
		score_left += 1
		score_left_node.set_text(str(score_left))
	else:
		score_right += 1
		score_right_node.set_text(str(score_right))
	
	var game_ended = false
	if score_left == SCORE_TO_WIN:
		winner_left.show()
		game_ended = true
	elif score_right == SCORE_TO_WIN:
		winner_right.show()
		game_ended = true
	
	if game_ended:
		$ExitGame.show()
		$Ball.rpc("stop")


func _on_exit_game_pressed():
	emit_signal("game_finished")
*/