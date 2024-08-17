using Godot;

public partial class CameraController : Camera3D
{
	//Distance of the camera from the target from the Z axis
	[Export] float distanceOffset = 10;
	//The height offset
	[Export] float heightOffset = 0f;
	//Interpolate the camera movement
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
	//Actual length of the X axis offset
	private float xOffset;
	private float mouseScrollMultiplier = 2;

	public override void _Ready()
	{
		player = GetTree().CurrentScene.GetNode<PlayerController>("Player");

		lengthRatio = Mathf.Tan(Mathf.Abs(Rotation.Y));
		xOffset = lengthRatio * distanceOffset;
		distance = new Vector3(-xOffset, distanceOffset + heightOffset, distanceOffset);
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
		//Mousewheel
		if(Input.IsActionJustPressed("MouseScrollUp") && distanceOffset > 1){
			distanceOffset -= scrollSpeed * mouseScrollMultiplier;
			xOffset -= (scrollSpeed * lengthRatio) * mouseScrollMultiplier;
			distance = new Vector3(-xOffset, distanceOffset + heightOffset, distanceOffset);
		}
		if(Input.IsActionJustPressed("MouseScrollDown")){
			distanceOffset += scrollSpeed * mouseScrollMultiplier;
			xOffset += (scrollSpeed * lengthRatio) * mouseScrollMultiplier;
			distance = new Vector3(-xOffset, distanceOffset + heightOffset, distanceOffset);
		}

		//Controller
		if(Input.IsActionPressed("ControllerScrollUp") && distanceOffset > 1){
			distanceOffset -= scrollSpeed;
			xOffset -= scrollSpeed * lengthRatio;
			distance = new Vector3(-xOffset, distanceOffset + heightOffset, distanceOffset);
		}
		if(Input.IsActionPressed("ControllerScrollDown")){
			distanceOffset += scrollSpeed;
			xOffset += scrollSpeed * lengthRatio;
			distance = new Vector3(-xOffset, distanceOffset + heightOffset, distanceOffset);
		}
	}
}
