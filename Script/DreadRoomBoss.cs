using Godot;
using System;

public partial class DreadRoomBoss : Node3D
{
    public override void _Ready()
    {
        GetTree().CurrentScene.GetNode<Sound>("Music").FadeOut();

        if(!GameState.firstBossSequence){
            GetNode<Area3D>("StartFightDialogue").Visible = false;
            GetNode<Area3D>("StartFightDialogue").Monitoring = false;
            GetNode<Area3D>("StartFightDialogue").SetDeferred("monitorable", false);
            GameState.firstBossSequence = false;
        }
    }

    public override void _Process(double delta)
    {
        if(GameState.bossDefeated){
            GetNode<Area3D>("AfterFightDialogue").Visible = true;
            GetNode<Area3D>("AfterFightDialogue").Monitoring = true;
            GetNode<Area3D>("AfterFightDialogue").SetDeferred("monitorable", true);
        }
    }

    void _on_start_fight_body_entered(Node3D body){
        if(body.Name == "Player"){
            GetNode<DreadBoss>("DreadBoss").fightStarted = true;
            GetNode<Area3D>("StartFight").QueueFree();
            GetTree().CurrentScene.GetNode<Sound>("BossMusic").FadeIn();
        }
    }
}
