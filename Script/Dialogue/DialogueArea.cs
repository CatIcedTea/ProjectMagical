using System;
using Godot;

public partial class DialogueArea : Area3D
{
	[Export] public Json DialogueFile;
	[Export] bool InteractableSprite = true;
	[Export] bool chronologicalDialogue = false;
	[Export] public DialogueType dialogueType;

	private SpeechBubble speechBubble;
	private Random random = new Random();
	private Godot.Collections.Array dialogueArr;
	private int line = -1;
	private int dialogueLength;

	//Different dialogue type
	public enum DialogueType{
		BoxDialogue,
		SpeechBubble,
		AreaDialogue
	}

	public override void _Ready(){
		//Gets the dialogue data and its length from the json
		dialogueArr = ((Godot.Collections.Dictionary)DialogueFile.Data)["Dialogue"].AsGodotArray();
		dialogueLength = dialogueArr.Count;

		//Add a speech bubble if it is of type SpeechBubble
		if(dialogueType == DialogueType.SpeechBubble){
			AddChild(ResourceLoader.Load<PackedScene>("res://Prefab/SpeechBubble.tscn").Instantiate());
			speechBubble = GetNode<SpeechBubble>("SpeechBubble");
			speechBubble.Visible = false;
		}

		//Set interaction sprite if true
		if(InteractableSprite)
			GetNode<AnimatedSprite3D>("InteractSprite").Visible = true;
		else
			GetNode<AnimatedSprite3D>("InteractSprite").Visible = false;
	}

	//Start the dialogue for the speech bubble depending on type of speech bubble
	public void startSpeechBubble(){
		if(!chronologicalDialogue){
			int randLine;
			randLine = random.Next(0, dialogueLength);
			while(randLine == line){
				randLine = random.Next(0, dialogueLength);
			}

			line = randLine;
			speechBubble.startDialogue((string)dialogueArr[line].AsGodotDictionary()["Text"]);
		}

		if(chronologicalDialogue){
			if(line < dialogueLength - 1)
				line++;
				
			speechBubble.startDialogue((string)dialogueArr[line].AsGodotDictionary()["Text"]);
		}
	}

	public Json getDialogueFile(){
		return DialogueFile;
	}
}
