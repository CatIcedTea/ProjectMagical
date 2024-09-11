using Godot;
using System;

public partial class LoopSound : AudioStreamPlayer
{
    public override void _Process(double delta)
    {
        if(!Playing){
            Play();
        }
    }
}
