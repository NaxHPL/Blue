using BlueFw.Utils;

namespace BlueFw.Coroutines;

public abstract class YieldInstruction {

    internal virtual void Release() {
        Clear();
        ReturnSelfToPool();
    }

    internal abstract bool Advance();

    protected abstract void Clear();

    protected abstract void ReturnSelfToPool();
}
