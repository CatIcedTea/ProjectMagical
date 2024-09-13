using Godot;
using System;

public partial class DreadBoss : CharacterBody3D
{
    public float health = 300f;
    public float damage = 4f;
    public float movementSpeed = 3.5f;
    public float attackRange = 20f;

    public bool fightStarted = false;

    private Vector3 velocity;
    private bool isAlive = true;
    private bool attacking = false;
    private bool canShootProjectile = false;
    private bool attackStillInRange;

    private AnimationPlayer animPlayer;
    private AnimationPlayer flashWhite;
    private NavigationAgent3D navigation;
    private Timer attackCD;
    private Vector3 attackPosition;

    private Random rand = new Random();
    

    private EnemyProjectileGun [] projectileLauncher = new EnemyProjectileGun[8];

    public override void _Ready()
    {
        animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        flashWhite = GetNode<AnimationPlayer>("FlashWhite");
        navigation = GetNode<NavigationAgent3D>("NavigationAgent3D");
        attackCD = GetNode<Timer>("AttackCooldown");

        Rotation = new Vector3(Rotation.X, GetTree().CurrentScene.GetNode<Camera3D>("MainCamera").Rotation.Y, Rotation.Z);
        
        projectileLauncher[0] = GetNode<Node3D>("ProjectileLauncher").GetNode<EnemyProjectileGun>("Forward");
        projectileLauncher[1] = GetNode<Node3D>("ProjectileLauncher").GetNode<EnemyProjectileGun>("Right");
        projectileLauncher[2] = GetNode<Node3D>("ProjectileLauncher").GetNode<EnemyProjectileGun>("Back");
        projectileLauncher[3] = GetNode<Node3D>("ProjectileLauncher").GetNode<EnemyProjectileGun>("Left");
        projectileLauncher[4] = GetNode<Node3D>("ProjectileLauncher").GetNode<EnemyProjectileGun>("ForwardRight");
        projectileLauncher[5] = GetNode<Node3D>("ProjectileLauncher").GetNode<EnemyProjectileGun>("ForwardLeft");
        projectileLauncher[6] = GetNode<Node3D>("ProjectileLauncher").GetNode<EnemyProjectileGun>("BackRight");
        projectileLauncher[7] = GetNode<Node3D>("ProjectileLauncher").GetNode<EnemyProjectileGun>("BackLeft");
    }

    public override void _PhysicsProcess(double delta)
    {
        //Handle gravity
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

        if(isAlive && fightStarted){
            //Handle movement
            if(animPlayer.CurrentAnimation != "TakeDamage" && animPlayer.CurrentAnimation != "AttackStartup" && !attacking){
                navigation.TargetPosition = GetTree().CurrentScene.GetNode<PlayerController>("Player").GlobalTransform.Origin;
                velocity = velocity.Lerp((navigation.GetNextPathPosition() - GlobalTransform.Origin).Normalized() * movementSpeed, 10f * (float)delta);
            }   
            else{
                velocity = velocity.Lerp(Vector3.Zero, 10f * (float)delta);
            }

            //Handle attacking
            if(attacking && animPlayer.CurrentAnimation != "AttackCharge" && animPlayer.CurrentAnimation != "AttackProjectile" && attackCD.IsStopped()){
                attackPosition = (GetTree().CurrentScene.GetNode<PlayerController>("Player").GlobalTransform.Origin - GlobalTransform.Origin).Normalized();

                if(animPlayer.CurrentAnimation != "AttackStartup" )
                    animPlayer.Play("AttackStartup");
            }

            if(animPlayer.CurrentAnimation == "AttackCharge" && attackCD.IsStopped()){
                velocity.X = Mathf.Lerp(velocity.X, attackPosition.X * attackRange, 100f * (float)delta);
                velocity.Z = Mathf.Lerp(velocity.Z, attackPosition.Z * attackRange, 100f * (float)delta);
            }
            if(animPlayer.CurrentAnimation == "AttackProjectile" && attackCD.IsStopped() && canShootProjectile){
                launchProjectile();
                canShootProjectile = false;
            }

        }

        Velocity = velocity;
        MoveAndSlide();
    }

    public void takeDamage(float damage){
        health -= damage;

        if(health > 0){
            flashWhite.Play("flash");
        }
        else{
            animPlayer.Play("Death");
            isAlive = false;
        }
    }

    void _on_animation_player_animation_finished(string anim){
        if(anim == "AttackStartup"){
            canShootProjectile = true;
            int attack = rand.Next(0, 2);
            if(attack == 0)
                animPlayer.Play("AttackProjectile");
            else
                animPlayer.Play("AttackCharge");
        }

        if(anim == "AttackCharge"){
            attackCD.Start();
            if(!attackStillInRange){
                attacking = false;
            }

            animPlayer.Play("Idle");
        }

        if(anim == "AttackProjectile"){
            attackCD.Start();
            animPlayer.Play("Idle");
            if(!attackStillInRange){
                attacking = false;
            }
        }

        if(anim == "Death"){
            GameState.timesCleared++;
            GameState.bossDefeated = true;
            GetTree().CurrentScene.GetNode<Sound>("BossMusic").FadeOut();
        }
    }

    void _on_attack_range_body_entered(Node3D body){
        if(body.Name == "Player"){
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

            player.takeDamage(damage);
        }
    }

    private void launchProjectile(){
        foreach (EnemyProjectileGun launcher in projectileLauncher)
        {
            launcher.launchProjectile();
        }
    }
}
