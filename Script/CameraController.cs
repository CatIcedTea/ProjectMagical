using Godot;

public partial class CameraController : Camera3D
{
	//Distance of the camera from the target from the Z axis
	[Export] float distanceOffset = 10;
	//The height offset
	[Export] float heightOffset = 0f;
	//Rate of interpolate of the camera movement
	[Export] float interpolationRate = 5f;
	//Speed to zoom in and out
	[Export] float scrollSpeed = 0.25f;
	[Export] float shakeAmount = 0.1f;
	[Export] CameraMode cameraMode = CameraMode.FollowPlayer;
	[Export] Node3D followObject;

	//Different modes of the camera
	public enum CameraMode{
		FollowPlayer,
		FollowObject,
		Cutscene
	}

	private PlayerController player;
	
	//Location of the camera
	private Vector3 distance;
	//Length ratio of the Z axis to X axis
	private float lengthRatio;
	//The shorter length of the distance
	private float distanceOffsetShort;
	//Mouse wheel scroll speed multiplier
	private float mouseScrollMultiplier = 2;
	//Current offset value for camera shake
	private float shakeVal = 0;

	private float maxZoomOut = 20f;
	private float maxZoomIn = 2f;

	private Node2D menu;
	private bool inMenu = false;

	public override void _Ready()
	{
		player = GetTree().CurrentScene.GetNode<PlayerController>("Player");

		Position = player.Position;

		menu = GetNode<CanvasLayer>("UI").GetNode<Node2D>("Menu");

		lengthRatio = Mathf.Tan(Mathf.Abs(Rotation.Y));
		distanceOffsetShort = lengthRatio * distanceOffset;
		distance = new Vector3(-distanceOffsetShort, distanceOffsetShort + heightOffset, distanceOffset);

		followObject = player;
	}

	public override void _PhysicsProcess(double delta)
	{	
		if(cameraMode == CameraMode.FollowPlayer){
			if(PlayerStatus.isDefeated){
				PlayerStatus.inMenu = true;
				inMenu = true;
				menu.GetNode<VBoxContainer>("Menu").Visible = false;
				menu.GetNode<VBoxContainer>("Settings").Visible = false;
				menu.GetNode<VBoxContainer>("GameOver").Visible = true;
				menu.GetNode<Label>("GameOverText").Visible = true;
			}
			if(GameState.isAtHome)
				GetNode<CanvasLayer>("UI").GetNode<TextureProgressBar>("HealthBar").Visible = false;

			//Determine camera mode
			if(cameraMode == CameraMode.FollowPlayer){
				Position = Position.Lerp(player.Position + distance, interpolationRate * (float)delta);
			}

			//Shake the camera if value is more than 0
			if(shakeVal != 0){
				SetHOffset(shakeVal);
				shakeVal = Mathf.Lerp(shakeVal, shakeVal*-1, 1f * (float)delta);
				shakeVal = Mathf.MoveToward(shakeVal, 0, 0.5f * (float)delta);

				shakeVal *= -1;
			}

			if(Input.IsActionJustPressed("Escape")){
				if(!inMenu){
					menu.GetNode<TextureButton>("EmptyButton").GrabFocus();
					PlayerStatus.inMenu = true;
					inMenu = true;
				}
				else{
					menu.GetNode<VBoxContainer>("Menu").Visible = true;
					menu.GetNode<VBoxContainer>("Settings").Visible = false;
					PlayerStatus.inMenu = false;
					inMenu = false;
					Engine.TimeScale = 1;
				}
			}

			if(inMenu){
				menu.Visible = true;
				Engine.TimeScale = 0;
			}
			else{
				menu.Visible = false;
			}
			
			handleZoom();
		}
		else if(cameraMode == CameraMode.FollowObject){
			Position = Position.Lerp(followObject.Position + distance, interpolationRate * (float)delta);
		}
	}

	//Scroll Zoom function
	private void handleZoom(){
		//Mousewheel scroll speed
		if(Input.IsActionJustPressed("MouseScrollUp") && distanceOffset > maxZoomIn){
			distanceOffset -= scrollSpeed * mouseScrollMultiplier;
			distanceOffsetShort -= (scrollSpeed * lengthRatio) * mouseScrollMultiplier;
			distance = new Vector3(-distanceOffsetShort, distanceOffsetShort + heightOffset, distanceOffset);
		}
		if(Input.IsActionJustPressed("MouseScrollDown") && distanceOffset < maxZoomOut){
			distanceOffset += scrollSpeed * mouseScrollMultiplier;
			distanceOffsetShort += (scrollSpeed * lengthRatio) * mouseScrollMultiplier;
			distance = new Vector3(-distanceOffsetShort, distanceOffsetShort + heightOffset, distanceOffset);
		}

		//Controller scroll speed
		if(Input.IsActionPressed("ControllerScrollUp") && distanceOffset > maxZoomIn){
			distanceOffset -= scrollSpeed;
			distanceOffsetShort -= scrollSpeed * lengthRatio;
			distance = new Vector3(-distanceOffsetShort, distanceOffsetShort + heightOffset, distanceOffset);
		}
		if(Input.IsActionPressed("ControllerScrollDown") && distanceOffset < maxZoomOut){
			distanceOffset += scrollSpeed;
			distanceOffsetShort += scrollSpeed * lengthRatio;
			distance = new Vector3(-distanceOffsetShort, distanceOffsetShort + heightOffset, distanceOffset);
		}
	}

	void _on_transition_animation_animation_finished(string anim){
		if(anim == "FadeInNextRoom")
			PlayerStatus.transitioningRoom = true;
		if(anim == "FadeOutNextRoom")
			PlayerStatus.inDialogue = false;
		if(anim == "FadeInNextScene")
			GetTree().ChangeSceneToFile(GameState.nextScene);
	}

	public void shakeScreen(){
		shakeVal = shakeAmount;
	}

	void _on_resume_pressed(){
		menu.GetNode<VBoxContainer>("Menu").Visible = true;
		menu.GetNode<VBoxContainer>("Settings").Visible = false;
		PlayerStatus.inMenu = false;
		Engine.TimeScale = 1;
		inMenu = false;
	}

	void _on_settings_pressed(){
		menu.GetNode<VBoxContainer>("Menu").Visible = false;
		menu.GetNode<VBoxContainer>("Settings").Visible = true;
		menu.GetNode<VBoxContainer>("Settings").GetNode<HSlider>("Master").GrabFocus();
	}

	void _on_quit_pressed(){
		PlayerStatus.inMenu = false;
		PlayerStatus.isDefeated = false;
		Engine.TimeScale = 1;
		GetTree().ChangeSceneToFile("res://Scene/MainMenu.tscn");
	}

	void _on_return_pressed(){
		menu.GetNode<TextureButton>("EmptyButton").GrabFocus();
		menu.GetNode<VBoxContainer>("Menu").Visible = true;
		menu.GetNode<VBoxContainer>("Settings").Visible = false;
	}

	void _on_restart_level_pressed(){
		PlayerStatus.inMenu = false;
		PlayerStatus.isDefeated = false;
		Engine.TimeScale = 1;
		GetTree().ChangeSceneToFile("res://Scene/DreadLevel.tscn");
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
        if((@event is InputEventKey or InputEventMouse) && inMenu){
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}
		else if(@event is InputEventJoypadButton or InputEventJoypadMotion{AxisValue: <-0.25f or> 0.25f}){
			Input.MouseMode = Input.MouseModeEnum.Hidden;
		}
    }
}
