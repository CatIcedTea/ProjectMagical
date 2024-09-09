using Godot;
using System;

public partial class DreadLevel : Node3D
{
    CameraController camera;
    DialogueManager dialogueManager;

    public override void _Ready(){
        GameState.isAtHome = false;

        camera = GetNode<CameraController>("MainCamera");
        dialogueManager = camera.GetNode<CanvasLayer>("UI").GetNode<DialogueManager>("DialogueManager");

        if(GameState.firstDreadSequence){
            dialogueManager.startDialogue(ResourceLoader.Load<Json>("res://Dialogue/DreadLevelDialogue/DreadLevelOpeningSequence1.json"));
            GameState.firstDreadSequence = false;
        }
    }
}
