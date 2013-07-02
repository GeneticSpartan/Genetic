using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Genetic.Input
{
    /// <summary>
    /// Manages input from a keyboard device.
    /// 
    /// Author: Tyler Gregory (GeneticSpartan)
    /// </summary>
    public class GenKeyboard
    {
        /// <summary>
        /// The current state of the keyboard.
        /// </summary>
        protected KeyboardState _keyboardState;

        /// <summary>
        /// The state of the keyboard during the previous update.
        /// </summary>
        protected KeyboardState _oldKeyboardState;

        /// <summary>
        /// The player index associated with the keyboard.
        /// </summary>
        protected PlayerIndex _playerIndex;

        /// <summary>
        /// An object used to retrieve input from a keyboard.
        /// </summary>
        /// <param name="playerIndex">The player index associated with the keyboard.</param>
        public GenKeyboard(PlayerIndex playerIndex)
        {
            _playerIndex = playerIndex;
            _keyboardState = Keyboard.GetState();
            _oldKeyboardState = _keyboardState;
        }

        /// <summary>
        /// Updates the current and previous keyboard states.
        /// </summary>
        public void Update()
        {
            _oldKeyboardState = _keyboardState;
            _keyboardState = Keyboard.GetState(_playerIndex);
        }

        /// <summary>
        /// Checks if the specified key is currently pressed.
        /// </summary>
        /// <param name="key">The keyboard key to check.</param>
        /// <returns>True if the key is currently pressed, false if not.</returns>
        public bool IsPressed(Keys key)
        {
            return _keyboardState.IsKeyDown(key);
        }

        /// <summary>
        /// Checks if the specified key is currently released.
        /// </summary>
        /// <param name="key">The keyboard key to check.</param>
        /// <returns>True if the key is currently released, false if not.</returns>
        public bool IsReleased(Keys key)
        {
            return _keyboardState.IsKeyUp(key);
        }

        /// <summary>
        /// Checks if the specified key was just pressed.
        /// </summary>
        /// <param name="key">The keyboard key to check.</param>
        /// <returns>True if the key was just pressed, false if not.</returns>
        public bool JustPressed(Keys key)
        {
            return (_oldKeyboardState.IsKeyUp(key) && _keyboardState.IsKeyDown(key));
        }

        /// <summary>
        /// Checks if the specified key was just released.
        /// </summary>
        /// <param name="key">The keyboard key to check.</param>
        /// <returns>True if the key was just released, false if not.</returns>
        public bool JustReleased(Keys key)
        {
            return (_oldKeyboardState.IsKeyDown(key) && _keyboardState.IsKeyUp(key));
        }
    }
}