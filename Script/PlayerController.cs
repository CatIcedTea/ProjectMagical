using Godot;
using System.Collections;

public partial class PlayerController : CharacterBody3D
{
	public float Speed = 7.5f;
	public float JumpVelocity = 4.5f;

	private	Camera3D camera;
	private DialogueManager dialogueManager;

    public override void _Ready()
    {
    	camera = GetTree().CurrentScene.GetNode<Camera3D>("MainCamera");
		dialogueManager = camera.GetNode<CanvasLayer>("UI").GetNode<DialogueManager>("DialogueManager");
    }

    public override void _PhysicsProcess(double delta)
	{
		//Set rotation basis to camera for movement direction
		GlobalBasis = camera.GlobalBasis;
		
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
