using Godot;

public partial class SpeechBubble : Node3D
{
	[Export] string dialogue = "";
	[Export] bool repeat = false;
	[Export] float repeatTime = 5;
	[Export] float lifetime = 5;

	ScrollingText text;
	Timer repeatTimer;
	Timer lifetimeTimer;

	public override void _Ready()
	{
		text = GetNode<SubViewportContainer>("SubViewportContainer").GetNode<SubViewport>("SubViewport").GetNode<AnimatedSprite2D>("SpeechBubble").GetNode<ScrollingText>("Text");
		repeatTimer = GetNode<Timer>("Start");
		lifetimeTimer = GetNode<Timer>("Lifetime");

		if(repeat){
			repeatTimer.WaitTime = repeatTime;
			repeatTimer.Start();
		}
		else{
			repeatTimer.WaitTime = 0.001f;
			repeatTimer.Autostart = false;
		}
		lifetimeTimer.WaitTime = lifetime;
	}

	public override void _Process(double delta)
	{
		//Billboard from pivot point
		Rotation = new Vector3(GetTree().CurrentScene.GetNode<Camera3D>("MainCamera").Rotation.X, GetTree().CurrentScene.GetNode<Camera3D>("MainCamera").Rotation.Y, Rotation.Z);
	}

	//Start the speech bubble dialogue
	public void startDialogue(string dialogueText){
		text.setTextCentered(dialogueText);
		Visible = true;
		lifetimeTimer.Start();
	}

	void _on_start_timeout(){
		text.setTextCentered(dialogue);
		Visible = true;
		lifetimeTimer.Start();
	}

	void _on_lifetime_timeout(){
		Visible = false;
		text.setTextCentered("");
		if(repeat)
			repeatTimer.Start();
	}
}
