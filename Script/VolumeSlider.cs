using Godot;
using System;

public partial class VolumeSlider : HSlider
{
    [Export] string sliderName;
	int index;
	public override void _Ready()
	{
		index = AudioServer.GetBusIndex(sliderName);
		Value = Mathf.DbToLinear(AudioServer.GetBusVolumeDb(index));
	}

	void _on_value_changed(float val){
		AudioServer.SetBusVolumeDb(index, (float)Mathf.LinearToDb(Value));
	}
}
