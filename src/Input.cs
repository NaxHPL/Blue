using Microsoft.Xna.Framework;
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
    /// Provides access to keyboard and mouse state.
    /// </summary>
    public static class KBM {

        /// <summary>
        /// The horizontal position of the mouse relative to the window.
        /// </summary>
        public static int MouseX => curMouseState.X;

        /// <summary>
        /// The vertical position of the mouse relative to the window.
        /// </summary>
        public static int MouseY => curMouseState.Y;

        /// <summary>
        /// The mouse position relative to the window as a <see cref="Point"/>.
        /// </summary>
        public static Point MousePositionPoint => curMouseState.Position;

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

        static MouseState prevMouseState, curMouseState;
        static KeyboardState prevKeyboardState, curKeyboardState;

        internal static void SetState(MouseState mouseState, KeyboardState keyboardState) {
            // Set mouse state
            prevMouseState = curMouseState;
            curMouseState = mouseState;

            MousePosition = curMouseState.Position.ToVector2();
            MouseMoved = curMouseState.Position != prevMouseState.Position;
            
            MouseScrollDelta = curMouseState.ScrollWheelValue - prevMouseState.ScrollWheelValue;
            MouseScrolledUp = MouseScrollDelta > 0;
            MouseScrolledDown = MouseScrollDelta < 0;

            HorizontalMouseScrollDelta = curMouseState.HorizontalScrollWheelValue - prevMouseState.HorizontalScrollWheelValue;
            MouseScrolledRight = HorizontalMouseScrollDelta > 0;
            MouseScrolledLeft = HorizontalMouseScrollDelta < 0;

            // Set keyboard state
            prevKeyboardState = curKeyboardState;
            curKeyboardState = keyboardState;
        }

        /// <summary>
        /// Returns true if <paramref name="button"/> is held down this frame.
        /// </summary>
        public static bool GetMouseButtonPressed(MouseButton button) {
            return button switch {
                MouseButton.Left     => curMouseState.LeftButton == ButtonState.Pressed,
                MouseButton.Right    => curMouseState.RightButton == ButtonState.Pressed,
                MouseButton.Middle   => curMouseState.MiddleButton == ButtonState.Pressed,
                MouseButton.XButton1 => curMouseState.XButton1 == ButtonState.Pressed,
                MouseButton.XButton2 => curMouseState.XButton2 == ButtonState.Pressed,
                _                    => false
            };
        }

        /// <summary>
        /// Returns true if <paramref name="button"/> was pressed this frame.
        /// </summary>
        public static bool GetMouseButtonDown(MouseButton button) {
            return button switch {
                MouseButton.Left     => prevMouseState.LeftButton == ButtonState.Released && curMouseState.LeftButton == ButtonState.Pressed,
                MouseButton.Right    => prevMouseState.RightButton == ButtonState.Released && curMouseState.RightButton == ButtonState.Pressed,
                MouseButton.Middle   => prevMouseState.MiddleButton == ButtonState.Released && curMouseState.MiddleButton == ButtonState.Pressed,
                MouseButton.XButton1 => prevMouseState.XButton1 == ButtonState.Released && curMouseState.XButton1 == ButtonState.Pressed,
                MouseButton.XButton2 => prevMouseState.XButton2 == ButtonState.Released && curMouseState.XButton2 == ButtonState.Pressed,
                _                    => false
            };
        }

        /// <summary>
        /// Returns true if <paramref name="button"/> was released this frame.
        /// </summary>
        public static bool GetMouseButtonUp(MouseButton button) {
            return button switch {
                MouseButton.Left     => prevMouseState.LeftButton == ButtonState.Pressed && curMouseState.LeftButton == ButtonState.Released,
                MouseButton.Right    => prevMouseState.RightButton == ButtonState.Pressed && curMouseState.RightButton == ButtonState.Released,
                MouseButton.Middle   => prevMouseState.MiddleButton == ButtonState.Pressed && curMouseState.MiddleButton == ButtonState.Released,
                MouseButton.XButton1 => prevMouseState.XButton1 == ButtonState.Pressed && curMouseState.XButton1 == ButtonState.Released,
                MouseButton.XButton2 => prevMouseState.XButton2 == ButtonState.Pressed && curMouseState.XButton2 == ButtonState.Released,
                _                    => false
            };
        }

        /// <summary>
        /// Returns true if <paramref name="key"/> is held down this frame.
        /// </summary>
        public static bool GetKeyPressed(Keys key) {
            return curKeyboardState.IsKeyDown(key);
        }

        /// <summary>
        /// Returns true if <paramref name="key"/> was pressed this frame.
        /// </summary>
        public static bool GetKeyDown(Keys key) {
            return prevKeyboardState.IsKeyUp(key) && curKeyboardState.IsKeyDown(key);
        }

        /// <summary>
        /// Returns true if <paramref name="key"/> was released this frame.
        /// </summary>
        public static bool GetKeyUp(Keys key) {
            return prevKeyboardState.IsKeyDown(key) && curKeyboardState.IsKeyUp(key);
        }
    }

    /// <summary>
    /// Provides access to game pad state
    /// </summary>
    public static class GamePad {
        // todo
    }

    internal static void SetState(MouseState mouseState, KeyboardState keyboardState) {
        KBM.SetState(mouseState, keyboardState);
    }
}
