namespace BlueFw.Coroutines;

internal class WaitSecondsInstruction : YieldInstruction {

    float seconds;
    bool isRealTime;
    float timer = 0f;

    internal void Initialize(float seconds, bool isRealTime) {
        this.seconds = seconds;
        this.isRealTime = isRealTime;
        timer = 0f;
    }

    internal override bool Advance() {
        timer += isRealTime ? Time.UnscaledDeltaTime : Time.DeltaTime;
        return timer >= seconds;
    }
}
