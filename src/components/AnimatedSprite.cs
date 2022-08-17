﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace BlueFw;

/// <summary>
/// A sprite with animation sequences.
/// </summary>
public class AnimatedSprite : Component, IUpdatable, IRenderable {

    /// <summary>
    /// Defines a single frame of animation.
    /// </summary>
    public struct FrameInfo {

        /// <summary>
        /// This frame's texture to draw.
        /// </summary>
        public Texture2D Texture;

        /// <summary>
        /// The region of the texture that will be drawn on this frame.
        /// If <see langword="null"/> (default), the whole texture is drawn.
        /// </summary>
        public Rectangle? SourceRect;

        /// <summary>
        /// The sprite's origin (pivot point) for the frame. Default is the top left.
        /// </summary>
        public Vector2 Origin;

        /// <summary>
        /// If not <see langword="null"/>, this will override the <see cref="Tint"/> property.
        /// </summary>
        public Color? TintOverride;

        /// <summary>
        /// Flip the sprite on the X axis on this frame?
        /// </summary>
        public bool FlipHorizontally {
            get => SpriteEffects.HasFlag(SpriteEffects.FlipHorizontally);
            set {
                SpriteEffects = value ?
                    SpriteEffects | SpriteEffects.FlipHorizontally :
                    SpriteEffects & ~SpriteEffects.FlipHorizontally;
            }
        }

        /// <summary>
        /// Flip the sprite on the Y axis on this frame?
        /// </summary>
        public bool FlipVertically {
            get => SpriteEffects.HasFlag(SpriteEffects.FlipVertically);
            set {
                SpriteEffects = value ?
                    SpriteEffects | SpriteEffects.FlipVertically :
                    SpriteEffects & ~SpriteEffects.FlipVertically;
            }
        }

        internal SpriteEffects SpriteEffects;
    }

    /// <summary></summary>
    public int UpdateOrder => 0;

    public int RenderLayer { get; set; }

    public float LayerDepth { get; set; }

    public Material Material { get; set; }

    public Rect Bounds {
        get { UpdateBounds(); return bounds; }
    }

    /// <summary>
    /// A color to tint the sprite.
    /// </summary>
    public Color Tint = Color.White;

    /// <summary>
    /// The amount of frames the animation should play per second.
    /// </summary>
    public float FramesPerSecond {
        get => frameTime == float.MaxValue ? 0f : 1f / frameTime;
        set {
            if (value < 0f) {
                throw new ArgumentOutOfRangeException(
                    nameof(FramesPerSecond),
                    value,
                    "An Animated Sprite's frames per second cannot be negative!"
                );
            }

            frameTime = value == 0f ? float.MaxValue : 1f / value;
        }
    }

    public string CurrentSequence { get; private set; } = null;

    /// <summary>
    /// Gets/sets whether the animation is paused.
    /// </summary>
    public bool IsPaused { get; private set; }

    FastList<FrameInfo[]> sequences = new FastList<FrameInfo[]>();
    Dictionary<string, int> sequenceIndicesByName = new Dictionary<string, int>();

    int previousSequenceIdx = 0;
    int currentSequenceIdx = 0;
    int currentFrameIdx = 0;
    PlayMode currentPlayMode = PlayMode.Loop;
    PlayDirection currentPlayDirection = PlayDirection.Forwards;

    float timer = 0f;
    float frameTime = 0.2f;

    Rect bounds;
    bool boundsDirty = false;

    public AnimatedSprite() {
        FrameInfo[] fallbackSequence = { new FrameInfo() };
        sequences.Add(fallbackSequence);
    }

    #region Sequences

    /// <summary>
    /// Sets the frames of an animation sequence.
    /// If a sequence named <paramref name="name"/> doesn't exist, it will be added.
    /// </summary>
    /// <param name="name">The name of the sequence.</param>
    /// <param name="frames">The sequence's animation frames.</param>
    public void SetSequence(string name, FrameInfo[] frames) {
        ArgumentNullException.ThrowIfNull(name, nameof(name));
        ArgumentNullException.ThrowIfNull(frames, nameof(frames));

        if (frames.Length == 0) {
            throw new ArgumentException("An animation sequence must have at least one frame!");
        }

        if (sequenceIndicesByName.ContainsKey(name)) {
            // A sequence with this name already exists
            sequences.Buffer[sequenceIndicesByName[name]] = frames;
        }
        else {
            // A sequence with this name doesn't exist yet
            sequenceIndicesByName.Add(name, sequences.Length);
            sequences.Add(frames);
        }
    }

    /// <summary>
    /// Gets the frame data for the specified sequence.
    /// </summary>
    public FrameInfo[] GetSequence(string sequence) {
        if (!sequenceIndicesByName.ContainsKey(sequence)) {
            throw new ArgumentException($"A sequence named \"{sequence}\" doesn't exist!", nameof(sequence));
        }

        return sequences[sequenceIndicesByName[sequence]];
    }

    /// <summary>
    /// Gets the number of frames in the specified sequence.
    /// </summary>
    public int GetFrameCount(string sequence) {
        return GetSequence(sequence).Length;
    }

    #endregion

    #region Animation Control

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sequence"></param>
    /// <param name="mode"></param>
    /// <param name="direction"></param>
    public void PlaySequence(string sequence, PlayMode mode = PlayMode.Loop, PlayDirection direction = PlayDirection.Forwards) {
        ArgumentNullException.ThrowIfNull(sequence, nameof(sequence));

        if (!sequenceIndicesByName.ContainsKey(sequence)) {
            throw new ArgumentException($"A sequence named \"{sequence}\" doesn't exist!", nameof(sequence));
        }

        PlaySequence(sequenceIndicesByName[sequence], mode, direction);
        CurrentSequence = sequence;
    }

    void PlaySequence(int sequenceIdx, PlayMode mode = PlayMode.Loop, PlayDirection direction = PlayDirection.Forwards) {
        // Check if playing the fallback sequence
        if (sequenceIdx == 0) {
            CurrentSequence = null;
        }

        previousSequenceIdx = currentSequenceIdx;

        currentSequenceIdx = sequenceIdx;
        currentPlayMode = mode;
        currentPlayDirection = direction;

        if (direction == PlayDirection.Forwards) {
            SetFrame(0);
        }
        else {
            SetFrame(sequences.Buffer[currentSequenceIdx].Length - 1);
        }

        IsPaused = false;
    }

    /// <summary>
    /// Stops the animation and resets it to the start of the sequence.
    /// </summary>
    public void Stop() {
        IsPaused = true;
        SetFrame(0);
    }

    /// <summary>
    /// Sets the current frame on the currently playing sequence.
    /// </summary>
    public void SetFrame(int frameIndex) {
        if (frameIndex < 0 || frameIndex >= sequences.Buffer[currentSequenceIdx].Length) {
            throw new ArgumentOutOfRangeException(
                nameof(frameIndex),
                frameIndex,
                $"Invalid frame index for the current animation sequence! Expected a value in the range [0,{sequences.Buffer[currentSequenceIdx].Length-1}]."
            );
        }

        currentFrameIdx = frameIndex;
        timer = 0f;
        boundsDirty = true;
    }

    #endregion

    #region Frame Events



    #endregion

    public void Update() {
        if (IsPaused) {
            return;
        }

        timer += Time.DeltaTime;

        if (timer >= frameTime) {
            AdvanceAnimation();
        }
    }

    void AdvanceAnimation() {
        bool reachedSequenceEnd =
            (currentPlayDirection == PlayDirection.Forwards && currentFrameIdx == sequences.Buffer[currentSequenceIdx].Length - 1) ||
            (currentPlayDirection == PlayDirection.Reverse && currentFrameIdx == 0);

        if (reachedSequenceEnd) {
            switch (currentPlayMode) {
                case PlayMode.Once:
                    Stop();
                    return;

                case PlayMode.OnceThenPrevious:
                    PlaySequence(previousSequenceIdx);
                    return;

                case PlayMode.PingPong:
                    currentPlayDirection = currentPlayDirection == PlayDirection.Forwards ?
                            PlayDirection.Reverse :
                            PlayDirection.Forwards;
                    break;

                case PlayMode.Clamp:
                    Stop();
                    if (currentPlayDirection == PlayDirection.Forwards) {
                        SetFrame(sequences.Buffer[currentSequenceIdx].Length - 1);
                    }
                    return;
            }
        }

        int nextFrame = currentFrameIdx + (currentPlayDirection == PlayDirection.Forwards ? 1 : -1);
        if (nextFrame < 0) {
            nextFrame = sequences[currentSequenceIdx].Length - 1;
        }
        else {
            nextFrame %= sequences[currentSequenceIdx].Length;
        }

        SetFrame(nextFrame);
    }

    protected override void OnEntityTransformChanged() {
        boundsDirty = true;
    }

    void UpdateBounds() {
        if (!boundsDirty) {
            return;
        }

        FrameInfo currentFrame = sequences.Buffer[currentSequenceIdx][currentFrameIdx];

        if (currentFrame.Texture == null) {
            bounds = Rect.Zero;
            boundsDirty = false;
            return;
        }

        Point size = currentFrame.SourceRect.HasValue ? currentFrame.SourceRect.Value.Size : currentFrame.Texture.Size();
        bounds.Size = size.ToVector2() * Transform.Scale;
        bounds.Position = Transform.Position - currentFrame.Origin * Transform.Scale;

        if (Transform.Rotation != 0f) {
            Vector2 topLeft = Transform.TransformPoint(new Vector2(bounds.X, bounds.Y));
            Vector2 topRight = Transform.TransformPoint(new Vector2(bounds.X + bounds.Width, bounds.Y));
            Vector2 bottomLeft = Transform.TransformPoint(new Vector2(bounds.X, bounds.Y + bounds.Height));
            Vector2 bottomRight = Transform.TransformPoint(new Vector2(bounds.X + bounds.Width, bounds.Y + bounds.Height));

            float minX = MathExt.Min(topLeft.X, topRight.X, bottomLeft.X, bottomRight.X);
            float maxX = MathExt.Max(topLeft.X, topRight.X, bottomLeft.X, bottomRight.X);
            float minY = MathExt.Min(topLeft.Y, topRight.Y, bottomLeft.Y, bottomRight.Y);
            float maxY = MathExt.Max(topLeft.Y, topRight.Y, bottomLeft.Y, bottomRight.Y);

            bounds.X = minX;
            bounds.Y = minY;
            bounds.Width = maxX - minX;
            bounds.Height = maxY - minY;
        }

        boundsDirty = false;
    }

    public void Render(SpriteBatch spriteBatch, Camera camera) {
        FrameInfo currentFrame = sequences.Buffer[currentSequenceIdx][currentFrameIdx];

        if (currentFrame.Texture == null) {
            return;
        }

        spriteBatch.Draw(
            currentFrame.Texture,
            Transform.Position,
            currentFrame.SourceRect,
            currentFrame.TintOverride.GetValueOrDefault(Tint),
            Transform.Rotation,
            currentFrame.Origin,
            Transform.Scale,
            currentFrame.SpriteEffects,
            0f
        );
    }

    protected override void OnDestroy() {
        for (int i = 0; i < sequences.Length; i++) {
            for (int j = 0; j < sequences.Buffer[i].Length; j++) {
                sequences.Buffer[i][j].Texture = null;
            }
        }
    }
}
