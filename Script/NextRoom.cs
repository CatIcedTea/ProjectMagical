using Godot;

public partial class NextRoom : Area3D
{
    public override void _Process(double delta)
    {
		//Lock next room until no more enemies left
        if(GetTree().GetNodeCountInGroup("Enemies") != 0)
			GetNode<CollisionShape3D>("CollisionShape3D").Disabled = true;
		else
			GetNode<CollisionShape3D>("CollisionShape3D").Disabled = false;
    }

    //Play fade in animation and it will emit an animation finished signal to load in the next room
    public void transitionNextRoom(){
		GetTree().CurrentScene.GetNode<Camera3D>("MainCamera").GetNode<AnimationPlayer>("TransitionAnimation").Play("FadeInNextRoom");
	}
}
