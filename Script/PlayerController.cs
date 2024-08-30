using Godot;
using System.Collections;

public partial class PlayerController : CharacterBody3D
{
	public float Speed = 7.5f;
	public float JumpVelocity = 4.5f;

	private AnimatedSprite3D playerSprite;
	private	Camera3D camera;
	private DialogueManager dialogueManager;
	private Node3D mascotPosition;

	private enum FacingDir{
		FacingRight,
		FacingLeft
	}

	private FacingDir facingDir = FacingDir.FacingRight;

    public override void _Ready()
    {
		playerSprite = GetNode<AnimatedSprite3D>("PlayerSprite");
    	camera = GetTree().CurrentScene.GetNode<Camera3D>("MainCamera");
		dialogueManager = camera.GetNode<CanvasLayer>("UI").GetNode<DialogueManager>("DialogueManager");
		mascotPosition = GetNode<Node3D>("Flip").GetNode<Node3D>("MascotPosition");

		//Set rotation basis to camera for movement direction
		GlobalBasis = camera.GlobalBasis;
    }

    public override void _PhysicsProcess(double delta)
	{
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
			Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
			if (direction != Vector3.Zero)
			{
				velocity.X = direction.X * Speed;
				velocity.Z = direction.Z * Speed;

				//Handle flipping
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
			else
			{
				velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
				velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
			}

			//Handle dodge
			if(Input.IsActionJustPressed("Dodge")){
				
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
}
