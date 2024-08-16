using System.IO;
using System.Reflection.Metadata.Ecma335;
using Godot;

public partial class DialogueManager : Node2D
{
	[Export] Json DialogueFile;
	string portraitPath = "res://Prefab/Portrait/Animation/";

	//The entire dictionary of the dialogue file
	Godot.Collections.Dictionary currentDialogue;
	//The array of dialogue
	Godot.Collections.Array dialogueArr;
	//The dialogue itself inside of the array
	Godot.Collections.Dictionary currentLine;

	ScrollingText text;
	RichTextLabel nameLeft;
	RichTextLabel nameRight;

	AnimatedSprite2D spriteLeft;
	AnimatedSprite2D spriteRight;
	AnimationPlayer animLeft;
	AnimationPlayer animRight;

	private int line;
	private bool inDialogue = false;
	
	public override void _Ready()
	{
		text = GetNode<ScrollingText>("Text");
		nameLeft = GetNode<RichTextLabel>("NameLeft");
		nameRight = GetNode<RichTextLabel>("NameRight");

		spriteLeft = GetNode<AnimatedSprite2D>("SpriteLeft");
		spriteRight = GetNode<AnimatedSprite2D>("SpriteRight");
		animLeft = GetNode<AnimationPlayer>("AnimationLeft");
		animRight = GetNode<AnimationPlayer>("AnimationRight");

		startDialogue(DialogueFile);
	}

	
	public override void _Process(double delta)
	{
		if(inDialogue){
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

	public void startDialogue(Json jsonDialogue){
		jsonDialogue = DialogueFile;

		currentDialogue = (Godot.Collections.Dictionary)DialogueFile.Data;
		dialogueArr = currentDialogue["Dialogue"].AsGodotArray();

		inDialogue = true;

		line = -1;
		nextLine();
	}

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
					GetNode<ColorRect>("LeftBox").Visible = true;
				else
					GetNode<ColorRect>("LeftBox").Visible = false;
			}
			if(currentLine.ContainsKey("SpeakRight")){
				if((bool)currentLine["SpeakRight"])
					GetNode<ColorRect>("RightBox").Visible = true;
				else
					GetNode<ColorRect>("RightBox").Visible = false;
			}

			if(currentLine.ContainsKey("SpriteLeft")){
				if(((string)currentLine["SpriteLeft"]).ToLower() == "none" || ((string)currentLine["SpriteLeft"]).ToLower() == "empty"){
					spriteLeft.SpriteFrames = null;
					animLeft.Play("RESET");
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
				if(ResourceLoader.Exists("res://Prefab/Portrait/Animation/" + currentLine["SpriteRight"] + ".tres")){
					spriteRight.SpriteFrames = ResourceLoader.Load<SpriteFrames>(portraitPath + currentLine["SpriteRight"] + ".tres");
					if(animRight.IsPlaying())
						animRight.Play("RESET");
					animRight.Play("MoveIn");
				}
			}
		}
		else{
			startDialogue(DialogueFile);
		}
	}

	//THIS IS FOR A TEST, DELETE LATER
	void _on_text_meta_clicked(Variant meta){
		OS.ShellOpen("https://docs.godotengine.org/en/stable/tutorials/ui/bbcode_in_richtextlabel.html");
	}
}
