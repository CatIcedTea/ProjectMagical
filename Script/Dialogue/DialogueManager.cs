using System.IO;
using System.Reflection.Metadata.Ecma335;
using Godot;

public partial class DialogueManager : Node2D
{
	//Path to access portrait sprites
	string portraitPath = "res://Prefab/Portrait/Animation/";

	//The entire dictionary of the dialogue file
	Godot.Collections.Dictionary currentDialogue;
	//The array of dialogue
	Godot.Collections.Array dialogueArr;
	//The dialogue itself inside of the array
	Godot.Collections.Dictionary currentLine;

	//Text elements
	ScrollingText text;
	RichTextLabel nameLeft;
	RichTextLabel nameRight;
	ColorRect speechLeft;
	ColorRect speechRight;

	//Portrait sprites and their animations
	AnimatedSprite2D spriteLeft;
	AnimatedSprite2D spriteRight;
	AnimationPlayer animDialogue;
	AnimationPlayer animLeft;
	AnimationPlayer animRight;

	//Current line index of the dialogue
	private int line;
	private bool inDialogue = false;
	
	public override void _Ready()
	{
		text = GetNode<Node2D>("Box").GetNode<ScrollingText>("Text");
		nameLeft = GetNode<Node2D>("Box").GetNode<RichTextLabel>("NameLeft");
		nameRight = GetNode<Node2D>("Box").GetNode<RichTextLabel>("NameRight");
		speechLeft = GetNode<Node2D>("Box").GetNode<ColorRect>("LeftBox");
		speechRight = GetNode<Node2D>("Box").GetNode<ColorRect>("RightBox");

		spriteLeft = GetNode<AnimatedSprite2D>("SpriteLeft");
		spriteRight = GetNode<AnimatedSprite2D>("SpriteRight");
		animDialogue = GetNode<AnimationPlayer>("AnimationDialogue");
		animLeft = GetNode<AnimationPlayer>("AnimationLeft");
		animRight = GetNode<AnimationPlayer>("AnimationRight");
	}

	
	public override void _Process(double delta)
	{
		if(inDialogue){
			//Advances the dialogue
			if(Input.IsActionJustReleased("Confirm")){
				if(text.VisibleCharacters == text.Text.Length)
					nextLine();
				else
					text.VisibleCharacters = text.Text.Length;
			}
		}

		if(Input.IsActionJustReleased("Escape"))
			GetTree().Quit();
	}

	//Starts the dialogue with the given file
	public void startDialogue(Json jsonDialogue){
		Visible = true;
		animDialogue.Play("StartDialogue");

		currentDialogue = (Godot.Collections.Dictionary)jsonDialogue.Data;
		dialogueArr = currentDialogue["Dialogue"].AsGodotArray();

		inDialogue = true;

		line = -1;
		nextLine();
	}

	//Process and advance to the next line of the dialogue
	private void nextLine(){
		if(line < dialogueArr.Count - 1){
			line++;
			currentLine = dialogueArr[line].AsGodotDictionary();

			if(currentLine.ContainsKey("Text"))
				text.setText((string)currentLine["Text"]);

			if(currentLine.ContainsKey("NameLeft"))
				nameLeft.Text = (string)currentLine["NameLeft"];
			if(currentLine.ContainsKey("NameRight"))
				nameRight.Text = (string)currentLine["NameRight"];

			if(currentLine.ContainsKey("SpeakLeft")){
				if((bool)currentLine["SpeakLeft"])
					speechLeft.Visible = true;
				else
					speechLeft.Visible = false;
			}
			if(currentLine.ContainsKey("SpeakRight")){
				if((bool)currentLine["SpeakRight"])
					speechRight.Visible = true;
				else
					speechRight.Visible = false;
			}

			if(currentLine.ContainsKey("SpriteLeft")){
				if(((string)currentLine["SpriteLeft"]).ToLower() == "none" || ((string)currentLine["SpriteLeft"]).ToLower() == "empty"){
					spriteLeft.SpriteFrames = null;
					animLeft.Play("RESET");
				}
				else if(((string)currentLine["SpriteLeft"]).ToLower() == "exit" ){
					animLeft.Play("MoveOut");
				}
				else if(ResourceLoader.Exists("res://Prefab/Portrait/Animation/" + currentLine["SpriteLeft"] + ".tres")){
					spriteLeft.SpriteFrames = ResourceLoader.Load<SpriteFrames>(portraitPath + currentLine["SpriteLeft"] + ".tres");
					if(animLeft.IsPlaying())
						animLeft.Play("RESET");
					animLeft.Play("MoveIn");
				}
			}
			if(currentLine.ContainsKey("SpriteRight")){
				if(((string)currentLine["SpriteRight"]).ToLower() == "none" || ((string)currentLine["SpriteRight"]).ToLower() == "empty"){
					spriteRight.SpriteFrames = null;
					animRight.Play("RESET");
				}
				else if(((string)currentLine["SpriteRight"]).ToLower() == "exit" ){
					animRight.Play("MoveOut");
				}
				if(ResourceLoader.Exists("res://Prefab/Portrait/Animation/" + currentLine["SpriteRight"] + ".tres")){
					spriteRight.SpriteFrames = ResourceLoader.Load<SpriteFrames>(portraitPath + currentLine["SpriteRight"] + ".tres");
					if(animRight.IsPlaying())
						animRight.Play("RESET");
					animRight.Play("MoveIn");
				}
			}
		}
		else{
			animDialogue.Play("ExitDialogue");
			animLeft.Play("MoveOut");
			animRight.Play("MoveOut");
			PlayerStatus.inDialogue = false;
		}
	}

	void _on_animation_dialogue_animation_finished(string name){
		if(name == "ExitDialogue"){
			Visible = false;
		}
	}

	//THIS IS FOR A TEST, DELETE LATER
	void _on_text_meta_clicked(Variant meta){
		OS.ShellOpen("https://docs.godotengine.org/en/stable/tutorials/ui/bbcode_in_richtextlabel.html");
	}
}
