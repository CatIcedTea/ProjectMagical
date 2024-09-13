using Godot;

public partial class EnemyProjectile : Node3D
{
    float speed = 6f;

    public override void _Ready()
    {
        GlobalPosition = GetParent<Node3D>().GlobalPosition;
        GlobalRotation = GetParent<Node3D>().GlobalRotation;
    }

    public override void _PhysicsProcess(double delta)
    {
        Translate(Vector3.Forward * speed * (float)delta);
    }

    void _on_area_3d_body_entered(Node3D body){
        if(body.Name == "Player"){
            PlayerController player = (PlayerController)body;

            if(!player.isDodging){
                player.takeDamage(3);
                QueueFree();
            }
        }
    }

    void _on_life_time_timeout(){
        QueueFree();
    }
}
