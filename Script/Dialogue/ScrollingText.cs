using Godot;

public partial class ScrollingText : RichTextLabel
{
	public override void _Ready()
	{
		VisibleCharacters = 0;
	}

	public override void _PhysicsProcess(double delta)
	{
		//Scrolls through the text character by character
		if(VisibleCharacters < Text.Length)
			VisibleCharacters++;
	}

	//Set current text to scroll
	public void setText(string text){
		VisibleCharacters = 0;
		Text = text;
	}

	public void setTextCentered(string text){
		VisibleCharacters = 0;
		Text = "[center]" + text + "[/center]";
	}
}
