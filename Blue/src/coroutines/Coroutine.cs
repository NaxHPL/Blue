using BlueFw.Utils;
using System;
using System.Collections.Generic;

namespace BlueFw.Coroutines;

public class Coroutine : YieldInstruction {

    #region Statics

    /// <summary>
    /// To be used within a coroutine to resume execution on the next frame.
    /// </summary>
    public static YieldInstruction WaitUntilNextFrame() {
        return null;
    }

    /// <summary>
    /// To be used within a coroutine to resume execution after some time.
    /// </summary>
    public static YieldInstruction WaitSeconds(float seconds, bool realTime = false) {
        WaitSecondsInstruction instruction = Pool<WaitSecondsInstruction>.Get();
        instruction.Initialize(seconds, realTime);
        return instruction;
    }

    /// <summary>
    /// To be used within a coroutine to pause execution until a condition is met.
    /// </summary>
    public static YieldInstruction WaitUntil(Func<bool> condition) {
        ArgumentNullException.ThrowIfNull(condition, nameof(condition));
        return WaitUntilInternal(condition, true);
    }

    /// <summary>
    /// To be used within a coroutine to pause execution while a condition is true.
    /// </summary>
    public static YieldInstruction WaitWhile(Func<bool> condition) {
        ArgumentNullException.ThrowIfNull(condition, nameof(condition));
        return WaitUntilInternal(condition, false);
    }

    static YieldInstruction WaitUntilInternal(Func<bool> predicate, bool target) {
        WaitUntilInstruction instruction = Pool<WaitUntilInstruction>.Get();
        instruction.Initialize(predicate, target);
        return instruction;
    }

    #endregion

    internal string Tag { get; private set; }

    internal bool IsPaused;

    Component parentComponent;
    IEnumerator<YieldInstruction> enumerator;
    YieldInstruction currentInstruction;

    internal void Initialize(string tag, IEnumerator<YieldInstruction> enumerator, Component parentComponent) {
        Tag = tag;
        IsPaused = false;

        this.enumerator = enumerator;
        this.parentComponent = parentComponent;
        currentInstruction = null;
    }

    internal override bool Advance() {
        if (IsPaused) {
            return false;
        }

        if (currentInstruction != null) {
            if (currentInstruction.Advance()) {
                currentInstruction.Release();
                currentInstruction = null;
            }
            else {
                return false;
            }
        }

        if (enumerator != null && enumerator.MoveNext()) {
            currentInstruction = enumerator.Current;
            return false;
        }

        return true;
    }

    internal override void Release() {
        parentComponent.CoroutineDone(this);
        base.Release();
    }

    protected override void Clear() {
        Tag = null;
        IsPaused = false;

        enumerator = null;
        parentComponent = null;
        currentInstruction = null;
    }

    protected override void ReturnSelfToPool() {
        Pool<Coroutine>.Return(this);
    }
}