using Godot;
using System;

public partial class PlayerStatus : Node
{
	public static bool inDialogue = false;
	public static bool inInteractionRange = false;
	public static bool transitioningRoom = false;
	public static bool inMenu = false;
	public static bool isDefeated = false;
}
