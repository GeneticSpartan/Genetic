using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Genetic.Input
{
    public class GenGamePad
    {
        /// <summary>
        /// The current state of the game pad.
        /// </summary>
        protected GamePadState _gamePadState;

        /// <summary>
        /// The state of the game pad during the previous update.
        /// </summary>
        protected GamePadState _oldGamePadState;

        /// <summary>
        /// The player index associated with the game pad.
        /// </summary>
        protected PlayerIndex _playerIndex;

        /// <summary>
        /// Gets the x position of the left thumbstick, a value between -1 and 1.
        /// </summary>
        public float ThumbStickLeftX
        {
            get { return _gamePadState.ThumbSticks.Left.X; }
        }

        /// <summary>
        /// Gets the y position of the left thumbstick, a value between -1 and 1.
        /// </summary>
        public float ThumbStickLeftY
        {
            get { return _gamePadState.ThumbSticks.Left.Y; }
        }

        /// <summary>
        /// Gets the x position of the right thumbstick, a value between -1 and 1.
        /// </summary>
        public float ThumbStickRightX
        {
            get { return _gamePadState.ThumbSticks.Right.X; }
        }

        /// <summary>
        /// Gets the y position of the right thumbstick, a value between -1 and 1.
        /// </summary>
        public float ThumbStickRightY
        {
            get { return _gamePadState.ThumbSticks.Right.Y; }
        }

        /// <summary>
        /// Gets the position of the left trigger, a value between 0 and 1.
        /// </summary>
        public float TriggerLeft
        {
            get { return _gamePadState.Triggers.Left; }
        }

        /// <summary>
        /// Gets the position of the right trigger, a value between 0 and 1.
        /// </summary>
        public float TriggerRight
        {
            get { return _gamePadState.Triggers.Right; }
        }

        /// <summary>
        /// Gets whether or not the game pad is currently connected.
        /// </summary>
        public bool IsConnected
        {
            get { return _gamePadState.IsConnected; }
        }

        /// <summary>
        /// Gets the current packet number of the game pad.
        /// If the packet number has not changed between two sequential game pad states, then the input has not changed.
        /// </summary>
        public int PacketNumber
        {
            get { return _gamePadState.PacketNumber; }
        }

        /// <summary>
        /// An object used to retrieve input from a game pad.
        /// </summary>
        /// <param name="playerIndex">The player index associated with the game pad.</param>
        public GenGamePad(PlayerIndex playerIndex = PlayerIndex.One)
        {
            _playerIndex = playerIndex;
        }

        /// <summary>
        /// Updates the current and previous game pad states.
        /// </summary>
        public void Update()
        {
            _oldGamePadState = _gamePadState;
            _gamePadState = GamePad.GetState(_playerIndex);
        }

        /// <summary>
        /// Checks if the specified button is currently pressed.
        /// </summary>
        /// <param name="button">The game pad button to check.</param>
        /// <returns>True if the button is currently pressed, false if not.</returns>
        public bool IsPressed(Buttons button)
        {
            return _gamePadState.IsButtonDown(button);
        }

        /// <summary>
        /// Checks if the specified button is currently released.
        /// </summary>
        /// <param name="button">The game pad button to check.</param>
        /// <returns>True if the button is currently released, false if not.</returns>
        public bool IsReleased(Buttons button)
        {
            return _gamePadState.IsButtonUp(button);
        }

        /// <summary>
        /// Checks if the specified button was just pressed.
        /// </summary>
        /// <param name="button">The game pad button to check.</param>
        /// <returns>True if the button was just pressed, false if not.</returns>
        public bool JustPressed(Buttons button)
        {
            return (_oldGamePadState.IsButtonUp(button) && _gamePadState.IsButtonDown(button)) ? true : false;
        }

        /// <summary>
        /// Checks if the specified button was just released.
        /// </summary>
        /// <param name="button">The game pad button to check.</param>
        /// <returns>True if the button was just released, false if not.</returns>
        public bool JustReleased(Buttons button)
        {
            return (_oldGamePadState.IsButtonDown(button) && _gamePadState.IsButtonUp(button)) ? true : false;
        }

        /// <summary>
        /// Sets the vibration speeds of the left and right motors of the game pad.
        /// </summary>
        /// <param name="leftMotor">The speed of the left motor, a value between 0.0 and 1.0. This motor is a low-frequency motor.</param>
        /// <param name="rightMotor">The speed of the right motor, a value between 0.0 and 1.0. This motor is a high-frequency motor.</param>
        /// <returns>True if the vibration of the motors were successfully set, false if not.</returns>
        public bool SetVibration(float leftMotor, float rightMotor)
        {
            return GamePad.SetVibration(_playerIndex, leftMotor, rightMotor);
        }
    }
}