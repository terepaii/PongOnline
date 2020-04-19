using Godot;
using System;

public class lobby : Control
{
	const ushort Port= 8000;

	LineEdit Address;
	Button HostButton;
	Button JoinButton;
	Label StatusOk;
	Label StatusFail;

	public override void _Ready()
	{
		Address = GetNode("Address") as LineEdit;
		HostButton = GetNode("HostButton") as Button;
		JoinButton = GetNode("JoinButton") as Button;
		StatusOk = GetNode("StatusOk") as Label;
		StatusFail = GetNode("StatusFail") as Label;

		GetTree().Connect("network_peer_connected", this, "PlayerConnected");
		GetTree().Connect("network_peer_disconnected", this, "PlayerDisconnected");
		GetTree().Connect("connected_to_server", this, "ConnectedOk");
		GetTree().Connect("connection_failed", this, "ConnectedFail");
		GetTree().Connect("server_disconnected", this, "ServerDisconnected");
	}

	///////////////////////////////////////////////////
	// Callback Functions
	///////////////////////////////////////////////////
	private void PlayerConnected(int Id)
	{
		PackedScene scene = ResourceLoader.Load("res://pong_multiplayer.tscn") as PackedScene; 
		Node2D Pong = scene.Instance() as Node2D;
		Pong.Connect("GameFinished", this, "EndGame", null, (int)ConnectFlags.Deferred);

		GetTree().Root.AddChild(Pong);
		Hide();
	}

	private void PlayerDisconnected(int Id)
	{
		if (GetTree().IsNetworkServer())
		{
			EndGame("Client Disconnected");
		}
		else
		{
			EndGame("Client Disconnected");
		}
	}

	private void ConnectedOk()
	{
		;
	}

	private void ConnectedFail()
	{
		// TODO: Connected Fail
		SetStatus("Couldn't Connect", false);

		// Remove Peer
		GetTree().NetworkPeer = null;

		HostButton.Disabled = true;
		JoinButton.Disabled = true;
	}

	private void ServerDisconnected()
	{
		EndGame("Server Disconnected");
	}


	///////////////////////////////////////////////////
	// Game Creation Functions
	///////////////////////////////////////////////////

	private void EndGame(string WithError)
	{
		string PongNodePath = "/root/Pong";
		if (HasNode(PongNodePath))
		{
			// E
			GetNode(PongNodePath).Free();

			// Show the Lobby node 
			Show();
		}

		GetTree().NetworkPeer = null;

		HostButton.Disabled = false;
		JoinButton.Disabled = false;

		SetStatus(WithError, false);
	}

	private void SetStatus(string Text, bool IsOk)
	{
		if (IsOk)
		{
			StatusOk.Text = Text;
			StatusFail.Text = "";
		}
		else
		{
			StatusOk.Text =  "";
			StatusFail.Text = Text;
		}
	}

	private void OnHostPressed()
	{
		NetworkedMultiplayerENet Host = new NetworkedMultiplayerENet();
		Host.CompressionMode = NetworkedMultiplayerENet.CompressionModeEnum.RangeCoder;

		Error Err = Host.CreateServer(Port, 1);
		switch (Err)
		{
			case Error.AlreadyInUse:
			SetStatus("Can't host, address already in use", false);
			return;
		}

		GetTree().NetworkPeer = Host;
		HostButton.Disabled = true;
		JoinButton.Disabled = true;
		SetStatus("Waiting for player...", true);
	}

	private void OnJoinPressed()
	{
		string IP = Address.Text;
		if (!IP.IsValidIPAddress())
		{
			SetStatus("IP Address is invalid", false);
			return;
		}

		NetworkedMultiplayerENet Host = new NetworkedMultiplayerENet();
		Host.CompressionMode = NetworkedMultiplayerENet.CompressionModeEnum.RangeCoder;
		Host.CreateClient(IP, Port);
		GetTree().NetworkPeer = Host;

		SetStatus("Connecting...", true);
	}
}

/*
extends Control

const DEFAULT_PORT = 8910 # An arbitrary number.

onready var address = $Address
onready var host_button = $HostButton
onready var join_button = $JoinButton
onready var status_ok = $StatusOk
onready var status_fail = $StatusFail

func _ready():
	# Connect all the callbacks related to networking.
	get_tree().connect("network_peer_connected", self, "_player_connected")
	get_tree().connect("network_peer_disconnected", self, "_player_disconnected")
	get_tree().connect("connected_to_server", self, "_connected_ok")
	get_tree().connect("connection_failed", self, "_connected_fail")
	get_tree().connect("server_disconnected", self, "_server_disconnected")

#### Network callbacks from SceneTree ####

# Callback from SceneTree.
func _player_connected(_id):
	# Someone connected, start the game!
	var pong = load("res://pong.tscn").instance()
	# Connect deferred so we can safely erase it from the callback.
	pong.connect("game_finished", self, "_end_game", [], CONNECT_DEFERRED)
	
	get_tree().get_root().add_child(pong)
	hide()


func _player_disconnected(_id):
	if get_tree().is_network_server():
		_end_game("Client disconnected")
	else:
		_end_game("Server disconnected")


# Callback from SceneTree, only for clients (not server).
func _connected_ok():
	pass # We don't need this function.


# Callback from SceneTree, only for clients (not server).
func _connected_fail():
	_set_status("Couldn't connect", false)
	
	get_tree().set_network_peer(null) # Remove peer.
	
	host_button.set_disabled(false)
	join_button.set_disabled(false)


func _server_disconnected():
	_end_game("Server disconnected")

	/*##### Game creation functions ######

func _end_game(with_error = ""):
	if has_node("/root/Pong"):
		# Erase immediately, otherwise network might show errors (this is why we connected deferred above).
		get_node("/root/Pong").free()
		show()
	
	get_tree().set_network_peer(null) # Remove peer.
	host_button.set_disabled(false)
	join_button.set_disabled(false)
	
	_set_status(with_error, false)


func _set_status(text, isok):
	# Simple way to show status.
	if isok:
		status_ok.set_text(text)
		status_fail.set_text("")
	else:
		status_ok.set_text("")
		status_fail.set_text(text)


func _on_host_pressed():
	var host = NetworkedMultiplayerENet.new()
	host.set_compression_mode(NetworkedMultiplayerENet.COMPRESS_RANGE_CODER)
	var err = host.create_server(DEFAULT_PORT, 1) # Maximum of 1 peer, since it's a 2-player game.
	if err != OK:
		# Is another server running?
		_set_status("Can't host, address in use.",false)
		return
	
	get_tree().set_network_peer(host)
	host_button.set_disabled(true)
	join_button.set_disabled(true)
	_set_status("Waiting for player...", true)


func _on_join_pressed():
	var ip = address.get_text()
	if not ip.is_valid_ip_address():
		_set_status("IP address is invalid", false)
		return
	
	var host = NetworkedMultiplayerENet.new()
	host.set_compression_mode(NetworkedMultiplayerENet.COMPRESS_RANGE_CODER)
	host.create_client(ip, DEFAULT_PORT)
	get_tree().set_network_peer(host)
	
	_set_status("Connecting...", true)*/

