using Godot;
using System;

public partial class DialogueArea : Area3D
{
	[Export] Json DialogueFile;

	public Json getDialogueFile(){
		return DialogueFile;
	}
}
