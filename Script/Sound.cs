using Godot;

public partial class Sound : AudioStreamPlayer
{
    [Export] bool autoPlay = false;
    [Export] bool setLooping = false;
    [Export] bool startFadeIn = false;
    [Export] float fadeSpeed = 45f;
    
    bool hasStarted = false;
    bool fadeOut = false;
    bool fadeIn = false;
    bool fadeTo = false;
    float fadeVal = 0;
    
    public override void _Ready()
    {
        if(startFadeIn){
            VolumeDb = -80f;
            FadeIn();
        }

        if(autoPlay){
            hasStarted = true;
            Play();
        }
    }
    public override void _PhysicsProcess(double delta)
    {
        if(!Playing && hasStarted){
            if(setLooping){
                Play();
            }
        }

        if(fadeOut && VolumeDb > -80){
            VolumeDb -= fadeSpeed * (float)delta;
            if(VolumeDb <= -80){
                VolumeDb = -80;
                fadeOut = false;
            }

            
        }

        if(fadeIn && VolumeDb < 0){
            VolumeDb += fadeSpeed * (float)delta;
            if(VolumeDb >= 0){
                VolumeDb = 0;
            }
        }
        else{
            fadeIn = false;
        }

        if(fadeTo && VolumeDb != fadeVal){
            if(VolumeDb < fadeVal){
                VolumeDb += fadeSpeed * (float)delta;
            }
            else if(VolumeDb > fadeVal){
                VolumeDb -= fadeSpeed * (float)delta;
            }
        }
        else{
            fadeTo = false;
        }
    }

    public void StopMusic(){
        setLooping = false;
        Stop();
    }

    public void StartMusic(){
        setLooping = true;
        Play();
    }

    public void FadeOut(){
        fadeOut = true;

        fadeIn = false;
        fadeTo = false;
    }

    public void FadeTo(float val){
        if(!hasStarted){
            hasStarted = true;
        }
        fadeVal = val;
        fadeTo = true;

        fadeIn = false;
        fadeOut = false;
    }

    public void FadeIn(){
        if(!hasStarted){
            hasStarted = true;
        }
        fadeIn = true;

        fadeOut = false;
        fadeTo = false;
    }
}
