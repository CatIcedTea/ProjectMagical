using Godot;
using System;

public partial class GameState : Node
{
    public static string nextScene;

    public static bool firstStartSequence = true;
    public static bool firstMidSequence = true;
    public static bool firstDreadSequence = true;
    public static bool firstBossSequence = true;
    public static bool bossDefeated = false;
    public static int timesCleared = 0;

    public static bool isAtHome = false;
}
