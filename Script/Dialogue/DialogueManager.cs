using Godot;

public partial class DialogueManager : Node2D
{
	//Path to access portrait sprites
	string portraitPath = "res://Prefab/Portrait/Animation/";
	string audioPath = "res://Asset/Audio/Dialogue/";
	string nextScene;

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

	//Portrait sprites and their animations
	AnimationPlayer transitionAnim;
	AnimatedSprite2D spriteLeft;
	AnimatedSprite2D spriteRight;
	AnimationPlayer animDialogue;
	Sprite2D namePlateLeft;
	Sprite2D namePlateRight;
	AudioStreamPlayer audioPlayer;
	
	AnimationPlayer speechLeft;
	AnimationPlayer speechRight;
	AnimationPlayer animLeft;
	AnimationPlayer animRight;

	//Current line index of the dialogue
	private int line;
	private bool inDialogue = false;
	
	public override void _Ready()
	{
		text = GetNode<Node2D>("Box").GetNode<ScrollingText>("Text");
		namePlateLeft = GetNode<Node2D>("Box").GetNode<Sprite2D>("NamePlateLeft");
		namePlateRight = GetNode<Node2D>("Box").GetNode<Sprite2D>("NamePlateRight");
		nameLeft = namePlateLeft.GetNode<RichTextLabel>("NameLeft");
		nameRight = namePlateRight.GetNode<RichTextLabel>("NameRight");
		speechLeft = GetNode<AnimationPlayer>("AnimationNamePlateLeft");
		speechRight = GetNode<AnimationPlayer>("AnimationNamePlateRight");
		audioPlayer = GetNode<AudioStreamPlayer>("AudioPlayer");

		transitionAnim = GetParent().GetParent().GetNode<AnimationPlayer>("TransitionAnimation");
		spriteLeft = GetNode<AnimatedSprite2D>("SpriteLeft");
		spriteRight = GetNode<AnimatedSprite2D>("SpriteRight");
		animDialogue = GetNode<AnimationPlayer>("AnimationDialogue");
		animLeft = GetNode<AnimationPlayer>("AnimationLeft");
		animRight = GetNode<AnimationPlayer>("AnimationRight");

		namePlateLeft.Visible = false;
		namePlateRight.Visible = false;
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

		if(transitionAnim.CurrentAnimation == "FadeInNextScene" && !transitionAnim.IsPlaying()){
			GetTree().ChangeSceneToFile(nextScene);
		}
	}

	//Starts the dialogue with the given file
	public void startDialogue(Json jsonDialogue){
		PlayerStatus.inDialogue = true;
		Visible = true;
		animDialogue.Play("StartDialogue");

		currentDialogue = (Godot.Collections.Dictionary)jsonDialogue.Data;
		dialogueArr = currentDialogue["Dialogue"].AsGodotArray();

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
				nameLeft.Text = "[center]" + (string)currentLine["NameLeft"] + "[/center]";
			if(currentLine.ContainsKey("NameRight"))
				nameRight.Text = "[center]" + (string)currentLine["NameRight"] + "[/center]";

			if(currentLine.ContainsKey("SpeakLeft")){
				if((bool)currentLine["SpeakLeft"]){
					namePlateLeft.Visible = true;
					speechLeft.Play("MoveIn");
				}
				else
					speechLeft.Play("MoveOut");
			}
			if(currentLine.ContainsKey("SpeakRight")){
				if((bool)currentLine["SpeakRight"]){
					namePlateRight.Visible = true;
					speechRight.Play("MoveIn");
				}
				else
					speechRight.Play("MoveOut");
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

			if(currentLine.ContainsKey("EmotionLeft")){
				spriteLeft.Play((string)currentLine["EmotionLeft"]);
			}
			if(currentLine.ContainsKey("EmotionRight")){
				spriteRight.Play((string)currentLine["EmotionRight"]);
			}

			if(currentLine.ContainsKey("Audio")){
				audioPlayer.Stream = ResourceLoader.Load<AudioStreamMP3>(audioPath + currentLine["Audio"] + ".mp3");
				audioPlayer.Play();
			}

			if(currentLine.ContainsKey("ChangeScene")){
				GameState.nextScene = currentLine["ChangeScene"] + ".tscn";
				ExitDialogue();
				transitionAnim.Play("FadeInNextScene");
			}
		}
		else{
			ExitDialogue();
		}
	}

	//Quit the dialogue mode helper method
	private void ExitDialogue(){
		animDialogue.Play("ExitDialogue");
		animLeft.Play("MoveOut");
		animRight.Play("MoveOut");
		speechLeft.Play("MoveOut");
		speechRight.Play("MoveOut");
		
		audioPlayer.Stop();
	}

	//Set the dialogue visibility off and set dialogue mode off
	void _on_animation_dialogue_animation_finished(string name){
		if(name == "StartDialogue"){
			inDialogue = true;
		}
		if(name == "ExitDialogue"){
			Visible = false;

			inDialogue = false;
			PlayerStatus.inDialogue = false;
		}
	}

	//THIS IS FOR A TEST, DELETE LATER
	void _on_text_meta_clicked(Variant meta){
		OS.ShellOpen("https://docs.godotengine.org/en/stable/tutorials/ui/bbcode_in_richtextlabel.html");
	}

	//Make the nameplates disappear when finishing moving out
	void _on_animation_name_plate_left_animation_finished(string anim){
		if(anim == "MoveOut")
			namePlateLeft.Visible = false;
	}
	void _on_animation_name_plate_right_animation_finished(string anim){
		if(anim == "MoveOut")
			namePlateRight.Visible = false;
	}

	void _on_animation_left_animation_finished(string anim){
		if(anim == "MoveOut")
			spriteLeft.SpriteFrames = null;
	}

	void _on_animation_right_animation_finished(string anim){
		if(anim == "MoveOut")
			spriteRight.SpriteFrames = null;
	}
}
