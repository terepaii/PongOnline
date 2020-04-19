using Godot;
using System;

public class paddle : Area2D
{

	private const int MOTION_SPEED = 250;

	int Margin = 100;

	[Export]
	bool Left = false;

	private float Motion = 0; 
	private bool YouLabelHidden = false;

	public override void _Ready()
	{
		if (Left)
		{
			Position = new Vector2(Margin, GetViewport().Size.y / 2);
		}
		else
		{
			Position = new Vector2(GetViewport().Size.x - Margin, GetViewport().Size.y / 2);
		}
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Process(float Delta)
	{
		if (IsNetworkMaster())
		{
			Motion = Input.GetActionStrength("move_down") - Input.GetActionStrength("move_up");

			if (!YouLabelHidden && Motion != 0)
			{
				HideYouLabel();
			}

			Motion *= MOTION_SPEED;

			// Send Position and Motion
			RpcUnreliable("SetPosAndMotion", Position, Motion);
		}
		else 
		{
			if (!YouLabelHidden)
			{
				HideYouLabel();
			}
		}		

		Translate(new Vector2(0, Motion * Delta)); 

		Position = new Vector2(Position.x, Mathf.Clamp(Position.y, 16, GetViewportRect().Size.y));
	}


	[Puppet]
	public void SetPosAndMotion(Vector2 Position, float Motion)
	{
		this.Position = Position;
		this.Motion = Motion;
	}

	private void HideYouLabel()
	{
		YouLabelHidden = true;
		Label YouLabel = GetNode("You") as Label;
		YouLabel.Hide();	
	} 

	private void OnPaddleEnterArea(Area2D Area)
	{
		if (IsNetworkMaster())
		{
			// TODO: Check if I need a random number here
			Area.Rpc("Bounce", Left, 1);
		}
	}
}


/*
extends Area2D

const MOTION_SPEED = 150

export var left = false

var Motion = 0
var you_hidden = false

onready var _screen_size_y = get_viewport_rect().size.y

func _process(delta):
	# Is the master of the paddle.
	if is_network_master():
		Motion = Input.get_action_strength("move_down") - Input.get_action_strength("move_up")
		
		if not you_hidden and Motion != 0:
			_hide_you_label()
		
		Motion *= MOTION_SPEED
		
		# Using unreliable to make sure position is updated as fast
		# as possible, even if one of the calls is dropped.
		rpc_unreliable("set_pos_and_motion", position, Motion)
	else:
		if not you_hidden:
			_hide_you_label()
	
	translate(Vector2(0, Motion * delta))
	
	# Set screen limits.
	position.y = clamp(position.y, 16, _screen_size_y - 16)


# Synchronize position and speed to the other peers.
puppet func set_pos_and_motion(p_pos, p_motion):
	position = p_pos
	Motion = p_motion


func _hide_you_label():
	you_hidden = true
	get_node("You").hide()


func _on_paddle_area_enter(area):
	if is_network_master():
		# Random for new direction generated on each peer.
		area.rpc("bounce", left, randf())
*/
