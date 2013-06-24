using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Genetic.Input
{
    /// <summary>
    /// Manages input from a game pad device.
    /// Extends features such as game pad vibration fading.
    /// 
    /// Author: Tyler Gregory (GeneticSpartan)
    /// </summary>
    public class GenGamePad
    {
        /// <summary>
        /// A special-case control scheme to use the variable values available to certain gamepad controls to move at variable speeds.
        /// </summary>
        public enum ButtonsSpecial {
            /// <summary>
            /// The left thumb stick control on a game pad.
            /// </summary>
            ThumbStickLeft,

            /// <summary>
            /// The right thumb stick control on a game pad.
            /// </summary>
            ThumbStickRight,

            /// <summary>
            /// The left trigger control on a game pad.
            /// </summary>
            TriggerLeft,

            /// <summary>
            /// The right trigger control on a game pad.
            /// </summary>
            TriggerRight,

            /// <summary>
            /// Represents no game pad control.
            /// </summary>
            None
        };

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
        /// Keeps track of the left motor speed when vibration is set.
        /// </summary>
        protected float _leftMotorSpeed;

        /// <summary>
        /// Keeps track of the right motor speed when vibration is set.
        /// </summary>
        protected float _rightMotorSpeed;

        /// <summary>
        /// Determines whether the gamepad vibration intensity decreases over time.
        /// </summary>
        protected bool _vibrateDecreasing;

        protected GenTimer _vibrateTimer;

        /// <summary>
        /// Gets the x position of the left thumbstick, a value between -1.0 and 1.0.
        /// </summary>
        public float ThumbStickLeftX
        {
            get { return _gamePadState.ThumbSticks.Left.X; }
        }

        /// <summary>
        /// Gets the y position of the left thumbstick, a value between -1.0 and 1.0.
        /// </summary>
        public float ThumbStickLeftY
        {
            get { return _gamePadState.ThumbSticks.Left.Y; }
        }

        /// <summary>
        /// Gets the x position of the right thumbstick, a value between -1.0 and 1.0.
        /// </summary>
        public float ThumbStickRightX
        {
            get { return _gamePadState.ThumbSticks.Right.X; }
        }

        /// <summary>
        /// Gets the y position of the right thumbstick, a value between -1.0 and 1.0.
        /// </summary>
        public float ThumbStickRightY
        {
            get { return _gamePadState.ThumbSticks.Right.Y; }
        }

        /// <summary>
        /// Gets the position of the left trigger, a value between 0.0 and 1.0.
        /// </summary>
        public float TriggerLeft
        {
            get { return _gamePadState.Triggers.Left; }
        }

        /// <summary>
        /// Gets the position of the right trigger, a value between 0.0 and 1.0.
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
        /// Gets if the gamepad is currently vibrating.
        /// </summary>
        public bool Vibrating
        {
            get { return _vibrateTimer.IsRunning; }
        }

        /// <summary>
        /// An object used to retrieve input from a game pad.
        /// </summary>
        /// <param name="playerIndex">The player index associated with the game pad.</param>
        public GenGamePad(PlayerIndex playerIndex = PlayerIndex.One)
        {
            _playerIndex = playerIndex;
            _leftMotorSpeed = 0f;
            _rightMotorSpeed = 0f;
            _vibrateDecreasing = false;
            _vibrateTimer = new GenTimer(0f, null, true);
        }

        /// <summary>
        /// Updates the current and previous game pad states.
        /// Handles game pad vibration.
        /// </summary>
        public void Update()
        {
            _oldGamePadState = _gamePadState;
            _gamePadState = GamePad.GetState(_playerIndex);

            if (_vibrateTimer.IsRunning)
            {
                _vibrateTimer.Update();

                if (!_vibrateTimer.IsRunning)
                    StopVibrate();
                else if (_vibrateDecreasing)
                {
                    float vibrateDecrease = _vibrateTimer.Remaining / _vibrateTimer.Duration;

                    GamePad.SetVibration(_playerIndex, _leftMotorSpeed * vibrateDecrease, _rightMotorSpeed * vibrateDecrease);
                }
            }
        }

        public void StopVibrate()
        {
            GamePad.SetVibration(_playerIndex, 0f, 0f);
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
            return (_oldGamePadState.IsButtonUp(button) && _gamePadState.IsButtonDown(button));
        }

        /// <summary>
        /// Checks if the specified button was just released.
        /// </summary>
        /// <param name="button">The game pad button to check.</param>
        /// <returns>True if the button was just released, false if not.</returns>
        public bool JustReleased(Buttons button)
        {
            return (_oldGamePadState.IsButtonDown(button) && _gamePadState.IsButtonUp(button));
        }

        /// <summary>
        /// Sets the vibration speeds of the left and right motors of the game pad.
        /// </summary>
        /// <param name="leftMotor">The speed of the left motor, a value between 0.0 and 1.0. This motor is a low-frequency motor.</param>
        /// <param name="rightMotor">The speed of the right motor, a value between 0.0 and 1.0. This motor is a high-frequency motor.</param>
        /// <param name="duration">The duration of the gamepad vibration, in seconds.</param>
        /// <param name="decreasing">A flag used to determine if the vibration speeds will decrease over time.</param>
        public void Vibrate(float leftMotor = 1f, float rightMotor = 0.5f, float duration = 0.2f, bool decreasing = false)
        {
            _leftMotorSpeed = leftMotor;
            _rightMotorSpeed = rightMotor;

            GamePad.SetVibration(_playerIndex, _leftMotorSpeed, _rightMotorSpeed);

            _vibrateDecreasing = decreasing;

            _vibrateTimer.Duration = duration;
            _vibrateTimer.Start(true);
        }
    }
}