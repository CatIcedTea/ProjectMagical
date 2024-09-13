using Godot;
using System;

public partial class DreadStarterRoom : Node3D
{
    public override void _Ready()
    {
        if(GameState.timesCleared == 0){
            GetNode<Area3D>("PortalTutorial").Monitoring = true;
            GetNode<Area3D>("PortalTutorial").SetDeferred("monitorable", true);
        }
    }
}
            
