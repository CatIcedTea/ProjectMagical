using Godot;

public partial class NextRoom : Area3D
{
	//Play fade in animation and it will emit an animation finished signal to load in the next room
	public void transitionNextRoom(){
		GetTree().CurrentScene.GetNode<Camera3D>("MainCamera").GetNode<AnimationPlayer>("TransitionAnimation").Play("FadeInNextRoom");
	}
}
