using Godot;
using System.Collections;

public partial class InteractionHandler : Area3D
{
	private DialogueManager dialogueManager;
	private Sprite3D interactNotification;

	//List of interactable objects in range
	private ArrayList interactionArrayList = new ArrayList();
	private bool inRange = false;
	private Area3D closestInteraction;

	public override void _Ready()
	{
		dialogueManager = GetTree().CurrentScene.GetNode<Camera3D>("MainCamera").GetNode<CanvasLayer>("UI").GetNode<DialogueManager>("DialogueManager");
		interactNotification = GetParent().GetNode<Sprite3D>("InteractNotification");
	}

	public override void _Process(double delta)
	{
		//If there is an interaction in range
		if(interactionArrayList.Count > 0){
			//Handles notification popup
			if(!PlayerStatus.inDialogue)
				interactNotification.Visible = true;
			else
				interactNotification.Visible = false;

			PlayerStatus.inInteractionRange = true;

			//When the player interacts with the closest interactable object
			if(Input.IsActionJustReleased("Interact")){
				closestInteraction = getClosestInteraction();

				//If it is a dialogue, display it depending on type
				if(closestInteraction.HasNode("IsDialogue")){
					DialogueArea closestDialogue = (DialogueArea)closestInteraction;
					handleDialogue(closestDialogue);
				}

				//If it is an interaction to generate and move to next room
				if(closestInteraction.HasNode("IsNextRoom")){
					interactNotification.Visible = false;
					NextRoom closestRoom = (NextRoom)closestInteraction;
					handleNextRoom(closestRoom);
				}
			}
		}
	}

	//Helper method to find the closest interaction point in range
	private Area3D getClosestInteraction(){
		Area3D closest = (Area3D)interactionArrayList[0];
		foreach(Area3D area in interactionArrayList){
			if(area.GlobalPosition.DistanceTo(GlobalPosition) < closest.GlobalPosition.DistanceTo(GlobalPosition))
				closest = area;
		}

		return closest;
	}

	//Handles the closest dialogue
	private void handleDialogue(DialogueArea closestDialogue){
		if(closestDialogue.dialogueType == DialogueArea.DialogueType.BoxDialogue){
				PlayerStatus.inDialogue = true;
				dialogueManager.startDialogue(closestDialogue.getDialogueFile());
			}

		if(closestDialogue.dialogueType == DialogueArea.DialogueType.SpeechBubble){
				if(!closestDialogue.GetNode<SpeechBubble>("SpeechBubble").Visible)
					closestDialogue.startSpeechBubble();
		}
	}

	//Handles next room interaction
	private void handleNextRoom(NextRoom closestRoom){
		PlayerStatus.inDialogue = true;
		interactionArrayList.Clear();
		closestRoom.transitionNextRoom();
	}

	//Add to interactable list of there is an interactable object in range
	void _on_area_entered(Area3D area){
		if(area.HasNode("IsInteractable")){
			interactionArrayList.Add(area);
		}
	}
	
	//Remove the interactable if out of the area
	void _on_area_exited(Area3D area){
		if(area.HasNode("IsInteractable")){
			interactionArrayList.Remove(area);
		}

		if(interactionArrayList.Count <= 0){
			interactNotification.Visible = false;
			PlayerStatus.inInteractionRange = false;
		}
	}
}
