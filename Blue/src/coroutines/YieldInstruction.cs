using BlueFw.Utils;

namespace BlueFw.Coroutines;

public class YieldInstruction {

    protected static T Get<T>() where T : YieldInstruction, new() {
        return Pool<T>.Get();
    }

    static void Return<T>(T instruction) where T : YieldInstruction, new() {
        Pool<T>.Return(instruction);
    }

    internal virtual void Release() {
        Clear();
        Return(this);
    }

    internal virtual bool Advance() { return  true; }

    protected virtual void Clear() { }
}
