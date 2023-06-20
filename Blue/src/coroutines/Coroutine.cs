using BlueFw.Utils;
using System;
using System.Collections.Generic;

namespace BlueFw.Coroutines;

public class Coroutine : YieldInstruction {

    #region Statics

    static readonly Dictionary<string, FastList<Coroutine>> coroutinesByTag = new Dictionary<string, FastList<Coroutine>>();

    /// <summary>
    /// Starts a coroutine.
    /// </summary>
    /// <remarks>
    /// Untagged coroutines cannot be paused or stopped.
    /// </remarks>
    public static void Start(IEnumerator<YieldInstruction> coroutine) {
        Start(coroutine, null);
    }

    /// <summary>
    /// Starts a coroutine and assigns it the given tag.
    /// </summary>
    public static void Start(IEnumerator<YieldInstruction> coroutine, string tag) {
        ArgumentNullException.ThrowIfNull(coroutine, nameof(coroutine));
        
        Coroutine cr = Get<Coroutine>();
        cr.Initialize(tag, coroutine);
        cr.Advance(); // advance to the first yield instruction
        CoroutineUpdater.Register(cr);

        if (tag != null) {
            if (!coroutinesByTag.ContainsKey(tag)) {
                coroutinesByTag.Add(tag, new FastList<Coroutine>());
            }

            coroutinesByTag[tag].Add(cr);
        }
    }

    /// <summary>
    /// Pauses all coroutines with the given tag.
    /// </summary>
    /// <param name="tag"></param>
    public static void Pause(string tag) {
        SetPaused(tag, true);
    }

    /// <summary>
    /// Resumes all coroutines with the given tag.
    /// </summary>
    /// <param name="tag"></param>
    public static void Resume(string tag) {
        SetPaused(tag, false);
    }

    static void SetPaused(string tag, bool paused) {
        if (!coroutinesByTag.ContainsKey(tag)) {
            return;
        }

        for (int i = 0; i < coroutinesByTag[tag].Length; i++) {
            coroutinesByTag[tag].Buffer[i].isPaused = paused;
        }
    }

    /// <summary>
    /// Stops all coroutines with the given tag.
    /// </summary>
    public static void Stop(string tag) {
        if (!coroutinesByTag.ContainsKey(tag)) {
            return;
        }

        for (int i = 0; i < coroutinesByTag[tag].Length; i++) {
            CoroutineUpdater.Unregister(coroutinesByTag[tag].Buffer[i]);
        }

        coroutinesByTag[tag].Clear();
    }

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
        WaitSecondsInstruction instruction = Get<WaitSecondsInstruction>();
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
    public static YieldInstruction WaitWhile(Func<bool> predicate) {
        ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));
        return WaitUntilInternal(predicate, false);
    }

    static YieldInstruction WaitUntilInternal(Func<bool> predicate, bool target) {
        WaitUntilInstruction instruction = Get<WaitUntilInstruction>();
        instruction.Initialize(predicate, target);
        return instruction;
    }

    #endregion

    string tag;
    IEnumerator<YieldInstruction> enumerator;
    YieldInstruction currentInstruction;
    bool isPaused;

    void Initialize(string tag, IEnumerator<YieldInstruction> enumerator) {
        this.tag = tag;
        this.enumerator = enumerator;
        currentInstruction = null;
        isPaused = false;
    }

    internal override bool Advance() {
        if (isPaused) {
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

    protected override void Clear() {
        tag = null;
        enumerator = null;
        currentInstruction = null;
        isPaused = false;
    }

    internal override void Release() {
        if (tag != null && coroutinesByTag.ContainsKey(tag)) {
            coroutinesByTag[tag].Remove(this);
        }

        base.Release();
    }

    protected override void ReturnSelfToPool() {
        Pool<Coroutine>.Return(this);
    }
}