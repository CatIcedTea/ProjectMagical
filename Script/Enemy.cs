using Godot;

public partial class Enemy : CharacterBody3D
{
    [Export] float movementSpeed = 2.5f;
    [Export] float damage = 10f;

    private NavigationAgent3D navigation;
    private Vector3 velocity;

    private enum movementType{
        None,
        Chase,
        Wandering
    }

    public override void _Ready()
    {
       navigation = GetNode<NavigationAgent3D>("NavigationAgent3D");
    }

    public override void _Process(double delta)
    {
        velocity = (navigation.GetNextPathPosition() - GlobalTransform.Origin).Normalized() * movementSpeed;
        navigation.TargetPosition = GetTree().CurrentScene.GetNode<PlayerController>("Player").GlobalTransform.Origin;

        Velocity = velocity;
        MoveAndSlide();
    }
}
