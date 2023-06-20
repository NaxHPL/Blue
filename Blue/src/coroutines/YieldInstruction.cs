using BlueFw.Utils;

namespace BlueFw.Coroutines;

public abstract class YieldInstruction {

    protected static T Get<T>() where T : YieldInstruction, new() {
        return Pool<T>.Get();
    }

    internal virtual void Release() {
        Clear();
        ReturnSelfToPool();
    }

    internal abstract bool Advance();

    protected abstract void Clear();

    protected abstract void ReturnSelfToPool();
}
