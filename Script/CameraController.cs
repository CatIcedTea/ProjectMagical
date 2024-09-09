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
	//Current offset value for camera shake
	private float shakeVal = 0;

	private float maxZoomOut = 20f;
	private float maxZoomIn = 2f;

	public override void _Ready()
	{
		player = GetTree().CurrentScene.GetNode<PlayerController>("Player");

		Position = player.Position;

		lengthRatio = Mathf.Tan(Mathf.Abs(Rotation.Y));
		distanceOffsetShort = lengthRatio * distanceOffset;
		distance = new Vector3(-distanceOffsetShort, distanceOffsetShort + heightOffset, distanceOffset);
	}

	public override void _PhysicsProcess(double delta)
	{	
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

		
		
		handleZoom();
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
}
