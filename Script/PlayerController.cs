using System.Runtime;
using Godot;

public partial class PlayerController : CharacterBody3D
{
	public float health = 20;
	public float currentHealth;
	public float damage = 5;
	public float Speed = 7.5f;
	public float JumpVelocity = 4.5f;
	public float dodgeSpeed = 12f;

	public bool isDodging = false;
	private bool canMove = true;

	private Sprite3D playerSprite;
	private AnimationPlayer animPlayer;
	private AnimationPlayer damageAnimation;
	private	CameraController camera;
	private TextureProgressBar healthBar;
	private DialogueManager dialogueManager;
	private Node3D mascotPosition;
	private Timer dodgeCooldown;
	private Timer hitSlowdown;
	private CpuParticles3D walkParticle;

	private CollisionShape3D attackBox;

	private AudioStreamPlayer3D hitConfirm;
	private AudioStreamPlayer3D damaged;
	private AudioStreamPlayer3D healSound;

	private enum FacingDir{
		FacingRight,
		FacingLeft,
		FacingFront,
		FacingBack
	}

	private Vector3 direction = Vector3.Zero;
	private FacingDir facingDir = FacingDir.FacingRight;
	private FacingDir frontBackDir = FacingDir.FacingFront;

	private Vector3 lastVelocity = new Vector3();

    public override void _Ready()
    {
		playerSprite = GetNode<Sprite3D>("PlayerSprite");
		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		damageAnimation = GetNode<AnimationPlayer>("DamageAnimation");
    	camera = GetTree().CurrentScene.GetNode<CameraController>("MainCamera");
		healthBar = camera.GetNode<CanvasLayer>("UI").GetNode<TextureProgressBar>("HealthBar");
		dialogueManager = camera.GetNode<CanvasLayer>("UI").GetNode<DialogueManager>("DialogueManager");
		mascotPosition = GetNode<Node3D>("Flip").GetNode<Node3D>("MascotPosition");
		dodgeCooldown = GetNode<Timer>("DodgeCooldown");
		hitSlowdown = GetNode<Timer>("HitSlowdown");
		walkParticle = GetNode<CpuParticles3D>("WalkParticle");

		attackBox = GetNode<Node3D>("Flip").GetNode<Area3D>("AttackBox").GetNode<CollisionShape3D>("CollisionShape3D");

		hitConfirm = GetNode<Node3D>("Audio").GetNode<AudioStreamPlayer3D>("HitConfirm");
		damaged = GetNode<Node3D>("Audio").GetNode<AudioStreamPlayer3D>("Damage");
		healSound = GetNode<Node3D>("Audio").GetNode<AudioStreamPlayer3D>("Heal");

		//Set rotation basis to camera for movement direction
		Rotation = new Vector3(Rotation.X, camera.Rotation.Y, Rotation.Z);

		currentHealth = health;
    }

    public override void _PhysicsProcess(double delta)
	{
		healthBar.Value = currentHealth;
		
		Vector3 velocity = Velocity;

		canMove = animPlayer.CurrentAnimation != "LightAttack" && animPlayer.CurrentAnimation != "Dodge" && animPlayer.CurrentAnimation != "HeavyAttack" && currentHealth > 0 && !PlayerStatus.inDialogue && !PlayerStatus.isDefeated;
		isDodging = animPlayer.CurrentAnimation == "Dodge";
		
		//Handle gravity
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		if((velocity.X != 0 || velocity.Z != 0) && IsOnFloor()){
			walkParticle.Emitting = true;
		}
		else{
			walkParticle.Emitting = false;
		}

		//If not in dialogue
		if(!PlayerStatus.inDialogue){

			//Handle movement
			Vector2 inputDir = Input.GetVector("Left", "Right", "Up", "Down");

			if(animPlayer.CurrentAnimation != "Dodge")
				direction = (camera.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

			if (direction != Vector3.Zero && canMove)
			{
				velocity.X = direction.X * Speed;
				velocity.Z = direction.Z * Speed;

				lastVelocity = velocity;

				if(canMove){
					if(inputDir.Y < -0.01f)
						animPlayer.Play("WalkBack");
					else
						animPlayer.Play("WalkFront");
				}
			}
			else
			{
				if(canMove)
					animPlayer.Play("Idle");
				velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
				velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
			}

			//Handle flipping
			if(animPlayer.CurrentAnimation != "LightAttack" && animPlayer.CurrentAnimation != "HeavyAttack"){
				if(inputDir.X > 0.01f && facingDir == FacingDir.FacingLeft){
					GetNode<Node3D>("Flip").Scale = new Vector3(1, 1 , 1);
					playerSprite.FlipH = false;
					facingDir = FacingDir.FacingRight;
				}
				if(inputDir.X < -0.01f && facingDir == FacingDir.FacingRight){
					GetNode<Node3D>("Flip").Scale = new Vector3(-1, 1 , 1);
					playerSprite.FlipH = true;
					facingDir = FacingDir.FacingLeft;
				}
			}

			//Handle attacks
			if(Input.IsActionJustPressed("LightAttack") && animPlayer.CurrentAnimation != "LightAttack" && animPlayer.CurrentAnimation != "HeavyAttack" && !GameState.isAtHome){
				animPlayer.Play("LightAttack");
			}

			if(Input.IsActionJustPressed("HeavyAttack") && animPlayer.CurrentAnimation != "LightAttack" && animPlayer.CurrentAnimation != "HeavyAttack" && !GameState.isAtHome){
				animPlayer.Play("HeavyAttack");
			}

			//Handle dodge
			if(Input.IsActionJustPressed("Dodge") && animPlayer.CurrentAnimation != "Dodge" && dodgeCooldown.IsStopped() && !GameState.isAtHome){
				animPlayer.Play("Dodge");
				dodgeCooldown.Start();
			}

			//Dodge movement
			if(animPlayer.CurrentAnimation == "Dodge"){
				if(direction != Vector3.Zero){
					velocity.X = Mathf.Lerp(velocity.X, direction.X * dodgeSpeed, 100 * (float)delta);
					velocity.Z = Mathf.Lerp(velocity.Z ,direction.Z * dodgeSpeed, 100 *(float)delta);
				}
				else{
					velocity.X = Mathf.Lerp(velocity.X, lastVelocity.Normalized().X * dodgeSpeed, 100 *(float)delta);
					velocity.Z = Mathf.Lerp(velocity.Z, lastVelocity.Normalized().Z * dodgeSpeed, 100 *(float)delta);
				}
			}

			//Attack lunges
			if(!attackBox.Disabled){
				if(direction != Vector3.Zero){
					velocity.X = direction.X * 6;
					velocity.Z = direction.Z * 6;
				}
				if(facingDir == FacingDir.FacingRight)
					velocity = (lastVelocity).Normalized() * 6;
				else
					velocity = (lastVelocity).Normalized() * 6;
			}
		}
		else{
			//If in dialogue
			if(PlayerStatus.inDialogue && animPlayer.CurrentAnimation != "Idle")
				animPlayer.Play("Idle");
			//Stop the player
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		//Moves the player
		Velocity = velocity;
		MoveAndSlide();
	}

	public void takeDamage(float damage){
		if(!isDodging && damageAnimation.CurrentAnimation != "Invulnerable" && damageAnimation.CurrentAnimation != "TakeDamage"){
			camera.shakeScreen();
			damageAnimation.Play("TakeDamage");
			hitSlowdown.Start();
			Engine.TimeScale = 0.5;
			currentHealth -= damage;

			if(currentHealth < 0){
				currentHealth = 0;
			}
		}
	}

	public void heal(float amount){
		currentHealth += amount;
		healSound.Play();
		if(currentHealth > health){
			currentHealth = health;
		}
	}

	//Handle attack hit detection
	void _on_attack_box_body_entered(Node3D body){
		if(body.IsInGroup("Enemies")){
			Enemy enemy = (Enemy)body;
			
			if(enemy.isAlive){
				hitConfirm.Play();
				if(animPlayer.CurrentAnimation == "LightAttack"){
					enemy.takeDamage(damage);
					GetNode<Timer>("Hitstop").Start();
				}
				else{
					enemy.takeDamage(damage*2);
					GetNode<Timer>("HitstopHeavy").Start();
				}
				camera.shakeScreen();

				
				Engine.TimeScale = 0.1;
			}
		}

		if(body.IsInGroup("Breakables")){
			Breakables breakables = (Breakables)body;

			if(animPlayer.CurrentAnimation == "LightAttack")
				breakables.takeDamage(damage);
			else
				breakables.takeDamage(damage*2);
		}

		if(body.IsInGroup("Boss")){
			DreadBoss boss = (DreadBoss)body;

			boss.takeDamage(damage);

			hitConfirm.Play();
			if(animPlayer.CurrentAnimation == "LightAttack"){
				boss.takeDamage(damage);
				GetNode<Timer>("Hitstop").Start();
			}
			else{
				boss.takeDamage(damage*2);
				GetNode<Timer>("HitstopHeavy").Start();
			}
			camera.shakeScreen();

			
			Engine.TimeScale = 0.1;
		}
	}

	void _on_hitstop_timeout(){
		Engine.TimeScale = 1;
	}

	void _on_hit_slowdown_timeout(){
		if(currentHealth > 0)
			Engine.TimeScale = 1;
	}

	void _on_damage_animation_animation_finished(string anim){
		if(anim == "TakeDamage"){
			if(currentHealth <= 0){
				PlayerStatus.isDefeated = true;
			}
			else
				damageAnimation.Play("Invulnerable");
		}
	}
}
