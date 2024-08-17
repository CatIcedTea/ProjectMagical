using Godot;
using System;

public partial class PlayerController : CharacterBody3D
{
	public float Speed = 7.5f;
	public float JumpVelocity = 4.5f;

	private bool inDialogueRange = false;
	private Json dialogueHolder;

	private	Camera3D camera;
	private DialogueManager dialogueManager;
	private Sprite3D interactNotification;

    public override void _Ready()
    {
    	camera = GetTree().CurrentScene.GetNode<Camera3D>("MainCamera");
		dialogueManager = camera.GetNode<CanvasLayer>("UI").GetNode<DialogueManager>("DialogueManager");
		interactNotification = GetNode<Sprite3D>("InteractNotification");
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

		if(!PlayerStatus.inDialogue){
			//Handle Jump.
			if (Input.IsActionJustPressed("Jump") && IsOnFloor())
			{
				velocity.Y = JumpVelocity;
			}
			if (Input.IsActionJustPressed("ControllerJump") && IsOnFloor() && !inDialogueRange)
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

			if(inDialogueRange){
				if(Input.IsActionJustReleased("Interact")){
					PlayerStatus.inDialogue = true;
					dialogueManager.startDialogue(dialogueHolder);
				}
			}
		}

		Velocity = velocity;
			MoveAndSlide();
	}

	void _on_interact_range_area_entered(Area3D area){
		if(area.Name == "DialogueArea"){
			interactNotification.Visible = true;
			inDialogueRange = true;

			dialogueHolder = ((DialogueArea)area).getDialogueFile();
		}
	}

	void _on_interact_range_area_exited(Area3D area){
		if(area.Name == "DialogueArea"){
			interactNotification.Visible = false;
			inDialogueRange = false;

			dialogueHolder = null;
		}
	}
}
