using Godot;

public partial class DreadLevel : Node3D
{
    CameraController camera;
    DialogueManager dialogueManager;

    public override void _Ready(){
        GameState.isAtHome = false;
        GameState.bossDefeated = false;

        camera = GetNode<CameraController>("MainCamera");
        dialogueManager = camera.GetNode<CanvasLayer>("UI").GetNode<DialogueManager>("DialogueManager");

        if(GameState.firstDreadSequence){
            dialogueManager.startDialogue(ResourceLoader.Load<Json>("res://Dialogue/DreadLevelDialogue/DreadLevelOpeningSequence1.json"));
            GameState.firstDreadSequence = false;
        }
        else if(GameState.timesCleared == 1){
            dialogueManager.startDialogue(ResourceLoader.Load<Json>("res://Dialogue/DreadLevelDialogue/DreadLevelContinue.json"));
        }
    }
}
