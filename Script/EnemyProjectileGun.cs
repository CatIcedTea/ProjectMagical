using Godot;
using System;

public partial class EnemyProjectileGun : Node3D
{
    [Export] bool aimedProjectile;

    PackedScene projectile;
    
    public override void _Ready()
    {
        projectile = ResourceLoader.Load<PackedScene>("res://Prefab/EnemyProjectile.tscn");
    }

    public void launchProjectile(){
        if(aimedProjectile){
            Vector3 playerPos = GetTree().CurrentScene.GetNode<Node3D>("Player").Position;
            LookAt(new Vector3(playerPos.X, Position.Y, playerPos.Z));
        }
        AddChild(projectile.Instantiate());
    }
}
