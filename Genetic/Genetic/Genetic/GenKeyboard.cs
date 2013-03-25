using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Genetic
{
    public class GenKeyboard
    {
        /// <summary>
        /// The current states of the keyboards.
        /// </summary>
        protected KeyboardState[] keyboardStates;

        /// <summary>
        /// The states of the keyboards during the previous update.
        /// </summary>
        protected KeyboardState[] oldKeyboardStates;

        public GenKeyboard()
        {
            keyboardStates = new KeyboardState[4];
            oldKeyboardStates = new KeyboardState[4];
        }

        /// <summary>
        /// Updates the current and previous keyboard states.
        /// </summary>
        public void Update()
        {
            oldKeyboardStates[0] = keyboardStates[0];
            oldKeyboardStates[1] = keyboardStates[1];
            oldKeyboardStates[2] = keyboardStates[2];
            oldKeyboardStates[3] = keyboardStates[3];

            keyboardStates[0] = Keyboard.GetState(PlayerIndex.One);
            keyboardStates[1] = Keyboard.GetState(PlayerIndex.Two);
            keyboardStates[2] = Keyboard.GetState(PlayerIndex.Three);
            keyboardStates[3] = Keyboard.GetState(PlayerIndex.Four);
        }

        /// <summary>
        /// Checks if the specified key is currently pressed.
        /// </summary>
        /// <param name="key">The keyboard key to check.</param>
        /// <param name="player">The player index number of the keyboard to check, a value of 1 through 4.</param>
        /// <returns>True, if the key is currently pressed. False, if not.</returns>
        public bool IsPressed(Keys key, int player = 1)
        {
            return keyboardStates[--player].IsKeyDown(key);
        }

        /// <summary>
        /// Checks if the specified key is currently released.
        /// </summary>
        /// <param name="key">The keyboard key to check.</param>
        /// <param name="player">The player index number of the keyboard to check, a value of 1 through 4.</param>
        /// <returns>True, if the key is currently released. False, if not.</returns>
        public bool IsReleased(Keys key, int player = 1)
        {
            return keyboardStates[--player].IsKeyUp(key);
        }

        /// <summary>
        /// Checks if the specified key was just pressed.
        /// </summary>
        /// <param name="key">The keyboard key to check.</param>
        /// <param name="player">The player index number of the keyboard to check, a value of 1 through 4.</param>
        /// <returns>True, if the key was just pressed. False, if not.</returns>
        public bool JustPressed(Keys key, int player = 1)
        {
            if (oldKeyboardStates[--player].IsKeyUp(key) && keyboardStates[player].IsKeyDown(key))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks if the specified key was just released.
        /// </summary>
        /// <param name="key">The keyboard key to check.</param>
        /// <param name="player">The player index number of the keyboard to check, a value of 1 through 4.</param>
        /// <returns>True, if the key was just released. False, if not.</returns>
        public bool JustReleased(Keys key, int player = 1)
        {
            if (oldKeyboardStates[--player].IsKeyDown(key) && keyboardStates[player].IsKeyUp(key))
                return true;
            else
                return false;
        }
    }
}