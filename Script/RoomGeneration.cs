using System;
using Godot;

public partial class RoomGeneration : Node3D
{
	[Export] PackedScene [] RoomList;
	AnimationPlayer transitionAnimation;

	private int roomCleared = 0;
	private int roomListSize;
	private int currentIndex = -1;

	public override void _Ready()
	{
		transitionAnimation = GetTree().CurrentScene.GetNode<Camera3D>("MainCamera").GetNode<AnimationPlayer>("TransitionAnimation");
		roomListSize = RoomList.Length;
	}

	public override void _Process(double delta)
	{
		if(PlayerStatus.transitioningRoom){
			generateNewRoom();
			PlayerStatus.transitioningRoom = false;
			transitionAnimation.Play("FadeOutNextRoom");
		}
	}

	//Remove old room and generate a new random room
	private void generateNewRoom(){
		roomCleared++;
		if(roomCleared >= 5){
			GetNode<Node3D>("CurrentRoom").Name = "OldRoom";

			GetNode<Node3D>("OldRoom").QueueFree();

			var currentRoom = ResourceLoader.Load<PackedScene>("res://Scene/DreadRooms/DreadRoomBoss.tscn").Instantiate();

			AddChild(currentRoom);
			currentRoom.Name = "CurrentRoom";

			currentRoom.GetNode<PlayerSpawn>("PlayerSpawn").spawnPlayer();
		}
		else{
			Random rand = new Random();

			int roomNum = rand.Next(roomListSize);
			while(roomNum == currentIndex){
				roomNum = rand.Next(roomListSize);
			}

			currentIndex = roomNum;

			GetNode<Node3D>("CurrentRoom").Name = "OldRoom";

			GetNode<Node3D>("OldRoom").QueueFree();

			var currentRoom = RoomList[roomNum].Instantiate();
			AddChild(currentRoom);
			currentRoom.Name = "CurrentRoom";

			currentRoom.GetNode<PlayerSpawn>("PlayerSpawn").spawnPlayer();
		}
	}
}
