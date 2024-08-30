using System;
using Godot;

public partial class EnemySpawner : Node3D
{
    [Export] PackedScene [] EnemyList;

    public override void _Ready()
    {
        Random rand = new Random();

		int enemyNum = rand.Next(EnemyList.Length);

        var enemy = EnemyList[enemyNum].Instantiate();
	    AddChild(enemy);
    }
}
