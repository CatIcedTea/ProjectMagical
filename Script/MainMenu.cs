using Godot;
using System;

public partial class MainMenu : Node2D
{
    VBoxContainer menu;
    AnimationPlayer animPlayer;

    private enum SelectedButton{
        Start,
        Quit,
        None
    }

    private SelectedButton selectedButton = SelectedButton.None;

    public override void _Ready()
    {
        menu = GetNode<Node2D>("Menu").GetNode<VBoxContainer>("Menu");
        animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        GetNode<TextureButton>("EmptyButton").GrabFocus();

        if(Input.IsActionJustPressed("Escape")){
            GetTree().Quit();
        }

        Bitmap bitmap = new Bitmap();
        bitmap.CreateFromImageAlpha(menu.GetNode<TextureButton>("Start").TextureHover.GetImage());
        menu.GetNode<TextureButton>("Start").TextureClickMask = bitmap;
    }

    void _on_start_pressed(){
        animPlayer.Play("FadeIn");
        selectedButton = SelectedButton.Start;
    }

    void _on_quit_pressed(){
        GetNode<Sound>("Music").FadeOut();
        animPlayer.Play("FadeIn");
        selectedButton = SelectedButton.Quit;
    }

    void _on_settings_pressed(){
        menu.Visible = false;
        GetNode<Node2D>("Menu").GetNode<VBoxContainer>("Settings").Visible = true;
        GetNode<Node2D>("Menu").GetNode<VBoxContainer>("Settings").GetNode<HSlider>("Master").GrabFocus();
    }

    void _on_return_pressed(){
        menu.Visible = true;
        GetNode<Node2D>("Menu").GetNode<VBoxContainer>("Settings").Visible = false;
        GetNode<TextureButton>("EmptyButton").GrabFocus();
    }

    void _on_animation_player_animation_finished(string anim){
        if(anim == "FadeIn"){
            if(selectedButton == SelectedButton.Start)
                GetTree().ChangeSceneToFile("res://Scene/HouseRoom.tscn");
            if(selectedButton == SelectedButton.Quit)
                GetTree().Quit();
        }
    }

    void _on_button_mouse_entered(){
        GetNode<AudioStreamPlayer>("ButtonHover").Play();
    }

    void _on_button_down(){
        GetNode<AudioStreamPlayer>("ButtonClick").Play();
    }

    //Hides mouse cursor
    public override void _Input(InputEvent @event)
    {
        if(@event is InputEventKey or InputEventMouse){
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}
		else if(@event is InputEventJoypadButton or InputEventJoypadMotion{AxisValue: <-0.25f or> 0.25f}){
			Input.MouseMode = Input.MouseModeEnum.Hidden;
		}
    }
}
