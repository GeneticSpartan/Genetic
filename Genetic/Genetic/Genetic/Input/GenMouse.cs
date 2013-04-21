using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Genetic.Input
{
    public class GenMouse
    {
        public enum Buttons { LeftButton, MiddleButton, RightButton, XButton1, XButton2 };

        /// <summary>
        /// The current states of the mouse.
        /// </summary>
        protected MouseState _mouseState;

        /// <summary>
        /// The state of the mouse during the previous update.
        /// </summary>
        protected MouseState _oldMouseState;

        /// <summary>
        /// The horizontal position of the mouse cursor.
        /// </summary>
        protected int _x = 0;

        /// <summary>
        /// The vertical position of the mouse cursor.
        /// </summary>
        protected int _y = 0;

        /// <summary>
        /// The change in the scroll wheel value between the current and previous updates.
        /// </summary>
        protected int _wheel = 0;

        /// <summary>
        /// Gets the horizontal position of the mouse cursor.
        /// </summary>
        public int X
        {
            get { return _x; }
        }

        /// <summary>
        /// Gets the vertical position of the mouse cursor.
        /// </summary>
        public int Y
        {
            get { return _y; }
        }

        /// <summary>
        /// Gets the change in the mouse scroll wheel value between the current and previous updates.
        /// </summary>
        public int Wheel
        {
            get { return _wheel; }
        }

        /// <summary>
        /// Gets the cumulative mouse scroll wheel value since the game started.
        /// </summary>
        public int TotalWheel
        {
            get { return _mouseState.ScrollWheelValue; }
        }

        public GenMouse()
        {
            _mouseState = new MouseState();
            _oldMouseState = new MouseState();
        }

        /// <summary>
        /// Updates the current and previous mouse states.
        /// </summary>
        public void Update()
        {
            _oldMouseState = _mouseState;
            _mouseState = Mouse.GetState();

            _x = _mouseState.X;
            _y = _mouseState.Y;

            _wheel = _mouseState.ScrollWheelValue - _oldMouseState.ScrollWheelValue;
        }

        /// <summary>
        /// Checks if the specified mouse button is currently pressed.
        /// </summary>
        /// <param name="button">The mouse button to check.</param>
        /// <returns>True if the button is currently pressed, false if not.</returns>
        public bool IsPressed(Buttons button)
        {
            return GetButtonState(button).Value == ButtonState.Pressed;
        }

        /// <summary>
        /// Checks if the specified mouse button is currently released.
        /// </summary>
        /// <param name="button">The mouse button to check.</param>
        /// <returns>True if the button is currently released, false if not.</returns>
        public bool IsReleased(Buttons button)
        {
            return GetButtonState(button).Value == ButtonState.Released;
        }

        /// <summary>
        /// Checks if the specified mouse button was just pressed.
        /// </summary>
        /// <param name="button">The mouse button to check.</param>
        /// <returns>True if the button was just pressed, false if not.</returns>
        public bool JustPressed(Buttons button)
        {
            return ((GetButtonState(button, true).Value == ButtonState.Released) && (GetButtonState(button).Value == ButtonState.Pressed)) ? true : false;
        }

        /// <summary>
        /// Checks if the specified mouse button was just released.
        /// </summary>
        /// <param name="button">The mouse button to check.</param>
        /// <returns>True if the button was just released, false if not.</returns>
        public bool JustReleased(Buttons button)
        {
            return ((GetButtonState(button, true).Value == ButtonState.Pressed) && (GetButtonState(button).Value == ButtonState.Released)) ? true : false;
        }

        /// <summary>
        /// Retrieves the mouse button state associated with the specified button.
        /// </summary>
        /// <param name="button">The mouse button to check.</param>
        /// <param name="isOld">A flag used to check either the previous or current mouse state.</param>
        /// <returns>The mouse button state associated with the specified button. Defaults to null if an unknown button is given.</returns>
        protected ButtonState? GetButtonState(Buttons button, bool isOld = false)
        {
            switch (button)
            {
                case Buttons.LeftButton:
                    return (isOld) ? _oldMouseState.LeftButton : _mouseState.LeftButton;
                case Buttons.MiddleButton:
                    return (isOld) ? _oldMouseState.MiddleButton : _mouseState.MiddleButton;
                case Buttons.RightButton:
                    return (isOld) ? _oldMouseState.RightButton : _mouseState.RightButton;
                case Buttons.XButton1:
                    return (isOld) ? _oldMouseState.XButton1 : _mouseState.XButton1;
                case Buttons.XButton2:
                    return (isOld) ? _oldMouseState.XButton2 : _mouseState.XButton2;
                default:
                    return null;
            }
        }
    }
}