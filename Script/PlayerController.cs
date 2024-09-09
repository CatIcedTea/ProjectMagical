using Godot;

public partial class PlayerController : CharacterBody3D
{
	public float health = 10;
	public float Speed = 7.5f;
	public float JumpVelocity = 4.5f;

	private AnimatedSprite3D playerSprite;
	private AnimationPlayer animPlayer;
	private	CameraController camera;
	private TextureProgressBar healthBar;
	private DialogueManager dialogueManager;
	private Node3D mascotPosition;
	private Timer dodgeCooldown;

	private CollisionShape3D attackBox;

	private enum FacingDir{
		FacingRight,
		FacingLeft
	}

	private Vector3 direction = Vector3.Zero;
	private FacingDir facingDir = FacingDir.FacingRight;

	private Vector3 lastVelocity = new Vector3();

    public override void _Ready()
    {
		playerSprite = GetNode<AnimatedSprite3D>("PlayerSprite");
		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    	camera = GetTree().CurrentScene.GetNode<CameraController>("MainCamera");
		healthBar = camera.GetNode<CanvasLayer>("UI").GetNode<TextureProgressBar>("HealthBar");
		dialogueManager = camera.GetNode<CanvasLayer>("UI").GetNode<DialogueManager>("DialogueManager");
		mascotPosition = GetNode<Node3D>("Flip").GetNode<Node3D>("MascotPosition");
		dodgeCooldown = GetNode<Timer>("DodgeCooldown");

		attackBox = GetNode<Node3D>("Flip").GetNode<Area3D>("AttackBox").GetNode<CollisionShape3D>("CollisionShape3D");

		//Set rotation basis to camera for movement direction
		Rotation = new Vector3(Rotation.X, camera.Rotation.Y, Rotation.Z);
    }

    public override void _PhysicsProcess(double delta)
	{
		healthBar.Value = health;
		
		Vector3 velocity = Velocity;
		
		//Handle gravity
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		//If not in dialogue
		if(!PlayerStatus.inDialogue){
			//Handle Jump.
			if (Input.IsActionJustPressed("Jump") && IsOnFloor())
			{
				velocity.Y = JumpVelocity;
			}
			if (Input.IsActionJustPressed("ControllerJump") && IsOnFloor() && !PlayerStatus.inInteractionRange)
			{
				velocity.Y = JumpVelocity;
			}

			//Handle movement
			Vector2 inputDir = Input.GetVector("Left", "Right", "Up", "Down");

			if(animPlayer.CurrentAnimation != "Dodge")
				direction = (camera.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

			if (direction != Vector3.Zero&& animPlayer.CurrentAnimation != "Dodge")
			{
				velocity.X = direction.X * Speed;
				velocity.Z = direction.Z * Speed;

				lastVelocity = velocity;

				//Handle flipping
				if( animPlayer.CurrentAnimation != "LightAttack"){
				if(inputDir.X > 0.01f && facingDir == FacingDir.FacingLeft){
					GetNode<Node3D>("Flip").Scale = new Vector3(1, 1 , 1);
					playerSprite.FlipH = true;
					facingDir = FacingDir.FacingRight;
				}
				if(inputDir.X < -0.01f && facingDir == FacingDir.FacingRight){
					GetNode<Node3D>("Flip").Scale = new Vector3(-1, 1 , 1);
					playerSprite.FlipH = false;
					facingDir = FacingDir.FacingLeft;
				}
				}
			}
			else
			{
				velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
				velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
			}

			//Handle attacks
			if(Input.IsActionJustPressed("LightAttack") && animPlayer.CurrentAnimation != "LightAttack" && !GameState.isAtHome){
				animPlayer.Play("LightAttack");
			}

			//Handle dodge
			if(Input.IsActionJustPressed("Dodge") && animPlayer.CurrentAnimation != "Dodge" && dodgeCooldown.IsStopped() && !GameState.isAtHome){
				animPlayer.Play("Dodge");
				dodgeCooldown.Start();
			}

			if(animPlayer.CurrentAnimation == "Dodge"){
				if(direction != Vector3.Zero){
					velocity.X = direction.X * 15;
					velocity.Z = direction.Z * 15;
				}
				else
					velocity = (lastVelocity).Normalized() * 15;
			}

			//Attack lunges
			if(!attackBox.Disabled){
				if(direction != Vector3.Zero){
					velocity.X = direction.X * 4;
					velocity.Z = direction.Z * 4;
				}
				if(facingDir == FacingDir.FacingRight)
					velocity = (lastVelocity).Normalized() * 2;
				else
					velocity = (lastVelocity).Normalized() * 2;
			}
		}
		else{
			//If in dialogue

			//Stop the player
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		//Moves the player
		Velocity = velocity;
		MoveAndSlide();
	}

	public void takeDamage(float damage){
		health -= damage;
	}

	//Handle attack hit detection
	void _on_attack_box_body_entered(Node3D body){
		if(body.IsInGroup("Enemies")){
			Enemy enemy = (Enemy)body;

			enemy.takeDamage(5, this);
			camera.shakeScreen();

			GetNode<Timer>("Hitstop").Start();
			Engine.TimeScale = 0.1;
		}

		if(body.IsInGroup("Breakables")){
			Breakables breakables = (Breakables)body;

			breakables.takeDamage(5);
		}
	}

	void _on_hitstop_timeout(){
		Engine.TimeScale = 1;
	}
}
