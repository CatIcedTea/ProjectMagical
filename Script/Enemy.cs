using Godot;

public partial class Enemy : CharacterBody3D
{
    [Export] float health = 20;
    [Export] float damage = 2f;
    [Export] float movementSpeed = 2.5f;
    [Export] float attackRange = 10f;
    

    private AnimationPlayer animPlayer;
    private NavigationAgent3D navigation;
    private Vector3 velocity;
    private bool isAlive = true;
    private Timer hitStun;
    private Timer attackCD;
    private bool attacking;
    private Vector3 attackPosition;

    private enum movementType{
        None,
        Chase,
        Wandering
    }

    private enum FacingDir{
        FacingLeft,
        FacingRight
    }

    private FacingDir facingDir = FacingDir.FacingLeft;

    public override void _Ready()
    {
        animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        navigation = GetNode<NavigationAgent3D>("NavigationAgent3D");
        Rotation = new Vector3(Rotation.X, GetTree().CurrentScene.GetNode<Camera3D>("MainCamera").Rotation.Y, Rotation.Z);

        hitStun = GetNode<Timer>("HitStun");
        attackCD = GetNode<Timer>("AttackCooldown");
    }

    public override void _Process(double delta)
    {
        if(isAlive){
            if(facingDir == FacingDir.FacingLeft){
                GetNode<Node3D>("Flip").Scale = new Vector3(1, 1 , 1);
            }
            else{
                GetNode<Node3D>("Flip").Scale = new Vector3(-1, 1 , 1);
            }

            if(health <= 0){
                animPlayer.Play("Death");
                isAlive = false;
                GetNode<CollisionShape3D>("CollisionShape3D").Disabled = true;
            }

            if(attacking && animPlayer.CurrentAnimation != "Attack" && attackCD.IsStopped() && hitStun.IsStopped()){
                attackPosition = (GetTree().CurrentScene.GetNode<PlayerController>("Player").GlobalTransform.Origin - GlobalTransform.Origin).Normalized();

                if(animPlayer.CurrentAnimation != "AttackStartup" )
                animPlayer.Play("AttackStartup");
            }

            if(animPlayer.CurrentAnimation == "Attack" && attackCD.IsStopped() && hitStun.IsStopped()){
                velocity = velocity.Lerp(attackPosition * attackRange, 70f * (float)delta);
            }
            if(animPlayer.CurrentAnimation != "TakeDamage" && animPlayer.CurrentAnimation != "AttackStartup" && attackCD.IsStopped() && hitStun.IsStopped()){
                navigation.TargetPosition = GetTree().CurrentScene.GetNode<PlayerController>("Player").GlobalTransform.Origin;
                velocity = velocity.Lerp((navigation.GetNextPathPosition() - GlobalTransform.Origin).Normalized() * movementSpeed, 10f * (float)delta);
            }   
            else{
                velocity = velocity.Lerp(Vector3.Zero, 10f * (float)delta);
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
        hitStun.Start();
    }

    void _on_animation_player_animation_finished(string anim){
        if(anim == "Death"){
            QueueFree();
        }

        if(anim == "AttackStartup"){
            animPlayer.Play("Attack");
        }
        if(anim == "Attack"){
            attackCD.Start();
            attacking = false;
            animPlayer.Play("Idle");
        }
        if(anim == "TakeDamage"){
            animPlayer.Play("Idle");
        }
    }

    void _on_attack_range_body_entered(Node3D body){
        if(body.Name == "Player" && attackCD.IsStopped() && hitStun.IsStopped() && animPlayer.CurrentAnimation != "AttackStartup"){
            attacking = true;
            //animPlayer.Play("AttackStartup");
        }
    }

    void _on_attack_box_body_entered(Node3D body){
        if(body.Name == "Player"){
            PlayerController player = (PlayerController)body;

            player.takeDamage(damage);
        }
    }
}
