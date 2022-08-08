﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BlueFw;

public enum MouseButton {
    Left,
    Right,
    Middle,
    XButton1,
    XButton2
}

/// <summary>
/// Provides access to the state of the input for the current frame.
/// </summary>
public static class Input {

    /// <summary>
    /// Provides access to keyboard state.
    /// </summary>
    public static class KB {

        /// <summary>
        /// Direct access to the keyboard state of the previous frame.
        /// </summary>
        public static KeyboardState PreviousState { get; private set; }

        /// <summary>
        /// Direct access to the keyboard state of the current frame.
        /// </summary>
        public static KeyboardState CurrentState { get; private set; }

        internal static void Update() {
            PreviousState = CurrentState;
            CurrentState = Keyboard.GetState();
        }

        /// <summary>
        /// Returns true if <paramref name="key"/> is held down this frame.
        /// </summary>
        public static bool GetKeyPressed(Keys key) {
            return CurrentState.IsKeyDown(key);
        }

        /// <summary>
        /// Returns true if <paramref name="key"/> was pressed this frame.
        /// </summary>
        public static bool GetKeyDown(Keys key) {
            return PreviousState.IsKeyUp(key) && CurrentState.IsKeyDown(key);
        }

        /// <summary>
        /// Returns true if <paramref name="key"/> was released this frame.
        /// </summary>
        public static bool GetKeyUp(Keys key) {
            return PreviousState.IsKeyDown(key) && CurrentState.IsKeyUp(key);
        }
    }

    /// <summary>
    /// Provides access to mouse state.
    /// </summary>
    public static class M {

        /// <summary>
        /// Direct access to the mouse state of the previous frame.
        /// </summary>
        public static MouseState PreviousState { get; private set; }

        /// <summary>
        /// Direct access to the mouse state of the current frame.
        /// </summary>
        public static MouseState CurrentState { get; private set; }

        /// <summary>
        /// The horizontal position of the mouse relative to the window.
        /// </summary>
        public static int MouseX => CurrentState.X;

        /// <summary>
        /// The vertical position of the mouse relative to the window.
        /// </summary>
        public static int MouseY => CurrentState.Y;

        /// <summary>
        /// The mouse position relative to the window as a <see cref="Point"/>.
        /// </summary>
        public static Point MousePositionPoint => CurrentState.Position;

        /// <summary>
        /// The mouse position relative to the window.
        /// </summary>
        public static Vector2 MousePosition { get; private set; }

        /// <summary>
        /// Did the mouse move this frame?
        /// </summary>
        public static bool MouseMoved { get; private set; }

        /// <summary>
        /// The change in scroll wheel value since the last frame.
        /// </summary>
        /// <remarks>
        /// A value &gt; 0 means the user scrolled up this frame.
        /// <br>A value &lt; 0 means the user scrolled down this frame.</br>
        /// </remarks>
        public static int MouseScrollDelta { get; private set; }

        /// <summary>
        /// Did the mouse scroll up this frame?
        /// </summary>
        public static bool MouseScrolledUp { get; private set; }

        /// <summary>
        /// Did the mouse scroll down this frame?
        /// </summary>
        public static bool MouseScrolledDown { get; private set; }

        /// <summary>
        /// The change in horizontal scroll wheel value since the last frame.
        /// </summary>
        /// <remarks>
        /// A value &gt; 0 means the user scrolled right this frame.
        /// <br>A value &lt; 0 means the user scrolled left this frame.</br>
        /// </remarks>
        public static int HorizontalMouseScrollDelta { get; private set; }

        /// <summary>
        /// Did the mouse scroll right this frame?
        /// </summary>
        public static bool MouseScrolledRight { get; private set; }

        /// <summary>
        /// Did the mouse scroll left this frame?
        /// </summary>
        public static bool MouseScrolledLeft { get; private set; }

        internal static void Update() {
            PreviousState = CurrentState;
            CurrentState = Mouse.GetState();

            MousePosition = CurrentState.Position.ToVector2();
            MouseMoved = CurrentState.Position != PreviousState.Position;

            MouseScrollDelta = CurrentState.ScrollWheelValue - PreviousState.ScrollWheelValue;
            MouseScrolledUp = MouseScrollDelta > 0;
            MouseScrolledDown = MouseScrollDelta < 0;

            HorizontalMouseScrollDelta = CurrentState.HorizontalScrollWheelValue - PreviousState.HorizontalScrollWheelValue;
            MouseScrolledRight = HorizontalMouseScrollDelta > 0;
            MouseScrolledLeft = HorizontalMouseScrollDelta < 0;
        }

        /// <summary>
        /// Returns true if <paramref name="button"/> is held down this frame.
        /// </summary>
        public static bool GetMouseButtonPressed(MouseButton button) {
            return button switch {
                MouseButton.Left => CurrentState.LeftButton == ButtonState.Pressed,
                MouseButton.Right => CurrentState.RightButton == ButtonState.Pressed,
                MouseButton.Middle => CurrentState.MiddleButton == ButtonState.Pressed,
                MouseButton.XButton1 => CurrentState.XButton1 == ButtonState.Pressed,
                MouseButton.XButton2 => CurrentState.XButton2 == ButtonState.Pressed,
                _ => false
            };
        }

        /// <summary>
        /// Returns true if <paramref name="button"/> was pressed this frame.
        /// </summary>
        public static bool GetMouseButtonDown(MouseButton button) {
            return button switch {
                MouseButton.Left => PreviousState.LeftButton == ButtonState.Released && CurrentState.LeftButton == ButtonState.Pressed,
                MouseButton.Right => PreviousState.RightButton == ButtonState.Released && CurrentState.RightButton == ButtonState.Pressed,
                MouseButton.Middle => PreviousState.MiddleButton == ButtonState.Released && CurrentState.MiddleButton == ButtonState.Pressed,
                MouseButton.XButton1 => PreviousState.XButton1 == ButtonState.Released && CurrentState.XButton1 == ButtonState.Pressed,
                MouseButton.XButton2 => PreviousState.XButton2 == ButtonState.Released && CurrentState.XButton2 == ButtonState.Pressed,
                _ => false
            };
        }

        /// <summary>
        /// Returns true if <paramref name="button"/> was released this frame.
        /// </summary>
        public static bool GetMouseButtonUp(MouseButton button) {
            return button switch {
                MouseButton.Left => PreviousState.LeftButton == ButtonState.Pressed && CurrentState.LeftButton == ButtonState.Released,
                MouseButton.Right => PreviousState.RightButton == ButtonState.Pressed && CurrentState.RightButton == ButtonState.Released,
                MouseButton.Middle => PreviousState.MiddleButton == ButtonState.Pressed && CurrentState.MiddleButton == ButtonState.Released,
                MouseButton.XButton1 => PreviousState.XButton1 == ButtonState.Pressed && CurrentState.XButton1 == ButtonState.Released,
                MouseButton.XButton2 => PreviousState.XButton2 == ButtonState.Pressed && CurrentState.XButton2 == ButtonState.Released,
                _ => false
            };
        }
    }

    /// <summary>
    /// Provides access to game pad state
    /// </summary>
    public static class GP {
        
        internal static void Update() {
            // todo
        }
    }

    /// <summary>
    /// Whether or not to handle keyboard input.
    /// </summary>
    public static bool EnableKeyboard = false;

    /// <summary>
    /// Whether or not to handle mouse input.
    /// </summary>
    public static bool EnableMouse = false;

    /// <summary>
    /// Whether or not to handle game pad input.
    /// </summary>
    public static bool EnableGamePad = false;

    internal static void Update() {
        if (EnableKeyboard) {
            KB.Update();
        }

        if (EnableMouse) {
            M.Update();
        }

        if (EnableGamePad) {
            GP.Update();
        }
    }
}
