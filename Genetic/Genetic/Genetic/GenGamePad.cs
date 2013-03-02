using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Genetic
{
    public class GenGamePad
    {
        /// <summary>
        /// The current state of the game pad.
        /// </summary>
        protected GamePadState gamePadState;

        /// <summary>
        /// The state of the game pad during the previous update.
        /// </summary>
        protected GamePadState oldGamePadState;

        public GenGamePad()
        {
            gamePadState = GamePad.GetState(PlayerIndex.One);
            oldGamePadState = gamePadState;
        }

        /// <summary>
        /// Updates the current and previous game pad states.
        /// </summary>
        public void Update()
        {
            oldGamePadState = gamePadState;
            gamePadState = GamePad.GetState(PlayerIndex.One);
        }

        /// <summary>
        /// Checks if the specified button is currently pressed.
        /// </summary>
        /// <param name="button">The game pad button to check.</param>
        /// <returns>True, if the button is currently pressed. False, if not.</returns>
        public bool IsPressed(Buttons button)
        {
            return gamePadState.IsButtonDown(button);
        }

        /// <summary>
        /// Checks if the specified button is currently released.
        /// </summary>
        /// <param name="button">The game pad button to check.</param>
        /// <returns>True, if the button is currently released. False, if not.</returns>
        public bool IsReleased(Buttons button)
        {
            return gamePadState.IsButtonUp(button);
        }

        /// <summary>
        /// Checks if the specified button was just pressed.
        /// </summary>
        /// <param name="button">The game pad button to check.</param>
        /// <returns>True, if the button was just pressed. False, if not.</returns>
        public bool JustPressed(Buttons button)
        {
            if (oldGamePadState.IsButtonUp(button) && gamePadState.IsButtonDown(button))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks if the specified button was just released.
        /// </summary>
        /// <param name="button">The game pad button to check.</param>
        /// <returns>True, if the button was just released. False, if not.</returns>
        public bool JustReleased(Buttons button)
        {
            if (oldGamePadState.IsButtonDown(button) && gamePadState.IsButtonUp(button))
                return true;
            else
                return false;
        }
    }
}