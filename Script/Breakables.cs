using Godot;
using System;

public partial class Breakables : StaticBody3D
{
    [Export] float health = 10f;

    AnimatedSprite3D sprite;

    private bool isDestroyed = false;
    private float shakeVal = 0;

    public override void _Ready()
    {
        sprite = GetNode<AnimatedSprite3D>("AnimatedSprite3D");
    }

    public override void _PhysicsProcess(double delta)
    {
        //Destroy the breakable when health is depleted
        if(health <= 0 && !isDestroyed){
            destroy();
            isDestroyed = true;
        }

        //Damage shake
        if(shakeVal != 0){
            sprite.Offset = new Vector2(shakeVal, 0);
            shakeVal *= -1;
            shakeVal = Mathf.MoveToward(shakeVal, 0, 3);
        }
    }

    //Change sprite to destroyed version
    private void destroy(){
        GetNode<AnimatedSprite3D>("AnimatedSprite3D").Play("destroyed");
        GetNode<CpuParticles3D>("CPUParticles3D").Emitting = true;
        GetNode<CollisionShape3D>("CollisionShape3D").Disabled = true;
    }

    public void takeDamage(float damage){
        if(!isDestroyed){
            health -= damage;
            shakeVal = 10f;
        }
    }
}
