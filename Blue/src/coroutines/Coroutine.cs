using BlueFw.Utils;
using System;
using System.Collections.Generic;

namespace BlueFw.Coroutines;

public class Coroutine : IYieldInstruction {

    #region Statics

    /// <summary>
    /// To be used within a coroutine to resume execution on the next frame.
    /// </summary>
    public static IYieldInstruction WaitUntilNextFrame() {
        return null;
    }

    /// <summary>
    /// To be used within a coroutine to resume execution after some time.
    /// </summary>
    public static IYieldInstruction WaitSeconds(float seconds, bool realTime = false) {
        return new WaitSecondsInstruction(seconds, realTime);
    }

    /// <summary>
    /// To be used within a coroutine to pause execution until a condition is met.
    /// </summary>
    public static IYieldInstruction WaitUntil(Func<bool> condition) {
        ArgumentNullException.ThrowIfNull(condition, nameof(condition));
        return WaitUntilInternal(condition, true);
    }

    /// <summary>
    /// To be used within a coroutine to pause execution while a condition is true.
    /// </summary>
    public static IYieldInstruction WaitWhile(Func<bool> condition) {
        ArgumentNullException.ThrowIfNull(condition, nameof(condition));
        return WaitUntilInternal(condition, false);
    }

    static IYieldInstruction WaitUntilInternal(Func<bool> predicate, bool target) {
        return new WaitUntilInstruction(predicate, target);
    }

    #endregion

    internal string Tag { get; private set; }

    internal bool IsPaused;

    Component parentComponent;
    IEnumerator<IYieldInstruction> enumerator;
    IYieldInstruction currentInstruction;

    internal void Initialize(string tag, IEnumerator<IYieldInstruction> enumerator, Component parentComponent) {
        Tag = tag;
        IsPaused = false;

        this.enumerator = enumerator;
        this.parentComponent = parentComponent;
        currentInstruction = null;
    }

    public bool Advance() {
        if (IsPaused) {
            return false;
        }

        if (currentInstruction != null) {
            if (currentInstruction.Advance()) {
                currentInstruction = null;
            }
            else {
                return false;
            }
        }

        if (enumerator.MoveNext()) {
            currentInstruction = enumerator.Current;
            return false;
        }

        return true;
    }

    internal void Release() {
        parentComponent.CoroutineDone(this);
        Clear();
        Pool<Coroutine>.Return(this);
    }

    protected void Clear() {
        Tag = null;
        IsPaused = false;

        enumerator = null;
        parentComponent = null;
        currentInstruction = null;
    }
}