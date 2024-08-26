using Godot;

public partial class PlayerSpawn : Node3D
{
	public void	spawnPlayer(){
		GetTree().CurrentScene.GetNode<CharacterBody3D>("Player").GlobalPosition = GlobalPosition;
		GetTree().CurrentScene.GetNode<CharacterBody3D>("Player").GetNode<AnimatedSprite3D>("Mascot").GlobalPosition = GlobalPosition;
		GetTree().CurrentScene.GetNode<Camera3D>("MainCamera").Position = GetTree().CurrentScene.GetNode<CharacterBody3D>("Player").Position;
	}
}
