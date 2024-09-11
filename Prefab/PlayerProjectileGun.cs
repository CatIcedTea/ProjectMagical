using Godot;
using System;

public partial class PlayerProjectileGun : Node3D
{
    PackedScene projectile;

    public override void _Ready()
    {
        projectile = ResourceLoader.Load<PackedScene>("res://Prefab/PlayerProjectile.tscn");
    }

    public void launchProjectile(){
        AddChild(projectile.Instantiate());
    }
}
