namespace BlueFw.Coroutines;

internal struct WaitSecondsInstruction : IYieldInstruction {

    readonly public float Seconds;
    readonly public bool IsRealTime;

    float timer = 0f;

    public WaitSecondsInstruction(float seconds, bool isRealTime) {
        Seconds = seconds;
        IsRealTime = isRealTime;
    }

    public bool Advance() {
        timer += IsRealTime ? Time.UnscaledDeltaTime : Time.DeltaTime;
        return timer >= Seconds;
    }
}
