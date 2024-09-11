using Godot;
using System;

public partial class Floor : MeshInstance3D
{
    void _on_area_3d_body_entered(Node3D body){
        body.QueueFree();
    }
}
