using System;

namespace BlueFw.Coroutines;

internal readonly struct WaitUntilInstruction : IYieldInstruction {

    readonly public Func<bool> Condition;
    readonly public bool Target;

    public WaitUntilInstruction(Func<bool> condition, bool target) {
        Condition = condition;
        Target = target;
    }

    public readonly bool Advance() {
        return Condition() == Target;
    }
}
