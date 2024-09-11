using Godot;

public partial class Enemy : CharacterBody3D
{
    [Export] float health = 20;
    [Export] float damage = 2f;
    [Export] float movementSpeed = 2.5f;
    [Export] float attackRange = 15f;
    [Export] bool isArmored = false;
    [Export] AttackType attackType = AttackType.Charge;
    

    private AnimationPlayer animPlayer;
    private AnimationPlayer flashWhite;
    private NavigationAgent3D navigation;
    private Vector3 velocity;
    private bool isAlive = true;
    private Timer hitStun;
    private Timer attackCD;
    private bool detected;
    private bool attacking;
    private bool attackStillInRange;
    private bool canShootProjectile = false;
    private Vector3 attackPosition;
    private EnemyProjectileGun gun;
    private Basis basis;

    private AudioStreamPlayer3D damaged;

    private enum movementType{
        None,
        Chase,
        Wandering
    }
    private enum AttackType{
        Charge,
        Ranged
    }

    private enum FacingDir{
        FacingLeft,
        FacingRight
    }

    private FacingDir facingDir = FacingDir.FacingLeft;

    public override void _Ready()
    {
        animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        flashWhite = GetNode<AnimationPlayer>("FlashWhite");
        navigation = GetNode<NavigationAgent3D>("NavigationAgent3D");
        Rotation = new Vector3(Rotation.X, GetTree().CurrentScene.GetNode<Camera3D>("MainCamera").Rotation.Y, Rotation.Z);

        hitStun = GetNode<Timer>("HitStun");
        attackCD = GetNode<Timer>("AttackCooldown");

        damaged = GetNode<Node3D>("Audio").GetNode<AudioStreamPlayer3D>("Damage");
        
        if(attackType == AttackType.Ranged){
            gun = GetNode<EnemyProjectileGun>("EnemyProjectileGun");
        }

        basis = GetTree().CurrentScene.GetNode<Camera3D>("MainCamera").Basis;
    }

    public override void _PhysicsProcess(double delta)
    {
        //Handle gravity
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

        if(isAlive){
            Vector3 rotatedVelocity = basis * velocity;

            if(rotatedVelocity.X < -0.1f && facingDir == FacingDir.FacingRight){
                GetNode<Node3D>("Flip").Scale = new Vector3(1, 1 , 1);
                GetNode<Node3D>("Flip").GetNode<Sprite3D>("Sprite3D").FlipH = false;
                facingDir = FacingDir.FacingLeft;
            }
            else if(rotatedVelocity.X > 0.1f && facingDir == FacingDir.FacingLeft){
                GetNode<Node3D>("Flip").Scale = new Vector3(-1, 1 , 1);
                GetNode<Node3D>("Flip").GetNode<Sprite3D>("Sprite3D").FlipH = true;
                facingDir = FacingDir.FacingRight;
            }

            //Handle death
            if(health <= 0){
                animPlayer.Play("Death");
                isAlive = false;
                GetNode<CollisionShape3D>("CollisionShape3D").Disabled = true;
            }

            //Handle attack
            if(attacking && animPlayer.CurrentAnimation != "Attack" && attackCD.IsStopped() && hitStun.IsStopped()){
                attackPosition = (GetTree().CurrentScene.GetNode<PlayerController>("Player").GlobalTransform.Origin - GlobalTransform.Origin).Normalized();

                if(animPlayer.CurrentAnimation != "AttackStartup" )
                    animPlayer.Play("AttackStartup");
            }

            //Handle attack
            if(animPlayer.CurrentAnimation == "Attack" && attackCD.IsStopped() && hitStun.IsStopped()){
                if( attackType == AttackType.Charge){
                    velocity.X = Mathf.Lerp(velocity.X, attackPosition.X * attackRange, 70f * (float)delta);
                    velocity.Z = Mathf.Lerp(velocity.Z, attackPosition.Z * attackRange, 70f * (float)delta);
                }
                else if(attackType == AttackType.Ranged && canShootProjectile){
                    gun.launchProjectile();
                    canShootProjectile = false;
                }
            }

            //Handle movement
            if(animPlayer.CurrentAnimation != "TakeDamage" && animPlayer.CurrentAnimation != "AttackStartup" && attackCD.IsStopped() && hitStun.IsStopped() && detected){
                
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
        if(isAlive){
            health -= damage;
            if(isArmored && health > 0){
                flashWhite.Play("flash");
            }
            else{
                animPlayer.Play("TakeDamage");
                hitStun.Start();
            }
        }
    }

    public void takeDamage(float damage, Node3D node){
        if(isAlive){
            health -= damage;
            if(isArmored && health > 0){
                flashWhite.Play("flash");
            }
            else{
                animPlayer.Play("TakeDamage");
                hitStun.Start();
            }
        }
    }

    void _on_animation_player_animation_finished(string anim){
        if(anim == "Death"){
            QueueFree();
        }

        if(anim == "AttackStartup"){
            canShootProjectile = true;
            animPlayer.Play("Attack");
        }
        if(anim == "Attack"){
            attackCD.Start();
            if(!attackStillInRange)
                attacking = false;
            animPlayer.Play("Idle");
        }
        if(anim == "TakeDamage"){
            if(isAlive)
                animPlayer.Play("Idle");
            else
                animPlayer.Play("Death");
        }
    }

    void _on_attack_range_body_entered(Node3D body){
        if(body.Name == "Player" && attackCD.IsStopped() && hitStun.IsStopped() && animPlayer.CurrentAnimation != "AttackStartup"){
            attacking = true;
            attackStillInRange = true;
        }
    }

    void _on_attack_range_body_exited(Node3D body){
        if(body.Name == "Player"){
            attackStillInRange = false;
        }
    }

    void _on_attack_box_body_entered(Node3D body){
        if(body.Name == "Player"){
            PlayerController player = (PlayerController)body;

            player.takeDamage(damage, Position);
        }
    }

    void _on_detection_range_body_entered(Node3D body){
        if(body.Name == "Player"){
            PlayerController player = (PlayerController)body;
            detected = true;
        }
    }
}
