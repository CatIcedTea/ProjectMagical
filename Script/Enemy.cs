using Godot;

public partial class Enemy : CharacterBody3D
{
    [Export] float health = 20;
    [Export] float damage = 10f;
    [Export] float movementSpeed = 2.5f;
    

    private AnimationPlayer animPlayer;
    private NavigationAgent3D navigation;
    private Vector3 velocity;
    private bool isAlive = true;

    private enum movementType{
        None,
        Chase,
        Wandering
    }

    public override void _Ready()
    {
        animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        navigation = GetNode<NavigationAgent3D>("NavigationAgent3D");
        Rotation = new Vector3(Rotation.X, GetTree().CurrentScene.GetNode<Camera3D>("MainCamera").Rotation.Y, Rotation.Z);
    }

    public override void _Process(double delta)
    {
        if(isAlive){
            if(health <= 0){
                animPlayer.Play("Death");
                isAlive = false;
                GetNode<CollisionShape3D>("CollisionShape3D").Disabled = true;
            }

            if(animPlayer.CurrentAnimation != "TakeDamage"){
                navigation.TargetPosition = GetTree().CurrentScene.GetNode<PlayerController>("Player").GlobalTransform.Origin;
                velocity = velocity.Lerp((navigation.GetNextPathPosition() - GlobalTransform.Origin).Normalized() * movementSpeed, 10f * (float)delta);
            }   
        }
        
        if(animPlayer.CurrentAnimation == "TakeDamage" || animPlayer.CurrentAnimation == "Death"){
            Vector3 directionToPlayer = GetTree().CurrentScene.GetNode<PlayerController>("Player").GlobalPosition.DirectionTo(GlobalPosition);
            velocity = new Vector3(directionToPlayer.X, 0, directionToPlayer.Z).Normalized()* 4f;
        }

        Velocity = velocity;
        MoveAndSlide();
    }

    public void takeDamage(float damage){
        health -= damage;
        animPlayer.Play("TakeDamage");
    }

    public void takeDamage(float damage, Node3D node){
        health -= damage;
        animPlayer.Play("TakeDamage");
        //Velocity = node.GlobalPosition.DirectionTo(GlobalPosition) * 25;
    }

    void _on_animation_player_animation_finished(string anim){
        if(anim == "Death"){
            QueueFree();
        }
    }
}
