using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Genetic
{
    public class GenGamePad
    {
        /// <summary>
        /// The current states of the game pads.
        /// </summary>
        protected GamePadState[] gamePadStates;

        /// <summary>
        /// The states of the game pads during the previous update.
        /// </summary>
        protected GamePadState[] oldGamePadStates;

        public GenGamePad()
        {
            gamePadStates = new GamePadState[4];
            oldGamePadStates = new GamePadState[4];

            //gamePadStates[0] = GamePad.GetState(PlayerIndex.One);
            //gamePadStates[1] = GamePad.GetState(PlayerIndex.Two);
            //gamePadStates[2] = GamePad.GetState(PlayerIndex.Three);
            //gamePadStates[3] = GamePad.GetState(PlayerIndex.Four);

            //oldGamePadStates[0] = gamePadStates[0];
            //oldGamePadStates[1] = gamePadStates[1];
            //oldGamePadStates[2] = gamePadStates[2];
            //oldGamePadStates[3] = gamePadStates[3];
        }

        /// <summary>
        /// Updates the current and previous game pad states.
        /// </summary>
        public void Update()
        {
            oldGamePadStates[0] = gamePadStates[0];
            oldGamePadStates[1] = gamePadStates[1];
            oldGamePadStates[2] = gamePadStates[2];
            oldGamePadStates[3] = gamePadStates[3];

            gamePadStates[0] = GamePad.GetState(PlayerIndex.One);
            gamePadStates[1] = GamePad.GetState(PlayerIndex.Two);
            gamePadStates[2] = GamePad.GetState(PlayerIndex.Three);
            gamePadStates[3] = GamePad.GetState(PlayerIndex.Four);
        }

        /// <summary>
        /// Checks if the specified button is currently pressed.
        /// </summary>
        /// <param name="button">The game pad button to check.</param>
        /// <param name="player">The player index number of the game pad to check, a value of 1 through 4.</param>
        /// <returns>True, if the button is currently pressed. False, if not.</returns>
        public bool IsPressed(Buttons button, int player = 1)
        {
            return gamePadStates[--player].IsButtonDown(button);
        }

        /// <summary>
        /// Checks if the specified button is currently released.
        /// </summary>
        /// <param name="button">The game pad button to check.</param>
        /// <param name="player">The player index number of the game pad to check, a value of 1 through 4.</param>
        /// <returns>True, if the button is currently released. False, if not.</returns>
        public bool IsReleased(Buttons button, int player = 1)
        {
            return gamePadStates[--player].IsButtonUp(button);
        }

        /// <summary>
        /// Checks if the specified button was just pressed.
        /// </summary>
        /// <param name="button">The game pad button to check.</param>
        /// <param name="player">The player index number of the game pad to check, a value of 1 through 4.</param>
        /// <returns>True, if the button was just pressed. False, if not.</returns>
        public bool JustPressed(Buttons button, int player = 1)
        {
            if (oldGamePadStates[--player].IsButtonUp(button) && gamePadStates[player].IsButtonDown(button))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks if the specified button was just released.
        /// </summary>
        /// <param name="button">The game pad button to check.</param>
        /// <param name="player">The player index number of the game pad to check, a value of 1 through 4.</param>
        /// <returns>True, if the button was just released. False, if not.</returns>
        public bool JustReleased(Buttons button, int player = 1)
        {
            if (oldGamePadStates[--player].IsButtonDown(button) && gamePadStates[player].IsButtonUp(button))
                return true;
            else
                return false;
        }
    }
}