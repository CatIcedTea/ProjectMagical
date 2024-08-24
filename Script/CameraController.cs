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

	//Different modes of the camera
	public enum CameraMode{
		FollowPlayer
	}

	CameraMode cameraMode = CameraMode.FollowPlayer;

	private PlayerController player;
	
	//Location of the camera
	private Vector3 distance;
	//Length ratio of the Z axis to X axis
	private float lengthRatio;
	//The shorter length of the distance
	private float distanceOffsetShort;
	//Mouse wheel scroll speed multiplier
	private float mouseScrollMultiplier = 2;

	public override void _Ready()
	{
		player = GetTree().CurrentScene.GetNode<PlayerController>("Player");

		lengthRatio = Mathf.Tan(Mathf.Abs(Rotation.Y));
		distanceOffsetShort = lengthRatio * distanceOffset;
		distance = new Vector3(-distanceOffsetShort, distanceOffsetShort + heightOffset, distanceOffset);
	}

	public override void _Process(double delta)
	{	
		//Determine camera mode
		if(cameraMode == CameraMode.FollowPlayer){
			Position = Position.Lerp(player.Position + distance, interpolationRate * (float)delta);
		}

		
		handleZoom();
	}

	//Scroll Zoom function
	private void handleZoom(){
		//Mousewheel scroll speed
		if(Input.IsActionJustPressed("MouseScrollUp") && distanceOffset > 1){
			distanceOffset -= scrollSpeed * mouseScrollMultiplier;
			distanceOffsetShort -= (scrollSpeed * lengthRatio) * mouseScrollMultiplier;
			distance = new Vector3(-distanceOffsetShort, distanceOffsetShort + heightOffset, distanceOffset);
		}
		if(Input.IsActionJustPressed("MouseScrollDown")){
			distanceOffset += scrollSpeed * mouseScrollMultiplier;
			distanceOffsetShort += (scrollSpeed * lengthRatio) * mouseScrollMultiplier;
			distance = new Vector3(-distanceOffsetShort, distanceOffsetShort + heightOffset, distanceOffset);
		}

		//Controller scroll speed
		if(Input.IsActionPressed("ControllerScrollUp") && distanceOffset > 1){
			distanceOffset -= scrollSpeed;
			distanceOffsetShort -= scrollSpeed * lengthRatio;
			distance = new Vector3(-distanceOffsetShort, distanceOffsetShort + heightOffset, distanceOffset);
		}
		if(Input.IsActionPressed("ControllerScrollDown")){
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
	}

	public void setInterpolationRate(float rate){
		interpolationRate = rate;
	}
}
