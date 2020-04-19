using Godot;
using System;

public class screen_shake : Node
{
    const Tween.TransitionType TRANS = Tween.TransitionType.Sine;
    const Tween.EaseType EASE = Tween.EaseType.InOut;

    int Priority = 0;
    double Amplitude = 0;
    Camera2D Camera;
    Tween ShakeTween;
    Timer Frequency;
    Timer Duration;

    Vector2 OriginalCameraPosition; 
    public override void _Ready()
    {
        Camera = GetParent() as Camera2D;
        ShakeTween = GetNode("ShakeTween") as Tween;
        Frequency = GetNode("Frequency") as Timer;
        Duration = GetNode("Duration") as Timer;
        OriginalCameraPosition = Camera.Position;
    }

    public void Start(float Duration = (float)0.2, double Frequency = 15, double Amplitude = 16, int Priority = 0)
    {
        if (Priority >= this.Priority)
        {
            this.Priority = Priority;
            this.Amplitude = Amplitude;
            this.Duration.WaitTime = Duration;
            this.Frequency.WaitTime = 1 / (float)Frequency;
            this.Duration.Start();
            this.Frequency.Start();

            NewShake();
        } 
    }
    private void NewShake()
    {
        Vector2 rand = new Vector2();
        RandomNumberGenerator Rng = new RandomNumberGenerator();
        rand.x = Rng.RandfRange((float)-Amplitude, (float)Amplitude);
        rand.y = Rng.RandfRange((float)-Amplitude, (float)Amplitude);

        ShakeTween.InterpolateProperty(Camera, "offset", Camera.Offset, rand, Frequency.WaitTime, TRANS, EASE);
        ShakeTween.Start(); 
    }

    private void Reset()
    {
        ShakeTween.InterpolateProperty(Camera, "offset", Camera.Offset, OriginalCameraPosition, Frequency.WaitTime, TRANS, EASE);
        ShakeTween.Start(); 
        this.Priority = 0;
    }
    private void OnFrequencyTimeout()
    {
        NewShake();
    }

    private void OnDurationTimeout()
    {
        Reset();
        Frequency.Stop();
    }
}
