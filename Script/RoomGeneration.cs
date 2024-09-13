using System;
using Godot;

public partial class RoomGeneration : Node3D
{
	[Export] PackedScene [] RoomList;
	AnimationPlayer transitionAnimation;

	private int roomCleared = -1;
	private int roomListSize;
	private int currentIndex = -1;
	private int [] roomOrder;

	public override void _Ready()
	{
		transitionAnimation = GetTree().CurrentScene.GetNode<Camera3D>("MainCamera").GetNode<AnimationPlayer>("TransitionAnimation");
		roomListSize = RoomList.Length;

		//Shuffles room order
		roomOrder = new int[roomListSize];
		for (int k = 0; k < roomOrder.Length; k++){
			roomOrder[k] = k;
		}

		for (int i = 0; i < roomOrder.Length; i++){
			int swapIndex = new Random().Next(0, roomOrder.Length);
			int tempVal = roomOrder[i];

			roomOrder[i] = roomOrder[swapIndex];
			roomOrder[swapIndex] = tempVal;
		}
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
		if(roomCleared > 6){
			GetNode<Node3D>("CurrentRoom").Name = "OldRoom";

			GetNode<Node3D>("OldRoom").QueueFree();

			var currentRoom = ResourceLoader.Load<PackedScene>("res://Scene/DreadRooms/DreadRoomBoss.tscn").Instantiate();

			AddChild(currentRoom);
			currentRoom.Name = "CurrentRoom";

			currentRoom.GetNode<PlayerSpawn>("PlayerSpawn").spawnPlayer();
		}
		else if(roomCleared < roomListSize - 1){
			GetNode<Node3D>("CurrentRoom").Name = "OldRoom";

			GetNode<Node3D>("OldRoom").QueueFree();

			var currentRoom = RoomList[roomOrder[roomCleared]].Instantiate();
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
