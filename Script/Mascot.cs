using Godot;

public partial class Mascot : AnimatedSprite3D
{
	[Export] float MovementInterpolationRate = 1.75f;

	//Interpolates and follow this position
	Node3D followPosition;

	CharacterBody3D player;

	Vector3 distance;

	public override void _Ready()
	{
		followPosition = GetParent().GetNode<Node3D>("Flip").GetNode<Node3D>("MascotPosition");
		player = GetParent<CharacterBody3D>();
		GlobalPosition = followPosition.GlobalPosition;

		GlobalBasis = player.GlobalBasis;
	}

	public override void _Process(double delta)
	{
		GlobalPosition = GlobalPosition.Lerp(followPosition.GlobalPosition, MovementInterpolationRate * (float)delta);

		distance = (Transform.Basis * new Vector3(GlobalPosition.X - player.GlobalPosition.X, 0, GlobalPosition.Y - player.GlobalPosition.Y)).Normalized();

		if(distance.X > 0.01f){
			FlipH = false;
		}
		if(distance.X < -0.01f){
			FlipH = true;
		}
	}
}
