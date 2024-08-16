using Godot;
using System;

public partial class ScrollingText : RichTextLabel
{
	public override void _Ready()
	{
		VisibleCharacters = 0;
	}

	public override void _PhysicsProcess(double delta)
	{
		if(VisibleCharacters < Text.Length)
			VisibleCharacters++;
	}

	//Set current text to scroll
	public void setText(string text){
		VisibleCharacters = 0;
		Text = text;
	}
}
