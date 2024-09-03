using System;
using Godot;

public partial class EnemySpawner : Node3D
{
    [Export] PackedScene [] EnemyList;

    public override void _Ready()
    {
        spawnEnemy();
    }

    public override void _Process(double delta)
    {
        if(GetTree().GetNodeCountInGroup("Enemies") == 0){
            //spawnEnemy();
        }
    }

    private void spawnEnemy(){
        var enemy = EnemyList[new Random().Next(EnemyList.Length)].Instantiate();
	    AddChild(enemy);
    }
}
