using Godot;
using System.Collections;
using System.Linq;

public partial class PlayerController : CharacterBody3D
{
	public float Speed = 7.5f;
	public float JumpVelocity = 4.5f;

	private bool inDialogueRange = false;
	private ArrayList dialogueArrayList = new ArrayList();

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

		//If not in dialogue
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

			//Handle dodge
			if(Input.IsActionJustPressed("Dodge")){
				
			}

			//Displays interaction notification if there is one nearby
			if(dialogueArrayList.Count > 0)
				interactNotification.Visible = true;
			else
				interactNotification.Visible = false;

			//Handles dialogue interaction depending on dialogue type
			if(inDialogueRange){
				if(Input.IsActionJustReleased("Interact")){
					DialogueArea closestDialogue = getClosestDialogue();

					if(closestDialogue.dialogueType == DialogueArea.DialogueType.BoxDialogue){
						PlayerStatus.inDialogue = true;
						dialogueManager.startDialogue(closestDialogue.getDialogueFile());
					}

					if(closestDialogue.dialogueType == DialogueArea.DialogueType.SpeechBubble){
						if(!closestDialogue.GetNode<SpeechBubble>("SpeechBubble").Visible)
							closestDialogue.startSpeechBubble();
					}
				}
			}
		}
		else{
			//If in dialogue

			//Disable the interact notification in dialogue
			interactNotification.Visible = false;

			//Stop the player
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		//Moves the player
		Velocity = velocity;
		MoveAndSlide();
	}

	//Helper method to find the closest dialogue
	private DialogueArea getClosestDialogue(){
		DialogueArea closest = (DialogueArea)dialogueArrayList[0];
		foreach(DialogueArea dialogue in dialogueArrayList){
			if(dialogue.Position.DistanceTo(Position) < closest.Position.DistanceTo(Position))
				closest = dialogue;
		}

		return closest;
	}

	//When in range of an interactable dialogue, add it to the arraylist
	void _on_interact_range_area_entered(Area3D area){
		if(area.HasNode("IsDialogue")){
			inDialogueRange = true;
			dialogueArrayList.Add((DialogueArea)area);
		}
	}

	//Upon exiting an interactable dialogue, remove it from the arraylist
	void _on_interact_range_area_exited(Area3D area){
		if(area.HasNode("IsDialogue")){
			inDialogueRange = false;
			dialogueArrayList.Remove(area);
		}
	}
}
