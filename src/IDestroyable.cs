namespace Blue;

internal interface IDestroyable {

    bool IsDestroyed { get; }

    void Destroy();
}
