using Godot;
using System;

public partial class HealthPickup : Node3D
{
    void _on_pickup_body_entered(Node3D body){
        if(body.Name == "Player"){
            PlayerController player = (PlayerController)body;

            if(player.currentHealth < player.health){
                player.heal(4);
                GetNode<AnimationPlayer>("AnimationPlayer").Play("pickup");
            }
        }
    }

    void _on_animation_player_animation_finished(string anim){
        if(anim == "pickup"){
            QueueFree();
        }
    }
}
