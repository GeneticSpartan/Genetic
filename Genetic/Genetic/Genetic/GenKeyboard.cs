using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Genetic
{
    public class GenKeyboard
    {
        /// <summary>
        /// The current state of the keyboard.
        /// </summary>
        protected KeyboardState keyboardState;

        /// <summary>
        /// The state of the keyboard during the previous update.
        /// </summary>
        protected KeyboardState oldKeyboardState;

        public GenKeyboard()
        {
            keyboardState = Keyboard.GetState(PlayerIndex.One);
            oldKeyboardState = keyboardState;
        }

        /// <summary>
        /// Updates the current and previous keyboard states.
        /// </summary>
        public void Update()
        {
            oldKeyboardState = keyboardState;
            keyboardState = Keyboard.GetState(PlayerIndex.One);
        }

        /// <summary>
        /// Checks if the specified key is currently pressed.
        /// </summary>
        /// <param name="key">The keyboard key to check.</param>
        /// <returns>True, if the key is currently pressed. False, if not.</returns>
        public bool IsPressed(Keys key)
        {
            return keyboardState.IsKeyDown(key);
        }

        /// <summary>
        /// Checks if the specified key is currently released.
        /// </summary>
        /// <param name="key">The keyboard key to check.</param>
        /// <returns>True, if the key is currently released. False, if not.</returns>
        public bool IsReleased(Keys key)
        {
            return keyboardState.IsKeyUp(key);
        }

        /// <summary>
        /// Checks if the specified key was just pressed.
        /// </summary>
        /// <param name="key">The keyboard key to check.</param>
        /// <returns>True, if the key was just pressed. False, if not.</returns>
        public bool JustPressed(Keys key)
        {
            if (oldKeyboardState.IsKeyUp(key) && keyboardState.IsKeyDown(key))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks if the specified key was just released.
        /// </summary>
        /// <param name="key">The keyboard key to check.</param>
        /// <returns>True, if the key was just released. False, if not.</returns>
        public bool JustReleased(Keys key)
        {
            if (oldKeyboardState.IsKeyDown(key) && keyboardState.IsKeyUp(key))
                return true;
            else
                return false;
        }
    }
}