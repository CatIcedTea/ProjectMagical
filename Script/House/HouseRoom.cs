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
            GetNode<Area3D>("MirrorPortal").Visible = false;
            GetNode<Area3D>("MirrorPortal").Monitoring = false;
            GetNode<Area3D>("MirrorPortal").SetDeferred("monitorable", false);
            dialogueManager.startDialogue(ResourceLoader.Load<Json>("res://Dialogue/RoomDialogue/OpeningSequence1.json"));

            GetNode<Area3D>("DoorDialogue").Monitoring = true;
            GetNode<Area3D>("DoorDialogue").SetDeferred("monitorable", true);

            GameState.firstStartSequence = false;
        }
        else if(GameState.timesCleared == 1){
            dialogueManager.startDialogue(ResourceLoader.Load<Json>("res://Dialogue/RoomDialogue/FirstClearSequence1.json"));
            GetNode<Area3D>("FirstClearCutscene").Monitoring = true;
            GetNode<Area3D>("FirstClearCutscene").SetDeferred("monitorable", true);
            GetNode<AnimationPlayer>("Cutscene").Play("FirstClearSetup");
            GetNode<DialogueArea>("MirrorPortal").DialogueFile = ResourceLoader.Load<Json>("res://Dialogue/RoomDialogue/FirstClearSequence2.json");
        }
        else if(GameState.timesCleared > 1){
            GetNode<DialogueArea>("MirrorPortal").DialogueFile = ResourceLoader.Load<Json>("res://Dialogue/RoomDialogue/PostClearWarp.json");
        }
    }

    void _on_door_dialogue_area_entered(Area3D area){
        if(area.GetParent().Name == "Player"){
            GetNode<Area3D>("MirrorPortal").Visible = true;
            GetNode<Area3D>("MirrorPortal").Monitoring = true;
            GetNode<Area3D>("MirrorPortal").SetDeferred("monitorable", true);
        }
    }

    void _on_cutscene_animation_finished(string anim){
        if(anim == "FirstClear"){
            PlayerStatus.inDialogue = false;
        }
    }

    void _on_first_clear_cutscene_body_entered(Node3D body){
        if(body.Name == "Player"){
            PlayerStatus.inDialogue = true;
            GetNode<AnimationPlayer>("Cutscene").Play("FirstClear");
            GetNode<Area3D>("FirstClearCutscene").QueueFree();
        }
    }
}
