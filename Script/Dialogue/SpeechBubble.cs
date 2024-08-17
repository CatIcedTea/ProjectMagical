using Godot;

public partial class SpeechBubble : Node3D
{
	[Export] Json dialogue;
	[Export] string testText = "";

	ScrollingText text;

	public override void _Ready()
	{
		text = GetNode<SubViewportContainer>("SubViewportContainer").GetNode<SubViewport>("SubViewport").GetNode<ScrollingText>("Text");
	}

	public override void _Process(double delta)
	{
		Rotation = new Vector3(GetTree().CurrentScene.GetNode<Camera3D>("MainCamera").Rotation.X, GetTree().CurrentScene.GetNode<Camera3D>("MainCamera").Rotation.Y, Rotation.Z);
	}

	void _on_timer_timeout(){
		text.setTextCentered(testText);
	}
}
