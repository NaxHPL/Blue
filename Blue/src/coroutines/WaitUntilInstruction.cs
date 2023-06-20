using BlueFw.Utils;
using System;

namespace BlueFw.Coroutines;

internal class WaitUntilInstruction : YieldInstruction {

    Func<bool> predicate;
    bool target;

    internal void Initialize(Func<bool> predicate, bool target) {
        this.predicate = predicate;
        this.target = target;
    }

    internal override bool Advance() {
        return predicate() == target;
    }

    protected override void Clear() {
        predicate = null;
    }

    protected override void ReturnSelfToPool() {
        Pool<WaitUntilInstruction>.Return(this);
    }
}
