using Godot;
public partial class HouseRoom : Node3D
{
    CameraController camera;
    DialogueManager dialogueManager;

    public override void _Ready(){
        GameState.isAtHome = true;

        camera = GetNode<CameraController>("MainCamera");
        dialogueManager = camera.GetNode<CanvasLayer>("UI").GetNode<DialogueManager>("DialogueManager");

        if(GameState.firstStartSequence){
            dialogueManager.startDialogue(ResourceLoader.Load<Json>("res://Dialogue/RoomDialogue/OpeningSequence1.json"));
            GameState.firstStartSequence = false;
        }
    }

    void _on_door_dialogue_area_entered(Area3D area){
        if(area.GetParent().Name == "Player"){
            GetNode<Area3D>("MirrorPortal").Visible = true;
            GetNode<Area3D>("MirrorPortal").Monitoring = true;
            GetNode<Area3D>("MirrorPortal").SetDeferred("monitorable", true);
        }
    }
}
