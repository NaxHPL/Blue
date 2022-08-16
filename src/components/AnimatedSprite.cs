using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

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

    /// <summary></summary> Yes, the summary is empty on purpose.
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

    /// <summary>
    /// Is the animation playing?
    /// </summary>
    public bool IsPlaying => !isPaused;

    FastList<FrameInfo[]> sequences = new FastList<FrameInfo[]>();
    Dictionary<string, int> sequenceIndicesByName = new Dictionary<string, int>();

    int currentSequence = 0;
    int currentFrameIdx = 0;
    bool isPaused = false;
    PlayMode currentPlayMode = PlayMode.Forwards;

    int pauseAtFrameIndex = -1;
    int stopAtFrameIndex = -1;

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
    /// If a sequence named <paramref name="name"/> doesn't exist, it will be added instead.
    /// </summary>
    /// <param name="name">The name of the sequence.</param>
    /// <param name="frames">The sequence's animation frames.</param>
    public void SetSequence(string name, FrameInfo[] frames) {
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
    /// Gets the number of frames in the specified sequence.
    /// </summary>
    public int GetFrameCount(string sequenceName) {
        if (!sequenceIndicesByName.ContainsKey(sequenceName)) {
            throw new ArgumentException($"A sequence named \"{sequenceName}\" doesn't exist!", nameof(sequenceName));
        }

        return sequences[sequenceIndicesByName[sequenceName]].Length;
    }

    #endregion

    #region Animation Control

    // play (params: sequence name, loop?, [play mode = forwards], [int startingFrameIndex = 0]

    /// <summary>
    /// Pauses the animation.
    /// </summary>
    public void Pause() {
        if (isPaused) {
            return;
        }

        isPaused = true;
        pauseAtFrameIndex = -1;
    }

    /// <summary>
    /// Pauses the animation once it reaches the specified frame index.
    /// </summary>
    /// <param name="frameIndex">The animation will play until this frame, then pause.</param>
    public void Pause(int frameIndex) {
        if (isPaused) {
            return;
        }

        pauseAtFrameIndex = frameIndex;
    }

    /// <summary>
    /// Unpauses the animation.
    /// </summary>
    public void Unpause() {
        if (!isPaused) {
            return;
        }

        isPaused = false;
        pauseAtFrameIndex = -1;
    }

    /// <summary>
    /// Stops the animation.
    /// </summary>
    public void Stop() {
        isPaused = true;
        pauseAtFrameIndex = -1;
        stopAtFrameIndex = -1;
        SetFrame(0);
    }

    /// <summary>
    /// Stops the animation once it reaches the specified frame index.
    /// </summary>
    /// <param name="frameIndex">The animation will play until this frame, then stop.</param>
    public void Stop(int frameIndex) {
        stopAtFrameIndex = frameIndex;
    }

    /// <summary>
    /// Sets the current frame on the currently playing sequence.
    /// </summary>
    public void SetFrame(int frameIndex) {
        if (frameIndex < 0 || frameIndex >= sequences.Buffer[currentSequence].Length) {
            throw new ArgumentOutOfRangeException(
                nameof(frameIndex),
                frameIndex,
                $"Invalid frame index for the current animation sequence! Expected a value in the range [0,{sequences.Buffer[currentSequence].Length-1}]."
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
        if (isPaused) {
            return;
        }

        timer += Time.DeltaTime;

        if (timer < frameTime) {
            return;
        }

        int nextFrame = currentFrameIdx + (currentPlayMode == PlayMode.Forwards ? 1 : -1);

        if (nextFrame < 0) {
            nextFrame = sequences[currentSequence].Length - 1;
        }
        else {
            nextFrame %= sequences[currentSequence].Length;
        }

        if (nextFrame == stopAtFrameIndex) {
            Stop();
        }
        else if (nextFrame == pauseAtFrameIndex) {
            Pause();
        }
        else {
            SetFrame(nextFrame);
        }

        timer = 0f;
    }

    protected override void OnEntityTransformChanged() {
        boundsDirty = true;
    }

    void UpdateBounds() {
        if (!boundsDirty) {
            return;
        }

        FrameInfo currentFrame = sequences.Buffer[currentSequence][currentFrameIdx];

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
        FrameInfo currentFrame = sequences.Buffer[currentSequence][currentFrameIdx];

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
}
