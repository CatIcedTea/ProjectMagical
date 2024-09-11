using System.Collections;
using Godot;

public partial class PlayerProjectile : Node3D
{
    private float speed = 25f;

    private ArrayList enemyList = new ArrayList();
    public override void _Ready()
    {
        GlobalPosition = GetParent<Node3D>().GlobalPosition;
        GlobalRotation = GetParent<Node3D>().GlobalRotation;
    }

    public override void _Process(double delta)
    {
        Translate(Vector3.Forward * speed * (float)delta);

        if(enemyList.Count > 0){
            //Vector3 closestEnemyPos = getClosestEnemyPosition();

            //LookAt(closestEnemyPos);
        }
    }

    private Vector3 getClosestEnemyPosition(){
        Node3D firstEnemy = (Node3D)enemyList[0];
        Vector3 closest = firstEnemy.GlobalPosition - GlobalPosition;
        
        foreach (Node3D enemy in enemyList)
        {
            if(enemy.GlobalPosition - GlobalPosition < closest)
                closest = enemy.GlobalPosition;
        }

        return closest;
    }

    void _on_hitbox_body_entered(Node3D body){
        if(body.IsInGroup("Enemies")){
            Enemy enemy = (Enemy)body;

            enemy.takeDamage(GetTree().CurrentScene.GetNode<PlayerController>("Player").damage);
            QueueFree();
        }
    }

    void _on_detection_body_entered(Node3D body){
        if(body.IsInGroup("Enemies")){
            enemyList.Add(body);
        }
    }

    void _on_lifetime_timeout(){
        QueueFree();
    }
}
