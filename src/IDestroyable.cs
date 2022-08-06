namespace BlueFw;

internal interface IDestroyable {

    bool IsDestroyed { get; }

    void Destroy();
}
